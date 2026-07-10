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

## Strategy Research Pipeline

Keep strategy work split into two stages:

1. Candidate generation: scheduled research scans create a ranked queue of hypotheses. Sources may include forums, papers, factor research, and project notes. Each candidate must pass first-filter questions: structural reason, testability with available data, and real sample size. The output is not a finished strategy.
2. Build and test: only filtered hypotheses become prototypes. Use existing helper functions, templates, architecture conventions, order-management patterns, and risk-check sequences instead of starting from a blank file.

Do not let candidates skip the queue. AI-built and hand-built strategies must pass the same backtesting and robustness battery before any capital exposure.

Where AI helps: candidate volume, code scaffolding, standard entry/exit patterns, backtest harnesses, red-team reviews, and surfacing implicit exposures.

Where AI fabricates: live-book awareness, real-time correlation to existing strategies, portfolio construction, and unconstrained codebase conventions. Treat portfolio construction as a manual human decision. Constrain agents with exact file layout, naming conventions, order management, and risk-check sequence.

Before any AI-generated strategy touches real capital, require all three gates in order:

1. Independent review by a different reviewer or model instance.
2. Manual review where the human can personally explain every part of the logic.
3. Observation on a secondary account, demo or small live, with the same broker and data feed long enough to confirm real fills track backtest assumptions.

The model generates candidates. The operator decides which ones are real.

Operational defaults:

- Put raw and filtered hypotheses in `wiki/research/strategy-queue.md`.
- Use `wiki/reference/hypothesis-template.md` for strategy pages.
- Use `wiki/reference/first-filter.md` before build/test work.
- Use `wiki/reference/backtest-battery.md` before treating any result as promotable.
- Use `wiki/research/candidate-generation-workflow.md` for scheduled Stage 1 scans.
