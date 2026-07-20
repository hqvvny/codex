---
type: strategy-hypothesis
updated: 2026-07-20
status: raw
verdict: untested
---

# MNQ-003 200 EMA Retest Continuation

## One-Line Thesis

On the 1m chart, if price is below the 200 EMA and retests it from below, short the rejection; if price is above the 200 EMA and retests it from above, long the rejection.

## Market Structure Context

- Instrument: NQ/MNQ, first test using DATA-001/DATA-002 NQU6 proxy.
- Session: not fixed yet; must be tested by session, especially NY open, London open, and full RTH.
- Trend/bias context: 200 EMA on 1m is the directional filter.
- Key liquidity levels: not included yet; future variant can add prior RTH high/low, overnight high/low, VWAP, and session open.
- Price relative to key levels: initial rule only uses price relative to 200 EMA.

## Edge Claim

- Structural reason: the 200 EMA can act as a widely watched dynamic trend boundary. In trend conditions, a retest may attract continuation traders and trap countertrend attempts.
- Who is likely trapped or forced: traders fading the move into the EMA may be forced out if price rejects and resumes trend; late breakout traders may use the retest as continuation entry.
- Why this should persist: trend-following and moving-average retest behavior is common, but it can decay badly in chop. This needs regime/session filters.

## Setup Definition

Initial mechanical version for first test:

- EMA: 200-period EMA on 1m close.
- Long bias: close is above the 200 EMA.
- Short bias: close is below the 200 EMA.
- Long retest: price was above the EMA, then the bar low touches or comes within a tolerance of the EMA, and the bar closes back above the EMA.
- Short retest: price was below the EMA, then the bar high touches or comes within a tolerance of the EMA, and the bar closes back below the EMA.
- Entry condition: enter on the close of the rejection bar, or next bar open in conservative backtest.
- Stop condition: not final. Candidate: fixed points, EMA plus/minus buffer, or rejection candle extreme plus buffer.
- Target condition: not final. Candidate: fixed R multiple, time exit, or opposite EMA recross.
- Invalidation: price closes decisively on the wrong side of the EMA after entry, or stop is hit.
- Expected R:R: must be tested at >= 1.5R. Anything below 1.5R should be rejected or labeled as substandard.

## Data Requirements

- Data source: DATA-001 RTH 1m and DATA-002 ETH/all-sessions 1m are available locally.
- Date range: DATA-001/DATA-002 cover 2017-04-17 to 2026-07-10.
- Timeframe: 1m.
- Session calendar: must split at least RTH, NY open window, London open window, and full ETH if using DATA-002.
- Costs: not set yet; first pass may use 0 points, but all reported stats must include cost provenance.
- Slippage: not set yet; first pass may use 0 points, but robustness must include slippage.
- Known gaps: contract-roll handling still not fully confirmed; 1m OHLC cannot resolve intrabar stop/target order when both are touched in one bar.

## Sample Size Expectation

Likely enough occurrences across 2017-2026, but sample size depends heavily on retest tolerance, trend-confirmation requirement, and cooldown rules.

## First Filter

- Structural reason: plausible but generic; needs proof that it is not just a common moving-average illusion.
- Testable with available data: yes.
- Real sample size: likely, not proven yet.
- PA or mechanical: hybrid. EMA is indicator-based; retest/rejection needs a mechanical definition or visual confirmation.
- R:R >= 1.5: not proven. Must be explicitly tested.
- Verdict: raw candidate. Needs exact retest, stop, target, and session rules before build/test.

## Build/Test Notes

First-pass test grid should avoid overfitting:

- Retest tolerance: 0, 2, 5, 10 points from EMA.
- Trend confirmation: close above/below EMA for 5, 10, or 20 prior bars.
- Entry: next bar open after rejection close.
- Stop: rejection candle extreme plus 2 points, or fixed 10/15/20/30 points.
- Target: 1.5R, 2R, 2.5R.
- Cooldown: at most one trade per direction per session, or one trade every 30 bars.
- Session splits: NY open, London open, full RTH, full ETH.

## Backtest Provenance

- Data: not run yet.
- Date range: not run yet.
- Costs: not run yet.
- Slippage: not run yet.
- Version/commit: pending.

## Robustness Battery

Required before any promotion:

- Compare long and short separately.
- Split by year and session.
- Compare against random-entry and EMA-cross baseline.
- Test sensitivity to tolerance and trend-confirmation length.
- Add realistic NQ/MNQ commission and slippage.
- Check same-bar stop/target ambiguity if using 1m OHLC.
- Inspect drawdown duration, not only PnL.
- Confirm it does not duplicate MNQ-002 open-drift exposure.

## Review Gates

- Independent review: not started.
- Manual review: not started.
- Secondary-account observation: not started.

## Outcome

Captured as a raw hypothesis. Do not build live/execution logic until the retest and risk rules are made mechanical and the first-pass backtest proves the idea survives costs and session splits.

## Links

- [[../strategy-queue]]
- [[../../reference/first-filter]]
- [[../../reference/backtest-battery]]
- [[../../reference/data-inventory]]
- [[../../concepts/trading-analysis-profile]]
