using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using dlTubeAvalonia.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using YoutubeExplode.Search;

namespace dlTubeAvalonia.Services;

public sealed class YoutubeSearchService
{
    // Constants
    const int MaxSearchResults = 200;
    
    // Services
    readonly ILogger<YoutubeSearchService>? _logger;
    readonly YoutubeClientService? _youtubeService;
    readonly HttpClient? _http;
    
    // Constructor
    public YoutubeSearchService()
    {
        _http = new HttpClient();
        _logger = Program.ServiceProvider.GetService<ILogger<YoutubeSearchService>>();
        TryGetYoutubeClientService( ref _youtubeService );
    }
    void TryGetYoutubeClientService( ref YoutubeClientService? _clientService )
    {
        try
        {
            _clientService = Program.ServiceProvider.GetService<YoutubeClientService>();
        }
        catch ( Exception e )
        {
            _logger?.LogError( e, e.Message );
        }
    }
    
    // Public Methods
    public async Task<IReadOnlyList<YoutubeSearchResult>> GetStreams( string query, int resultsPerPage )
    {
        if ( _youtubeService?.YoutubeClient is null )
        {
            _logger?.LogError( "Youtube client is null!" );
            return new List<YoutubeSearchResult>();   
        }
        
        IAsyncEnumerator<VideoSearchResult> enumerator =
            _youtubeService.YoutubeClient.Search.GetVideosAsync( query ).GetAsyncEnumerator();

        List<VideoSearchResult> results = [ ];

        int sanitizedResultsPerPage = Math.Min( resultsPerPage, MaxSearchResults );

        // Move to the first item in the enumerator
        bool hasResults = await enumerator.MoveNextAsync();

        for ( int i = 0; i < sanitizedResultsPerPage && hasResults; i++ )
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
            customResults.Add( new YoutubeSearchResult
            {
                Title = v.Title,
                Duration = v.Duration.ToString() ?? "00:00:00",
                Url = v.Url,
                Image = await GetImageBitmap( v.Thumbnails[ 0 ].Url )
            } );
        }
        
        _logger.LogError( "Count: " + results.Count );
        
        return customResults;
    }
    
    // Private Methods
    async Task<Bitmap?> GetImageBitmap( string imageUrl )
    {
        byte[]? bytes = await YoutubeImageService.LoadImageBytesFromUrlAsync( imageUrl, _http );

        if ( bytes is null )
            return null;

        using MemoryStream memoryStream = new( bytes );
        return new Bitmap( memoryStream );
    }
}