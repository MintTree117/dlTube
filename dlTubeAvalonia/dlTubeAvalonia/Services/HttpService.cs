using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using dlTubeAvalonia.Models;

namespace dlTubeAvalonia.Services;

// Singleton Service
public class HttpService
{
    readonly HttpClient _http = new();
    readonly ILogger<HttpService>? _logger = Program.ServiceProvider.GetService<ILogger<HttpService>>();
    
    public async Task<ServiceReply<MemoryStream?>> TryGetImageStream( string apiPath, Dictionary<string, object>? parameters = null, string? authToken = null )
    {
        try
        {
            SetAuthHttpHeader( authToken );
            string path = BuildQueryString( apiPath, parameters );
            HttpResponseMessage httpResponse = await _http.GetAsync( path );
            
            return await HandleImageStreamHttpResponse( httpResponse );
        }
        catch ( Exception e )
        {
            _logger?.LogError( e, e.Message );
            return HandleHttpException<MemoryStream?>( e, "Get" );
        }
    }
    public async Task<ServiceReply<T?>> TryGetRequest<T>( string apiPath, Dictionary<string, object>? parameters = null, string? authToken = null )
    {
        try
        {
            SetAuthHttpHeader( authToken );
            string path = BuildQueryString( apiPath, parameters );
            HttpResponseMessage httpResponse = await _http.GetAsync( path );
            return await HandleJsonHttpResponse<T?>( httpResponse );
        }
        catch ( Exception e )
        {
            return HandleHttpException<T?>( e, "Get" );
        }
    }
    public async Task<ServiceReply<T?>> TryPostRequest<T>( string apiPath, object? body = null, string? authToken = null )
    {
        try
        {
            SetAuthHttpHeader( authToken );
            HttpResponseMessage httpResponse = await _http.PostAsJsonAsync( apiPath, body );
            return await HandleJsonHttpResponse<T?>( httpResponse );
        }
        catch ( Exception e )
        {
            return HandleHttpException<T?>( e, "Post" );
        }
    }
    public async Task<ServiceReply<T?>> TryPutRequest<T>( string apiPath, object? body = null, string? authToken = null )
    {
        try
        {
            SetAuthHttpHeader( authToken );
            HttpResponseMessage httpResponse = await _http.PutAsJsonAsync( apiPath, body );
            return await HandleJsonHttpResponse<T?>( httpResponse );
        }
        catch ( Exception e )
        {
            return HandleHttpException<T?>( e, "Put" );
        }
    }
    public async Task<ServiceReply<T?>> TryDeleteRequest<T>( string apiPath, Dictionary<string, object>? parameters = null, string? authToken = null )
    {
        try
        {
            SetAuthHttpHeader( authToken );
            string path = BuildQueryString( apiPath, parameters );
            HttpResponseMessage httpResponse = await _http.DeleteAsync( path );
            return await HandleJsonHttpResponse<T?>( httpResponse );
        }
        catch ( Exception e )
        {
            return HandleHttpException<T?>( e, "Delete" );
        }
    }
    
    static string BuildQueryString( string apiPath, Dictionary<string, object>? parameters )
    {
        if ( parameters is null )
            return apiPath;

        NameValueCollection query = [ ];

        foreach ( KeyValuePair<string, object> param in parameters )
        {
            query.Add( param.Key, param.Value.ToString() );
        }

        return $"{apiPath}?{query}";
    }
    
    async Task<ServiceReply<T?>> HandleJsonHttpResponse<T>( HttpResponseMessage httpResponse )
    {
        // Handle string edge-case: json has trouble with strings
        if ( typeof( T ) == typeof( string ) )
        {
            string responseString = await httpResponse.Content.ReadAsStringAsync();
            return new ServiceReply<T?>( ( T ) ( object ) responseString );
        }
        
        // Early out if operation was not successful
        if ( !httpResponse.IsSuccessStatusCode )
            return await HandleHttpError<T>( httpResponse );
        
        var getReply = await httpResponse.Content.ReadFromJsonAsync<T>();

        return getReply is not null
            ? new ServiceReply<T?>( getReply )
            : new ServiceReply<T?>( ServiceErrorType.NotFound, "No data returned from request" );
    }
    async Task<ServiceReply<MemoryStream?>> HandleImageStreamHttpResponse( HttpResponseMessage httpResponse )
    {
        if ( !httpResponse.IsSuccessStatusCode ) 
            return await HandleHttpError<MemoryStream?>( httpResponse );
        
        await using Stream stream = await httpResponse.Content.ReadAsStreamAsync();
        MemoryStream memoryStream = new();
        await stream.CopyToAsync( memoryStream ); // Copy the stream to a MemoryStream
        await stream.DisposeAsync();
        
        return new ServiceReply<MemoryStream?>( memoryStream );
    }
    async Task<ServiceReply<T?>> HandleHttpError<T>( HttpResponseMessage httpResponse )
    {
        string errorContent = await httpResponse.Content.ReadAsStringAsync();
        ServiceErrorType errorType = ServiceReply<object>.GetHttpServiceErrorType( httpResponse.StatusCode );
        _logger?.LogError( $"{errorContent}" );
        return new ServiceReply<T?>( errorType, errorContent );
    }
    
    ServiceReply<T?> HandleHttpException<T>( Exception e, string requestType )
    {
        _logger?.LogError( e, e.Message );
        return new ServiceReply<T?>( ServiceErrorType.ServerError, $"{requestType}: Exception occurred while sending API request." );
    }
    void SetAuthHttpHeader( string? token )
    {
        _http.DefaultRequestHeaders.Authorization = !string.IsNullOrWhiteSpace( token )
            ? new System.Net.Http.Headers.AuthenticationHeaderValue( "Bearer", token )
            : null;
    }
}