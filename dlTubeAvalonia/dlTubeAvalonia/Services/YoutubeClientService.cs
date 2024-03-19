using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using YoutubeExplode;

namespace dlTubeAvalonia.Services;

public sealed class YoutubeClientService
{
    public YoutubeClient? YoutubeClient { get; init; }

    public YoutubeClientService()
    {
        var logger = Program.ServiceProvider.GetService<ILogger<YoutubeClientService>>();

        try
        {
            YoutubeClient = new YoutubeClient();
        }
        catch ( Exception e )
        {
            logger?.LogError( e, e.Message );
        }
    }
}