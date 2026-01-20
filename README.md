**專案名稱與描述**
- **專案**: DBT
- **描述**: 一組以 .NET 為主的工具與服務集合，包含背景工作服務 (Worker)、同步服務、Web API、日誌套件與 XEL 解析工具，用於資料擷取、封存與同步處理。

**技術棧**
- **語言/平台**: C#, .NET (觀察到輸出目標為 net8.0)
- **Web/API**: ASP.NET Core
- **背景工作**: Worker Service (ArchivingService, DataSyncService)
- **日誌**: Serilog (Logging.Serilog 專案)
- **測試**: MSTest ([DBT.Tests](DBT.Tests))

**專案架構概覽**
- 該解決方案以多專案 (solution) 形式組織，主要目錄:
  - ArchivingService — 背景封存服務
  - DataSyncService — 資料同步服務與佇列處理
  - WebApplication1 — 範例或管理型 Web API
  - XelParser — XEL 解析工具（命令列/程式庫）
  - Logging.Serilog — Serilog 擴充與設定
  - DataAccess / Entities — 資料存取層與實體模型

**快速上手**
- 前置需求:
  - 安裝 .NET 8 SDK（或與專案相符的 SDK 版本）

- 建置專案:
```powershell
dotnet restore
dotnet build
```

- 執行測試:
```powershell
dotnet test
```

**專案資料夾結構 (摘要)**
- 根目錄包含: [DBT.sln](DBT.sln)
- 主要專案目錄:
  - [ArchivingService](ArchivingService)
  - [DataSyncService](DataSyncService)
  - [WebApplication1](WebApplication1)
  - [XelParser](XelParser)
  - [Logging.Serilog](Logging.Serilog)
  - [DataAccess](DataAccess)
  - [DBT.Tests](DBT.Tests)

**主要功能**
- 背景封存與處理資料流
- 資料同步與任務佇列處理
- Web API 範例與管理端點
- XEL 檔案解析工具
- 可插拔的 Serilog 日誌擴充

### Web API (WebApplication1) 文件說明
- 文件位置：WebApplication1/README.md  
- 說明：包含如何本機啟動 API、Swagger UI 使用、主要路由與範例請求/回應、設定說明 (連線字串 / 環境變數) 及健康檢查端點。
- 快速連結：
  - ./WebApplication1/README.md（詳細 API 文件與範例）

**開發流程與工作流程**
- 使用 solution 檔管理多專案，建置與測試以 `dotnet` 工具鏈執行。
- 建議分支策略: feature 分支開發 + pull request 合併（倚賴 CI 實作測試與掃描）。

**程式碼風格 / 約定**
- C# 專案遵循常見的 .NET 命名與結構慣例（類別 PascalCase、私有欄位 `_camelCase`、非同步方法後綴 `Async` 等）。
- 專案中已有 `.github` 下的說明檔（例如 [.github/copilot-instructions.md](.github/copilot-instructions.md)），請參考以取得專案特定指引。

**測試**
- 單元測試專案: [DBT.Tests](DBT.Tests)，使用 MSTest。可用 `dotnet test` 來執行。

**貢獻指南**
- 建議先在本地建立 feature 分支、加入測試、提交 PR。
- 請參考專案內 `.github` 與 `copilot-instructions.md` 取得更多貢獻規範與自動化 CI/CD 指引。

**授權**
- 若專案未在倉庫中明確提供授權檔，請在提交前新增 `LICENSE` 檔以說明授權條款。

----
更多內部細節與指引請參考:
- [.github/copilot-instructions.md](.github/copilot-instructions.md)
- [DBT.sln](DBT.sln)
