using System;
using System.Collections.Generic;
using System.IO;
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
        return await TryGetRequest<List<ArchiveCategory>>( ApiPathGetCategories, null, apiKey );
    }
    public async Task<ServiceReply<ArchiveSearch?>> SearchVideosAsync( string? apiKey, Dictionary<string,object>? parameters )
    {
        return await TryGetRequest<ArchiveSearch>( ApiPathSearch, parameters, apiKey );
    }
    public async Task<ServiceReply<bool>> DownloadStreamAsync( string? apiKey, Dictionary<string, object>? httpParameters, string downloadPath )
    {
        ServiceReply<Stream?> streamReply = await TryGetRequest<Stream>( ApiPathSearch, httpParameters, apiKey );

        if ( !streamReply.Success || streamReply.Data is null )
        {
            return new ServiceReply<bool>( streamReply.ErrorType, streamReply.Message );
        }

        try
        {
            Stream inputStream = streamReply.Data;
            Stream outputStream = File.Open( downloadPath, FileMode.Create );

            await inputStream.CopyToAsync( outputStream );
            await outputStream.DisposeAsync();
            await inputStream.DisposeAsync();

            return new ServiceReply<bool>( true );
        }
        catch ( Exception e )
        {
            _logger?.LogError( e, e.Message );
            return new ServiceReply<bool>( ServiceErrorType.IoError );
        }
    }
}