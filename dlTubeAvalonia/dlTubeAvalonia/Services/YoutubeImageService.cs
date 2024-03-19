using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace dlTubeAvalonia.Services;

public sealed class YoutubeImageService
{
    public static async Task<byte[]?> LoadImageBytesFromUrlAsync( string imageUrl, HttpClient client )
    {
        try
        {
            HttpResponseMessage response = await client.GetAsync( imageUrl );

            if ( response.IsSuccessStatusCode )
            {
                await using Stream stream = await response.Content.ReadAsStreamAsync();
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync( memoryStream ); // Copy the stream to a MemoryStream
                return memoryStream.ToArray();
            }
        }
        catch ( Exception ex )
        {
            Console.WriteLine( $"Failed to load image from URL: {ex.Message}" );
        }

        return null;
    }
}