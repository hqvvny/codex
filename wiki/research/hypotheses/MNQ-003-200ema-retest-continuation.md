---
type: strategy-hypothesis
updated: 2026-07-21
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
- `outputs/MNQ-003-ema-retest-depth-report.md`
- `outputs/MNQ-003-ema-retest-depth-summary.csv`
- `outputs/MNQ-003-ema-retest-depth-summary.json`
- `outputs/MNQ003EmaLimitEntryStrategy.cs`

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

## NinjaTrader 8 EMA Limit Entry Variant

Artifact: `outputs/MNQ003EmaLimitEntryStrategy.cs`.

Purpose: visual Strategy Analyzer / chart-strategy variant where entry is placed directly at the 200 EMA with a fixed 50-point stop.

Default parameters:

- `EmaPeriod = 200`.
- `TrendBars = 10`.
- `EntryOffsetPoints = 0`, so entry limit is exactly at the EMA.
- `StopLossPoints = 50`.
- `UseProfitTarget = 1`.
- `TargetPoints = 50`, so default target is 1R with the 50-point stop. Set `UseProfitTarget = 0` to test stop-only plus session close / max-hold behavior.
- `MaxHoldBars = 0`, meaning no time exit. Set a positive number for max-hold exits.
- `DirectionMode = 0`, both long and short. Use `1` for long-only and `2` for short-only.

Implementation note: this version does not wait for a rejection close. If trend is confirmed above EMA, it posts a buy limit at the EMA. If trend is confirmed below EMA, it posts a sell-short limit at the EMA. This is a different hypothesis branch than the first Python test.

User-reported NT8 result for EMA limit-entry variant:

- Direction: long-only (`DirectionMode = 1`).
- Entry: buy limit at 200 EMA.
- Stop: 50 points.
- Target: 50 points, 1:1R.
- Fees: included in NinjaTrader result.
- Net profit: about $167,000.
- Win rate: 52.11%.
- Sharpe ratio: 0.24.
- Sortino ratio: 0.41.
- Max recovery time: 441 days.

Interpretation: this is the first promising MNQ-003 branch. A 52.11% win rate at 1:1R after fees is directionally meaningful. However, Sharpe 0.24 and 441-day recovery are still weak from a prop-firm evaluation perspective. This needs exported Strategy Analyzer data before conclusions: trade count, profit factor, max drawdown, year split, exact date range, instrument, trading-hours template, commission template, and slippage/fill settings.

User-reported NT8 1.5R result for same long-only branch:

- Direction: long-only (`DirectionMode = 1`).
- Entry: buy limit at 200 EMA.
- Stop: 50 points.
- Target: 75 points, 1.5:1R.
- Fees: included in NinjaTrader result.
- Win rate: 45.60%.
- Sharpe ratio: 0.31.
- Sortino ratio: 0.54.
- Max recovery time: 357 days.

Interpretation: 1.5R improves Sharpe, Sortino, and max recovery versus the 1:1R result, while the win rate falls as expected. This is a healthier risk-adjusted direction than the 1R branch, but R:R only just meets the minimum standard and still needs exported Strategy Analyzer data before comparing net expectancy, drawdown shape, yearly consistency, and profit factor.

User-reported NT8 2R result for same long-only branch:

- Direction: long-only (`DirectionMode = 1`).
- Entry: buy limit at 200 EMA.
- Stop: 50 points.
- Target: 100 points, 2:1R.
- Fees: included in NinjaTrader result.
- Net profit: about $318,000.
- Win rate: 41.80%.
- Sharpe ratio: 0.35.
- Sortino ratio: 0.66.
- Max recovery time: 354 days.

Interpretation: 2R is the best user-reported risk-adjusted variant so far among 1R, 1.5R, and 2R. It has the lowest win rate, as expected, but the highest Sharpe and Sortino and the highest reported net profit, while max recovery is slightly better than 1.5R. It still needs exported Strategy Analyzer data before drawing conclusions about profit factor, drawdown shape, outlier dependence, and year-by-year consistency.

User-reported NT8 2R result with stronger trend filter:

- Direction: long-only (`DirectionMode = 1`).
- Entry: buy limit at 200 EMA.
- Trend filter: 35 trend bars.
- Stop: 50 points.
- Target: 100 points, 2:1R.
- Fees: included in NinjaTrader result.
- Net profit: about $316,100.
- Win rate: 42.00%.
- Sharpe ratio: 0.37.
- Sortino ratio: 0.72.
- Max recovery time: 357 days.

Interpretation: the 35-bar trend filter slightly improves win rate, Sharpe, and Sortino versus the default 10-bar 2R run, but gives up a small amount of net profit and has similar max recovery. This suggests trend-filter strength matters, but the improvement is incremental rather than a decisive breakthrough.

User-reported year consistency note for the 2R long-only branch: 2024 was the only negative year; all other years were positive. This improves the interpretation of the long max-recovery statistic: the problem appears concentrated in one weak regime/year rather than broad year-by-year failure. Needs Strategy Analyzer export before verifying exact year split, date range, and whether the 2024 drawdown explains the reported max recovery.

Verified NT8 trade-list review for 2R / 35 trend bars:

- Source export: `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-20 07-33.csv`.
- Review artifact: `outputs/MNQ-003-nt8-tradelist-2026-07-20-0733-report.md`.
- Platform/instrument: NinjaTrader Strategy Analyzer trade list, `NQ 09-26`, 1 contract.
- Date range in trade list: 2016-01-04 20:01 to 2026-07-17 20:16.
- Trades: 5,489.
- Fees: included, $28,323.24 total, about $5.16 per trade.
- Net profit: $316,128.76.
- Profit factor: 1.109.
- Win rate: 42.03%.
- Average trade: $57.59.
- Trade-list max drawdown: -$46,336.40.
- Longest closed recovery calculated from trade-list equity: about 360.7 days.
- Year split: 2024 is the only negative year at -$25,974.40; all other calendar years are positive.
- 2024 weak clusters: March-April and September-November.
- Exit mix: stop loss 2,764 trades for -$2.78M; profit target 1,307 trades for +$2.61M; session-close exits 1,418 trades for +$489.8k.

Interpretation update: the exported trade list confirms the user-reported 2R / 35-trend-bars headline. The result is promising because it is positive in 10 of 11 calendar years after fees, but profit factor is modest and 2024 shows a real regime failure. Session-close exits contribute materially, so this branch should be treated as "2R bracket plus session-close behavior", not a pure fixed 2R system.

NY-session-only export check:

- User later identified `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-20 07-33.csv` as the same setup but NY-session-only.
- File check indicates it is not NY-session-only: it still contains 5,489 trades from 2016-01-04 to 2026-07-17, matching the prior full-session trade-list review.
- Entries appear across almost all platform hours, including 00:00-14:00, with only about 34.5% of entries falling in a broad 15:00-22:00 platform-time bucket.
- Conclusion: treat this file as the prior broad-session export until a true NY-session-only export is provided. A true NY-only export should have materially fewer trades and entry times constrained to the NY/RTH session.

Verified NY-session-only summary export:

- Source export: `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-20 07-46.csv`.
- Review artifact: `outputs/MNQ-003-nt8-ny-session-summary-2026-07-20-0746-report.md`.
- Session context from user: same 2R / 35 trend bars branch, but NY session only.
- Export type: Strategy Analyzer summary, not trade list.
- Date range shown in summary: 2016-01-01 to 2026-07-20.
- Trades: 2,792.
- Fees: included; exported fees are $14,406.72.
- Slippage: 0.
- Net profit: $124,356.28.
- Profit factor: 1.11.
- Win rate: 46.42%.
- Max drawdown: -$20,834.88.
- Sharpe ratio: 0.22.
- Sortino ratio: 0.36.
- Max recovery time: 489 days.
- Average trade: $44.54.

Interpretation update: NY-only reduces trade count, net profit, and absolute drawdown, while win rate improves and profit factor is roughly unchanged versus broad session. However, Sharpe, Sortino, average trade, and max recovery are worse. This is not a clear improvement over the broad-session branch unless the user's main objective is lower absolute drawdown. A NY-only trade-list export is needed to check year split and whether 2024 improves.

Verified Asia-to-NY-open summary export:

- Source export: `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-20 07-52.csv`.
- Review artifact: `outputs/MNQ-003-nt8-asia-to-ny-open-summary-2026-07-20-0752-report.md`.
- Session context from user: same 2R / 35 trend bars branch, Asia through NY open.
- Export type: Strategy Analyzer summary, not trade list.
- Date range shown in summary: 2016-01-01 to 2026-07-20.
- Trades: 3,715.
- Fees: included; exported fees are $19,169.40.
- Slippage: 0.
- Net profit: $201,095.60.
- Profit factor: 1.10.
- Win rate: 40.59%.
- Max drawdown: -$40,737.80.
- Sharpe ratio: 0.27.
- Sortino ratio: 0.52.
- Max recovery time: 531.98 days.
- Average trade: $54.13.

Interpretation update: Asia-to-NY-open keeps more of the broad-session profit than NY-only and has a stronger average trade than NY-only, but drawdown remains close to broad session and recovery is the worst of the tested session slices. This is not a clear improvement; it mainly shows that pre-NY contributes meaningful profit but also long stagnation risk.

## NinjaTrader 8 Advanced Filter Variant

Artifact: `outputs/MNQ003EmaLimitEntryAdvancedStrategy.cs`.

Purpose: keep the 200 EMA limit-entry core but make the next research filters testable from Strategy Analyzer without editing code.

Default core setup:

- `DirectionMode = 1`, long-only.
- `EmaPeriod = 200`.
- `TrendBars = 35`.
- `EntryOffsetPoints = 0`.
- `StopLossPoints = 50`.
- `UseProfitTarget = 1`.
- `TargetPoints = 100`, matching 2:1R.
- `MaxHoldBars = 0`.

Optional filters and controls:

- Session windows: `UseSessionWindows`, `Session1StartTime`, `Session1EndTime`, plus optional Session 2 and Session 3.
- Weak-hour exclusion: `UseWeakHourFilter`, `WeakHoursCsv` defaulting to `10,11,21` platform hours from the broad-session trade-list diagnostic.
- EMA slope filter: `UseEmaSlopeFilter`, `EmaSlopeLookbackBars`, `MinEmaSlopePoints`.
- Higher-timeframe trend filter: `UseHtfTrendFilter`, `HtfBarsPeriodMinutes`, `HtfEmaPeriod`, `HtfSlopeLookbackBars`, `MinHtfSlopePoints`.
- ATR regime filter: `UseAtrFilter`, `AtrPeriod`, `MinAtrPoints`, `MaxAtrPoints`.
- Trade frequency controls: `CooldownBars`, `MaxTradesPerSession`, `MaxTradesPerDay`.
- Time exit: `UseTimeExitBeforeSessionEnd`, `TimeExitTime`.

Initial suggested tests:

- Baseline compatibility: all optional filters off, core defaults should approximate the prior 2R / 35-trend-bars behavior.
- EMA slope only: `UseEmaSlopeFilter = 1`, vary `EmaSlopeLookbackBars` 10/20/40 and `MinEmaSlopePoints` 2.5/5/10.
- Weak hours only: `UseWeakHourFilter = 1` with `WeakHoursCsv = 10,11,21`.
- HTF trend only: `UseHtfTrendFilter = 1`, test `HtfBarsPeriodMinutes` 15 and 60.
- Cooldown only: test `CooldownBars` 30/60/120.
- Max trades: test `MaxTradesPerSession = 1` and `MaxTradesPerDay = 1`.

Advanced baseline verification:

- Source export: `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-20 10-27.csv`.
- Review artifact: `outputs/MNQ-003-nt8-advanced-baseline-tradelist-2026-07-20-1027-report.md`.
- Strategy: `MNQ003EmaLimitEntryAdvancedStrategy`.
- Intended configuration: all new optional filters disabled, matching 2R / 35-trend-bars long-only baseline.
- Trades: 5,489, exactly matching the prior simple-strategy baseline.
- Net profit: $297,643.36 after fees.
- Profit factor: 1.102.
- Win rate: 41.94%.
- Fees: $31,616.64 total, about $5.76 per trade.
- Trade-list max drawdown: -$48,082.40.
- Longest closed recovery: about 360.8 days.
- Year split: 2024 remains the only negative year; 2016 is nearly flat at $268.28 after the higher fee assumption.

Interpretation update: the advanced strategy passes compatibility on signal count and broad behavior. The lower net profit versus the simple baseline appears mostly due to a different/higher fee template plus minor fill-price differences. Keep commission template and fill settings identical for all future filter comparisons.

Slippage 1 baseline verification:

- Source export: `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-21 12-23.csv`.
- Review artifact: `outputs/MNQ-003-nt8-slippage1-tradelist-2026-07-21-1223-report.md`.
- Strategy: `MNQ003EmaLimitEntryStrategy`.
- User-stated configuration: long-only, whole day, 200 EMA, entry offset 0 points, 35 trend bars, max hold bars 0, quantity 1, 50-point stop, 100-point target, NinjaTrader monthly fees, slippage 1, since 2016.
- Date range in trade list: 2016-01-04 20:01 to 2026-07-20 18:42.
- Trades: 5,494.
- Net profit: $298,890.96 after fees and slippage.
- Fees: $28,349.04 total, about $5.16 per trade.
- Profit factor: 1.103.
- Win rate: 41.92%.
- Average trade: $54.40.
- Trade-list max drawdown: -$47,788.40.
- Longest closed recovery: about 360.8 days.
- Year split: 2024 remains the only negative year; 2016 and 2022 remain thin positive.
- 2024 weak clusters remain March-April and September-November.
- Weak platform entry hours in this export include 10, 21, 2, and 11.

Interpretation update: this is the most realistic baseline so far because it includes slippage 1 and fees. The strategy remains promising, but the same weaknesses remain: modest PF around 1.10, about one-year recovery, 2024 regime failure, and material dependence on session-close exits.

## NinjaTrader 8 Clean Filter Variant

Artifact: `outputs/MNQ003EmaLimitEntryCleanStrategy.cs`.

Purpose: replace the overloaded Advanced variant for practical Strategy Analyzer testing. This version keeps only the parameters we actually want to vary next.

Core defaults:

- `OrderQuantity = 1`.
- `DirectionMode = 1`, long-only.
- `EmaPeriod = 200`.
- `TrendBars = 35`.
- `EntryOffsetPoints = 0`.
- `StopLossPoints = 50`.
- `TargetPoints = 100`, 2:1R.
- `MaxHoldBars = 0`.

Usable filters:

- Trade window: `UseTradeWindow`, `TradeStartTime`, `TradeEndTime`.
- Weak-hour exclusion: `UseWeakHourFilter`, `WeakHoursCsv`, default `10,11,21,2`.
- EMA slope: `UseEmaSlopeFilter`, `EmaSlopeLookbackBars`, `MinEmaSlopePoints`.
- Trade frequency: `CooldownBars`, `MaxTradesPerSession`.

Removed from the overloaded Advanced version: HTF trend filter, ATR filter, multiple session windows, max trades per day, and time-exit-before-session-end. These can be reintroduced only if a clean single-filter test proves they are needed.

Recommended first tests:

- Baseline: all filters off.
- Weak hours only: `UseWeakHourFilter = 1`, `WeakHoursCsv = 10,11,21,2`.
- EMA slope only: `UseEmaSlopeFilter = 1`, `EmaSlopeLookbackBars = 20`, `MinEmaSlopePoints = 5`.
- Cooldown only: `CooldownBars = 60`.
- Max trades only: `MaxTradesPerSession = 1`.

Python/offline filter scout:

- Report: `outputs/MNQ-003-filter-scout-2026-07-21.md`.
- Source baseline: `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-21 12-23.csv`.
- Method: fast offline filters applied to the already exported NT trade list, preserving baseline Ninja fills but not changing later signal generation. Treat as scouting, not final Strategy Analyzer proof.
- Baseline: 5,494 trades, $298,890.96 net, PF 1.103, 41.92% win rate, -$47,788.40 max drawdown, about 360.8-day recovery, 2024 net -$27,758.40.
- Best conservative candidate: skip platform hours 10, 11, and 21. Offline result: 4,948 trades, $338,788.32 net, PF 1.128, 42.06% win rate, -$32,929.92 max drawdown, 350.7-day recovery, 2024 net -$18,078.00.
- Best aggressive candidate: skip platform hours 10, 11, 21, and 2. Offline result: 4,478 trades, $344,918.52 net, PF 1.146, 42.32% win rate, -$25,512.60 max drawdown, 406.0-day recovery, 2024 net -$4,445.48; drawback is that 2016 turns negative.
- Cooldown filters were not attractive in the offline scout; they generally reduced net profit and worsened recovery.
- Max trades per day/session proxies were not attractive as standalone filters.
- EMA slope checks using DATA-002 were directionally mixed: PF/avg trade improved in stricter variants, but trade count collapsed and recovery became much worse. Do not prioritize EMA slope until weak-hour filters are confirmed in real NT8 runs.

Next NT8 test priority: first run Clean strategy with `UseWeakHourFilter = 1`, `WeakHoursCsv = 10,11,21`; second run `UseWeakHourFilter = 1`, `WeakHoursCsv = 10,11,21,2`.

Verified NT8 weak-hour filter result:

- Source export: `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-21 06-49.csv`.
- Review artifact: `outputs/MNQ-003-nt8-weak-hours-10-11-21-tradelist-2026-07-21-0649-report.md`.
- Strategy: `MNQ003EmaLimitEntryCleanStrategy`.
- User context: weak-hour filter enabled with the first/conservative variant, intended platform hours `10,11,21`.
- Date range in trade list: 2016-01-04 20:01 to 2026-07-20 18:42.
- Trades: 5,104.
- Net profit: $329,006.56 after fees.
- Fees: $22,253.44 total, about $4.36 per trade.
- Profit factor: 1.121.
- Win rate: 42.07%.
- Average trade: $64.46.
- Trade-list max drawdown: -$37,140.72.
- Longest closed recovery: about 354.7 days.
- 2024 net: -$18,990.20.
- Year split: 2016 turns slightly negative at -$799.76; 2024 remains negative but improves versus baseline.
- Hour check: hours 10 and 11 are absent; hour 21 still has 7 entries for -$5,145.52, likely due to timestamp/session-boundary behavior or filter setup details.

Interpretation update: the first weak-hour filter variant is confirmed as an improvement direction versus the slippage-1 baseline: higher net, PF, win rate, average trade, lower max drawdown, slightly shorter recovery, and less 2024 damage. It does not solve 2024, and the fee template appears different from the baseline, so the next comparison should keep commission/slippage settings identical.

Verified NT8 aggressive weak-hour filter result:

- Source export: `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-21 07-01.csv`.
- Review artifact: `outputs/MNQ-003-nt8-weak-hours-10-11-21-2-tradelist-2026-07-21-0701-report.md`.
- Strategy: `MNQ003EmaLimitEntryCleanStrategy`.
- User context: weak-hour filter enabled with the aggressive variant, intended platform hours `10,11,21,2`.
- Date range in trade list: 2016-01-04 20:01 to 2026-07-20 18:42.
- Trades: 4,999.
- Net profit: $355,414.36 after fees.
- Fees: $21,795.64 total, about $4.36 per trade.
- Profit factor: 1.134.
- Win rate: 42.37%.
- Average trade: $71.10.
- Trade-list max drawdown: -$36,977.64.
- Longest closed recovery: about 355.8 days.
- 2024 net: -$19,152.12.
- Year split: only 2024 is negative; 2016 remains barely positive at $292.68.
- Hour check: hours 10 and 11 are absent; hour 2 still has 5 entries and hour 21 still has 7 entries, likely due to timestamp/session-boundary behavior or filter setup details.

Interpretation update: this is the best headline MNQ-003 variant so far among tested weak-hour filters. It improves net profit, PF, win rate, average trade, and drawdown versus baseline, while keeping 2016 barely positive. It does not improve 2024 versus the conservative weak-hour filter, so the next research target should be 2024 regime/session behavior rather than more hour pruning.

Weak-hour OOS / overfit check:

- Report: `outputs/MNQ-003-weak-hour-oos-2026-07-21-report.md`.
- Detailed CSV: `outputs/MNQ-003-weak-hour-oos-2026-07-21.csv`.
- Source baseline: `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-21 12-23.csv`.
- Method: offline hour filters applied to the baseline NT trade list.
- Baseline 2022-2026 OOS: 3,119 trades, $150,805.96 net, PF 1.081, -$47,788.40 max drawdown.
- Skip 10/11/21/2 2022-2026 OOS: 2,550 trades, $196,722 net, PF 1.130, -$24,222 max drawdown.
- Skip 10/11/21 2022-2026 OOS: 2,771 trades, $178,537 net, PF 1.107, -$32,930 max drawdown.
- A brute-force search of 1- to 4-hour combinations optimized on 2016-2021 showed classic overfitting: the best IS hour sets often degraded 2022-2026 badly.

Interpretation update: the weak-hour concept still has overfit risk, but `10/11/21/2` passes a useful OOS smoke test. It is not merely one of the top in-sample-mined combinations; it leaves 2016-2021 essentially unchanged while improving the later and harder 2022-2026 period. Treat it as a plausible session-quality filter, not a proven standalone edge.

Verified NT8 aggressive weak-hour filter with baseline-like fees:

- Source export: `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-21 07-19.csv`.
- Review artifact: `outputs/MNQ-003-nt8-weak-hours-10-11-21-2-fee-matched-tradelist-2026-07-21-0719-report.md`.
- Strategy: `MNQ003EmaLimitEntryCleanStrategy`.
- User context: aggressive weak-hour filter, intended platform hours `10,11,21,2`.
- Date range in trade list: 2016-01-04 20:01 to 2026-07-20 18:42.
- Trades: 4,999.
- Net profit: $351,415.16 after fees.
- Fees: $25,794.84 total, about $5.16 per trade, matching the slippage-1 baseline fee template.
- Profit factor: 1.133.
- Win rate: 42.15%.
- Average trade: $70.30.
- Trade-list max drawdown: -$37,336.84.
- Longest closed recovery: about 355.8 days.
- 2024 net: -$19,605.72.
- Year split: only 2024 is negative; 2016 is nearly flat at $83.08.

Interpretation update: this removes the biggest caveat from the prior aggressive weak-hour export. With the same approximate per-trade fee template as the slippage-1 baseline, `10/11/21/2` remains a clear improvement: higher net profit, PF, win rate, average trade, and lower max drawdown. It is the current best MNQ-003 candidate configuration, pending 2024 regime validation and exact platform-hour/session mapping.

LucidFlex funded-account risk review:

- Report: `outputs/MNQ-003-lucid-flex-risk-report-2026-07-21.md`.
- Detailed sizing CSV: `outputs/MNQ-003-lucid-flex-risk-sizing-2026-07-21.csv`.
- 50k expectancy sketch: `outputs/MNQ-003-lucid-flex-50k-expectancy-2026-07-21.md`.
- 50k 2 MNQ expectancy sketch: `outputs/MNQ-003-lucid-flex-50k-2mnq-expectancy-2026-07-21.md`.
- Source export: `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-21 07-19.csv`.
- LucidFlex rules checked from Lucid Trading Help Center on 2026-07-21: no daily loss limit, no consistency rule, account MLL is EOD trailing drawdown, and max position limits are much larger than the safe size for this strategy.
- Simulation method: EOD balance by trade exit date, no payout withdrawals, no intraday mark-to-market breach model, source 1 NQ PnL scaled linearly to MNQ equivalents.
- 1 NQ source size breaches every listed LucidFlex account size early in 2016.
- $25k is not viable with normal 1 MNQ execution; historical path breaches even at 1 MNQ.
- $50k survives at 1 MNQ, with minimum modeled buffer about $709.
- $100k survives at 2 MNQ, with minimum modeled buffer about $418; 1 MNQ is safer.
- $150k survives at 3 MNQ, with minimum modeled buffer about $627; 2 MNQ is safer.
- Main risk read: MNQ-003 should be treated as a micro-contract strategy for LucidFlex. The binding constraint is not Lucid's max position size, it is the account's EOD MLL buffer during clustered losing periods.
- For 50k at 1 MNQ, historical average is about $7.03 per trade or $13.05 per trading day before profit split. At 90% split, the 90 EUR account fee breaks even after roughly 8-9 average trading days, before payout-rule caveats.
- Rolling-start 50k/1 MNQ sketches show positive approximate EV after the 90 EUR fee across 20-252 trading day horizons, but breach risk rises from about 0.34% over 20 days to about 13.56% over 252 days.
- For 50k at 2 MNQ, average expectancy doubles to about $14.06 per trade or $26.11 per trading day before split, but the actual full historical path breaches on 2016-02-05 after 24 trading days. Rolling-start breach risk is about 14.70% over 20 days, 23.24% over 30 days, 38.02% over 60 days, and 47.69% over 252 days, so 2 MNQ is too fragile from account start despite positive average EV.

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

## Retest Depth Diagnostic

Generated from `outputs/MNQ-003-ema-retest-trades.csv`.

Definitions:

- Long retest depth uses signal candle low relative to EMA.
- Short retest depth uses signal candle high relative to EMA.
- Penetration means price actually pushed through the EMA in the wrong direction; trend-side touches count as 0 penetration.

| Side | Signals | Avg Abs Distance | Median Abs Distance | P90 Abs Distance | Crossed EMA | Avg Penetration When Crossed |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| All | 26,396 | 5.36 pts | 5.53 pts | 9.37 pts | 8.74% | 1.87 pts |
| Long | 14,574 | 5.43 pts | 5.62 pts | 9.41 pts | 8.52% | 1.89 pts |
| Short | 11,822 | 5.27 pts | 5.39 pts | 9.32 pts | 9.02% | 1.85 pts |

Interpretation: the 10-point tolerance includes many retests that never actually pierce the EMA. Most signals are trend-side touches inside the tolerance band. When price does pierce the EMA, it only goes through by about 1.9 points on average. Next test should compare 2/5/7.5/10 point tolerance and split trend-side touches from pierce-and-reclaim/reject events.

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
