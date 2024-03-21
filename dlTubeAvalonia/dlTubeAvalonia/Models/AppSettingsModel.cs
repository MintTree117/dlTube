namespace dlTubeAvalonia.Models;

public sealed record AppSettingsModel
{
    public string ApiKey { get; init; } = string.Empty;
    public string DownloadLocation { get; init; } = string.Empty;
    public string BackgroundImageName { get; init; } = string.Empty;
}