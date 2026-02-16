---
post_title: "Issue 處理 Agentic Workflow 設計"
author1: "GitHub Copilot"
post_slug: "issue-agentic-workflow-design"
microsoft_alias: "github-copilot"
featured_image: "https://example.com/featured.png"
categories:
  - internal
tags:
  - gh-aw
  - issues
  - automation
ai_note: "本文件由 AI 協助產生"
summary: "說明 issue 處理 workflow 的觸發、工具與輸出設計。"
post_date: "2026-02-16"
---

## 架構概述

此 workflow 以 gh-aw Markdown 定義，編譯後產生對應的 `.lock.yml` GitHub Actions 工作流程。

## 元件設計

- 觸發條件：`issues`（opened/reopened/edited）與 `workflow_dispatch`。
- AI 引擎：預設 Copilot 引擎，開啟 strict 模式。
- 工具：GitHub MCP（issues、labels）工具集。
- 安全輸出：`add-labels` 與 `add-comment`。

## 資料流程

1. 觸發時讀取 issue 標題與內容。
2. 依內容判斷分類並選擇允許標籤。
3. 透過 safe-outputs 寫入標籤與回覆。
4. 若缺少資訊，改為留言請求補充。

## 錯誤處理

- 若工具或資料不足，回覆需說明原因並請求補充資訊。
- 避免加入多重標籤或重複留言。

## 安全性與權限

- 設定 `permissions: issues: read`，寫入由 safe-outputs 承擔。
- `roles: all` 允許任何提交 issue 的使用者觸發。
- `network: {}` 禁止外部網路存取。

## 測試策略

- 以 `gh aw compile issue-processor --strict` 驗證 workflow 定義。
