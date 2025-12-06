using BlazorBooksPoc.Models;

namespace BlazorBooksPoc.Services;

public interface IBookService
{
    Task<List<Book>> GetListAsync(string? title = null, string? author = null, bool? isRead = null, string? sortOrder = null);
    Task<Book?> GetByIdAsync(int id);
    Task CreateAsync(Book book);
    Task UpdateAsync(Book book);
    Task DeleteAsync(int id);
}
