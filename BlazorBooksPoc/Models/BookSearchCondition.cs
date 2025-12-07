namespace BlazorBooksPoc.Models;

public class BookSearchCondition
{
    public string? Title { get; set; }

    public string? Author { get; set; }

    public ReadFilter ReadFilter { get; set; } = ReadFilter.All;

    public SortOrder SortOrder { get; set; } = SortOrder.CreatedAtDesc;
}
