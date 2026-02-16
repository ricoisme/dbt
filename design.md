---
post_title: "非 Copilot Issue 處理設計"
author1: "GitHub Copilot"
post_slug: "non-copilot-issue-workflow-design"
microsoft_alias: "copilot"
featured_image: ""
categories: ["automation"]
tags: ["github", "agentic-workflow", "design"]
ai_note: "ai-assisted"
summary: "描述非 Copilot issue 的 GitHub Agentic Workflow 設計與安全輸出策略。"
post_date: "2026-02-16"
---

## 架構概觀

- 使用 gh-aw markdown workflow (`.github/workflows/issue-non-copilot-processor.md`) 定義單一工作流程。
- 觸發條件為 issues 的 opened/reopened 事件，並透過前置條件與提示詞判斷是否為 Copilot 建立。
- 透過 safe-outputs `add-comment` 產出回覆，避免直接寫入權限。

## 資料流程

1. 讀取 `github.event.issue` 的標題、內容、作者與標籤。
2. 若作者帳號或標籤包含 copilot 關鍵字，立即停止。
3. 生成摘要與缺漏資訊建議，透過安全輸出回覆於 issue。

## 安全與權限

- `permissions` 僅保留讀取權限；寫入行為透過 safe-outputs 交由後置作業處理。
- `network` 設定為空物件以禁止外部網路存取。
- `roles: all` 允許非團隊成員的 issue 觸發工作流程。
