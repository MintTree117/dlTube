using System.Diagnostics;
using System.Threading.Tasks;

namespace dlTubeAvalonia.Services;

public sealed class FFmpegService : IFFmpegService
{
    bool? _isFFmpegInstalled;

    public async Task<bool> CheckFFmpegInstallationAsync()
    {
        _isFFmpegInstalled ??= await IsFFmpegInstalledAsync();
        return _isFFmpegInstalled.Value;
    }

    static async Task<bool> IsFFmpegInstalledAsync()
    {
        try
        {
            using Process process = new Process();
            process.StartInfo.FileName = "ffmpeg";
            process.StartInfo.Arguments = "-version";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();

            // Read the output to ensure the command was executed
            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            // A simple check. If ffmpeg is not recognized, an error is typically thrown.
            // Additionally, you might check if the output contains version information.
            return process.ExitCode == 0 && !string.IsNullOrWhiteSpace( output );
        }
        catch
        {
            // An exception is likely thrown if ffmpeg is not installed or not in the PATH
            return false;
        }
    }
}