---
description: "針對 issues 進行自動分類、標籤與回覆的 agentic workflow。"
name: "Issue Processor Agent"
strict: true
timeout-minutes: 5
on:
  issues:
    types: [opened, reopened]
  workflow_dispatch:
permissions:
  issues: read
roles: all
tools:
  github:
    lockdown: true
    read-only: true
    toolsets: [issues, labels]
safe-outputs:
  add-labels:
    allowed: [bug, feature, enhancement, documentation, question, help-wanted, good-first-issue]
  add-comment: {}
network: {}
---

## Issue 處理 Agent

請在 workflow 觸發時處理目前的 issue，目標是完成分類與回覆。

### 步驟

1. 讀取 issue 的標題與內容，判斷最適合的分類標籤。
2. 若分類明確，使用 safe-outputs 加入一個允許的標籤。
3. 若資訊不足或無法判斷分類，改為留言請求補充細節，且不要加入標籤。
4. 留言時請包含：
   - 分類結果或需要補充的資訊
   - 你判斷的原因（簡短）
   - 後續建議（1-2 點）

### 標籤規則

允許的標籤：`bug`, `feature`, `enhancement`, `documentation`, `question`, `help-wanted`, `good-first-issue`。

### 回覆範本

```markdown
### 🧭 Issue 分類結果

**建議標籤**：{label_name 或需要補充的資訊}
**原因**：{簡短說明}
**後續建議**：
- {建議 1}
- {建議 2}
```
