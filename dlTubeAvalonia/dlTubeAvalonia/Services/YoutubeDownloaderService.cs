using System;
using System.Collections.Generic;
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
    public string? VideoThumbnail => _video?.Thumbnails?.FirstOrDefault()?.Url;

    public async Task<bool> GetStreamManifest()
    {
        _streamManifest = await _youtube.Videos.Streams.GetManifestAsync( videoUrl );
        _video = await _youtube.Videos.GetAsync( videoUrl );
        return _streamManifest is not null && _video is not null;
    }
    public List<string> GetStreamInfo( int steamType )
    {
        return steamType switch
        {
            0 => GetMixedStreams(),
            1 => GetAudioStreams(),
            2 => GetVideoStreams(),
            _ => throw new Exception( "Invalid Stream Type!" )
        };
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
}