using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using dlTubeAvalonia.Models;

namespace dlTubeAvalonia.Services;

public sealed class ArchiveService( HttpClient _http ) : HttpService( _http )
{
    const string ApiPath = "api/archive";
    const string ApiPathGet = $"{ApiPath}/get?"; 
    
    public async Task<ApiReply<List<ArchiveItem>?>> SearchVideosAsync( Dictionary<string,object>? parameters )
    {
        ApiReply<List<ArchiveItem>?> res = await TryGetRequest<List<ArchiveItem>>( ApiPathGet, parameters, null );
        return res;
    }
}