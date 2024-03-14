using System.Threading.Tasks;

namespace dlTubeAvalonia.Services;

public interface IFFmpegService
{
    Task<bool> CheckFFmpegInstallationAsync();
}