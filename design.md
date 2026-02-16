---
post_title: "Issue Agentic Workflow Design"
author1: "GitHub Copilot"
post_slug: "issue-agentic-workflow-design"
microsoft_alias: "copilot"
featured_image: "https://example.com/placeholder.png"
categories: ["internal"]
tags: ["agentic-workflow", "issues"]
ai_note: "true"
summary: "Describe the design for the issue triage agentic workflow."
post_date: "2026-02-16"
---

## 架構概述

- 使用 gh-aw 的單一工作流程執行模式。
- 以 `.github/workflows/issue-triage.md` 定義工作流程並編譯 lock 檔。

## 工作流程設計

- 觸發條件：`issues` 的 `opened` 事件。
- 權限：僅需 `contents: read` 與 `issues: read`。
- 工具：GitHub toolsets `context`、`repos`、`issues`、`labels`。
- Network：不開放對外網路存取（`network: {}`）。

## 安全輸出

- `add-comment`：新增 triage comment（最多 1 則）。
- `add-labels`：新增最多 3 個 labels，限制在允許清單。

## 例外處理

- 若 labels 不存在，僅回覆 comment 並略過標籤。
- 若 issue 內容不足，優先標記 `needs-info`。
