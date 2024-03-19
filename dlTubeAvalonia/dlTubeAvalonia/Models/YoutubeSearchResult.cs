using Avalonia.Media.Imaging;

namespace dlTubeAvalonia.Models;

public sealed record YoutubeSearchResult
{
    public string Title { get; init; } = string.Empty;
    public string Duration { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public Bitmap? Image { get; init; }
}