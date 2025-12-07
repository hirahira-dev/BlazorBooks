# Blazor 書籍管理アプリ design.md

## 0. このドキュメントの目的

このドキュメントは、Blazor 本 Part3/Part4 のサンプルアプリ「書籍管理アプリ（Book CRUD アプリ）」の最新仕様をまとめる。現在の実装の振る舞いを正しく記録し、今後の拡張やリファクタリングの軸として活用する。

## 1. アプリ概要

### 1.1 コンセプト

- 読んだ本・読みたい本を管理する個人向けの書籍管理アプリ。
- ドメインはシンプルだが、サービス層・バリデーション・一覧画面の検索/フィルタ/並び替えなど実務的な構成を備える。
- 現状はローカル SQLite + Blazor Web App（サーバー側インタラクティブ）で CRUD を完結させる。

### 1.2 技術スタック

- .NET 10 / ASP.NET Core Minimal Hosting
- Blazor Web App（Server インタラクティブのみ。WebAssembly は不使用）
- Entity Framework Core（SQLite。将来 Azure SQL を想定）
- ブートストラップ標準スタイル + シンプルな Razor コンポーネント構成

## 2. スコープ

### 2.1 現在実装済みのスコープ（Part3 相当）

- 書籍の一覧表示（検索/フィルタ/並び替え付き）
- 書籍の新規登録
- 書籍の編集
- 書籍の削除
- サービス層経由のデータアクセスと DataAnnotations バリデーション

### 2.2 将来拡張（Part4 で扱う）

- Azure SQL への移行（`UseSqlServer` への差し替え）
- Azure App Service へのデプロイ
- Entra ID 認証とユーザーごとのデータ絞り込み

### 2.3 採用しないもの（制約）

- Repository / Unit of Work / CQRS / DDD といった複雑なアーキテクチャ
- MediatR・AutoMapper 等の外部ライブラリ
- MudBlazor 等の UI コンポーネントライブラリ（必要に応じて別途検討）

## 3. ドメイン・データモデル

### 3.1 Book エンティティ

- `Id` (int, 主キー)
- `Title` (string, 必須/最大100文字)
- `Author` (string?, 最大100文字)
- `Memo` (string?, 最大1000文字)
- `IsRead` (bool, 読了フラグ)
- `CreatedAt` (DateTime, 作成日時。新規作成時は現在時刻を設定)

> 定義は `Models/Book.cs` を参照。

### 3.2 検索条件

- `Title` / `Author` の部分一致検索
- `ReadFilter`（All/Read/Unread）で読了・未読のフィルタ
- `SortOrder`（CreatedAtDesc/CreatedAtAsc）で作成日の降順・昇順並び替え

> `Models/BookSearchCondition.cs`, `ReadFilter.cs`, `SortOrder.cs` を参照。

## 4. 画面仕様とルーティング

### 4.1 書籍一覧 `/books`

- 画面上部に「新規作成」ボタン。
- 検索フォーム：タイトル/著者のテキスト入力、読了状態のドロップダウン、作成日の並び順切替を備える。
- 「検索」ボタンで現在の条件に基づき一覧を再取得。
- テーブル表示項目：タイトル、著者、読了（チェックボックスの表示のみ）、作成日、編集ボタン列。
- 行全体をクリックすると編集ページへ遷移。編集ボタンも個別に用意。
- 取得失敗時はエラーアラートを表示し、再取得は手動検索で行う。

### 4.2 書籍新規登録 `/books/create`

- BookForm コンポーネントを利用した入力フォーム。
- バリデーション：Title 必須/100 文字、Author 100 文字、Memo 1000 文字。
- 送信成功で一覧ページへ遷移。失敗時はエラーアラートを表示。

### 4.3 書籍編集 `/books/edit/{id:int}`

- 指定 ID の書籍を初期読み込み。存在しない場合はエラー文言を表示。
- BookForm を編集モードで表示し、保存ボタンで更新。
- 削除ボタンを表示し、ブラウザーの確認ダイアログで確認後に削除。成功時は一覧へ遷移。

### 4.4 共通フォーム BookForm

- タイトル/著者/メモ/読了の入力とバリデーションメッセージ表示を担当。
- 編集モードでは削除ボタンを追加表示する。

## 5. アーキテクチャ

### 5.1 プロジェクト構成（主要部分）

```
/Components
  /Books (一覧・作成・編集・共通行/フォーム)
  /Layout (MainLayout, NavMenu など)
/Data
  AppDbContext.cs
/Models
  Book.cs, BookSearchCondition.cs, ReadFilter.cs, SortOrder.cs
/Services
  IBookService.cs, BookService.cs
```

### 5.2 サービス層

- DI 登録：`AddDbContext<AppDbContext>(UseSqlite)` と `AddScoped<IBookService, BookService>` を `Program.cs` で設定。
- `IBookService` で CRUD と検索/フィルタ/ソート API を定義し、UI はサービス経由でデータへアクセスする。
- `BookService` は `AppDbContext` を用いて検索条件をクエリに反映、作成時は `CreatedAt` を現在時刻に設定し、更新/削除は存在確認のうえ反映。

### 5.3 永続化

- SQLite を `Data Source=app.db` で使用。マイグレーションは通常の EF Core コマンドを想定。
- アプリ起動時に `SeedSampleData` でサンプルデータを自動投入（初回のみ）。

## 6. UX/デザイン方針

- Bootstrap をベースに、シンプルなテーブル + フォーム UI。
- 行クリックで編集遷移できるようにし、操作のショートカットを提供。
- 重大な API 失敗時は画面内アラートで通知。詳細ログはサーバー側に出力。

## 7. 今後の検討メモ

- 作成日時のタイムゾーン方針（現状はサーバーのローカル時刻を使用）を明文化する。
- ナビゲーションや表示ラベルの文字化け箇所を整備する。
- Part4 以降でのクラウド対応（Entra ID・Azure SQL・App Service）を追加設計する。
