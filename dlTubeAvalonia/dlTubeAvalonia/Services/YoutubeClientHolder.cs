using YoutubeExplode;

namespace dlTubeAvalonia.Services;

public sealed class YoutubeClientHolder
{
    public YoutubeClient YoutubeClient { get; init; } = new();
}