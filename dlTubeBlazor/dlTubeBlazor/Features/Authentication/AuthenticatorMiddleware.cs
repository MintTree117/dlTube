using System.Net;
using System.Text.Json;

namespace dlTubeBlazor.Features.Authentication;

public sealed class AuthenticatorMiddleware
{
    const string InvalidIpMessage = "Your ip address is not authorized.";
    const string InvalidKeyMessage = "Your access key is invalid or has expired.";
    
    readonly RequestDelegate _next;
    readonly ILogger<AuthenticatorMiddleware> _logger;
    AuthenticatorService _authenticatorService = null!;

    // Constructor
    public AuthenticatorMiddleware( RequestDelegate next, ILogger<AuthenticatorMiddleware> logger, IHostEnvironment environment )
    {
        _next = next;
        _logger = logger;
    }
    
    // Main
    public async Task InvokeAsync( HttpContext httpContext  )
    {
        try
        {
            if ( !ShouldAuthenticate( httpContext ) )
            {
                await _next( httpContext );
                return;
            }
            
            IServiceScope scope = httpContext.RequestServices.CreateScope();
            _authenticatorService = scope.ServiceProvider.GetRequiredService<AuthenticatorService>();
            
            if ( !await HandleIpValidation( httpContext ) )
                return;

            if ( !await HandleKeyValidation( httpContext ) )
                return;
            
            await _next( httpContext );
        }
        catch ( Exception e )
        {
            await HandleAuthenticationException( httpContext, e );
        }
    }
    
    // Utils
    static bool ShouldAuthenticate( HttpContext httpContext )
    {
        PathString path = httpContext.Request.Path;
        return path.StartsWithSegments( "/api/stream/" );
    }
    async Task<bool> HandleIpValidation( HttpContext httpContext )
    {
        return true;
        
        if ( _authenticatorService.ValidateIp( httpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty ) )
            return true;

        JsonSerializerOptions options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        string json = JsonSerializer.Serialize( InvalidIpMessage, options );
        await httpContext.Response.WriteAsync( json );
        return false;
    }
    async Task<bool> HandleKeyValidation( HttpContext httpContext )
    {
        if ( !httpContext.Request.Headers.ContainsKey( "Authorization" ) )
        {
            // No Authorization header present
            httpContext.Response.StatusCode = ( int ) HttpStatusCode.Unauthorized;
            await httpContext.Response.WriteAsync( InvalidKeyMessage );
            return false;
        }

        string? authToken = httpContext.Request.Headers.Authorization;

        // Validate authToken with your authenticator service
        if ( _authenticatorService.ValidateToken( authToken ?? string.Empty ) ) 
            return true;
        
        httpContext.Response.StatusCode = ( int ) HttpStatusCode.Unauthorized;
        await httpContext.Response.WriteAsync( InvalidKeyMessage );
        return false;

    }
    async Task HandleAuthenticationException( HttpContext httpContext, Exception e )
    {
        _logger.LogError( e, e.Message );
        httpContext.Response.StatusCode = ( int ) HttpStatusCode.InternalServerError;
        httpContext.Response.ContentType = "application/json";

        JsonSerializerOptions options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        string json = JsonSerializer.Serialize( "Failed to authenticate your request!", options );
        await httpContext.Response.WriteAsync( json );
    }
}