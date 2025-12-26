using BlazorBooks.Models;

namespace BlazorBooks.Services;

public interface IBookService
{
    /// <summary>
    /// 一覧を取得（condition が null なら条件指定なし）
    /// </summary>
    /// <returns></returns>
    Task<List<Book>> GetListAsync(BookSearchCondition? condition = null);

    /// <summary>
    /// IDを指定して1件取得
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<Book?> GetByIdAsync(int id);

    /// <summary>
    /// 新規作成
    /// </summary>
    /// <param name="book"></param>
    /// <returns></returns>
    Task<Book> CreateAsync(Book book);

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="book"></param>
    /// <returns></returns>
    Task UpdateAsync(Book book);

    /// <summary>
    /// 削除
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task DeleteAsync(int id);
}
