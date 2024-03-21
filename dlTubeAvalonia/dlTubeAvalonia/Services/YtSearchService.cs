using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using YoutubeExplode.Search;
using dlTubeAvalonia.Models;

namespace dlTubeAvalonia.Services;

public sealed class YtSearchService
{
    // Constants
    const int MaxSearchResults = 200;
    
    // Services
    readonly ILogger<YtSearchService>? _logger = Program.ServiceProvider.GetService<ILogger<YtSearchService>>();
    readonly YtClientService? _youtubeService = Program.ServiceProvider.GetService<YtClientService>();
    readonly HttpService? _httpService = Program.ServiceProvider.GetService<HttpService>();
    
    // Constructor

    // Public Methods
    public async Task<ServiceReply<IReadOnlyList<YoutubeSearchResult>>> GetStreams( string query, int resultsPerPage )
    {
        if ( !CheckServices( out string message ) )
            return new ServiceReply<IReadOnlyList<YoutubeSearchResult>>( ServiceErrorType.AppError, message );
        
        IAsyncEnumerator<VideoSearchResult> enumerator = _youtubeService!.YoutubeClient!.Search.GetVideosAsync( query ).GetAsyncEnumerator();
        List<VideoSearchResult> results = [ ];
        int cappedResultsPerPage = Math.Min( resultsPerPage, MaxSearchResults );

        // Move to the first item in the enumerator
        bool hasResults = await enumerator.MoveNextAsync();

        for ( int i = 0; i < cappedResultsPerPage && hasResults; i++ )
        {
            VideoSearchResult c = enumerator.Current;

            if ( !( string.IsNullOrWhiteSpace( c.Title ) && string.IsNullOrWhiteSpace( c.Url ) ) )
                results.Add( c );

            hasResults = await enumerator.MoveNextAsync();
        }

        await enumerator.DisposeAsync();
        
        // CUSTOM MAPPING
        List<YoutubeSearchResult> customResults = [ ];
        
        foreach ( VideoSearchResult v in results )
        {
            // ERROR HERE
            Bitmap? img = await Utils.GetImageBitmap( v.Thumbnails[ 0 ].Url, _httpService );
            
            customResults.Add( new YoutubeSearchResult
            {
                Title = v.Title,
                Duration = v.Duration.ToString() ?? "00:00:00",
                Url = v.Url,
                Image = img
            } );
        }

        return new ServiceReply<IReadOnlyList<YoutubeSearchResult>>( customResults );
    }

    bool CheckServices( out string message )
    {
        message = string.Empty;
        
        if ( _httpService is null )
        {
            message = "Youtube client is null!";
            return false;
        }
        if ( _youtubeService?.YoutubeClient is null )
        {
            message = "Youtube client is null!";
            return false;
        }

        return true;
    }
}