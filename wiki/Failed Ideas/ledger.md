---
type: failed-ideas-ledger
updated: 2026-07-09
status: active
---

# Failed Ideas Ledger

Read this before starting any new substantive work. Every abandoned idea belongs here with the reason it failed so future sessions do not repeat dead ends.

| Date | Idea | Context | Reason It Failed | Replacement / Next Move | Links |
| --- | --- | --- | --- | --- | --- |
| 2026-07-09 | Push to GitHub via unauthenticated HTTPS | Tried to push `main` to `https://github.com/hqvvny/codex.git` from the local repo | Git could not read an interactive GitHub username/token in this environment | Use SSH remote after adding a GitHub SSH key, or provide a configured credential helper/token | [[../work-items/wiki-setup]] |
| 2026-07-09 | Push to GitHub via SSH before key setup | Tried to push to `git@github.com:hqvvny/codex.git` | No local SSH public key exists, so GitHub returned `Permission denied (publickey)` | Create an SSH key locally, add it to GitHub, then run `git push -u origin main` | [[../work-items/wiki-setup]] |
| 2026-07-10 | MNQ-001 opening range liquidity sweep | Seed hypothesis for NY open MNQ sweep/reclaim or continuation after opening-range liquidity run | User judged the idea as low-quality/not worth pursuing; it was also still undefined, with unknown R:R and no registered data source | Do not build this generic opening-range sweep idea; generate a fresh candidate with stronger structural reason and clearer testability | [[../research/hypotheses/MNQ-001-opening-range-liquidity-sweep]] |
