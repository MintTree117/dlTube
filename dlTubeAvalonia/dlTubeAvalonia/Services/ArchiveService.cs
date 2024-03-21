using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using dlTubeAvalonia.Models;

namespace dlTubeAvalonia.Services;

public sealed class ArchiveService
{
    // Constants
    const string NoHttpMessage = "Failed to obatain HttpService!";
    const string ApiPath = "api/archive";
    const string ApiPathGetCategories = $"{ApiPath}/categories";
    const string ApiPathSearch = $"{ApiPath}/search";
    
    // Services
    readonly ILogger<ArchiveService>? _logger = Program.ServiceProvider.GetService<ILogger<ArchiveService>>();
    readonly HttpService? _http = Program.ServiceProvider.GetService<HttpService>();
    
    // Public Methods
    public async Task<ServiceReply<List<ArchiveCategory>?>> GetCategoriesAsync( string? apiKey )
    {
        return _http is not null
            ? await _http.TryGetRequest<List<ArchiveCategory>>( ApiPathGetCategories, null, apiKey )
            : new ServiceReply<List<ArchiveCategory>?>( ServiceErrorType.AppError, NoHttpMessage );
    }
    public async Task<ServiceReply<ArchiveSearch?>> SearchVideosAsync( string? apiKey, Dictionary<string,object>? parameters )
    {
        return _http is not null
            ? await _http.TryGetRequest<ArchiveSearch>( ApiPathSearch, parameters, apiKey )
            : new ServiceReply<ArchiveSearch?>( ServiceErrorType.AppError, NoHttpMessage );
    }
    public async Task<ServiceReply<bool>> DownloadStreamAsync( string? apiKey, Dictionary<string, object>? httpParameters, string downloadPath )
    {
        if ( _http is null )
            return new ServiceReply<bool>( ServiceErrorType.AppError, NoHttpMessage );
        
        ServiceReply<Stream?> streamReply = await _http.TryGetRequest<Stream>( ApiPathSearch, httpParameters, apiKey );

        if ( !streamReply.Success || streamReply.Data is null )
            return new ServiceReply<bool>( streamReply.ErrorType, streamReply.Message );

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