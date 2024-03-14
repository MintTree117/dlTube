using System.Collections.Generic;
using System.Threading.Tasks;
using dlTubeAvalonia.Enums;

namespace dlTubeAvalonia.Services;

public interface IYoutubeService
{
    string? VideoName { get; }
    string? VideoThumbnail { get; }
    
    Task<bool> GetStreamManifest();
    Task<byte[]?> GetThumbnailBytes();
    List<string> GetStreamInfo( StreamType steamType );
    Task<bool> Download( string filepath, StreamType type, int quality );
}