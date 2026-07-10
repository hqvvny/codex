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

The user has no budget for paid market data. Strategy work must therefore use available/free/exportable data first and be explicit about limitations.

## Available Stack

- Account context: Apex Tradovate-funded environment.
- Execution/observation candidate: Tradovate and/or NinjaTrader.
- Default instrument: MNQ.
- Preferred development mode: local Python research where possible, then NinjaTrader/Tradovate observation before any capital promotion.

## Operating Rules

- Do not assume paid futures data.
- Do not recommend paid data as the default next step.
- Use Tradovate/NinjaTrader exports or accessible history if available.
- If data is exported manually, record exact export path, date range, timeframe, timezone, session handling, and missing-data caveats.
- If data comes from a platform chart, treat it as research-grade only until export/replay provenance is clear.
- Backtest results from limited history must be labelled limited and not treated as robust.
- Execution tests should happen in simulation/secondary observation first, never straight into a main funded account workflow.

## Practical Plan

1. Confirm whether NinjaTrader can connect to the user's Apex Tradovate account.
2. Confirm whether NinjaTrader can export MNQ historical minute data from that connection.
3. If export works, define a local canonical data folder and CSV schema.
4. Build ingestion around that schema, not around a paid vendor.
5. Backtest only over the proven exported date range.
6. Use NinjaTrader/Tradovate for observation and fill-behavior checks.

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
