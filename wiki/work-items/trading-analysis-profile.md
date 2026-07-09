---
type: work-item
updated: 2026-07-09
status: complete
verdict: useful
---

# Trading Analysis Profile

## Goal

Capture the user's default market-analysis lens and trading assumptions so future strategy, backtest, and setup discussions start from the right frame.

## Context

The user provided a standing "How to think" profile for trading work. It emphasizes MNQ index-futures context, session timing, liquidity, risk/reward, invalidation, and prop-firm-style backtest evaluation.

## Decisions

- Persist the profile in `CLAUDE.md` as active agent behavior.
- Add a concept page in `wiki/concepts/` for long-term memory and Obsidian navigation.
- Keep the profile as defaults, not hard constraints, so the user can override instrument, timeframe, session, or risk model in a specific task.

## Outcome

Future market work should default to MNQ, 1m/5m entries, 15m/1H bias, NY/London open focus, and 1% max risk per trade. Backtest numbers require data, date range, and cost assumptions.

## Links

- [[../concepts/trading-analysis-profile]]
