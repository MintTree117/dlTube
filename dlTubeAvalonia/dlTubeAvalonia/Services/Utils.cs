using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using dlTubeAvalonia.Models;

namespace dlTubeAvalonia.Services;

public static class Utils
{
    public static async Task<byte[]> GetImageBytes( MemoryStream stream )
    {
        byte[] bytes = stream.ToArray();
        await stream.DisposeAsync();
        return bytes;
    }
    public static async Task<Bitmap?> GetImageBitmap( string imageUrl, HttpService http )
    {
        ServiceReply<MemoryStream?> reply = await http.TryGetImageStream( imageUrl );

        if ( !reply.Success || reply.Data is null )
        {
            Console.WriteLine( reply.PrintDetails() );
            return null;   
        }
        
        try
        {
            reply.Data.Position = 0; // Never forget again!
            Bitmap map = new( reply.Data );
            await reply.Data.DisposeAsync();
            return map;
        }
        catch ( Exception e )
        {
            Console.WriteLine( e + e.Message );
            await reply.Data.DisposeAsync();
            return null;
        }
    }
}