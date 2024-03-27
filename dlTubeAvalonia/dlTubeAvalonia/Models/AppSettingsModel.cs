using System.Collections.Generic;

namespace dlTubeAvalonia.Models;

public sealed record AppSettingsModel
{
    public string ApiKey { get; init; } = string.Empty;
    public string DownloadLocation { get; init; } = string.Empty;
    public string FFmpegFilepath { get; init; } = string.Empty;
    public string SelectedBackgroundImage { get; init; } = string.Empty;

    public const string TransparentBackgroundKeyword = "Transparent";
    public const string DefaultBackgroundImage = "dlTubeAvalonia.Assets.Backgrounds.forest.jpg";
    public static IReadOnlyList<string> BackgroundImages { get; } = [
        TransparentBackgroundKeyword,
        "dlTubeAvalonia.Assets.Backgrounds.space.jpg",
        "dlTubeAvalonia.Assets.Backgrounds.night_lights.jpg",
        "dlTubeAvalonia.Assets.Backgrounds.forest.jpg",
        "dlTubeAvalonia.Assets.Backgrounds.concert.jpg",
    ];
}