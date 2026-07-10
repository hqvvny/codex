---
type: reference
updated: 2026-07-10
status: active
verdict: authoritative-link
---

# Data Execution Stack

## Authoritative Source

This page records the current practical stack constraints for strategy research, execution, and observation.

## Current Constraint

The user has no budget for paid market data. Strategy work must therefore use zero-budget, available, free, account-included, or exportable data first and be explicit about limitations.

## Available Stack

- Account context: Apex Tradovate-funded environment.
- Execution/observation candidate: Tradovate and/or NinjaTrader if workable.
- Default instrument: MNQ.
- Preferred development mode: local Python research where possible, then available-platform observation before any capital promotion.

## Operating Rules

- Do not assume paid futures data.
- Do not recommend paid data as the default next step.
- Use any free/account-included/exportable data source that can be documented.
- Use Tradovate/NinjaTrader exports or accessible history if available, but do not make them mandatory.
- If data is exported manually, record exact export path, date range, timeframe, timezone, session handling, and missing-data caveats.
- If data comes from a platform chart, treat it as research-grade only until export/replay provenance is clear.
- Backtest results from limited history must be labelled limited and not treated as robust.
- Execution tests should happen in simulation/secondary observation first, never straight into a main funded account workflow.

## Zero-Budget Data Ladder

Use sources in this order:

1. Account/platform-included futures data that can be exported, e.g. NinjaTrader or Tradovate if the Apex connection permits it.
2. Free downloadable futures or proxy data, e.g. Stooq for daily/hourly/5-minute data where suitable.
3. Free web data/proxy instruments, e.g. Yahoo Finance `NQ=F`, only for rough prototyping and never as robust intraday proof.
4. Manual observation logs and forward paper/sim tracking when historical data is too thin.
5. Paid vendors only as a future upgrade path, not as the default plan.

## Practical Plan

1. Inventory all zero-budget data access: NinjaTrader, Tradovate, Stooq, Yahoo Finance, manual export, platform replay, and any account-included feeds.
2. Test whether NinjaTrader can export MNQ historical minute data from the Apex/Tradovate connection.
3. Test free sources for NQ/MNQ proxies and document their timeframe limits.
4. Define a local canonical data folder and CSV schema.
5. Build ingestion around the schema, not around a paid vendor.
6. Backtest only over the proven available date range.
7. Use available execution/observation stack for fill-behavior checks.

## Data Provenance Checklist

For every dataset, record:

- Source platform or API.
- Export method.
- Instrument and contract handling.
- Date range.
- Timeframe.
- Timezone.
- ETH/RTH session treatment.
- Commission and fee assumptions.
- Slippage assumptions.
- Known gaps or roll issues.

## Related

- [[backtest-battery]]
- [[first-filter]]
- [[../concepts/trading-analysis-profile]]
