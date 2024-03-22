using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Avalonia.Media.Imaging;
using YoutubeExplode.Search;
using dlTubeAvalonia.Models;

namespace dlTubeAvalonia.Services;

public sealed class YoutubeBrowser
{
    // Constants
    const int MaxSearchResults = 200;
    
    // Services
    readonly YoutubeClientHolder _youtubeService = Program.ServiceProvider.GetService<YoutubeClientHolder>()!;
    readonly ImageLoader _imageLoader = Program.ServiceProvider.GetService<ImageLoader>()!;

    // Public Methods
    public async Task<ServiceReply<IReadOnlyList<YoutubeSearchResult>>> GetStreams( string query, int resultsPerPage )
    {
        IAsyncEnumerator<VideoSearchResult> enumerator = _youtubeService.YoutubeClient.Search.GetVideosAsync( query ).GetAsyncEnumerator();
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
            string url = v.Thumbnails.Count > 0
                ? v.Thumbnails[ 0 ].Url
                : "";
            
            ServiceReply<Bitmap?> reply = await _imageLoader.GetImageBitmap( url );
            
            customResults.Add( new YoutubeSearchResult
            {
                Title = v.Title,
                Duration = v.Duration.ToString() ?? "00:00:00",
                Url = v.Url,
                Image = reply.Data
            } );
        }
        
        return new ServiceReply<IReadOnlyList<YoutubeSearchResult>>( customResults );
    }
}