using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;
using dlTubeBlazor.Client.Dtos;

namespace dlTubeBlazor.Youtube;

public sealed class YoutubeBrowser( ILogger<YoutubeBrowser> logger )
{
    readonly YoutubeClient _youtube = new();
    
    StreamManifest? _streamManifest;
    Video? _video;
    
    // "Constructor"
    public async Task<bool> TryInitialize( string videoUrl )
    {
        try
        {
            _streamManifest = await _youtube.Videos.Streams.GetManifestAsync( videoUrl );
            _video = await _youtube.Videos.GetAsync( videoUrl );

            return _streamManifest is not null && _video is not null;
        }
        catch ( Exception e )
        {
            logger.LogError( e,e.Message );
            return false;
        }
    }
    public async Task<StreamInfo?> GetStreamInfo()
    {
        if ( _video is null )
            return null;
        
        return new StreamInfo
        {
            Title = _video.Title,
            Duration = _video.Duration.ToString() ?? "00:00:00",
            ImageUrl = _video.Thumbnails.Any() ? _video.Thumbnails[ 0 ].Url : string.Empty,
            Qualities =
            [
                await GetMixedStreams(),
                await GetAudioStreams(),
                await GetVideoStreams()
            ]
        };
    }

    // Get Streams
    async Task<List<string>> GetMixedStreams()
    {
        return await Task.Run( () => {
            List<MuxedStreamInfo> _mixedStreams = _streamManifest!.GetMuxedStreams().ToList();
            List<string> _mixedSteamQualities = [ ];

            for ( int i = 0; i < _mixedStreams.Count; i++ )
            {
                MuxedStreamInfo stream = _mixedStreams[ i ];
                _mixedSteamQualities.Add( $"{i + 1} : {stream.VideoResolution} px - {stream.Bitrate} bps - {stream.Container}" );
            }

            return _mixedSteamQualities;
        } );
    }
    async Task<List<string>> GetAudioStreams()
    {
        return await Task.Run( () => {
            List<AudioOnlyStreamInfo> streams = _streamManifest!.GetAudioOnlyStreams().ToList();
            List<string> _audioSteamQualities = [ ];

            for ( int i = 0; i < streams.Count; i++ )
            {
                AudioOnlyStreamInfo stream = streams[ i ];
                _audioSteamQualities.Add( $"{i + 1} : {stream.Bitrate} bps - {stream.Container}" );
            }

            return _audioSteamQualities;
        } );
    }
    async Task<List<string>> GetVideoStreams()
    {
        return await Task.Run( () => {
            List<VideoOnlyStreamInfo> streams = _streamManifest!.GetVideoOnlyStreams().ToList();
            List<string> _videoSteamQualities = [ ];

            for ( int i = 0; i < streams.Count; i++ )
            {
                VideoOnlyStreamInfo stream = streams[ i ];
                _videoSteamQualities.Add( $"{i + 1} : {stream.VideoResolution} px - {stream.Container}" );
            }

            return _videoSteamQualities;
        } );
    }
}