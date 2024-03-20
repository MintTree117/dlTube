using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace dlTubeAvalonia.Services;

public class HttpService
{
    readonly HttpClient Http;
    readonly ILogger<HttpService>? _logger;
    
    public HttpService()
    {
        Http = new HttpClient();
        _logger = Program.ServiceProvider.GetService<ILogger<HttpService>>();
    }
    
    public async Task<ApiReply<T?>> TryGetRequest<T>( string apiPath, Dictionary<string, object>? parameters = null, string? authToken = null )
    {
        try
        {
            SetAuthHttpHeader( authToken );
            string path = BuildQueryString( apiPath, parameters );
            HttpResponseMessage httpResponse = await Http.GetAsync( path );
            return await HandleHttpResponse<T?>( httpResponse );
        }
        catch ( Exception e )
        {
            return HandleHttpException<T?>( e, "Get" );
        }
    }
    public async Task<ApiReply<T?>> TryPostRequest<T>( string apiPath, object? body = null, string? authToken = null )
    {
        try
        {
            SetAuthHttpHeader( authToken );
            HttpResponseMessage httpResponse = await Http.PostAsJsonAsync( apiPath, body );
            return await HandleHttpResponse<T?>( httpResponse );
        }
        catch ( Exception e )
        {
            return HandleHttpException<T?>( e, "Post" );
        }
    }
    public async Task<ApiReply<T?>> TryPutRequest<T>( string apiPath, object? body = null, string? authToken = null )
    {
        try
        {
            SetAuthHttpHeader( authToken );
            HttpResponseMessage httpResponse = await Http.PutAsJsonAsync( apiPath, body );
            return await HandleHttpResponse<T?>( httpResponse );
        }
        catch ( Exception e )
        {
            return HandleHttpException<T?>( e, "Put" );
        }
    }
    public async Task<ApiReply<T?>> TryDeleteRequest<T>( string apiPath, Dictionary<string, object>? parameters = null, string? authToken = null )
    {
        try
        {
            SetAuthHttpHeader( authToken );
            string path = BuildQueryString( apiPath, parameters );
            HttpResponseMessage httpResponse = await Http.DeleteAsync( path );
            return await HandleHttpResponse<T?>( httpResponse );
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
    async Task<ApiReply<T?>> HandleHttpResponse<T>( HttpResponseMessage httpResponse )
    {
        // Handle string edge-case: json has trouble with strings
        if ( typeof( T ) == typeof( string ) )
        {
            string responseString = await httpResponse.Content.ReadAsStringAsync();
            return new ApiReply<T?>( ( T ) ( object ) responseString );
        }
        
        // Early out if operation was successful
        if ( httpResponse.IsSuccessStatusCode )
        {
            var getReply = await httpResponse.Content.ReadFromJsonAsync<T>();

            return getReply is not null
                ? new ApiReply<T?>( getReply )
                : new ApiReply<T?>( ServiceErrorType.NotFound, "No data returned from request" );
        }
        
        // Handle http error code
        string errorContent = await httpResponse.Content.ReadAsStringAsync();
        ServiceErrorType errorType = ApiReply<object>.GetHttpServiceErrorType( httpResponse.StatusCode );
        _logger?.LogError( $"{errorContent}" );
        return new ApiReply<T?>( errorType, errorContent );
    }
    
    ApiReply<T?> HandleHttpException<T>( Exception e, string requestType )
    {
        _logger?.LogError( e, $"{requestType}: Exception occurred while sending API request." );
        return new ApiReply<T?>( ServiceErrorType.ServerError, e.Message );
    }
    void SetAuthHttpHeader( string? token )
    {
        Http.DefaultRequestHeaders.Authorization = !string.IsNullOrWhiteSpace( token )
            ? new System.Net.Http.Headers.AuthenticationHeaderValue( "Bearer", token )
            : null;
    }
}