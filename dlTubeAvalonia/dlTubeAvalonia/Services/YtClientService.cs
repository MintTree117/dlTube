using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using YoutubeExplode;

namespace dlTubeAvalonia.Services;

public sealed class YtClientService
{
    public YoutubeClient? YoutubeClient { get; init; }

    public YtClientService()
    {
        var logger = Program.ServiceProvider.GetService<ILogger<YtClientService>>();

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