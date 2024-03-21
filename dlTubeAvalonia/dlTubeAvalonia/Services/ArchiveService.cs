using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using dlTubeAvalonia.Models;

namespace dlTubeAvalonia.Services;

public sealed class ArchiveService : HttpService
{
    // Constants
    const string ApiPath = "api/archive";
    const string ApiPathGetCategories = $"{ApiPath}/categories";
    const string ApiPathSearch = $"{ApiPath}/search";
    
    // Services
    readonly ILogger<ArchiveService>? _logger = Program.ServiceProvider.GetService<ILogger<ArchiveService>>();
    
    // Public Methods
    public async Task<ServiceReply<List<ArchiveCategory>?>> GetCategoriesAsync( string? apiKey )
    {
        ServiceReply<List<ArchiveCategory>?> res = await TryGetRequest<List<ArchiveCategory>>( ApiPathGetCategories, null, apiKey );
        return res;
    }
    public async Task<ServiceReply<ArchiveSearch?>> SearchVideosAsync( string? apiKey, Dictionary<string,object>? parameters )
    {
        ServiceReply<ArchiveSearch?> res = await TryGetRequest<ArchiveSearch>( ApiPathSearch, parameters, apiKey );
        return res;
    }
}