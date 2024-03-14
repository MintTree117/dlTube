namespace dlTubeAvalonia.Models;

public sealed record ArchiveItem
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string ImageUrl { get; init; } = string.Empty;
}