# Project Instructions

## Wiki

This project has an LLM-maintained wiki in `wiki/`. Obsidian is the human viewer; the markdown files are the persistent memory.

Standing rules:

- Before starting any substantive work: read `wiki/index.md` and `wiki/Failed Ideas/ledger.md` first.
- Before finishing: update the relevant wiki page, append a dated line to `wiki/log.md`, and add a Failed Ideas row on any abandonment.

Immutability rule: the wiki links to raw sources such as data, code, outputs, and external references. It never copies their contents in as the source of truth.

Obsidian note: Obsidian rewrites `.obsidian/graph.json` while running, so edit that file only when Obsidian is closed.

## Trading Analysis Defaults

When discussing markets, strategies, backtests, or trade ideas, lead with market structure before indicators or entries:

- Start with trend, session context, liquidity levels, and where price is relative to key levels.
- Default instrument: MNQ, Micro Nasdaq futures.
- Default timeframe focus: 1m/5m lower-timeframe entries, 15m/1H higher-timeframe bias.
- Default session focus: NY open and London open.
- Default risk model: fixed fractional, 1% risk per trade max.
- Consider time of day on every setup, especially NY open at 9:30 ET and London open at 3:00 AM ET.
- Treat session manipulation and Judas sweeps as core index-futures context.

Frame every strategy idea in terms of edge, risk/reward, and invalidation. If R:R is below 1.5:1, say so clearly.

When reviewing backtests, evaluate like a prop firm reviewer: drawdown shape, session consistency, edge degradation over time, and raw PnL. Never present a backtest number without data source, date range, and cost assumptions; if provenance is missing, label the number as a guess.

Distinguish price action concepts from indicator-based rules. FVG, order block, liquidity sweep, BOS/CHoCH, and VWAP need visual confirmation and contextual judgment. Indicator-based rules can be made mechanical and compiled into testable conditions.
