using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace dlTubeAvalonia.Services;

public sealed class FFmpegService( ILogger<FFmpegService>? _logger )
{
    bool? _isFFmpegInstalled;

    public async Task<bool> CheckFFmpegInstallationAsync()
    {
        _isFFmpegInstalled ??= await IsFFmpegInstalledAsync( _logger );
        return _isFFmpegInstalled.Value;
    }
    static async Task<bool> IsFFmpegInstalledAsync( ILogger<FFmpegService>? logger )
    {
        try
        {
            // FFmpeg process
            using Process process = new();
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
            
            bool success = process.ExitCode == 0 && !string.IsNullOrWhiteSpace( output );

            if ( !success )
                logger?.LogError( $"FFmpeg initialization fail: {error}" );

            return success;
        }
        catch( Exception e )
        {
            logger?.LogError( e, e.Message );
            return false;
        }
    }
}