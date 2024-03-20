namespace dlTubeAvalonia.Models;

public sealed record ArchiveSearchRequest
{
    public string? CategoryName { get; init; }
    public string? StreamType { get; init; }
    public string? SortType { get; init; }
    public int ResultCount { get; init; } = 10;
}