---
post_title: "非 Copilot Issue 處理需求"
author1: "GitHub Copilot"
post_slug: "non-copilot-issue-workflow-requirements"
microsoft_alias: "copilot"
featured_image: ""
categories: ["automation"]
tags: ["github", "agentic-workflow", "issues"]
ai_note: "ai-assisted"
summary: "定義非 Copilot 建立 issue 的處理需求與觸發條件。"
post_date: "2026-02-16"
---

## 需求

- WHEN GitHub issue 被建立或重新開啟且建立者不是 Copilot，THE SYSTEM SHALL 啟動 agentic workflow 進行處理。
- WHEN issue 建立者帳號包含 "copilot" 或標籤包含 "copilot" 類型字樣，THE SYSTEM SHALL 不輸出任何寫入動作並結束流程。
- WHEN workflow 處理非 Copilot issue，THE SYSTEM SHALL 產出摘要與缺漏資訊清單並以安全輸出回覆於該 issue。
- WHEN workflow 發生錯誤或缺少必要資料導致無法完成任務，THE SYSTEM SHALL 不建立任何新的 issue、討論或 PR，並以 noop 或無輸出結束流程。
