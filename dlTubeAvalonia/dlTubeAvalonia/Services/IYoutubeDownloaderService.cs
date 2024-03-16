using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dlTubeAvalonia.Enums;

namespace dlTubeAvalonia.Services;

public interface IYoutubeDownloaderService
{
    string? VideoName { get; }
    TimeSpan? VideoDuration { get; }
    string? VideoThumbnail { get; }
    
    Task<bool> GetStreamManifest();
    Task<byte[]?> GetThumbnailBytes();
    List<string> GetStreamInfo( StreamType steamType );
    Task<bool> Download( string filepath, StreamType type, int quality );
}