using BlazorBooksPoc.Data;
using BlazorBooksPoc.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorBooksPoc.Services;

public class BookService : IBookService
{
    private readonly AppDbContext _context;

    public BookService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Book>> GetListAsync(
        string? title = null,
        string? author = null,
        ReadFilter readFilter = ReadFilter.All,
        SortOrder sortOrder = SortOrder.CreatedAtDesc)
    {
        IQueryable<Book> query = _context.Books.AsQueryable();

        if (!string.IsNullOrWhiteSpace(title))
        {
            query = query.Where(b => b.Title.Contains(title));
        }

        if (!string.IsNullOrWhiteSpace(author))
        {
            query = query.Where(b => b.Author != null && b.Author.Contains(author));
        }

        if (readFilter != ReadFilter.All)
        {
            query = readFilter switch
            {
                ReadFilter.Read => query.Where(b => b.IsRead),
                ReadFilter.Unread => query.Where(b => !b.IsRead),
                _ => query
            };
        }

        query = sortOrder switch
        {
            SortOrder.CreatedAtAsc => query.OrderBy(b => b.CreatedAt),
            _ => query.OrderByDescending(b => b.CreatedAt)
        };

        return await query.ToListAsync();
    }

    public async Task<Book?> GetByIdAsync(int id)
    {
        return await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task CreateAsync(Book book)
    {
        book.CreatedAt = DateTime.Now;
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Book book)
    {
        var existing = await _context.Books.FirstOrDefaultAsync(b => b.Id == book.Id);
        if (existing == null)
        {
            return;
        }

        existing.Title = book.Title;
        existing.Author = book.Author;
        existing.Memo = book.Memo;
        existing.IsRead = book.IsRead;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var existing = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
        if (existing == null)
        {
            return;
        }

        _context.Books.Remove(existing);
        await _context.SaveChangesAsync();
    }
}
