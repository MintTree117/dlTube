using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace dlTubeAvalonia.Services;

public sealed class YoutubeDownloaderService( string videoUrl )
{
    readonly YoutubeClient _youtube = new();
    StreamManifest? _streamManifest;
    Video? _video;

    List<MuxedStreamInfo>? _mixedStreams;
    List<AudioOnlyStreamInfo>? _audioStreams;
    List<VideoOnlyStreamInfo>? _videoStreams;

    List<string>? _mixedSteamQualities;
    List<string>? _audioSteamQualities;
    List<string>? _videoSteamQualities;

    public string? VideoName => _video?.Title;
    public string? VideoThumbnail => _video?.Thumbnails.FirstOrDefault()?.Url;

    public async Task<bool> GetStreamManifest()
    {
        _streamManifest = await _youtube.Videos.Streams.GetManifestAsync( videoUrl );
        _video = await _youtube.Videos.GetAsync( videoUrl );
        return _streamManifest is not null && _video is not null;
    }
    public List<string> GetStreamInfo( StreamType steamType )
    {
        return steamType switch
        {
            StreamType.Mixed => GetMixedStreams(),
            StreamType.Audio => GetAudioStreams(),
            StreamType.Video => GetVideoStreams(),
            _ => throw new Exception( "Invalid Stream Type!" )
        };
    }
    public async Task<bool> Download( string filepath, StreamType type, int quality )
    {
        try
        {
            switch ( type )
            {
                case StreamType.Mixed:
                    await DownloadMixed( filepath, quality );
                    break;
                case StreamType.Audio:
                    await DownloadAudio( filepath, quality );
                    break;
                case StreamType.Video:
                    await DownloadVideo( filepath, quality );
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return true;
        }
        catch ( Exception e )
        {
            Console.WriteLine( e + e.Message );
            return false;
        }
    }
    
    List<string> GetMixedStreams()
    {
        if ( _mixedSteamQualities is not null )
            return _mixedSteamQualities;

        _mixedStreams = _streamManifest!.GetMuxedStreams().ToList();
        _mixedSteamQualities = [ ];

        for ( int i = 0; i < _mixedStreams.Count; i++ )
        {
            MuxedStreamInfo stream = _mixedStreams[ i ];
            _mixedSteamQualities.Add( $"{i + 1}: {stream.VideoResolution}px - {stream.Bitrate}bps - {stream.Container}" );
        }

        return _mixedSteamQualities;
    }
    List<string> GetAudioStreams()
    {
        if ( _audioSteamQualities is not null )
            return _audioSteamQualities;

        _audioStreams = _streamManifest!.GetAudioOnlyStreams().ToList();

        _audioSteamQualities = [ ];

        for ( int i = 0; i < _audioStreams.Count; i++ )
        {
            AudioOnlyStreamInfo stream = _audioStreams[ i ];
            _audioSteamQualities.Add( $"{i + 1}: {stream.Bitrate}bps - {stream.Container}" );
        }

        return _audioSteamQualities;
    }
    List<string> GetVideoStreams()
    {
        if ( _videoSteamQualities is not null )
            return _videoSteamQualities;

        _videoStreams = _streamManifest!.GetVideoOnlyStreams().ToList();

        _videoSteamQualities = [ ];

        for ( int i = 0; i < _videoStreams.Count; i++ )
        {
            VideoOnlyStreamInfo stream = _videoStreams[ i ];
            _videoSteamQualities.Add( $"{i + 1}: {stream.VideoResolution}px - {stream.Container}" );
        }

        return _videoSteamQualities;
    }

    async Task DownloadMixed( string outputDirectory, int selection )
    {
        if ( _mixedStreams is null || _mixedStreams.Count <= 0 )
            throw new Exception( "Internal App Error: No mixed streams found!!!" );

        MuxedStreamInfo selectedStream = _mixedStreams[ selection ];
        string path = GetDownloadPath( outputDirectory, selectedStream.Container.Name );
        await _youtube.Videos.Streams.DownloadAsync( selectedStream, path );
    }
    async Task DownloadAudio( string outputDirectory, int selection )
    {
        if ( _audioStreams is null || _audioStreams.Count <= 0 )
            throw new Exception( "Internal App Error: No audio streams found!!!" );

        AudioOnlyStreamInfo selectedStream = _audioStreams[ selection ];
        string path = GetDownloadPath( outputDirectory, selectedStream.Container.Name );
        await _youtube.Videos.Streams.DownloadAsync( selectedStream, path );
    }
    async Task DownloadVideo( string outputDirectory, int selection )
    {
        if ( _videoStreams is null || _videoStreams.Count <= 0 )
            throw new Exception( "Internal App Error: No video streams found!!!" );

        VideoOnlyStreamInfo selectedStream = _videoStreams[ selection ];
        string path = GetDownloadPath( outputDirectory, selectedStream.Container.Name );
        await _youtube.Videos.Streams.DownloadAsync( selectedStream, path );
    }

    string GetDownloadPath( string outputDirectory, string fileExtension )
    {
        string videoName = SanitizeVideoName( _video!.Title );
        string fileName = $"{videoName}.{fileExtension}";
        return Path.Combine( outputDirectory, fileName );

        static string SanitizeVideoName( string videoName )
        {
            return Path.GetInvalidFileNameChars().Aggregate( videoName,
                ( current, invalidChar ) => current.Replace( invalidChar, '-' ) );
        }
    }
}