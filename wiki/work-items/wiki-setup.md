---
type: work-item
updated: 2026-07-09
status: complete
verdict: useful
---

# Wiki Setup

## Goal

Create a persistent LLM-maintained markdown wiki for this workspace, with agent rules, automation hooks, a current-state generator, and Obsidian graph coloring.

## Context

The workspace began as an empty projectless Codex folder with `work/` and `outputs/` directories. It was initialized as a git repository on 2026-07-09. Git-based push/pull hooks still need a remote before cross-machine sync can operate.

## Decisions

- Domain: projectless Codex workspace / knowledge work.
- Main unit of work: work items.
- Main content folder: `wiki/work-items/`.
- OS-specific hooks use bash because the detected OS is macOS.
- Git was initialized after the wiki setup so the wiki can be committed from the start.

## Outcome

The wiki skeleton, schema, failed-ideas ledger, hot-page generator, CLAUDE rules, Claude hooks, Obsidian graph config, and git repository were created.

## Links

- [[../SCHEMA]]
- [[../Failed Ideas/ledger]]
- [[../hot]]
