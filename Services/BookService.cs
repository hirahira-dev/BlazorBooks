using BlazorBooks.Data;
using BlazorBooks.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorBooks.Services;

public class BookService(AppDbContext db) : IBookService
{
    /// <summary>
    /// 一覧取得（検索／フィルタ／並び替えに対応）
    /// </summary>
    public async Task<List<Book>> GetListAsync(BookSearchCondition? condition = null)
    {
        // Books テーブルに対するクエリを組み立てる（まだDBには問い合わせない）
        var query = db.Books.AsQueryable();

        // 一覧表示は「読み取り専用」なので、変更追跡を無効にして軽くする
        query = query.AsNoTracking();

        // タイトルの部分一致検索（未入力ならスキップ）
        if (!string.IsNullOrWhiteSpace(condition?.Title))
        {
            query = query.Where(b => b.Title.Contains(condition.Title));
        }

        // 著者の部分一致検索（Author が null の可能性があるため null チェック）
        if (!string.IsNullOrWhiteSpace(condition?.Author))
        {
            query = query.Where(b => b.Author != null && b.Author.Contains(condition.Author));
        }

        // 読了フィルタ
        if (condition?.ReadFilter == ReadFilter.Read)
        {
            query = query.Where(b => b.IsRead);
        }
        else if (condition?.ReadFilter == ReadFilter.Unread)
        {
            query = query.Where(b => !b.IsRead);
        }

        // 並び替え（指定がなければ「新しい順」を既定とする）
        query = condition?.SortOrder == SortOrder.CreatedAtAsc
            ? query.OrderBy(b => b.CreatedAt)
            : query.OrderByDescending(b => b.CreatedAt);

        // ここで初めてDBへ問い合わせる（SQLが実行される）
        return await query.ToListAsync();
    }

    /// <summary>
    /// ID指定で1件取得
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<Book?> GetByIdAsync(int id)
    {
        return await db.Books.FindAsync(id);
    }

    /// <summary>
    /// 新規作成
    /// </summary>
    /// <param name="book"></param>
    /// <returns></returns>
    public async Task<Book> CreateAsync(Book book)
    {
        book.CreatedAt = DateTime.Now;

        db.Books.Add(book);
        await db.SaveChangesAsync();

        return book;
    }

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="book"></param>
    /// <returns></returns>
    public async Task UpdateAsync(Book book)
    {
        // 既に追跡されているエンティティなので、SaveChangesAsyncを呼ぶだけで更新される
        await db.SaveChangesAsync();
    }

    /// <summary>
    /// 削除
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task DeleteAsync(int id)
    {
        // 対象データ存在しない場合は何もしない
        var book = await db.Books.FindAsync(id);
        if (book is null) return;

        db.Books.Remove(book);
        await db.SaveChangesAsync();
    }
}
