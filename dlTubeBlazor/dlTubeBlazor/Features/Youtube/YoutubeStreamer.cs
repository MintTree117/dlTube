using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using dlTubeBlazor.Client.Enums;

namespace dlTubeBlazor.Features.Youtube;

public sealed class YoutubeStreamer( ILogger<YoutubeBrowser> logger )
{
    // Fields
    readonly ILogger<YoutubeBrowser> _logger = logger;
    readonly YoutubeClient _youtube = new();
    StreamManifest _streamManifest = null!;

    // Public Methods
    public async Task<bool> TryInitialize( string videoUrl )
    {
        try
        {
            _streamManifest = await _youtube.Videos.Streams.GetManifestAsync( videoUrl );
            return true;
        }
        catch ( Exception e )
        {
            _logger.LogError( e, e.Message );
            return false;
        }
    }
    public async Task<Stream?> Stream( StreamType streamType, int qualityIndex )
    {
        try
        {
            IStreamInfo? streamInfo = streamType switch
            {
                StreamType.Mixed => GetMuxedStreamInfo( _streamManifest, qualityIndex ),
                StreamType.Audio => GetAudioOnlyStreamInfo( _streamManifest, qualityIndex ),
                StreamType.Video => GetVideoOnlyStreamInfo( _streamManifest, qualityIndex ),
                _ => null
            };

            if ( streamInfo is null )
            {
                _logger.LogError( "StreamInfo is null upon info request!" );
                return null;
            }

            Stream stream = new MemoryStream();
            await _youtube.Videos.Streams.CopyToAsync( streamInfo, stream );
            stream.Position = 0;
            return stream;
        }
        catch ( Exception e )
        {
            _logger.LogError( e, e.Message );
            return null;
        }
    }
    
    // Private Methods
    static MuxedStreamInfo? GetMuxedStreamInfo( StreamManifest manifest, int qualityIndex )
    {
        List<MuxedStreamInfo> streams = manifest.GetMuxedStreams().ToList();
        bool isValidQualityIndex = qualityIndex >= 0 && qualityIndex < streams.Count;
        return isValidQualityIndex ? streams[ qualityIndex ] : null;
    }
    static AudioOnlyStreamInfo? GetAudioOnlyStreamInfo( StreamManifest manifest, int qualityIndex )
    {
        List<AudioOnlyStreamInfo> streams = manifest.GetAudioOnlyStreams().ToList();
        bool isValidQualityIndex = qualityIndex >= 0 && qualityIndex < streams.Count;
        return isValidQualityIndex ? streams[ qualityIndex ] : null;
    }
    static VideoOnlyStreamInfo? GetVideoOnlyStreamInfo( StreamManifest manifest, int qualityIndex )
    {
        List<VideoOnlyStreamInfo> streams = manifest.GetVideoOnlyStreams().ToList();
        bool isValidQualityIndex = qualityIndex >= 0 && qualityIndex < streams.Count;
        return isValidQualityIndex ? streams[ qualityIndex ] : null;
    }
}