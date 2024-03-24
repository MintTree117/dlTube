using dlTubeBlazor.Client;
using dlTubeBlazor.Client.Dtos;
using dlTubeBlazor.Client.Enums;

namespace dlTubeBlazor.Features.Youtube;

public static class YoutubeEndpoints
{
    public static void MapYoutubeEndpoints( this IEndpointRouteBuilder app )
    {
        app.MapGet( HttpConsts.GetStreamInfo, async ( string url, StreamType type, YoutubeBrowser yt ) => 
        {
            if ( !await yt.TryInitialize( url ) )
                return Results.Problem( "Failed to initialize youtube client!" );

            StreamInfo? info = await yt.GetStreamInfo( type );

            Console.WriteLine( url );
            Console.WriteLine( info?.Title );

            return info is not null
                ? Results.Ok( info )
                : Results.NotFound();
        } );

        app.MapGet( HttpConsts.GetStreamDownload, async ( string url, StreamType type, int quality, YoutubeStreamer yt ) =>
        {
            if ( !await yt.TryInitialize( url ) )
                return Results.Problem( "Failed to initialize youtube client!" );

            Stream? stream = await yt.Stream( type, quality );

            return stream is not null
                ? Results.File( stream )
                : Results.NotFound();
        } );
    }
}