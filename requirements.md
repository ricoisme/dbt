---
post_title: "Issue 處理 Agentic Workflow 需求"
author1: "GitHub Copilot"
post_slug: "issue-agentic-workflow-requirements"
microsoft_alias: "github-copilot"
featured_image: "https://example.com/featured.png"
categories:
  - internal
tags:
  - gh-aw
  - issues
  - automation
ai_note: "本文件由 AI 協助產生"
summary: "定義針對 GitHub issues 自動分類與回覆的需求。"
post_date: "2026-02-16"
---

## 需求範圍

本需求涵蓋 GitHub Agentic Workflows（gh-aw）在 issue 事件觸發時的自動處理行為。

## 使用者故事

- 作為維護者，我希望 issues 在建立或重新開啟時能自動分類，讓後續處理更有效率。
- 作為維護者，我希望 workflow 能在需要時詢問缺漏資訊，避免無法判斷的問題被忽略。

## 需求（EARS）

- WHEN 有新的 issue 被建立或重新開啟，THE SYSTEM SHALL 讀取該 issue 的標題與內容並進行分類。
- WHEN 分類完成且可判斷類型，THE SYSTEM SHALL 為該 issue 加上一個允許的標籤。
- WHEN 標籤已成功新增，THE SYSTEM SHALL 留下包含分類理由與後續建議的回覆。
- IF 無法判斷分類或資訊不足，THEN THE SYSTEM SHALL 留下需要補充資訊的回覆。

## 非功能性需求

- 工作流程必須使用最小權限設定。
- 工作流程必須透過安全輸出（safe-outputs）進行寫入操作。
