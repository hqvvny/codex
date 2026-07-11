---
type: strategy-hypothesis
updated: 2026-07-10
status: testing
verdict: weak-baseline
---

# MNQ-002 Open Long 15:30 Exit 16:00

## One-Line Thesis

If we go long at the RTH market open around 15:30 platform time and close at 16:00, the trade is positive on average.

## Market Structure Context

- Instrument: NQ/MNQ proxy via DATA-001, labelled `NQU6`.
- Session: RTH open, DATA-001 RTH-only export.
- Trend/bias context: none.
- Key liquidity levels: not used.
- Price relative to key levels: not used.

## Edge Claim

- Structural reason: possible opening drift or early-session buy pressure.
- Who is likely trapped or forced: not specified.
- Why this should persist: not proven. Current test is a mechanical baseline, not a structural edge proof.

## Setup Definition

- Entry condition: long at 15:30 bar open for exact local-clock variant.
- Stop condition: none.
- Target condition: exit at 16:00 bar close.
- Invalidation: weak or negative after costs, poor yearly consistency, or drawdown too large for prop constraints.
- Expected R:R: not defined. There is no stop/target, so R:R cannot be evaluated. This is below the normal strategy standard until a risk model exists.

## Data Requirements

- Data source: DATA-001 MotiveWave NQU6 1m RTH export.
- Date range: 2017-04-17 to 2026-07-10.
- Timeframe: 1m.
- Session calendar: RTH-only MotiveWave export, likely Europe/Berlin platform time.
- Costs: first pass used 0.0 points round turn.
- Slippage: first pass used 0.0 points.
- Known gaps: no ETH context, timezone not fully confirmed, contract-roll handling not fully confirmed.

## Sample Size Expectation

Exact local-clock 15:30 to 16:00 variant produced 2,314 trades. Session-aligned first-bar plus 30m variant produced 2,185 trades.

## First Filter

- Structural reason: weak but plausible.
- Testable with available data: yes, with DATA-001 RTH-only limitations.
- Real sample size: yes.
- PA or mechanical: mechanical.
- R:R >= 1.5: no R:R because there is no stop/target.
- Verdict: baseline-test only; not promotable as a strategy.

## Build/Test Notes

Generated with:

```bash
python3 scripts/open_time_baseline.py --input data/processed/DATA-001/ohlcv_1m.csv --output-dir outputs --entry-time 15:30 --exit-time 16:00 --hold-minutes 30 --min-bars 300 --point-value 20 --round-turn-cost-points 0
```

Artifacts:

- `outputs/MNQ-002-open-long-baseline-summary.json`
- `outputs/MNQ-002-local-clock-trades.csv`
- `outputs/MNQ-002-session-aligned-trades.csv`

## Backtest Provenance

- Data: DATA-001 MotiveWave NQU6 1m RTH export.
- Date range: 2017-04-17 to 2026-07-10.
- Costs: 0.0 points round turn.
- Slippage: 0.0 points.
- Version/commit: pending at time of page creation.

## Gross Baseline Result

Exact local-clock 15:30 to 16:00:

- Trades: 2,314.
- Gross/net average: 1.48 points per trade before costs/slippage.
- Win rate: 52.42%.
- Profit factor: 1.065.
- Max drawdown: -2,170.5 points.
- Weak years: 2017, 2022, 2024.

Session-aligned first RTH bar plus 30m:

- Trades: 2,185.
- Gross/net average: 0.98 points per trade before costs/slippage.
- Win rate: 52.17%.
- Profit factor: 1.04.
- Max drawdown: -2,923.5 points.
- Weak years: 2017, 2022, 2024.

## Robustness Battery

Not passed. Required before any promotion:

- Add realistic NQ/MNQ costs and slippage.
- Split by year/regime.
- Add drawdown duration.
- Test short mirror and flat benchmark.
- Test entry/exit delay.
- Test with ETH/all-sessions DATA-002 once available.
- Define risk model or stop/target before R:R can be evaluated.
- Deferred follow-up: use MNQ-002 as a benchmark and test structural filters rather than optimizing the raw time entry. Reminder suggestion created for 2026-07-11 16:00 Europe/Berlin.

Candidate filter families to evaluate later:

- ETH/overnight context once DATA-002 exists.
- Previous RTH close/open gap direction.
- Prior day range location.
- Opening volatility/range regime.
- Higher-timeframe trend/bias.
- News/no-news split if a zero-budget calendar source becomes available.

## Review Gates

- Independent review: not started.
- Manual review: not started.
- Secondary-account observation: not started.

## Outcome

The thesis is gross positive on DATA-001, but weak. It should be treated as a benchmark or scouting clue, not a strategy. Profit factor is too close to 1, drawdown is large, yearly consistency is poor, and no R:R exists because there is no stop.

## Links

- [[../strategy-queue]]
- [[../../reference/data-inventory]]
- [[../../reference/backtest-battery]]
- [[../../concepts/trading-analysis-profile]]
