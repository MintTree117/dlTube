using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace dlTubeAvalonia.Services;

public sealed class FFmpegChecker
{
    readonly ILogger<FFmpegChecker>? _logger = Program.ServiceProvider.GetService<ILogger<FFmpegChecker>>();
    
    bool? _isFFmpegInstalled;

    public async Task<bool> CheckFFmpegInstallationAsync()
    {
        _isFFmpegInstalled ??= await IsFFmpegInstalledAsync( _logger );
        return _isFFmpegInstalled.Value;
    }
    static async Task<bool> IsFFmpegInstalledAsync( ILogger<FFmpegChecker>? logger )
    {
        Process? process = null;
        
        try
        {
            // FFmpeg process
            process = new Process();
            process.StartInfo.FileName = "ffmpeg";
            process.StartInfo.Arguments = "-version";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false; // TODO: Test if need true
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
        catch ( Exception e )
        {
            logger?.LogError( e, e.Message );
            return false;
        }
        finally
        {
            if ( process is not null )
            {
                if ( !process.HasExited )
                      process.Kill();
                
                process.Dispose();
            }
        }
    }
}