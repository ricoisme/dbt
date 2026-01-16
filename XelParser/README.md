# XelParser

一個高效能的 .NET 主控台應用程式，用於解析 SQL Server Extended Events (XEL) 檔案並將其轉換為易於閱讀的文字格式。

## 概述

XelParser 是一個輕量級工具，專為批次處理 SQL Server Extended Events 記錄檔 (.xel) 而設計。它從指定的輸入資料夾讀取 XEL 檔案，解析事件資料，並將結果輸出為文字檔案，以便於分析和疑難排解。

## 功能特點

- **批次處理**：自動處理輸入資料夾中的所有 XEL 檔案
- **平行執行**：利用非同步作業提升效能
- **錯誤處理**：完整的錯誤收集和報告機制
- **UTF-8 支援**：完整支援國際化內容的 Unicode 字元
- **簡易設定**：基於 JSON 的設定檔，方便快速設定

## 先決條件

- .NET 8.0 或更新版本
- SQL Server Extended Events 記錄檔 (.xel)

## 安裝

複製儲存庫並建構專案：

```bash
git clone <repository-url>
cd DBT/XelParser
dotnet build
```

## 設定

在 `appsettings.json` 中設定輸入和輸出資料夾：

```json
{
    "InputFolder": "D:/path/to/xel/files",
    "OutputFolder": "D:/path/to/output"
}
```

### 設定選項

| 設定 | 說明 |
|---------|-------------|
| `InputFolder` | 包含要處理的 XEL 檔案的目錄 |
| `OutputFolder` | 儲存轉換後文字檔案的目錄 |

## 使用方式

從命令列執行應用程式：

```bash
dotnet run
```

應用程式將會：
1. 從 `InputFolder` 讀取所有 XEL 檔案
2. 解析每個檔案並提取事件資料
3. 將輸出儲存為 `.txt` 檔案至 `OutputFolder`
4. 顯示處理狀態和遇到的任何錯誤

### 輸出範例

```
Parsing blocked_queries_0_133123456789.xel started...
Parsing blocked_queries_0_133123456789.xel completed.
Parsing blocked_queries_1_133123456790.xel started...
Parsing blocked_queries_1_133123456790.xel completed.
Processing completed with no errors.
XelParser Done
```

## 運作原理

XelParser 使用 `Microsoft.SqlServer.XEvent.XELite` 函式庫來高效率地串流和解析 XEL 檔案。處理流程：

1. 掃描輸入資料夾中的所有檔案
2. 使用平行工作非同步處理每個檔案
3. 使用 `XEFileEventStreamer` 從每個 XEL 檔案讀取事件
4. 將每個事件轉換為其字串表示形式
5. 將彙總的輸出寫入與原檔名相同但加上 `.txt` 副檔名的文字檔案

> [!NOTE]
> 如果輸出資料夾不存在，將會自動建立。

## 錯誤處理

應用程式能妥善處理常見情況：

- **輸入資料夾不存在**：應用程式會正常結束而不報錯
- **檔案讀取錯誤**：錯誤會被收集並在處理後顯示
- **串流錯誤**：`EndOfStreamException` 會被捕捉並報告
- **事件解析錯誤**：個別事件的錯誤不會停止整個檔案的處理

所有錯誤會在處理結束時顯示，方便檢閱。

## 相依套件

- [Microsoft.SqlServer.XEvent.XELite](https://www.nuget.org/packages/Microsoft.SqlServer.XEvent.XELite/) - XEL 檔案解析
- [Microsoft.Extensions.Configuration.Json](https://www.nuget.org/packages/Microsoft.Extensions.Configuration.Json/) - 設定管理
- [Microsoft.Extensions.Logging](https://www.nuget.org/packages/Microsoft.Extensions.Logging/) - 記錄框架

## 使用案例

- **效能分析**：提取和分析 SQL Server 效能資料
- **封鎖分析**：解析封鎖偵測 Extended Events 工作階段
- **死結調查**：將死結圖表和相關事件轉換為可讀格式
- **稽核檢閱**：處理安全性稽核事件以符合合規性要求
- **疑難排解**：快速轉換 XEL 檔案以進行除錯

## 限制

- 輸入檔案必須是有效的 SQL Server Extended Events (.xel) 檔案
- 此工具以字串格式輸出原始事件資料；結構化分析可能需要額外的解析
- 大型 XEL 檔案在處理過程中可能會消耗大量記憶體

## 效能

應用程式使用非同步平行處理來同時處理多個檔案，使其在大量 XEL 檔案的批次作業中非常高效。

---

使用 .NET 8.0 建構
