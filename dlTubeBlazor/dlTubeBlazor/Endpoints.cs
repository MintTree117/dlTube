using dlTubeBlazor.Client.Dtos;
using dlTubeBlazor.Client.Enums;

namespace dlTubeBlazor;

public static class Endpoints
{
    public static void MapReadMenuEndpoints( this IEndpointRouteBuilder app )
    {
        app.MapGet( "api/stream/info", async ( StreamType type, YoutubeBrowser yt ) => 
        {
            if ( !await yt.TryInitialize() )
                return Results.Problem( "Failed to initialize youtube client!" );

            StreamInfo? info = await yt.GetStreamInfo( type );

            return info is not null
                ? Results.Ok( info )
                : Results.NotFound();
        } );

        app.MapGet( "api/stream/download", async ( string url, StreamType type, int quality, YoutubeStreamer yt ) =>
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