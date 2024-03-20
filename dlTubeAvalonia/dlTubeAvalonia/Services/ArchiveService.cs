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
    readonly ILogger<ArchiveService>? _logger;
    
    // Constructor
    public ArchiveService()
    {
        _logger = Program.ServiceProvider.GetService<ILogger<ArchiveService>>();
    }
    
    // Public Methods
    public async Task<ApiReply<List<ArchiveCategory>?>> GetCategoriesAsync()
    {
        ApiReply<List<ArchiveCategory>?> res = await TryGetRequest<List<ArchiveCategory>>( ApiPathGetCategories, null, null );
        return res;
    }
    public async Task<ApiReply<ArchiveSearch?>> SearchVideosAsync( Dictionary<string,object>? parameters )
    {
        ApiReply<ArchiveSearch?> res = await TryGetRequest<ArchiveSearch>( ApiPathSearch, parameters, null );
        return res;
    }
}