---
post_title: "Issue Agentic Workflow Requirements"
author1: "GitHub Copilot"
post_slug: "issue-agentic-workflow-requirements"
microsoft_alias: "copilot"
featured_image: "https://example.com/placeholder.png"
categories: ["internal"]
tags: ["agentic-workflow", "issues"]
ai_note: "true"
summary: "Define requirements for the issue triage agentic workflow."
post_date: "2026-02-16"
---

## 需求概述

- 針對新建立的 GitHub issue 自動進行初步分類與回覆。

## 需求（EARS）

- WHEN 有新的 issue 被建立，THE SYSTEM SHALL 分析 issue 內容並產生
  分類結果。
- WHEN issue 資訊不足，THE SYSTEM SHALL 標記 `needs-info` 並要求補充
  重製步驟、預期/實際結果與環境資訊。
- WHEN 分類完成，THE SYSTEM SHALL 以 comment 回覆摘要與下一步。
- THE SYSTEM SHALL 只使用既有 labels，並限制在允許清單內。
- THE SYSTEM SHALL 不自動關閉或指派 issue。

## 驗收條件

- 新 issue 觸發時會產生一則 triage comment。
- 會套用至多三個既有 labels。
- workflow 編譯成功並產生 lock 檔案。
