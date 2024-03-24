using System.Net;
using System.Text.Json;

namespace dlTubeBlazor.Features.Authentication;

public sealed class AuthenticatorMiddleware
{
    readonly RequestDelegate _next;
    readonly ILogger<AuthenticatorMiddleware> _logger;
    AuthenticatorService _authenticatorService = null!;

    public AuthenticatorMiddleware( RequestDelegate next, ILogger<AuthenticatorMiddleware> logger, IHostEnvironment environment )
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync( HttpContext httpContext  )
    {
        try
        {
            IServiceScope scope = httpContext.RequestServices.CreateScope();
            _authenticatorService = scope.ServiceProvider.GetRequiredService<AuthenticatorService>();
            
            if ( !_authenticatorService.ValidateIp( httpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty ) )
            {
                JsonSerializerOptions options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                string json = JsonSerializer.Serialize( "Your ip address is not authenticated!", options );
                await httpContext.Response.WriteAsync( json );
                return;
            }

            if ( !_authenticatorService.ValidateToken( httpContext.Response.Headers.Authorization.ToString() ) )
            {
                JsonSerializerOptions options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                string json = JsonSerializer.Serialize( "Your access token is invalid or expired!", options );
                await httpContext.Response.WriteAsync( json );
                return;
            }
            
            await _next( httpContext );
        }
        catch ( Exception ex )
        {
            _logger.LogError( ex, ex.Message );
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = ( int ) HttpStatusCode.InternalServerError;

            JsonSerializerOptions options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            string json = JsonSerializer.Serialize( "Failed to authenticate your request!", options );
            await httpContext.Response.WriteAsync( json );
        }
    }
}