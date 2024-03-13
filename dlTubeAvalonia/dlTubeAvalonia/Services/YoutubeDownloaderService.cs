using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
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
    byte[]? _thumbnailBytes;
    
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
        try
        {
            _streamManifest = await _youtube.Videos.Streams.GetManifestAsync( videoUrl );
            _video = await _youtube.Videos.GetAsync( videoUrl );
            return _streamManifest is not null && _video is not null;
        }
        catch ( Exception e )
        {
            Console.WriteLine( e );
            return false;
        }
    }
    public async Task<byte[]?> GetThumbnailBytes()
    {
        if ( _thumbnailBytes is not null || _video?.Thumbnails[ 0 ].Url is null )
            return _thumbnailBytes;

        _thumbnailBytes = await LoadImageBytesFromUrlAsync( _video.Thumbnails[ 0 ].Url );
        return _thumbnailBytes;
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
                    Console.WriteLine( "Invalid stream type!" );
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
    
    async Task DownloadMixed( string outputDirectory, int selection )
    {
        if ( _mixedStreams is null || _mixedStreams.Count <= 0 )
            throw new Exception( "Internal App Error: No mixed streams found!!!" );

        MuxedStreamInfo selectedStream = _mixedStreams[ selection ];
        string path = GetDownloadPath( outputDirectory, selectedStream.Container.Name );
        await _youtube.Videos.Streams.DownloadAsync( selectedStream, path );
        await AddThumbnail( path );
    }
    async Task DownloadAudio( string outputDirectory, int selection )
    {
        if ( _audioStreams is null || _audioStreams.Count <= 0 )
            throw new Exception( "Internal App Error: No audio streams found!!!" );

        AudioOnlyStreamInfo selectedStream = _audioStreams[ selection ];
        string path = GetDownloadPath( outputDirectory, selectedStream.Container.Name );
        await _youtube.Videos.Streams.DownloadAsync( selectedStream, path );
        await AddThumbnail( path );
    }
    async Task DownloadVideo( string outputDirectory, int selection )
    {
        if ( _videoStreams is null || _videoStreams.Count <= 0 )
            throw new Exception( "Internal App Error: No video streams found!!!" );

        VideoOnlyStreamInfo selectedStream = _videoStreams[ selection ];
        string path = GetDownloadPath( outputDirectory, selectedStream.Container.Name );
        await _youtube.Videos.Streams.DownloadAsync( selectedStream, path );
        await AddThumbnail( path );
    }
    async Task AddThumbnail( string videoPath )
    {
        if ( _thumbnailBytes is null )
            return;

        string thumbnailPath = Path.Combine( Path.GetTempPath(), "thumbnail.jpg" );
        string convertedThumbnailPath = Path.Combine( Path.GetTempPath(), "thumbnail_converted.jpg" );
        string tempVideoPath = Path.Combine( Path.GetTempPath(), $"video_temp{Path.GetExtension( videoPath )}" );

        try
        {
            await File.WriteAllBytesAsync( thumbnailPath, _thumbnailBytes );
            await CreateJpgCopyFFMPEG( thumbnailPath, convertedThumbnailPath );
            await CreateVideoWithImageFFMPEG( videoPath, convertedThumbnailPath, tempVideoPath );

            if ( !File.Exists( tempVideoPath ) )
                return;

            File.Delete( videoPath ); // Delete original file
            File.Move( tempVideoPath, videoPath ); // Move the temp file to original path
        }
        catch ( Exception e )
        {
            Console.WriteLine( e );
        }
        finally
        {
            if ( File.Exists( thumbnailPath ) )
                File.Delete( thumbnailPath );
            if ( File.Exists( convertedThumbnailPath ) )
                File.Delete( convertedThumbnailPath );
            if ( File.Exists( tempVideoPath ) )
                File.Delete( tempVideoPath );
        }
    }
    
    static async Task<byte[]?> LoadImageBytesFromUrlAsync( string imageUrl )
    {
        try
        {
            using var client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync( imageUrl );

            if ( response.IsSuccessStatusCode )
            {
                await using Stream stream = await response.Content.ReadAsStreamAsync();
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync( memoryStream ); // Copy the stream to a MemoryStream
                return memoryStream.ToArray();
            }
        }
        catch ( Exception ex )
        {
            Console.WriteLine( $"Failed to load image from URL: {ex.Message}" );
        }

        return null;
    }
    static async Task CreateJpgCopyFFMPEG( string inputPath, string outputPath )
    {
        var conversionProcessStartInfo = new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = $"-i \"{inputPath}\" \"{outputPath}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        
        using Process conversionProcess = new();
        conversionProcess.StartInfo = conversionProcessStartInfo;
        conversionProcess.Start();
        await conversionProcess.WaitForExitAsync();
    }
    static async Task CreateVideoWithImageFFMPEG( string videoPath, string convertedThumbnailPath, string tempOutputPath )
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "ffmpeg", // Or the full path to the ffmpeg executable
            Arguments = $"-i \"{videoPath}\" -i \"{convertedThumbnailPath}\" -map 0 -map 1 -c copy -disposition:v:1 attached_pic \"{tempOutputPath}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        using Process process = new();
        process.StartInfo = processStartInfo;
        process.Start();
        
        //string output = await process.StandardOutput.ReadToEndAsync();
        //string error = await process.StandardError.ReadToEndAsync();
        
        await process.WaitForExitAsync();

        //Console.WriteLine( output );
        //Console.WriteLine( error );
    }
}