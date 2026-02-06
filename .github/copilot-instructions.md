# GitHub Copilot Instructions

此文件用於引導 AI 助理 (GitHub Copilot) 在本專案中的行為與輸出風格。

## 核心原則 (Core Principles)
- **回應語言**: 一律使用 **繁體中文 (Traditional Chinese, zh-TW)**。
- **程式碼風格**: 簡潔、可讀性高，並符合專案技術堆疊的最佳實踐。
- **指令系統**: 本專案擁有 **74 個專業 instruction 檔案**，涵蓋 30+ 種程式語言與框架。詳見 [docs/instructions-readme.md](../docs/instructions-readme.md)。
- **自動套用**: 當您在特定檔案類型中工作時，相應的 instructions 會自動載入並套用。

### 優先套用 Skills 規則
- 當同時存在一般 instruction（`.github/instructions/`）與專門的 Skill（`.github/skills/*/SKILL.md`）時，**應優先套用對應 Skill 的專業規則**，因為 Skill 提供領域特定（domain-specific）且具體的操作指引。
- 使用流程：
  1. 若任務屬於某個技能領域，先以 `read_file` 讀取該 Skill 的 `SKILL.md`。
  2. 依 `SKILL.md` 的指引執行步驟（包含範例、驗證與回報格式）。
  3. 若 `SKILL.md` 與一般 instruction 衝突，遵循 Skill 的規則並在回報中註明採用該 Skill 的原因。
- 範例：若處理 C# 相關問題，優先讀取並遵循 `.github/skills/csharp-code-gatekeeper/SKILL.md` 的建議（最佳實踐、安全性檢查、命名和風格規則）。
- 註記：在自動化任務或多階段工作流中，請明確註明所使用的 Skill 名稱與版本以利可追溯性。

## 專有名詞對照表 (Glossary)
在解釋或生成文字時，請遵守以下核心術語對照：

| 英文 | 繁體中文 | 英文 | 繁體中文 |
| :--- | :--- | :--- | :--- |
| create | 建立 | object | 物件 |
| queue | 佇列 | stack | 堆疊 |
| code | 程式碼 | library | 函式庫 |
| class | 類別 | function | 函式 |
| package | 套件 | dependency | 相依性 |
| transaction | 交易 | memory | 記憶體 |

> **完整對照表**: 請參考 `.vscode/settings.json` 中的 `copilot.chat.termMappings` 設定

## 開發規範 (Development Guidelines)

### Git Commit 規範

遵循 Conventional Commits 1.0.0 規範，使用繁體中文撰寫。

> **📖 完整規範**: `.github/copilot-commit-message-instructions.md`

### 程式碼審查與品質標準 (Code Review & Quality Standards)

遵循 SOLID 原則、高內聚低耦合、DRY、KISS 與安全優先的設計理念。

> **📖 完整規範**: `.github/copilot-review-instructions.md`  
> 涵蓋設計原則、命名風格、品質指標、最佳實踐等完整主題

#### ASP.NET Core Web API 規範

**Controller**: 以 `PascalCase + Controller` 命名，繼承 `ControllerBase`，使用 `[ApiController]` 和路由屬性  
**Action**: 動詞命名 (GetUser, CreateOrder)，所有方法必須 `async`，使用 `ActionResult<T>`  
**HTTP 動詞**: GET (查詢)、POST (建立)、PUT (更新)、DELETE (刪除)  
**最佳實踐**: 使用 `ProducesResponseType`、XML 註解、路由約束、模型驗證

> **📖 完整指南**: `.github/instructions/aspnet-rest-apis.instructions.md`  
> 涵蓋 API 設計、資料存取、身份驗證、測試、效能最佳化等完整主題

### 專案環境 (Project Context)

本專案是一組以 .NET 為主的工具與服務集合，包含：

- **技術堆疊**: 
  - .NET 8.0 (主要平台)
  - ASP.NET Core Web API
  - Worker Services (背景服務)
  - Serilog (日誌記錄)
  - MSTest (單元測試)
  
- **專案結構**:
  - `WebApplication1`: ASP.NET Core Web API
  - `ArchivingService`: 背景封存服務
  - `DataSyncService`: 資料同步服務
  - `XelParser`: XEL 解析工具
  - `Logging.Serilog`: Serilog 擴充函式庫
  - `DBT.Tests`: 測試專案

- **配置檔案**: `.vscode/settings.json`、`.github/instructions/`（74 個專業指令）、`.github/copilot-setup-steps.yml`

### 建置與測試 (Build & Test)

**建置專案**:
```powershell
# 還原 NuGet 套件
dotnet restore DBT.sln

# 建置整個解決方案
dotnet build DBT.sln --configuration Debug

# 建置特定專案
dotnet build WebApplication1/WebApplication1.csproj
```

**執行測試**:
```powershell
# 執行所有測試
dotnet test

# 執行特定測試專案
dotnet test DBT.Tests/DBT.Tests.csproj

# 產生測試報告與覆蓋率
dotnet test --logger "trx;LogFileName=test_results.trx" --collect:"XPlat Code Coverage"
```

**驗證變更**:
在提交變更之前，請確保：
1. ✅ 所有測試通過：`dotnet test`
2. ✅ 程式碼建置成功：`dotnet build DBT.sln`
3. ✅ 遵循程式碼風格指南
4. ✅ 包含適當的單元測試（覆蓋率 ≥ 90%）

### 測試與效能標準

**測試框架**: MSTest/xUnit (C#)、Vitest/Jest (JS/TS)、pytest (Python)、Playwright (E2E)  
**測試模式**: AAA 模式 (Arrange-Act-Assert)，命名清楚描述意圖  
**覆蓋率目標**: 單元測試 ≥ 90%、整合測試 ≥ 80%、E2E 關鍵流程 100%

**效能最佳化**:
- 前端: 圖片壓縮、延遲載入、程式碼分割、CDN
- 後端: 資料庫索引、快取機制、非同步處理、連線池
- 監控: Application Insights、效能剖析工具、負載測試

> **📖 完整指南**: `.github/instructions/performance-optimization.instructions.md`
## 可用的 Prompts 指南 (Available Prompts Guide)

本專案提供 **28 個專業 prompt 檔案**，涵蓋各種開發場景。

### 主要類別

**C# 開發** (4個): 非同步程式設計、文件註解、MSTest 測試、MCP Server  
**資料庫** (3個): SQL 最佳化、程式碼審查、Entity Framework Core  
**.NET 最佳實踐** (3個): 程式碼品質、設計模式、版本升級  
**文件產生** (7個): 技術文件、README、規格文件、多語系翻譯  
**專案工具** (4個): API 專案建立、資料夾結構、EditorConfig、程式碼審查  
**AI 輔助** (7個): Prompt 建議、Agent 推薦、Instructions 推薦、模型選擇

### 使用方式

```bash
# 在 Chat 中使用
/prompt-name
@workspace 使用 csharp-async prompt 審查這段程式碼
```

### 常見開發情境

1. **開發 Web API**: aspnet-code-api → dotnet-best-practices → csharp-docs → csharp-mstest
2. **資料庫開發**: ef-core → sql-optimization → sql-code-review
3. **程式碼品質**: dotnet-design-pattern-review → review-and-refactor → csharp-async
4. **文件撰寫**: documentation-writer → create-readme → create-specification

> **📖 完整清單**: [docs/prompt-readme.md](../docs/prompt-readme.md)

## 可用的 Instructions 指南 (Available Instructions Guide)

本專案擁有 **74 個專業 instruction 檔案**，自動為不同檔案類型提供客製化指導。

### 主要類別

**AI 與 Agent** (6個): Agent、Skills、Prompt 工程  
**程式語言** (22個): C#/.NET (10)、Python (5)、JavaScript/TypeScript (3)、其他 (4)  
**框架** (9個): 前端 (6)、後端 (3)  
**雲端基礎設施** (14個): Azure (3)、Microsoft 365 (3)、Power BI (6)、Kubernetes (2)  
**DevOps 測試** (8個): CI/CD (4)、測試 (3)  
**安全品質** (6個): OWASP、程式碼審查、效能最佳化  
**文件規範** (5個): Markdown、多語系、規格驅動開發

### 自動套用機制

當您編輯特定檔案時，相應的 instructions 會自動載入：

```
*.cs        → csharp.instructions
*.ts        → typescript-5-es2022.instructions
*.py        → python.instructions
Dockerfile  → containerization-docker.instructions
*.bicep     → bicep-code-best-practices.instructions
```

### 手動引用

```bash
@workspace 請依據 security-and-owasp.instructions 審查這段程式碼
@workspace 使用 performance-optimization.instructions 最佳化查詢
@workspace 根據 dotnet-architecture-good-practices 審查專案結構
```

### 開發工作流程

**新專案**: aspnet-code-api → dotnet-architecture → github-actions → security-owasp  
**日常開發**: csharp/python/typescript → code-review-generic → performance-optimization  
**程式碼審查**: code-review-generic → security-owasp → dotnet-design-pattern-review

> **📖 完整清單與說明**: [docs/instructions-readme.md](../docs/instructions-readme.md)

### 注意事項

- ✅ **保持一致性**: 遵循專案既定的編碼風格與命名慣例
- ✅ **安全優先**: 所有程式碼都應通過 OWASP 安全性檢查
- ✅ **測試驅動**: 先寫測試，再寫實作 (TDD)
- ✅ **效能意識**: 在設計階段就考慮效能影響
- ⚠️ **不確定時**: 如果您不知道答案，請不要隨意猜測，請直接詢問我
- ⚠️ **重大變更**: 涉及架構或重大功能變更時，請先討論方案
