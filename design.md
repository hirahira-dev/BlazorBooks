# Blazor 書籍管理アプリ design.md

## 0. このドキュメントの目的

このドキュメントは、Blazor本 Part3/Part4 で解説するサンプルアプリ
「書籍管理アプリ（Book CRUD アプリ）」 の設計方針をまとめたものです。

主な目的は以下の通り：

- Codex（VS Code 拡張）のような AI コーディングエージェントが実装しやすいように、仕様・構成・制約を明文化する
- 実装と書籍の内容がずれないように、**ブック全体の設計の"軸"**を固定する

## 1. アプリ概要

### 1.1 コンセプト

- 自分の読んだ本・読みたい本を管理する 個人用の書籍管理アプリ
- ドメインはシンプルだが、**実務的な構成（サービス層・バリデーション・一覧画面の改善など）**を丁寧に組み込む
- Part3 では ローカル開発用の SQLite + Blazor Web App による CRUD に集中する
- Part4 で Azure SQL / App Service / Entra ID 認証などクラウド関連を追加する

### 1.2 技術スタック

- .NET 10
- Blazor Web App（サーバー側レンダリング + インタラクティブ）
- ASP.NET Core（最小ホスティング）
- Entity Framework Core（SQLite → 将来的に Azure SQL）
- 開発環境：VS Code + Codex（AIコード支援）想定（書籍ではVisual Studio 2026を想定。AI支援は使わない）
- Blazor Web App（サーバー側レンダリング + インタラクティブ、InteractiveServer モードを使用）
  - このサンプルでは WebAssembly モードは使用しない

## 2. スコープ

### 2.1 Part3 のスコープ（この design.md で主に対象とする）

**書籍管理アプリの基本 CRUD 機能**

- 書籍一覧（READ）
- 新規登録（CREATE）
- 編集（UPDATE）
- 削除（DELETE）

**その他の重要な要件**

- SQLite + EF Core を使ったデータ永続化
- サービス層（IBookService / BookService）による責務分離
- EditForm + DataAnnotations バリデーション
- 一覧画面の UX 改善（検索・フィルタ・並び替え）
- 軽いリファクタリング（フォーム共通化・フォルダ整理）

### 2.2 Part4 で扱う予定（本アプリの将来拡張）

※設計上は意識しておくが、実装は Part4 側で行う。

- Azure SQL への移行（UseSqlite → UseSqlServer）
- Azure App Service へのデプロイ
- Entra ID による認証追加
- ログインユーザー単位のデータ絞り込み（自分の書籍のみ表示など）

## 2.3 あえて採用しないもの（設計上の制約）

このサンプルアプリは「入門～初級者向けの Blazor/EF Core 学習」を目的とするため、
以下のような高度なパターンやライブラリは採用しない。

- Repository パターン / Unit of Work / CQRS / DDD などの複雑なアーキテクチャ
- MediatR や AutoMapper などの外部ライブラリ
- MudBlazor 等のコンポーネントライブラリ（必要なら別記事で扱う）

UI 層は IBookService を介して DB とやり取りする、シンプルなレイヤ構成にとどめる。

## 3. 機能要件

### 3.1 書籍一覧

登録済みの書籍をテーブル形式で一覧表示する

**表示項目**

- タイトル
- 著者
- 読了フラグ（チェックまたはアイコン）
- 登録日時

**一覧から以下の操作ができる**

- 新規作成画面への遷移
- 編集画面への遷移
- 削除操作（ボタン）

### 3.2 書籍新規登録

以下の項目を入力して書籍を登録できる

- タイトル（必須）
- 著者（任意）
- メモ（任意・複数行）
- 読了フラグ（bool）

**バリデーション**

- タイトル：必須、最大文字数（例：100文字）など
- 著者・メモ：最大文字数のみ（厳密すぎない範囲）

登録完了後は一覧画面に戻る

### 3.3 書籍編集

- 一覧から選択した書籍の内容を編集できる
- URL に ID を含める（例：/books/edit/{id:int}）
- 初期表示時に DB から該当の書籍を読み込む
- フォーム内容は新規登録とほぼ共通

### 3.4 書籍削除

- 編集画面から削除ボタンを押して削除
- 簡易な確認ダイアログ（confirm() 相当）を表示
- 削除後は一覧画面に戻る
- Part3 では物理削除でよい（論理削除はコラムで触れる程度）

### 3.5 一覧画面の UX 改善（Part3 後半）

- タイトル・著者名での検索
- 読了・未読でのフィルタリング
- 登録日の昇順・降順での並び替え
- 一覧1行をコンポーネント化（BookListRow.razor）して再利用性と見通しを向上
- 行クリックで編集画面に遷移できるようにする（任意）

**実装方針**

一覧の検索・フィルタ・ソートは、IBookService 内で LINQ を使って実装する。

- UI からは「検索キーワード」「読了フラグ」「ソート順」などのパラメータを渡す
- BookService 内で IQueryable<Book> に Where / OrderBy を適用する
- 今回は件数が少ない前提のため、パフォーマンス最適化は行わない

## 4. 非機能要件（ライト）

**対象ブラウザ**：最新のモダンブラウザ（Chrome/Edge 最新版程度）

**パフォーマンス**

- 今回は少数データ前提（学習用）なので性能チューニングは最小限

**セキュリティ**

- Part3 時点では認証なしのローカル動作前提
- 外部公開は Part4 での認証・デプロイを前提とする

**ログ**

- ASP.NET Core の標準ログに任せ、特別なログ実装は不要（必要なら Part4 で強化）

## 5. 画面設計（概要）

### 5.1 ルーティング

- `/books`：書籍一覧ページ
- `/books/create`：新規作成ページ
- `/books/edit/{id:int}`：編集ページ

将来的に Entra ID を導入した際は、[Authorize] 属性やポリシーを付加する想定。

### 5.2 ページ構成

- `BooksList.razor`（一覧）
- `BookCreate.razor`（新規作成）
- `BookEdit.razor`（編集・削除）

### 5.3 コンポーネント候補

**BookForm.razor**

新規作成・編集で共通に使うフォームコンポーネント

**BookListRow.razor**

一覧の1行分のコンポーネント（タイトル・著者・読了・操作ボタンなど）

## 6. ドメインモデル

### 6.1 Book エンティティ

```csharp
public class Book
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Author { get; set; }

    public string? Memo { get; set; }

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }
}
```

**重要な注記**

- CreatedAt の扱い
  - Part3 では `DateTime.Now`（ローカル時刻）で設定する
  - Part4 で Azure SQL / 本番運用を扱う際に、UTC を前提とした扱いに変更し、その理由や考え方を解説する（学習ネタとする）
- 将来的な拡張例（Part3 では必須ではないが設計上は想定）
  - 評価（Rating）
  - カテゴリ（enum or 別テーブル）
  - CreatedBy（Entra ID のユーザーIDと紐付ける）

## 7. アーキテクチャ設計

### 7.1 全体構成

- Blazor Web App（Server 互換）のプロジェクトを 1 つ
- 同一プロジェクト内に以下を配置
  - UI（Razor コンポーネント）
  - ドメインモデル
  - EF Core DbContext
  - サービス層（アプリケーションロジック）

### 7.2 フォルダ構成（案）

プロジェクト直下に以下のように配置する。

```
/Pages
  /Books
    - BooksList.razor
    - BookCreate.razor
    - BookEdit.razor

/Components
  /Books
    - BookForm.razor
    - BookListRow.razor

/Models
  - Book.cs

/Data
  - AppDbContext.cs

/Services
  - IBookService.cs
  - BookService.cs
```

Codex にはこの構成を前提としてファイル生成を依頼する。

### 7.3 DI とサービス層

**DI 登録（Program.cs）の想定**

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=app.db"));

builder.Services.AddScoped<IBookService, BookService>();
```

**サービス層の責務**

- DB アクセス（EF Core）
- 検索・フィルタ・ソートなど一覧ロジック
- 例外の簡易ラップ（必要に応じて）

UI（Razor）側は極力 IBookService だけを見るようにし、
AppDbContext に直接依存しないようにする（解説は Part3 後半）。

## 8. 永続化設計

### 8.1 SQLite（Part3）

- ローカル開発用 DB として SQLite を使用
- 接続文字列：`Data Source=app.db`
- データベースはプロジェクトルートまたは AppData 相当のディレクトリに作成する想定

**マイグレーション**

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 8.2 Azure SQL（Part4）

Part4 で扱う内容だが、設計上は以下を想定：

- `UseSqlServer(connectionString)` に切り替えるだけで動作するように設計
- Book モデルや AppDbContext の構造は SQLite/Azure SQL 共通
- 環境ごとの接続文字列は `appsettings.*.json` や App Service の構成で切り替え

## 9. バリデーション・エラーハンドリング

### 9.1 入力バリデーション

- DataAnnotations を基本とする
  - `[Required]`
  - `[StringLength(100)]` など
- クライアント側：EditForm + ValidationSummary/ValidationMessage
- サーバー側：EF Core の制約違反が発生した場合は、
  例外をそのまま投げず簡易なメッセージに変換するか、Part3 では最小限の扱いにとどめる

### 9.2 エラーハンドリング

- Part3 では、致命的なエラー時はエラー画面に遷移してもよい
- 細かい例外分類・ログ出力などは Part4（運用・クラウド）で強化

## 10. コーディング規約・命名

C# の一般的な命名規約に従う

- クラス名・メソッド名：PascalCase
- ローカル変数：camelCase
- Razor コンポーネント名は基本 PascalCase.razor
- サービスインターフェースは `I{名前}Service` 形式
  - 例：`IBookService`
- 非同期メソッドは Async サフィックスを付ける
  - 例：`GetBooksAsync`

Codex に指示する際は、これらを守るようプロンプトで補足する。

## 11. 開発フロー（Codex 向けメモ）

Codex に投げるタスクを整理する際、ざっくり以下の順を推奨：

1. プロジェクト作成 (.NET 10 Blazor Web App)
2. Book モデルと AppDbContext の作成
3. SQLite 設定とマイグレーション
4. 一覧ページ（最小構成）の作成
5. 新規作成ページの作成（EditForm + Validation）
6. 編集・削除ページの作成
7. CRUD 全体の動作確認
8. サービス層の導入（IBookService / BookService）
9. 一覧ページの改善（検索・フィルタ・並び替え・コンポーネント化）
10. 軽いリファクタリング（共通フォーム化・フォルダ整理）

書籍の Part3 の章順ともほぼ一致させておく。

## 12. 今後の TODO / メモ

**方針決定が必要な項目**

- CreatedAt のタイムゾーン方針（UTC固定かローカルか）をどこかで明示する

**将来的な機能拡張**

- カテゴリ・評価などの追加
- ログインユーザーごとの蔵書管理（Entra ID 連携後）

**デザイン・UX**

- デザイン（CSS/レイアウト）は最小限とし、見た目より機能と構成の学習を優先する
