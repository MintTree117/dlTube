using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Search;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace dlTubeAvaloniaCrossPlatform.Services;

public sealed class YoutubeSearchService
{
    const int MaxSearchResults = 200;
    readonly YoutubeClient _youtube = new();
    
    public async Task<IReadOnlyList<VideoSearchResult>> GetStreams( string query, int resultsPerPage )
    {
        IAsyncEnumerator<VideoSearchResult> enumerator =
            _youtube.Search.GetVideosAsync( query ).GetAsyncEnumerator();

        List<VideoSearchResult> results = [ ];

        int sanitizedResultsPerPage = Math.Min( resultsPerPage, MaxSearchResults );

        // Move to the first item in the enumerator
        bool hasResults = await enumerator.MoveNextAsync();

        for ( int i = 0; i < sanitizedResultsPerPage && hasResults; i++ )
        {
            VideoSearchResult c = enumerator.Current;

            if ( !( string.IsNullOrWhiteSpace( c.Title ) || string.IsNullOrWhiteSpace( c.Url ) ) )
                results.Add( c );

            hasResults = await enumerator.MoveNextAsync();
        }

        await enumerator.DisposeAsync();
        return results;
    }
    public async Task<string> GetAudioStream( VideoId videoId )
    {
        StreamManifest manifest = await _youtube.Videos.Streams.GetManifestAsync( videoId );
        IStreamInfo audioStreamInfo = manifest.GetVideoOnlyStreams().ToList()[ 0 ];
        return audioStreamInfo.Url;
    }
}