# GitHub Copilot Instructions

此文件用於引導 AI 助理 (GitHub Copilot) 在本專案中的行為與輸出風格。

## 核心原則 (Core Principles)
- **回應語言**: 一律使用 **繁體中文 (Traditional Chinese, zh-TW)**。
- **程式碼風格**: 簡潔、可讀性高，並符合 .NET/C# 的最佳實踐 (依據專案設定推斷)。

## 專有名詞對照表 (Glossary)
在解釋或生成文字時，請嚴格遵守以下術語對照：

| 英文 (English) | 繁體中文 (Traditional Chinese) |
| :--- | :--- |
| create | 建立 |
| object | 物件 |
| queue | 佇列 |
| stack | 堆疊 |
| information | 資訊 |
| invocation | 呼叫 |
| code | 程式碼 |
| running | 執行 |
| library | 函式庫 |
| schematics | 原理圖 |
| building | 建構 |
| Setting up | 設定 |
| package | 套件 |
| video | 影片 |
| for loop | for 迴圈 |
| class | 類別 |
| Concurrency | 平行處理 |
| Transaction | 交易 |
| Transactional | 交易式 |
| Code Snippet | 程式碼片段 |
| Code Generation | 程式碼產生器 |
| Any Class | 任意類別 |
| Scalability | 延展性 |
| Dependency Package | 相依套件 |
| Dependency Injection | 相依性注入 |
| Reserved Keywords | 保留字 |
| Metadata | Metadata |
| Clone | 複製 |
| Memory | 記憶體 |
| Built-in | 內建 |
| Global | 全域 |
| Compatibility | 相容性 |
| Function | 函式 |
| Refresh | 重新整理 |
| document | 文件 |
| example | 範例 |
| demo | 展示 |
| quality | 品質 |
| tutorial | 指南 |
| recipes | 秘訣 |
| byte | 位元組 |
| bit | 位元 |

## 開發規範 (Development Guidelines)

### Git Commit 規範
- **格式**: 遵循 **Conventional Commits 1.0.0**，並強制包含 **Issue ID**。
- **結構**: `<type>(<scope>): <description> (Issue #<issue-id>)`
- **語言**: 必須使用 **繁體中文 (Traditional Chinese)**。
- **類型**: feat, fix, docs, style, refactor, perf, test, chore。
- **範例**:
  - `feat(auth): 新增使用者登入 API (Issue #123)`
  - `fix(order): 修正訂單金額計算錯誤 (Issue #456)`
  - `docs(readme): 更新專案安裝說明 (Issue #789)`

### 程式碼審查與品質標準 (Code Review & Quality Standards)
- **設計原則**: 嚴格遵守 **SOLID** 原則與 **高內聚低耦合**。
- **命名風格**: 
  - Class/Method/Property 使用 **PascalCase**。
  - 變數/參數使用 **camelCase**。
  - 私有欄位使用 `_camelCase`。
  - 花括號必須換行 (Allman style)。
- **品質指標**: 
  - 循環複雜度 (Cyclomatic Complexity) <= 20。
  - 可維護性指數 (Maintainability Index) >= 50。
- **最佳實踐**:
  - 全面使用 `async/await`。
  - 透過建構函式注入 (Constructor Injection) 相依性。
  - 後端必須驗證所有前端輸入 (FluentValidation/Data Annotations)。

### 專案環境 (Project Context)
- **技術堆疊**: 本專案推測為 .NET 環境 (基於 `.gitignore` 與 VS Code 設定)。
- **主要配置**: 
  - `.vscode/settings.json` 包含了詳細的 Copilot 設定與術語定義。
  - `我的英雄學院/` 目錄包含專案相關圖片資源。

### 測試 (Testing)
- 若需撰寫測試，請優先將相關測試整合在同一測試套件 (Suite) 中。
## 可用的 Prompts 指南 (Available Prompts Guide)

本專案在 `.github/prompts/` 目錄下提供了 28 個專業 prompt 檔案，涵蓋各種開發場景。詳細說明請參閱 [docs/prompt-readme.md](../docs/prompt-readme.md)。

### 快速參考

#### C# 開發
- **`csharp-async.prompt.md`** - C# 非同步程式設計最佳實踐
- **`csharp-docs.prompt.md`** - C# XML 文件註解標準
- **`csharp-mstest.prompt.md`** - MSTest 單元測試指導
- **`csharp-mcp-server-generator.prompt.md`** - 建立 C# MCP Server 專案

#### SQL 與資料庫
- **`sql-optimization.prompt.md`** - SQL 效能最佳化
- **`sql-code-review.prompt.md`** - SQL 程式碼審查
- **`ef-core.prompt.md`** - Entity Framework Core 最佳實踐

#### .NET 最佳實踐
- **`dotnet-best-practices.prompt.md`** - .NET/C# 程式碼品質標準
- **`dotnet-design-pattern-review.prompt.md`** - 設計模式審查
- **`dotnet-upgrade.prompt.md`** - .NET 版本升級與代碼現代化指南

#### 文件產生
- **`documentation-writer.prompt.md`** - Diátaxis 框架技術文件
- **`create-readme.prompt.md`** - 產生專案 README.md 檔案
- **`readme-blueprint-generator.prompt.md`** - 智慧分析專案並產生全面文件
- **`create-specification.prompt.md`** - 建立 AI 可讀的技術規格文件
- **`gen-specs-as-issues.prompt.md`** - 識別缺失功能並轉化為規格與 Issue
- **`mkdocs-translations.prompt.md`** - 自動翻譯 MkDocs 文件堆疊
- **`write-coding-standards-from-file.prompt.md`** - 從現有代碼自動生成編碼規範

#### 專案架構與工具
- **`aspnet-code-api.prompt.md`** - 建立 ASP.NET Core Web API 專案
- **`folder-structure-blueprint-generator.prompt.md`** - 分析與文件化專案資料夾結構
- **`editorconfig.prompt.md`** - 產生 .editorconfig 設定檔案
- **`review-and-refactor.prompt.md`** - 程式碼審查與重構

#### AI 輔助工具
- **`suggest-awesome-github-copilot-prompts.prompt.md`** - 推薦相關 prompts
- **`prompt-builder.prompt.md`** - 建立高品質 prompt
- **`suggest-awesome-github-copilot-agents.prompt.md`** - 推薦並安裝 Copilot Agents
- **`suggest-awesome-github-copilot-collections.prompt.md`** - 推薦並安裝資源合集 (Collections)
- **`suggest-awesome-github-copilot-instructions.prompt.md`** - 推薦並安裝 Instructions
- **`model-recommendation.prompt.md`** - 推薦最適合的 AI 模型
- **`generate-custom-instructions-from-codebase.prompt.md`** - 分析代碼演進並生成 Custom Instructions

### 使用方式

在 GitHub Copilot Chat 中使用 prompts 的方式：

1. **使用 Slash Command**:
   ```
   /prompt-name
   ```

2. **直接參考 Prompt**:
   ```
   @workspace 使用 csharp-async prompt 審查這段程式碼
   ```

3. **選取程式碼後使用**:
   - 選取要審查或重構的程式碼
   - 在 Chat 中輸入相關 prompt 名稱
   - 變數 `${selection}` 會自動帶入選取的程式碼

### 常見開發情境

#### 情境 1: 開發新的 Web API
```
1. aspnet-code-api - 建立專案結構
2. folder-structure-blueprint-generator - 規劃資料夾組織
3. dotnet-best-practices - 遵循開發標準
4. csharp-docs - 加入 XML 註解
5. csharp-mstest - 撰寫單元測試
```

#### 情境 2: SQL 資料庫開發
```
1. ef-core - Entity Framework Core 設計
2. sql-optimization - 查詢效能最佳化
3. sql-code-review - 安全性與品質審查
```

#### 情境 3: 程式碼品質提升
```
1. dotnet-design-pattern-review - 設計模式審查
2. review-and-refactor - 程式碼重構
3. csharp-async - 非同步模式檢查
4. csharp-mstest - 完善測試覆蓋率
```

#### 情境 4: 文件撰寫
```
1. documentation-writer - 建立 Diátaxis 框架文件
2. create-readme - 產生 README
3. create-specification - 撰寫技術規格
```

### 組合使用建議

- **完整 .NET 開發**: `dotnet-best-practices` + `dotnet-design-pattern-review` + `csharp-async` + `csharp-docs`
- **資料庫全面檢查**: `sql-optimization` + `sql-code-review` + `ef-core`
- **文件完整性**: `documentation-writer` + `create-readme` + `create-specification`

### 尋找更多 Prompts

使用 `suggest-awesome-github-copilot-prompts` 可以：
- 根據專案內容推薦相關 prompts
- 從 [GitHub awesome-copilot](https://github.com/github/awesome-copilot) 發現新工具
- 避免重複安裝已有的 prompts
如果你不知道答案，請不要隨意猜測，請直接詢問我。
