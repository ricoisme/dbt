---
name: "Issue Intake (Non-Copilot)"
description: "Process issues created by non-Copilot users and respond with a triage summary."
on:
  issues:
    types: [opened, reopened]
permissions:
  contents: read
  pull-requests: read
  issues: read
  actions: read
  checks: read
  statuses: read
roles: all
network: {}
safe-outputs:
  add-comment:
    max: 1
    target: "triggering"
    hide-older-comments: true
timeout-minutes: 5
---

## 非 Copilot Issue 初步處理

請處理 GitHub issue #${{ github.event.issue.number }}，並只在需要時輸出 **一則** `add-comment` safe output。

### 規則

1. 若作者帳號或任一標籤「包含」`copilot`、`ai`、或 `ai-generated` 任一字樣（不分大小寫、子字串比對），直接結束，不輸出 safe output。
2. 若為非 Copilot issue，輸出繁體中文回覆，內容包含：
   - 摘要（1-3 點）
   - 問題類型（bug / feature / question / documentation / other）
   - 缺漏資訊（重現步驟、期望/實際結果、環境版本、日誌等）
   - 下一步建議
3. 若資料不足（例如 issue 內文為空）、權限不足或任何錯誤，不輸出任何 safe output 並結束。

### 回覆格式

- 摘要：
  - ...
- 問題類型：...
- 缺漏資訊：
  - ...
- 下一步建議：...