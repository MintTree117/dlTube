using System.Collections.Generic;

namespace dlTubeAvalonia.Models;

public sealed record AppSettingsModel
{
    public string ApiKey { get; init; } = string.Empty;
    public string DownloadLocation { get; init; } = string.Empty;
    public string SelectedBackgroundImage { get; init; } = string.Empty;

    public const string TransparentBackgroundKeyword = "Transparent";
    public const string DefaultBackgroundImage = "dlTubeAvalonia.Assets.space.jpg";
    public static IReadOnlyList<string> BackgroundImages { get; } = [
        "dlTubeAvalonia.Assets.space.jpg",
        "dlTubeAvalonia.Assets.dsotm.jpg"
    ];
}