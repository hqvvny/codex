---
type: strategy-hypothesis
updated: 2026-07-20
status: testing
verdict: failed-first-pass
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
- Retest tolerance: maximum 10 points over/under the EMA; if price pushes more than 10 points through the EMA, no entry.
- Entry condition: next bar open after rejection candle.
- Stop condition: first mechanical version uses rejection candle extreme plus 2 points buffer.
- Target condition: 1R target per user specification.
- Trade frequency: multiple trades allowed, but no overlapping positions in the first Python test.
- Invalidation: price closes decisively on the wrong side of the EMA after entry, or stop is hit.
- Expected R:R: first user-specified version uses 1R. This is below the normal >=1.5R research standard and must be labeled as such.

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

- Data: DATA-001 MotiveWave NQU6 1m RTH export.
- Date range: 2017-04-17 to 2026-07-10.
- Costs: 0.0 points round turn.
- Slippage: 0.0 points.
- Version/commit: pending at time of run.

Generated with:

```bash
python3 scripts/mnq003_ema_retest.py --input data/processed/DATA-001/ohlcv_1m.csv --output-dir outputs --ema-period 200 --tolerance-points 10 --trend-bars 10 --stop-buffer-points 2 --risk-reward 1.0 --same-bar-policy stop_first --point-value 20 --round-turn-cost-points 0
```

Artifacts:

- `scripts/mnq003_ema_retest.py`
- `outputs/MNQ-003-ema-retest-report.md`
- `outputs/MNQ-003-ema-retest-summary.json`
- `outputs/MNQ003EmaRetestStrategy.cs`

## NinjaTrader 8 Visual Backtest

Artifact: `outputs/MNQ003EmaRetestStrategy.cs`.

Purpose: visual Strategy Analyzer / chart-strategy version of the first MNQ-003 mechanical rule, so trades can be inspected in NinjaTrader 8.

Default parameters:

- `EmaPeriod = 200`.
- `MaxRetestPoints = 10`.
- `TrendBars = 10`.
- `StopBufferPoints = 2`.
- `RiskReward = 1`.
- `DirectionMode = 0`, meaning both long and short. Use `1` for long-only, `2` for short-only.
- `TradeStartTime = 0`, `TradeEndTime = 235959`, so session scope is controlled mainly by the NinjaTrader data series/trading hours template unless these fields are changed.

Implementation note: entry is signaled on the rejection candle close. Stop is set at the rejection candle extreme plus/minus buffer. Target is calculated after entry fill using actual fill price, so 1R is based on the platform fill rather than the signal candle close.

## First Python Result

Conservative same-bar policy: if stop and target are both touched inside the same 1m candle, stop fills first.

| Side | Trades | Net Pts | Avg Pts | Win Rate | PF | Max DD Pts |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| All | 26,396 | -2,911.50 | -0.110 | 48.09% | 0.957 | -3,735.00 |
| Long | 14,574 | -701.50 | -0.048 | 48.44% | 0.981 | -1,704.25 |
| Short | 11,822 | -2,210.00 | -0.187 | 47.65% | 0.930 | -2,352.75 |

Interpretation: the raw full-RTH 1m EMA retest rule fails before costs under conservative fills. Longs are near breakeven before costs; shorts are clearly weak.

Sensitivity:

- Adding max risk cap of 10 points worsens the result: all-trades PF 0.928 and avg -0.159 points.
- `target_first` same-bar policy flips the result positive with all-trades PF 1.100, but this is not trustworthy on 1m OHLC because it assumes target fills before stop inside ambiguous candles.
- The gap between stop-first and target-first proves the setup is highly intrabar-fill sensitive.

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

First mechanical version fails under conservative assumptions. Do not optimize the raw full-RTH 1m rule directly. If continuing, test a more structural second version: long-only, session-window restriction, EMA slope/HTF trend filter, or displacement requirement before retest.

## Links

- [[../strategy-queue]]
- [[../../reference/first-filter]]
- [[../../reference/backtest-battery]]
- [[../../reference/data-inventory]]
- [[../../concepts/trading-analysis-profile]]
