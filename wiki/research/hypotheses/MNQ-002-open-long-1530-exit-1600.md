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
- `outputs/MNQ-002-open-long-strategy.pine`
- `outputs/MNQ002OpenLongStrategy.cs`
- `outputs/MNQ-002-ninjatrader-export-comparison.md`
- `outputs/MNQ-002-ninjatrader-export-comparison.csv`
- `outputs/MNQ-002-ninjatrader-export-comparison.json`

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

## Overnight Negative Filter Diagnostic

Definition: overnight negative means current RTH entry open is below the previous RTH session close.

Generated with:

```bash
python3 scripts/mnq002_overnight_filter.py --rth-input data/processed/DATA-001/ohlcv_1m.csv --eth-input data/processed/DATA-002/ohlcv_1m.csv --output-dir outputs --hold-minutes 30 --min-bars 300 --point-value 20 --round-turn-cost-points 0
```

Artifacts:

- `outputs/MNQ-002-overnight-filter-summary.json`
- `outputs/MNQ-002-overnight-negative-trades.csv`
- `outputs/MNQ-002-overnight-non-negative-trades.csv`

Gross zero-cost result:

| Filter | Trades | Avg Points | Win Rate | Profit Factor | Max DD |
| --- | ---: | ---: | ---: | ---: | ---: |
| Overnight negative | 963 | 2.14 | 53.17% | 1.0843 | -1,367.25 pts |
| Overnight non-negative | 1,221 | 0.05 | 51.35% | 1.002 | -2,227.5 pts |

Interpretation: overnight negative improves the benchmark and appears directionally useful. It is still weak before costs/slippage, has poor years such as 2022 and 2024, and still has no stop/target R:R. Treat as a candidate filter family, not a strategy.

## TradingView Strategy Tester

Artifact: `outputs/MNQ-002-open-long-strategy.pine`.

Purpose: visual Strategy Tester review of the MNQ-002 baseline and overnight-negative filter directly on a TradingView 1m NQ/MNQ chart.

Important caveat: the local Python baseline enters at the first RTH bar open. Pine strategy orders signaled on the first RTH bar generally fill according to TradingView's strategy fill model, often next bar open unless Strategy Tester properties are changed. Treat this Pine script as a visual sanity-check layer, not as a perfect duplicate of the CSV backtest.

## NinjaTrader 8 Strategy Analyzer

Artifact: `outputs/MNQ002OpenLongStrategy.cs`.

Purpose: NinjaTrader 8 Strategy Analyzer and chart-strategy version of MNQ-002 with the same all/overnight-negative/overnight-non-negative filter modes.

Important caveat: as with TradingView, a 1m OHLC Strategy Analyzer run may not replicate the local Python first-RTH-open fill exactly unless order timing and fill resolution are configured for that purpose. Use this as a platform-side review tool before considering any execution logic.

Install note: this artifact is NinjaScript/C# and must be compiled as a NinjaTrader 8 Strategy `.cs` file. It is not JavaScript; pasting it into a `.js` editor will fail on C# syntax.

Current platform note: user screenshot shows a NinjaTrader Web / Tradovate-style Code Editor using `.js`, and no visible Strategy Analyzer. For that environment, use the Python backtest as the statistical source of truth and build a JavaScript visual indicator/marker script rather than a C# Strategy Analyzer script.

First NT8 export comparison:

- Source files: `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-11 12-53.csv`, `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-11 12-54.csv`, `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-11 12-55.csv`, and `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-11 01-00.csv`.
- User confirmed `12-53.csv` is `All`, `12-55.csv` is `OvernightNonNegativeOnly`, and `01-00.csv` is `OvernightNegativeOnly`. `12-54.csv` is a duplicate export of `All`.
- The all-style run reports 2,454 trades, $72,725 gross net profit, 52.49% win rate, $29.64 average trade, PF 1.07, and max drawdown -$43,340 with 0 fees/slippage. Using NQ $20/point, this equals about +1.482 points/trade and -2,167 points max drawdown, matching the local Python baseline closely.
- `OvernightNonNegativeOnly` reports 1,367 trades, $15,100 gross net profit, 51.06% win rate, $11.05 average trade, PF 1.03, and max drawdown -$34,460 with 0 fees/slippage. Using NQ $20/point, this equals about +0.553 points/trade and -1,723 points max drawdown.
- `OvernightNegativeOnly` reports 1,086 trades, $57,565 gross net profit, 54.24% win rate, $53.01 average trade, PF 1.12, and max drawdown -$22,085 with 0 fees/slippage. Using NQ $20/point, this equals about +2.651 points/trade and -1,104.25 points max drawdown.
- Interpretation: the NT8 split confirms that overnight-negative carries most of the observed edge, while non-negative is weak. Still not strategy-grade: PF 1.12 is modest, max recovery is 856 days, and there is still no stop/target R:R.

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
