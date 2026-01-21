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

#### 通用設計原則
- **SOLID 原則**: 單一職責、開放封閉、里氏替換、介面隔離、相依反轉
- **高內聚低耦合**: 模組內部緊密相關，模組間依賴最小化
- **DRY 原則**: Don't Repeat Yourself，避免程式碼重複
- **KISS 原則**: Keep It Simple, Stupid，保持簡單
- **安全優先**: 遵循 OWASP Top 10 安全性最佳實踐

#### C# / .NET 命名風格
- **類別/方法/屬性**: 使用 **PascalCase** (例: `UserService`, `GetUserById`)
- **介面**: 前綴 `I` + **PascalCase** (例: `IUserRepository`)
- **私有欄位**: `_camelCase` (例: `_userRepository`)
- **區域變數/參數**: **camelCase** (例: `userId`, `userName`)
- **常數**: **PascalCase** 或 **UPPER_CASE** (例: `MaxRetryCount` 或 `MAX_RETRY_COUNT`)
- **花括號**: 必須換行 (Allman style)

#### JavaScript / TypeScript 命名風格
- **類別/介面**: **PascalCase** (例: `UserService`, `IUser`)
- **函式/方法/變數**: **camelCase** (例: `getUserById`, `userName`)
- **常數**: **UPPER_SNAKE_CASE** (例: `MAX_RETRY_COUNT`)
- **React 元件**: **PascalCase** (例: `UserProfile`)
- **檔案名稱**: **kebab-case** (例: `user-service.ts`) 或 **PascalCase** (React 元件)

#### Python 命名風格
- **類別**: **PascalCase** (例: `UserService`)
- **函式/方法/變數**: **snake_case** (例: `get_user_by_id`, `user_name`)
- **常數**: **UPPER_SNAKE_CASE** (例: `MAX_RETRY_COUNT`)
- **私有方法/屬性**: 前綴單底線 `_method_name`
- **模組**: **snake_case** (例: `user_service.py`)

#### ASP.NET Core Web API 規範

##### Controller 規範
- **命名**: 使用 **PascalCase** + `Controller` 後綴 (例: `UsersController`, `OrdersController`)
- **繼承**: 繼承自 `ControllerBase` (API) 或 `Controller` (MVC 視圖)
- **路由**: 使用 `[Route("api/[controller]")]` 或明確路由 `[Route("api/v1/users")]`
- **版本控制**: 建議使用 API 版本控制 (例: `api/v1/users`, `api/v2/users`)
- **回應類型**: 使用 `[Produces("application/json")]` 明確指定
- **單一職責**: 每個 Controller 只處理一個資源或相關操作

##### Action 規範
- **命名**: 使用具描述性的動詞 + 名詞 (例: `GetUser`, `CreateOrder`, `UpdateProduct`, `DeleteItem`)
- **HTTP 方法屬性**:
  - `[HttpGet]` - 查詢資料 (GET /api/users, GET /api/users/{id})
  - `[HttpPost]` - 建立資源 (POST /api/users)
  - `[HttpPut]` - 完整更新 (PUT /api/users/{id})
  - `[HttpPatch]` - 部分更新 (PATCH /api/users/{id})
  - `[HttpDelete]` - 刪除資源 (DELETE /api/users/{id})
- **路由範本**: 
  - 集合資源: `[HttpGet]` 或 `[HttpGet("")]`
  - 單一資源: `[HttpGet("{id}")]` 或 `[HttpGet("{id:int}")]`
  - 子資源: `[HttpGet("{userId}/orders")]`
- **回應類型**: 
  - 成功: `Ok(data)` (200), `Created(uri, data)` (201), `NoContent()` (204)
  - 失敗: `NotFound()` (404), `BadRequest(error)` (400), `Unauthorized()` (401)
- **非同步**: 所有 Action 必須為 `async` 並回傳 `Task<IActionResult>` 或 `Task<ActionResult<T>>`
- **模型驗證**: 使用 `[FromBody]`, `[FromQuery]`, `[FromRoute]` 明確指定來源
- **授權**: 使用 `[Authorize]` 保護需要驗證的端點
- **參數驗證**: 檢查 `ModelState.IsValid` 或使用 `[ApiController]` 自動驗證

##### 完整範例
```csharp
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// 取得所有使用者清單
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers([FromQuery] UserQueryParameters parameters)
    {
        var users = await _userService.GetUsersAsync(parameters);
        return Ok(users);
    }

    /// <summary>
    /// 依 ID 取得單一使用者
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        
        if (user == null)
        {
            return NotFound($"使用者 ID {id} 不存在");
        }

        return Ok(user);
    }

    /// <summary>
    /// 建立新使用者
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserRequest request)
    {
        var user = await _userService.CreateUserAsync(request);
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    /// <summary>
    /// 更新使用者資訊
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
    {
        var result = await _userService.UpdateUserAsync(id, request);
        
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// 刪除使用者
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var result = await _userService.DeleteUserAsync(id);
        
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}
```

##### 最佳實踐
- ✅ 使用 `[ApiController]` 屬性啟用自動模型驗證和錯誤回應
- ✅ 使用 `ProducesResponseType` 記錄所有可能的回應狀態碼（Swagger 文件）
- ✅ 使用 XML 註解 `<summary>` 描述每個 Action 的用途
- ✅ 參數使用路由約束（例: `{id:int}`, `{guid:guid}`）
- ✅ 複雜查詢使用 DTO 物件而非多個參數
- ✅ 使用 `ActionResult<T>` 取得型別安全與彈性回應
- ✅ 避免在 Controller 中撰寫業務邏輯，委派給 Service 層
- ✅ 使用分頁處理大量資料回應
- ✅ 實作全域例外處理中介軟體
- ✅ 使用 FluentValidation 或 Data Annotations 進行模型驗證

##### 延伸閱讀
本專案包含完整的 ASP.NET REST API 開發指南，涵蓋以下主題：

**📖 參考檔案**: `.github/instructions/aspnet-rest-apis.instructions.md`

**涵蓋內容**:
- **API 設計基礎**: REST 架構原則、資源導向 URL 設計、HTTP 動詞使用
- **專案結構**: 特性資料夾組織、領域驅動設計、分層架構
- **Controller vs Minimal APIs**: 兩種方法的比較與適用場景
- **資料存取模式**: Entity Framework Core、Repository 模式、資料庫遷移
- **身份驗證與授權**: JWT Bearer Token、OAuth 2.0、角色與政策型授權
- **驗證與錯誤處理**: FluentValidation、全域例外處理、RFC 7807 問題詳情
- **API 版本控制**: 版本控制策略、Swagger/OpenAPI 整合
- **記錄與監控**: 結構化記錄、Application Insights、效能監控
- **測試策略**: 單元測試、整合測試、端對端測試
- **效能最佳化**: 快取策略、分頁、壓縮、非同步程式設計
- **部署與 DevOps**: 容器化、CI/CD 管道、健康檢查

**使用方式**:
- 當在 `*.cs` 或 `*.json` 檔案中工作時，此 instruction 會自動套用
- 手動參考: `@workspace 請依據 aspnet-rest-apis.instructions 設計 API`

#### 品質指標
- **循環複雜度 (Cyclomatic Complexity)**: <= 20
- **可維護性指數 (Maintainability Index)**: >= 50
- **測試覆蓋率**: >= 80% (核心業務邏輯 >= 90%)
- **程式碼重複率**: <= 5%

#### 非同步程式設計最佳實踐
- **C#**: 全面使用 `async/await`，避免 `.Wait()` 或 `.Result`
- **JavaScript/TypeScript**: 使用 `async/await` 或 Promise，避免回呼地獄
- **Python**: 使用 `async/await` (asyncio) 處理 I/O 密集操作
- **命名慣例**: 非同步方法後綴 `Async` (C#) 或保持一致命名 (JS/Python)

#### 相依性注入最佳實踐
- **建構函式注入 (Constructor Injection)**: 主要方式，確保相依性在物件建立時就緒
- **屬性注入 (Property Injection)**: 僅用於可選相依性
- **方法注入 (Method Injection)**: 僅用於特定操作所需的相依性
- **避免服務定位器 (Service Locator)**: 違反相依反轉原則

#### 輸入驗證與安全性
- **後端必須驗證所有前端輸入**: 使用 FluentValidation (C#) 或類似框架
- **參數化查詢**: 防止 SQL Injection
- **輸出編碼**: 防止 XSS 攻擊
- **HTTPS Only**: 所有網路通訊必須加密
- **密鑰管理**: 使用環境變數或密鑰管理服務，**禁止硬編碼**

### 專案環境 (Project Context)
- **技術堆疊**: 多語言專案，支援 .NET、Python、JavaScript/TypeScript、React、Angular、Vue 等
- **主要配置**: 
  - `.vscode/settings.json` 包含了詳細的 Copilot 設定與術語定義
  - `.github/instructions/` 包含 74 個專業指令檔案
  - `我的英雄學院/` 目錄包含專案相關圖片資源

### 測試標準 (Testing Standards)

#### 測試框架選擇
- **C#**: MSTest, xUnit, NUnit
- **JavaScript/TypeScript**: Vitest, Jest, Playwright
- **Python**: pytest, unittest
- **E2E 測試**: Playwright (支援 .NET 與 TypeScript)

#### 測試組織
- **AAA 模式**: Arrange (準備) → Act (執行) → Assert (斷言)
- **命名慣例**: 清楚描述測試意圖 (例: `Should_ReturnUser_When_UserExists`)
- **測試套件**: 將相關測試整合在同一測試套件 (Suite) 中
- **測試獨立性**: 每個測試應該獨立運行，不依賴其他測試

#### 測試覆蓋率目標
- **單元測試**: 核心業務邏輯 >= 90%
- **整合測試**: 關鍵 API 端點與資料庫操作 >= 80%
- **E2E 測試**: 關鍵使用者流程 100%

### 效能最佳化 (Performance Optimization)

#### 前端效能
- **資源最佳化**: 圖片壓縮 (WebP, AVIF)、程式碼分割、Tree-shaking
- **延遲載入**: 圖片 `loading="lazy"`、動態 import 元件
- **快取策略**: 瀏覽器快取、Service Worker、CDN
- **打包最佳化**: Webpack, Vite, esbuild 設定最佳化

#### 後端效能
- **資料庫查詢**: 使用索引、避免 N+1 查詢、查詢最佳化
- **快取機制**: Redis、Memcached、應用程式層快取
- **非同步處理**: 使用訊息佇列 (RabbitMQ, Azure Service Bus) 處理耗時任務
- **連線池**: 資料庫連線池、HTTP 連線重用

#### 監控與分析
- **APM 工具**: Application Insights, New Relic, Datadog
- **效能剖析**: Chrome DevTools, dotTrace, Py-Spy
- **負載測試**: k6, JMeter, Gatling
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

### 可用的 Instructions 指南 (Available Instructions Guide)

本專案擁有 **74 個專業 instruction 檔案**，自動為不同檔案類型提供客製化指導。

#### 主要類別

**AI 與 Agent 開發** (6 個檔案)
- 建立自訂 Copilot Agent、Agent Skills、Prompt 工程

**程式語言** (22 個檔案)
- C#/.NET (10個): csharp, dotnet-architecture, blazor, maui, wpf 等
- Python (5個): python, langchain, dataverse, mcp-server
- JavaScript/TypeScript (3個): typescript-5, nodejs-vitest, azure-functions
- 其他 (4個): dart-flutter, R, shell, powershell

**框架** (9 個檔案)
- 前端 (6個): angular, react, vue3, nextjs, nextjs-tailwind, html-css
- 後端 (3個): aspnet-rest-apis, nestjs, wordpress

**雲端與基礎設施** (14 個檔案)
- Azure (3個): logic-apps, bicep, terraform
- Microsoft 365 (3個): declarative-agents, mcp-m365, typespec
- Power BI (6個): custom-visuals, data-modeling, dax, devops, report-design, security
- Kubernetes (2個): deployment, manifests

**DevOps 與測試** (8 個檔案)
- CI/CD: github-actions, azure-pipelines, ansible, docker
- 測試: playwright-typescript, playwright-dotnet, pester-5

**安全性與品質** (6 個檔案)
- security-owasp, code-review-generic, gilfoyle-code-review
- performance-optimization

**文件與規範** (5 個檔案)
- markdown, localization, instructions, spec-driven-workflow

詳細說明請參閱 **[docs/instructions-readme.md](../docs/instructions-readme.md)**

### 使用 Instructions 的方式

#### 自動套用
當您在符合特定模式的檔案中工作時，相應的 instructions 會自動載入：
- 編輯 `*.cs` 檔案 → 自動套用 `csharp.instructions`
- 編輯 `*.ts` 檔案 → 自動套用 `typescript-5-es2022.instructions`
- 編輯 `*.py` 檔案 → 自動套用 `python.instructions`
- 編輯 Dockerfile → 自動套用 `containerization-docker.instructions`

#### 手動引用
在 Copilot Chat 中手動參考特定 instruction：
```
@workspace 請依據 security-and-owasp.instructions 審查這段程式碼的安全性
@workspace 使用 performance-optimization.instructions 最佳化這個查詢
@workspace 根據 dotnet-architecture-good-practices 審查這個專案結構
```

### 開發工作流程建議

#### 新專案啟動
1. 使用 `aspnet-code-api` 或相應框架 instruction 建立專案結構
2. 參考 `dotnet-architecture-good-practices` 規劃架構
3. 設定 `github-actions-ci-cd` 建立 CI/CD 管道
4. 套用 `security-and-owasp` 進行安全性檢查

#### 日常開發
1. 遵循語言特定的 instruction (csharp, python, typescript 等)
2. 使用 `code-review-generic` 進行自我審查
3. 執行 `performance-optimization` 檢查效能瓶頸
4. 參考測試 instruction (playwright, pester 等) 撰寫測試

#### 程式碼審查
1. 使用 `code-review-generic` 或 `gilfoyle-code-review` 進行全面審查
2. 檢查 `security-and-owasp` 確保安全性
3. 驗證設計模式符合 `dotnet-design-pattern-review` (若為 .NET)
4. 確認效能符合 `performance-optimization` 標準

### 注意事項

- ✅ **充分利用 Instructions**: 74 個專業指令檔案是您的最佳開發夥伴
- ✅ **保持一致性**: 遵循專案既定的編碼風格與命名慣例
- ✅ **安全優先**: 所有程式碼都應通過 OWASP 安全性檢查
- ✅ **測試驅動**: 先寫測試，再寫實作 (TDD)
- ✅ **效能意識**: 在設計階段就考慮效能影響
- ⚠️ **不確定時**: 如果您不知道答案，請不要隨意猜測，請直接詢問我
- ⚠️ **重大變更**: 涉及架構或重大功能變更時，請先討論方案
