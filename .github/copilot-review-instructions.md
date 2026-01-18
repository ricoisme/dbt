# GitHub Copilot Code Review Instructions

此文件定義了 GitHub Copilot 在進行程式碼審查 (Code Review) 時的指導原則與標準。請依據以下規範提供建議與修正。

## 1. 專案環境與技術堆疊 (Project Context)
- **語言**: C#
- **框架版本**: .NET 6 ~ .NET 10
- **應用程式類型**: ASP.NET Core Web API

## 2. 設計原則 (Design Principles)
程式碼必須符合 **SOLID** 原則與 **高內聚低耦合 (High Cohesion, Low Coupling)** 的要求：
- **S (SRP)**: 單一職責原則 - 類別與方法應專注於單一功能。
- **O (OCP)**: 開放封閉原則 - 對擴充開放，對修改封閉。
- **L (LSP)**: 里氏替換原則 - 子類別必須能替換基底類別。
- **I (ISP)**: 介面隔離原則 - 針對不同需求建立專用介面，避免肥大介面。
- **D (DIP)**: 依賴反轉原則 - 依賴抽象介面，而非具體實作。

## 3. 命名規則與程式碼風格 (Naming Rules & Code Style)
請確保程式碼符合團隊規範與 Microsoft C# Coding Conventions：
- **PascalCase**: 用於 類別 (Class)、方法 (Method)、屬性 (Property)、介面 (Interface, 以 I 開頭)、常數 (Constant)。
  - 例: `CustomerService`, `GetCustomerById`, `ICustomerRepository`
- **camelCase**: 用於 區域變數 (Local Variable)、方法參數 (Parameter)。
  - 例: `customerName`, `orderId`
- **Private Fields**: 使用 camelCase 並以底線 `_` 開頭。
  - 例: `_logger`, `_context`
- **花括號**: 必須換行 (Allman style)。

## 4. 程式碼品質指標 (Code Quality Metrics)
在審查時，請特別檢查以下指標。若未達標，請建議重構：
- **循環複雜度 (Cyclomatic Complexity)**: 
  - 每個方法的複雜度必須 **<= 20**。
  - 若超過，建議拆分為多個小方法或簡化邏輯。
- **可維護性指數 (Maintainability Index)**: 
  - 必須 **>= 50**。
  - 確保程式碼易於閱讀、理解與維護。

## 5. 最佳實踐 (Best Practices)
- **非同步設計**: Web API 方法應全面使用 `async/await` 模式。
- **依賴注入 (DI)**: 所有外部服務與依賴應透過 Constructor Injection 注入。
- **錯誤處理**: 避免空的 catch 區塊，確保有適當的 Logging 與 Exception Handling。
- **API 規範**: 遵循 RESTful API 設計原則，使用正確的 HTTP Status Codes。
- **前端驗證 (Frontend Validation)**: 任何來自前端的輸入資料必須在後端進行驗證，不可僅依賴前端驗證邏輯。建議使用 FluentValidation 或 Data Annotations。
