namespace dlTubeBlazor.Client.Dtos;

public sealed record StreamInfo
{
    public string Title { get; init; } = string.Empty;
    public string Duration { get; init; } = string.Empty;
    public string ImageUrl { get; init; } = string.Empty;

    public List<string> Qualities { get; init; } = [ ];
}