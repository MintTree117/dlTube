using System.Collections.Generic;
using System.Threading.Tasks;

namespace dlTubeAvalonia.Services;

public interface IHttpService
{
    Task<ApiReply<T?>> TryGetRequest<T>( string apiPath, Dictionary<string, object>? parameters = null, string? authToken = null );
    Task<ApiReply<T?>> TryPostRequest<T>( string apiPath, object? body = null, string? authToken = null );
    Task<ApiReply<T?>> TryPutRequest<T>( string apiPath, object? body = null, string? authToken = null );
    Task<ApiReply<T?>> TryDeleteRequest<T>( string apiPath, Dictionary<string, object>? parameters = null, string? authToken = null );
}