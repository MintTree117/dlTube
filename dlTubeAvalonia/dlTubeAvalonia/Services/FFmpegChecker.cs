using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace dlTubeAvalonia.Services;

public sealed class FFmpegChecker : BaseService
{
    bool? _isFFmpegInstalled;

    public async Task<bool> CheckFFmpegInstallationAsync()
    {
        _isFFmpegInstalled ??= await IsFFmpegInstalledAsync();
        return _isFFmpegInstalled.Value;
    }
    async Task<bool> IsFFmpegInstalledAsync()
    {
        // FFmpeg process
        using Process process = new();
        process.StartInfo.FileName = "ffmpeg";
        process.StartInfo.Arguments = "-version";
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.UseShellExecute = false; // TODO: Test if need true
        process.StartInfo.CreateNoWindow = true;
        
        try
        {
            process.Start();

            // Read the output to ensure the command was executed
            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();
            
            await process.WaitForExitAsync();

            bool success = process.ExitCode == 0 && !string.IsNullOrWhiteSpace( output );

            if ( !success )
                Logger.LogWithConsole( $"FFmpeg initialization fail: {error}" );

            return success;
        }
        catch ( Exception e )
        {
            Logger.LogWithConsole( ExString( e ) );
            return false;
        }
        finally
        {
            if ( !process.HasExited )
                process.Kill();
        }
    }
}