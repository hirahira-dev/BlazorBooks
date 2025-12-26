using System.ComponentModel.DataAnnotations;

namespace BlazorBooks.Models;

public class Book
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Author { get; set; }

    [StringLength(1000)]
    public string? Memo { get; set; }

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}