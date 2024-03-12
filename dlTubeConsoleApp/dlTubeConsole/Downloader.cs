using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace dlTubeConsole;

public class Downloader( string videoUrl, DownloadType type )
{
    readonly YoutubeClient _youtube = new();
    StreamManifest? _streamManifest;
    Video? _video;

    List<MuxedStreamInfo>? _mixedStreams;
    List<AudioOnlyStreamInfo>? _audioStreams;
    List<VideoOnlyStreamInfo>? _videoStreams;

    public async Task<bool> GetStreamManifest()
    {
        _streamManifest = await _youtube.Videos.Streams.GetManifestAsync( videoUrl );
        _video = await _youtube.Videos.GetAsync( videoUrl );
        return _streamManifest is not null && _video is not null;
    }
    public List<string> GetStreams()
    {
        return type switch
        {
            DownloadType.MIXED => GetMixedStreams(),
            DownloadType.AUDIO => GetAudioStreams(),
            DownloadType.VIDEO => GetVideoStreams(),
            _ => throw new ArgumentOutOfRangeException( nameof( type ), type, null )
        };
    }

    public async Task Download( string filepath, int selection )
    {
        switch ( type )
        {
            case DownloadType.MIXED:
                await DownloadMixed( filepath, selection );
                break;
            case DownloadType.AUDIO:
                await DownloadAudio( filepath, selection );
                break;
            case DownloadType.VIDEO:
                await DownloadVideo( filepath, selection );
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    List<string> GetMixedStreams()
    {
        List<string> qualities = [ ];
        _mixedStreams = _streamManifest!.GetMuxedStreams().ToList();

        for ( var i = 0; i < _mixedStreams.Count; i++ )
        {
            MuxedStreamInfo stream = _mixedStreams[ i ];
            qualities.Add( $"{i + 1}: {stream.VideoResolution}px - {stream.Bitrate}bps - {stream.Container}" );
        }

        return qualities;
    }
    List<string> GetAudioStreams()
    {
        List<string> qualities = [ ];
        _audioStreams = _streamManifest!.GetAudioOnlyStreams().ToList();

        for ( var i = 0; i < _audioStreams.Count; i++ )
        {
            AudioOnlyStreamInfo stream = _audioStreams[ i ];
            qualities.Add( $"{i + 1}: {stream.Bitrate}bps - {stream.Container}" );
        }

        return qualities;
    }
    List<string> GetVideoStreams()
    {
        List<string> qualities = [ ];
        _videoStreams = _streamManifest!.GetVideoOnlyStreams().ToList();

        for ( var i = 0; i < _videoStreams.Count; i++ )
        {
            VideoOnlyStreamInfo stream = _videoStreams[ i ];
            qualities.Add( $"{i + 1}: {stream.VideoResolution}px - {stream.Container}" );
        }

        return qualities;
    }

    async Task DownloadMixed( string outputDirectory, int selection )
    {
        if ( _mixedStreams is null || _mixedStreams.Count <= 0 )
            throw new Exception( "Internal App Error: No mixed streams found!!!" );

        MuxedStreamInfo selectedStream = _mixedStreams[ selection - 1 ];
        string path = GetDownloadPath( outputDirectory, selectedStream.Container.Name );
        await _youtube.Videos.Streams.DownloadAsync( selectedStream, path );
    }
    async Task DownloadAudio( string outputDirectory, int selection )
    {
        if ( _audioStreams is null || _audioStreams.Count <= 0 )
            throw new Exception( "Internal App Error: No audio streams found!!!" );

        AudioOnlyStreamInfo selectedStream = _audioStreams[ selection - 1 ];
        string path = GetDownloadPath( outputDirectory, selectedStream.Container.Name );
        await _youtube.Videos.Streams.DownloadAsync( selectedStream, path );
    }
    async Task DownloadVideo( string outputDirectory, int selection )
    {
        if ( _videoStreams is null || _videoStreams.Count <= 0 )
            throw new Exception( "Internal App Error: No video streams found!!!" );

        VideoOnlyStreamInfo selectedStream = _videoStreams[ selection - 1 ];
        string path = GetDownloadPath( outputDirectory, selectedStream.Container.Name );
        await _youtube.Videos.Streams.DownloadAsync( selectedStream, path );
    }

    string GetDownloadPath( string outputDirectory, string fileExtension )
    {
        string videoName = SanitizeVideoName( _video!.Title );
        var fileName = $"{videoName}.{fileExtension}";
        return Path.Combine( outputDirectory, fileName );

        static string SanitizeVideoName( string videoName )
        {
            return Path.GetInvalidFileNameChars().Aggregate( videoName,
                ( current, invalidChar ) => current.Replace( invalidChar, '-' ) );
        }
    }
}