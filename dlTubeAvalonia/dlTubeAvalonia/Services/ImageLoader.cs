using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Avalonia.Media.Imaging;
using dlTubeAvalonia.Models;

namespace dlTubeAvalonia.Services;

public sealed class ImageLoader
{
    readonly ILogger<ImageLoader>? _logger = Program.ServiceProvider.GetService<ILogger<ImageLoader>>();
    readonly HttpController _http;

    public ImageLoader( HttpController http )
    {
        _http = http;
    }
    
    public async Task<byte[]> GetImageBytes( MemoryStream stream )
    {
        byte[] bytes = stream.ToArray();
        await stream.DisposeAsync();
        return bytes;
    }
    public async Task<ServiceReply<MemoryStream?>> GetImageStream( string imageUrl )
    {
        ServiceReply<Stream?> reply = await _http.TryGetStream( imageUrl );

        if ( !reply.Success || reply.Data is null )
        {
            Console.WriteLine( "Fail get http stream" );
            return new ServiceReply<MemoryStream?>( reply.ErrorType, reply.Message );   
        }

        MemoryStream memoryStream = new();
        await reply.Data.CopyToAsync( memoryStream ); // Copy the stream to a MemoryStream
        await reply.Data.DisposeAsync();
        
        return new ServiceReply<MemoryStream?>( memoryStream );
    }
    public async Task<ServiceReply<Bitmap?>> GetImageBitmap( string imageUrl )
    {
        ServiceReply<Stream?> reply = await _http.TryGetStream( imageUrl );

        if ( !reply.Success || reply.Data is null )
        {
            Console.WriteLine( "Fail get http stream" );
            return new ServiceReply<Bitmap?>( reply.ErrorType, reply.Message );
        }
        
        /*ServiceReply<MemoryStream?> reply = await GetImageStream( imageUrl );

        if ( !reply.Success || reply.Data is null )
        {
            if ( reply.Data is not null )
                await reply.Data.DisposeAsync();
            
            Console.WriteLine( "Fail get image bitmap" );
            return new ServiceReply<Bitmap?>( reply.ErrorType, reply.Message );   
        }*/
        
        Stream mStream = reply.Data;
        
        try
        {
            mStream.Position = 0; // Never forget again!
            
            if (!mStream.CanRead)
                Console.WriteLine("Cant read");
            Bitmap map = new( mStream );
            await mStream.DisposeAsync();
            return new ServiceReply<Bitmap?>( map );
        }
        catch ( Exception e ) // EXCEPTION HERE
        {
            _logger?.LogError( e, e.Message );
            Console.WriteLine( e + e.Message );
            await mStream.DisposeAsync();
            return new ServiceReply<Bitmap?>( ServiceErrorType.AppError, "Fail get image stream" );
        }
    }
}