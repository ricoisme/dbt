---
description: 'Dotnet 除錯工作流程：分析錯誤、修正錯誤、驗收'
name: 'dotnet-debugger'
tools: ['read', 'search', 'edit', 'execute', 'agent']
target: 'vscode'
infer: true
---

# Dotnet Debugger Workflow

## 簡介
此 agent 提供一個結構化的 .NET 除錯工作流程：
1. 分析錯誤（Analysis）
2. 修正錯誤（Fix）
3. 驗收（Acceptance / Verify）

設計上會使用手動或按鈕式 handoffs，並在需要時使用 `runSubagent` 呼叫更專門的子代理（例如 `debug`, `csharp-dotnet-janitor`, `tdd-green` 等）。

## handoffs
當使用者與此 agent 互動結束後，會顯示下列建議按鈕，方便順序流轉：

handoffs:
  - label: 分析錯誤
    agent: debug
    prompt: '開始分析目標程式碼或錯誤日誌，列出重現步驟與可能原因。'
    send: false
  - label: 修正錯誤
    agent: csharp-dotnet-janitor
    prompt: '依據分析結果執行修正，產生修正說明與對應 PR 檔案變更。'
    send: false
  - label: 驗收與測試
    agent: tdd-green
    prompt: '執行測試或補足測試用例，確認錯誤已被修正且無回歸。'
    send: false

## 使用說明 (流程與範例)

流程概述：

1) 分析錯誤（呼叫 `debug` 子代理）

```javascript
// 範例：使用 runSubagent 呼叫 debug 來分析錯誤
const analysis = await runSubagent({
  description: '分析 .NET 錯誤與堆疊',
  prompt: `你是 Debug specialist。
Context:
- Repository: ${repositoryName}
- Target files: ${changedFiles}

Task:
1. 讀取錯誤日誌與相關程式碼
2. 列出重現步驟、可能問題點與優先修正項目
3. 輸出分析報告到: ${basePath}/debug/analysis.md`
});
```

2) 修正錯誤（呼叫 `csharp-dotnet-janitor` 或相似實作代理）

```javascript
// 範例：依據分析結果進行修正
const fix = await runSubagent({
  description: '依分析結果修正 .NET 程式碼',
  prompt: `你是 C# janitor specialist。
Context:
- Analysis report: ${basePath}/debug/analysis.md
- Code base: ${basePath}/src/

Task:
1. 針對列出的優先修正項目實作安全且可測試的更動
2. 更新或新增必要的單元測試
3. 產出修改清單與變更摘要 (path/to/changes.md)`
});
```

3) 驗收（呼叫 `tdd-green` 或 `tdd-refactor` 進行測試驗收）

```javascript
// 範例：確認修正已通過測試
const verify = await runSubagent({
  description: '執行測試並驗收修正',
  prompt: `你是 TDD Green specialist。
Context:
- Changes: ${basePath}/changes/
- Test runner: dotnet test

Task:
1. 執行完整測試 (unit + integration as applicable)
2. 報告失敗的測試與回歸
3. 當所有關鍵測試通過，輸出驗收報告: ${basePath}/debug/acceptance.md`
});
```

## 輸出預期
- `debug/analysis.md`：錯誤分析與重現步驟
- `changes/`：實作變更（包含檔案差異）
- `debug/acceptance.md`：驗收結果與測試摘要

## 注意事項與最佳實務
- 使用最小權限原則，只開啟必要的 tools（此代理預設列出 `read, search, edit, execute, agent`）
- 若要自動執行修正，請在呼叫 `runSubagent` 前明確授權
- 子代理的可用性取決於環境與組織設定，handoffs 若指向不存在的 agent 會被忽略

## 範例交互建議（使用者對話示例）
- 使用者："我的 PR 在 CI 上出現 NullReferenceException，協助我分析並修正。"
- 此 agent 回應："我會先分析錯誤並建議修正；是否要我開始分析（按 '分析錯誤'）？"

---

請依照上方流程操作，或手動選擇 handoff 按鈕逐步交付。此檔案參考了 repository 中既有 agent 範例與 agent 建檔指南。
