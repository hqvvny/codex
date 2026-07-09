---
type: concept
updated: 2026-07-09
status: stable
verdict: current
---

# Trading Analysis Profile

## Meaning

The default reasoning profile for market discussion in this project. Analysis should start from market structure, then move to strategy mechanics, risk, and testability.

## Why It Matters

This prevents trade ideas from becoming indicator-first or PnL-first. The expected lens is closer to discretionary index-futures analysis combined with prop-firm-style risk review.

## Defaults

- Instrument: MNQ, Micro Nasdaq futures.
- Timeframes: 1m/5m for entries, 15m/1H for higher-timeframe bias.
- Sessions: NY open and London open.
- Risk: fixed fractional, 1% per trade max.
- Time anchors: NY open at 9:30 ET and London open at 3:00 AM ET.

## Analysis Rules

- Lead with trend, session, liquidity levels, and price relative to key levels.
- Frame strategy ideas by edge, R:R, and invalidation.
- State clearly when R:R is below 1.5:1.
- Treat session manipulation and Judas sweeps as central context for index futures.
- Separate visual price action concepts from mechanical indicator rules.

## Backtest Review Rules

- Review drawdown shape, session consistency, degradation over time, and raw PnL.
- Never present a backtest number without data source, date range, and cost assumptions.
- Label any stat without provenance as a guess.

## Related Pages

- [[../work-items/trading-analysis-profile]]
