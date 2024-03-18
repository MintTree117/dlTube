using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using dlTubeAvaloniaCrossPlatform.Models;
using Microsoft.Extensions.Logging;

namespace dlTubeAvaloniaCrossPlatform.Services;

public sealed class ArchiveService( HttpClient _http, ILogger<ArchiveService>? _logger ) : HttpService( _http, _logger )
{
    const string ApiPath = "api/archive";
    const string ApiPathGet = $"{ApiPath}/get";
    
    public async Task<ApiReply<ArchiveSearch?>> SearchVideosAsync( Dictionary<string,object>? parameters )
    {
        ApiReply<ArchiveSearch?> res = await TryGetRequest<ArchiveSearch>( ApiPathGet, parameters, null );
        return res;
    }
}