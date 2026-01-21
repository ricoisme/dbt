---
title: csharp-code-gatekeeper
description: 團隊專用的 C# 程式碼品質守門員 Skill — 提供命名規則、風格檢查、最佳實踐與安全性建議，並在生成、審查或修正 C# 程式碼時強制套用團隊標準。
authors:
  - GitHub Copilot / Team
---

# csharp-code-gatekeeper

目的：為團隊提供一致且可執行的 C# 程式碼品質指導。當生成、審查或修正 C# 程式碼時，必須使用此 skill 來讓輸出符合團隊標準。

觸發情境：
- 新增或修改 `*.cs` 檔案時的程式碼產生或自動修正。
- 拉取請求 (PR) 中要求「使用團隊標準重新格式化或修正」時。

核心守則（摘要）:
- 命名規則：類別與公開成員使用 PascalCase；介面前綴 `I`；私有欄位使用 `_camelCase`；區域變數與參數使用 camelCase；常數使用 PascalCase 或 ALL_CAPS。
- 檔案與命名空間：檔案路徑與命名空間應與 repo 結構一致，類別名稱與檔名一對一。
- 程式碼風格：採用 Allman 花括號風格（大括號換行）、每個方法盡量不超過 50 行、避免深層巢狀（>4 層）。
- 非同步：所有 I/O 與外部呼叫使用 `async/await`，非同步方法後綴 `Async`。
- 相依注入：使用建構式注入；避免 Service Locator。
- SOLID 與單一職責：方法與類別應單一職責，避免大型 God Class。

安全與穩定性原則：
- 禁止硬編碼機密；使用環境變數或秘密管理系統。
- 資料庫查詢採用參數化查詢或 ORM 的參數化 API，防止 SQL Injection。
- 對外 URL、檔案路徑等輸入採白名單/驗證，避免 SSRF 或路徑穿越。
- 不使用不安全的反序列化（例如 Python 的 pickle 類似風險），若必要採嚴格型別檢查。

最佳實踐檢查列表（必檢項）:
- 命名一致性檢查
- 可空性標註與 Null 檢查（C# nullable annotations）
- 非同步實作是否正確使用 `ConfigureAwait(false)`（library/背景工作視情境）
- 例外處理：不要吞掉例外；記錄並回傳合理錯誤
- 日誌：使用結構化日誌（ILogger），避免在生產中輸出敏感資訊
- 單元測試：重要邏輯應該具備單元測試，新增/變更需建議對應測試

建議自動化工具與整合：
- Roslyn 分析器（可自建規則或使用 StyleCop/EditorConfig）
- EditorConfig 與 .editorconfig 規範化格式化
- dotnet-format 作為自動格式化工具
- SonarQube / SonarCloud 做靜態分析與安全檢測
- Dependabot / Renovate 保持相依性更新

使用範例（當 assistant 產生或修正程式碼時必須執行的步驟）：
1. 先檢查命名、命名空間與檔案路徑一致性；必要時重命名使其一致。
2. 檢查並修正程式碼風格（格式化、縮排、花括號樣式）。
3. 檢查非同步方法命名與 `async`/`await` 正確性。
4. 檢查安全性事項（硬編碼秘密、參數化查詢、輸入驗證）；若發現問題，回傳可修正的程式碼片段與簡短風險說明。
5. 若修改可能破壞相依性或 API，提供遞增兼容修正與建議的測試。

範例規則（快速檢核）：
- 類別：`public class UserService` → 檔案 `UserService.cs`；命名空間 `MyOrg.Project.Services`。
- 介面：`public interface IUserRepository`。
- 私有欄位：`private readonly IUserRepository _userRepository;`。
- 非同步方法：`public async Task<UserDto> GetUserAsync(int id)`。

回傳格式（assistant 在應用 skill 時）：
- 如果只是審查：回傳「檢查清單」與逐項建議（每項包含 修正建議 + 風險/原因 + 建議的程式碼片段）。
- 如果要修正：回傳完整修正版的檔案內容（只包含必要變更），並標註變更摘要與影響範圍。

擴充方向（可選）：
- 提供 Roslyn analyzer 規則清單與範例實作
- 建立 CI 工作流程樣板（例如 `.github/workflows/`）以在 PR 中自動執行檢查

聯絡與責任範圍：
- 此 skill 專注於程式碼品質與安全性建議；若需強制化（例如 PR blocking、CI 規則），建議由團隊將上述建議轉為 CI 規則與 analyzer 規則。

---
