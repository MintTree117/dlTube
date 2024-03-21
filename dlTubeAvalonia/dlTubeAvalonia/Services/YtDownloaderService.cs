using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using dlTubeAvalonia.Enums;
using dlTubeAvalonia.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace dlTubeAvalonia.Services;

public sealed class YtDownloaderService( string videoUrl )
{
    // Constants
    const string YoutubeClientFailMessage = "Failed to initialize YoutubeClientService!";
    const string TempThumbnailFileName = "thumbnail.jpg";
    const string TempThumbnailConvertedFileName = "thumbnail_converted.jpg";
    
    // Services
    readonly ILogger<YtDownloaderService>? _logger = Program.ServiceProvider.GetService<ILogger<YtDownloaderService>>();
    readonly YtClientService? _youtubeService = Program.ServiceProvider.GetService<YtClientService>();
    readonly FFmpegService? _ffmpegService = Program.ServiceProvider.GetService<FFmpegService>();
    readonly HttpService? _httpService = Program.ServiceProvider.GetService<HttpService>();
    
    // Video Url From Constructor
    readonly string _videoUrl = videoUrl;
    
    // Stream Data
    public string? VideoName => _video?.Title;
    public TimeSpan? VideoDuration => _video?.Duration;
    public string? VideoImage => _video?.Thumbnails.FirstOrDefault()?.Url;
    public byte[]? ThumbnailBytes => _thumbnailBytes;
    StreamManifest? _streamManifest;
    Video? _video;
    byte[]? _thumbnailBytes;
    bool _hasFFmeg;
    
    // Streams
    List<MuxedStreamInfo> _mixedStreams = [ ];
    List<AudioOnlyStreamInfo> _audioStreams = [ ];
    List<VideoOnlyStreamInfo> _videoStreams = [ ];
    
    // Stream Qualities
    List<string>? _mixedSteamQualities;
    List<string>? _audioSteamQualities;
    List<string>? _videoSteamQualities;
    
    // Public Methods
    public async Task<ServiceReply<bool>> TryInitialize()
    {
        // Youtube Client Service
        if ( _youtubeService?.YoutubeClient is null )
        {
            _logger?.LogError( YoutubeClientFailMessage );
            return new ServiceReply<bool>( ServiceErrorType.AppError, YoutubeClientFailMessage );
        }
        
        try
        {
            _streamManifest = await _youtubeService.YoutubeClient.Videos.Streams.GetManifestAsync( _videoUrl );
            _video = await _youtubeService.YoutubeClient.Videos.GetAsync( _videoUrl );
            _hasFFmeg = _ffmpegService is not null && await _ffmpegService.CheckFFmpegInstallationAsync();
            
            // Get Video Image Data
            if ( _hasFFmeg )
                _thumbnailBytes = await GetThumbnailBytes();
            
            return _streamManifest is not null && _video is not null
                ? new ServiceReply<bool>( true )
                : new ServiceReply<bool>( ServiceErrorType.NotFound );
        }
        catch ( Exception e )
        {
            _logger?.LogError( e, e.Message );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }
    }
    public async Task<List<string>> GetStreamInfo( StreamType steamType )
    {
        return steamType switch
        {
            StreamType.Mixed => await GetMixedStreams(),
            StreamType.Audio => await GetAudioStreams(),
            StreamType.Video => await GetVideoStreams(),
            _ => throw new Exception( "Invalid Stream Type!" )
        };
    }
    public async Task<ServiceReply<bool>> Download( string filepath, StreamType type, int qualityIndex )
    {
        try
        {
            IStreamInfo streamInfo = type switch
            {
                StreamType.Mixed => _mixedStreams[ qualityIndex ],
                StreamType.Audio => _audioStreams[ qualityIndex ],
                StreamType.Video => _videoStreams[ qualityIndex ],
                _ => throw new ArgumentOutOfRangeException( nameof( type ), type, null )
            };

            //if ( streamInfo is null )
                //return new ApiReply<bool>( ServiceErrorType.NotFound );

            string path = ConstructDownloadPath( filepath, streamInfo.Container.Name );
            
            await _youtubeService!.YoutubeClient!.Videos.Streams.DownloadAsync( streamInfo, path );

            if ( _hasFFmeg )
                await AddImage( path );
            
            return new ServiceReply<bool>( true );
        }
        catch ( Exception e )
        {
            Console.WriteLine( e + e.Message );
            return new ServiceReply<bool>( ServiceErrorType.ServerError );
        }
    }

    // Get Streams
    async Task<List<string>> GetMixedStreams()
    {
        return await Task.Run( () => {
            if ( _mixedSteamQualities is not null )
                return _mixedSteamQualities;

            _mixedStreams = _streamManifest!.GetMuxedStreams().ToList();
            _mixedSteamQualities = [ ];

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
            if ( _audioSteamQualities is not null )
                return _audioSteamQualities;

            _audioStreams = _streamManifest!.GetAudioOnlyStreams().ToList();

            _audioSteamQualities = [ ];

            for ( int i = 0; i < _audioStreams.Count; i++ )
            {
                AudioOnlyStreamInfo stream = _audioStreams[ i ];
                _audioSteamQualities.Add( $"{i + 1} : {stream.Bitrate} bps - {stream.Container}" );
            }

            return _audioSteamQualities;
        } );
    }
    async Task<List<string>> GetVideoStreams()
    {
        return await Task.Run( () => {
            if ( _videoSteamQualities is not null )
                return _videoSteamQualities;

            _videoStreams = _streamManifest!.GetVideoOnlyStreams().ToList();

            _videoSteamQualities = [ ];

            for ( int i = 0; i < _videoStreams.Count; i++ )
            {
                VideoOnlyStreamInfo stream = _videoStreams[ i ];
                _videoSteamQualities.Add( $"{i + 1} : {stream.VideoResolution} px - {stream.Container}" );
            }

            return _videoSteamQualities;
        } );
    }
    
    // Get Download Path
    string ConstructDownloadPath( string outputDirectory, string fileExtension )
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
    
    // Image
    async Task<byte[]?> GetThumbnailBytes()
    {
        if ( _thumbnailBytes is not null || _video?.Thumbnails[ 0 ].Url is null )
            return _thumbnailBytes;

        if ( _httpService is null )
            return Array.Empty<byte>();

        ServiceReply<MemoryStream?> reply = await _httpService.TryGetImageStream( _video.Thumbnails[ 0 ].Url );

        if ( !reply.Success || reply.Data is null )
            return null;

        _thumbnailBytes = await Utils.GetImageBytes( reply.Data );
        return _thumbnailBytes;
    }
    async Task AddImage( string videoPath )
    {
        if ( _thumbnailBytes is null || !_hasFFmeg )
            return;

        string tempThumbnailPath = Path.Combine( Path.GetTempPath(), TempThumbnailFileName );
        string tempConvertedThumbnailPath = Path.Combine( Path.GetTempPath(), TempThumbnailConvertedFileName );
        string tempVideoPath = Path.Combine( Path.GetTempPath(), $"video_temp{Path.GetExtension( videoPath )}" );

        try
        {
            await File.WriteAllBytesAsync( tempThumbnailPath, _thumbnailBytes );
            await CreateJpgCopyFFMPEG( tempThumbnailPath, tempConvertedThumbnailPath );
            await CreateVideoWithImageFFMPEG( videoPath, tempConvertedThumbnailPath, tempVideoPath );

            if ( !File.Exists( tempVideoPath ) )
                return;

            File.Delete( videoPath ); // Delete original file
            File.Move( tempVideoPath, videoPath ); // Move the temp file to original path
        }
        catch ( Exception e )
        {
            _logger?.LogError( e, e.Message );
        }
        finally
        {
            if ( File.Exists( tempThumbnailPath ) )
                File.Delete( tempThumbnailPath );
            
            if ( File.Exists( tempConvertedThumbnailPath ) )
                File.Delete( tempConvertedThumbnailPath );
            
            if ( File.Exists( tempVideoPath ) )
                File.Delete( tempVideoPath );
        }
    }
    static async Task CreateJpgCopyFFMPEG( string inputPath, string outputPath )
    {
        ProcessStartInfo conversionProcessStartInfo = new()
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
        ProcessStartInfo processStartInfo = new()
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