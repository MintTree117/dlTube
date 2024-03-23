using System.Collections.Specialized;
using System.Net.Http.Json;
using System.Web;

namespace dlTubeBlazor.Client.Services;

public sealed class HttpService
{
    readonly HttpClient Http = new();
    
    public async Task<T?> TryGetRequest<T>( string apiPath, Dictionary<string, object>? parameters = null )
    {
        try
        {
            string path = GetQueryParameters( apiPath, parameters );
            HttpResponseMessage httpResponse = await Http.GetAsync( path );
            return await HandleHttpResponse<T?>( httpResponse, "Get" );
        }
        catch ( Exception e )
        {
            Utils.WriteLine( e );
            return default;
        }
    }
    public async Task<T?> TryPostRequest<T>( string apiPath, object? body = null )
    {
        try
        {
            HttpResponseMessage httpResponse = await Http.PostAsJsonAsync( apiPath, body );
            return await HandleHttpResponse<T?>( httpResponse, "Post" );
        }
        catch ( Exception e )
        {
            Utils.WriteLine( e );
            return default;
        }
    }
    public async Task<T?> TryPutRequest<T>( string apiPath, object? body = null )
    {
        try
        {
            HttpResponseMessage httpResponse = await Http.PutAsJsonAsync( apiPath, body );
            return await HandleHttpResponse<T?>( httpResponse, "Put" );
        }
        catch ( Exception e )
        {
            Utils.WriteLine( e );
            return default;
        }
    }
    public async Task<T?> TryDeleteRequest<T>( string apiPath, Dictionary<string, object>? parameters = null )
    {
        try
        {
            string path = GetQueryParameters( apiPath, parameters );
            HttpResponseMessage httpResponse = await Http.DeleteAsync( path );
            return await HandleHttpResponse<T?>( httpResponse, "Delete" );
        }
        catch ( Exception e )
        {
            Utils.WriteLine( e );
            return default;
        }
    }

    static string GetQueryParameters( string apiPath, Dictionary<string, object>? parameters )
    {
        if ( parameters is null )
            return apiPath;
        
        NameValueCollection query = HttpUtility.ParseQueryString( string.Empty );
        
        foreach ( KeyValuePair<string, object> param in parameters )
        {
            query[ param.Key ] = param.Value.ToString();
        }

        return $"{apiPath}?{query}";
    }
    static async Task<T?> HandleHttpResponse<T>( HttpResponseMessage httpResponse, string requestTypeName )
    {
        if ( typeof( T ) == typeof( string ) )
        {
            string responseString = await httpResponse.Content.ReadAsStringAsync();
            return ( T ) ( object ) responseString;
        }
        
        if ( httpResponse.IsSuccessStatusCode )
        {
            var parsedItem = await httpResponse.Content.ReadFromJsonAsync<T>();

            return parsedItem is not null
                ? parsedItem
                : default;
        }

        string errorContent = await httpResponse.Content.ReadAsStringAsync();

        switch ( httpResponse.StatusCode )
        {
            case System.Net.HttpStatusCode.BadRequest:
                Console.WriteLine( $"{requestTypeName}: Bad request: {errorContent}" );
                return default;
            case System.Net.HttpStatusCode.NotFound:
                Console.WriteLine( $"{requestTypeName}: Not found: {errorContent}" );
                return default;
            case System.Net.HttpStatusCode.Unauthorized:
                Console.WriteLine( $"{requestTypeName}: Unauthorized: {errorContent}" );
                return default;
            case System.Net.HttpStatusCode.Conflict:
                Console.WriteLine( $"{requestTypeName}: Conflict: {errorContent}" );
                return default;
            case System.Net.HttpStatusCode.InternalServerError:
                Console.WriteLine( $"{requestTypeName}: Server error: {errorContent}" );
                return default;
            default:
                Console.WriteLine( $"{requestTypeName}: Other error: {httpResponse.StatusCode}, Content: {errorContent}" );
                return default;
        }
    }
}