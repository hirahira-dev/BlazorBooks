namespace BlazorBooks.Models;

/// <summary>
/// 一覧画面の検索条件をまとめるモデル
/// </summary>
public class BookSearchCondition
{
    // タイトルの部分一致検索（未入力なら条件なし）
    public string? Title { get; set; }

    // 著者の部分一致検索（未入力なら条件なし）
    public string? Author { get; set; }

    // 読了状態のフィルタ（All / Read / Unread）
    public ReadFilter ReadFilter { get; set; } = ReadFilter.All;

    // 並び順（登録日の昇順 / 降順）
    public SortOrder SortOrder { get; set; } = SortOrder.CreatedAtDesc;
}

/// <summary>
/// 読了フィルタの選択肢
/// </summary>
public enum ReadFilter
{
    // すべて
    All = 0,

    // 読了のみ
    Read = 1,

    // 未読のみ
    Unread = 2,
}

/// <summary>
/// 並び順の選択肢
/// </summary>
public enum SortOrder
{
    // 登録日：新しい順
    CreatedAtDesc = 0,

    // 登録日：古い順
    CreatedAtAsc = 1,
}