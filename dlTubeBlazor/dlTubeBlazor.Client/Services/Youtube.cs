using System.Collections.Specialized;
using System.Net;
using System.Net.Http.Json;
using System.Web;
using dlTubeBlazor.Client.Dtos;
using Microsoft.JSInterop;

namespace dlTubeBlazor.Client.Services;

public sealed class Youtube()
{
    readonly HttpClient _http = new();
    readonly IJSRuntime _jsRuntime = null;

    public async Task<StreamInfo?> GetStreamInfo( Dictionary<string, object> parameters )
    {
        _http.BaseAddress = new Uri( "https://localhost:7166" );
        
        try
        {
            string path = GetQueryParameters( HttpConsts.GetStreamInfo, parameters );
            HttpResponseMessage httpResponse = await _http.GetAsync( path );

            if ( !httpResponse.IsSuccessStatusCode )
                return await HandleError<StreamInfo>( httpResponse, "Get" );

            return await httpResponse.Content.ReadFromJsonAsync<StreamInfo>();
        }
        catch ( Exception e )
        {
            Utils.WriteLine( e );
            return default;
        }
    }
    public async Task<bool> TryDownloadStream( Dictionary<string, object> parameters )
    {
        try
        {
            string path = GetQueryParameters( HttpConsts.GetStreamDownload, parameters );
            
            await using Stream stream = await _http.GetStreamAsync( path );
            await using MemoryStream memoryStream = new();
            await stream.CopyToAsync( memoryStream );
            
            byte[] bytes = memoryStream.ToArray();
            await _jsRuntime.InvokeVoidAsync( "downloadFileFromStream", "yourFileName.ext", bytes );
            return true;
        }
        catch ( Exception e )
        {
            Utils.WriteLine( e );
            return false;
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
    static async Task<T?> HandleError<T>( HttpResponseMessage httpResponse, string requestTypeName )
    {
        string errorContent = await httpResponse.Content.ReadAsStringAsync();

        switch ( httpResponse.StatusCode )
        {
            case HttpStatusCode.BadRequest:
                Console.WriteLine( $"{requestTypeName}: {httpResponse.StatusCode}: {errorContent}" );
                return default;
            case HttpStatusCode.NotFound:
                Console.WriteLine( $"{requestTypeName}: {httpResponse.StatusCode}: {errorContent}" );
                return default;
            case HttpStatusCode.Unauthorized:
                Console.WriteLine( $"{requestTypeName}: {httpResponse.StatusCode}: {errorContent}" );
                return default;
            case HttpStatusCode.Conflict:
                Console.WriteLine( $"{requestTypeName}: {httpResponse.StatusCode}: {errorContent}" );
                return default;
            case HttpStatusCode.InternalServerError:
                Console.WriteLine( $"{requestTypeName}: {httpResponse.StatusCode}: {errorContent}" );
                return default;
            default:
                Console.WriteLine( $"{requestTypeName}: {httpResponse.StatusCode}: {httpResponse.StatusCode}, Content: {errorContent}" );
                return default;
        }
    }
}