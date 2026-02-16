---
name: "Issue Intake (Non-Copilot)"
description: "Process issues created by non-Copilot users and respond with a triage summary."
on:
  issues:
    types: [opened, reopened]
if: ${{ !contains(github.event.issue.user.login, 'copilot') }}
permissions:
  issues: read
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

你需要針對 GitHub issue #${{ github.event.issue.number }} 進行初步處理，並以 **safe output** 新增一則回覆評論。

### 判斷條件

- 若 issue 作者帳號包含 "copilot" 字樣，或標籤中包含 "copilot"、"ai"、"ai-generated" 相關字樣，**不要**輸出任何 safe outputs 並結束流程。

### 輸出目標

對於非 Copilot issue，請產出一則繁體中文回覆，內容需包含：

1. **摘要**：1-3 點重點摘要。
2. **問題類型**：bug / feature / question / documentation / other 其中一種。
3. **可能缺漏資訊**：列出缺少的重現步驟、期望/實際結果、環境版本或日誌等。
4. **下一步建議**：告知維護者或提問者接下來應提供的資訊。

### 回覆格式

請使用以下格式回覆：

- 摘要：
  - ...
- 問題類型：...
- 缺漏資訊：
  - ...
- 下一步建議：...

### 錯誤處理與限制

- 若流程發生錯誤（例如 API 失敗、權限不足、輸入格式不正確）、缺少必要資料或無法完成任務，請**不要**建立任何新的 issue、討論或 PR，也不要新增評論。
- 在上述情況下，請使用 `noop` safe output（或直接不輸出任何 safe output）並結束流程。

### 安全輸出

使用 `add-comment` safe output 發佈回覆，且只輸出 **一則** 評論。
