---
type: reference
updated: 2026-07-10
status: active
verdict: authoritative-link
---

# Data Inventory

This page registers local datasets. It links to raw sources and records provenance; it does not copy raw data into the wiki as source of truth.

## Registered Datasets

| ID | Path | Instrument Label | Timeframe | Session Label | Rows | First Timestamp | Last Timestamp | Status | Notes |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| DATA-001 | `/Users/farell.trades/NQU6 - 1 min - RTH.csv` | NQU6 / likely NQ continuous or current-contract export label | 1 minute | RTH | 1,006,460 | 2017-04-17 17:55 | 2026-07-10 20:17 | usable-for-research | No header. Format appears `YYYYMMDD HHMMSS,open,high,low,close,volume`. Ingestor profile found no parse errors, duplicate timestamps, negative volume, or OHLC sanity violations. |
| DATA-002 | `/Users/farell.trades/NQU6 - 1 min - ETH.csv` | NQU6 / likely NQ continuous or current-contract export label | 1 minute | ETH/all-sessions | 3,106,544 | 2017-04-17 17:55 | 2026-07-10 22:59 | usable-for-research | No header. Format appears `YYYYMMDD HHMMSS,open,high,low,close,volume`. Ingestor profile found no parse errors, duplicate timestamps, negative volume, or OHLC sanity violations. |

## DATA-001 Notes

- Source platform: MotiveWave, confirmed from user screenshot on 2026-07-10.
- Export method: MotiveWave `Export Data` dialog, confirmed from user screenshot on 2026-07-10.
- Timezone: likely Europe/Berlin platform time, inferred from common session starts at 15:30/14:30 and ends at 22:59/21:59. Confirm manually before time-of-day research.
- Contract handling: file name says `NQU6`; however the date range spans 2017-2026, so this is probably a continuous series or platform-adjusted symbol export rather than a single September 2026 contract. Confirm before contract-roll-sensitive research.
- Session treatment: MotiveWave RTH export checkbox was enabled. Row counts commonly show 450 or 435 minutes per day, with some holiday/partial days. Confirm exact session template.
- ETH coverage: not present in DATA-001 as a full overnight session. The export used the RTH checkbox, and common starts/ends are RTH-like platform times such as 15:30/22:59 or 14:30/21:59. A separate ETH/all-sessions export is required for overnight, premarket, Asia/London, gap, or prior-overnight-high/low research.
- Known limitations: no header, no cost/slippage assumptions, no explicit timezone metadata, no explicit roll metadata.
- Export settings shown: Symbol `NQU6`, Bar Size `1 min`, `All Local` enabled, `Regular Trading Hours (RTH) Data` enabled, `Export Chart Data Including Study Values` disabled, format `CSV - yyyyMMdd HHmmss,O,H,L,C,V`.
- Study export caveat: if `Export Chart Data Including Study Values` is enabled, expect additional per-bar study values only if the study exposes exportable values. Do not assume raw Big Trades/time-and-sales events are exported unless verified by comparing a small test export with and without the checkbox.
- Dataset config: `config/datasets/DATA-001.json`.
- Ingestor: `scripts/ingest_ohlcv_csv.py`.
- Loader: `lib/market_data.py`.
- Session summarizer: `scripts/summarize_dataset.py`.
- Time-of-day profiler: `scripts/time_of_day_profile.py`.
- Range-build profiler: `scripts/range_build_profile.py`.
- Open-time baseline tester: `scripts/open_time_baseline.py`.
- MNQ-002 overnight filter tester: `scripts/mnq002_overnight_filter.py`.
- Profile artifact: `outputs/DATA-001-profile.json`.
- Session summary artifact: `outputs/DATA-001-session-summary.csv`.
- Time-of-day artifacts: `outputs/DATA-001-time-of-day-session-minute.csv`, `outputs/DATA-001-time-of-day-session-hour.csv`, `outputs/DATA-001-time-of-day-local-time.csv`, and `outputs/DATA-001-time-of-day-summary.json`.
- Range-build artifacts: `outputs/DATA-001-range-build-detail.csv`, `outputs/DATA-001-range-build-summary.csv`, and `outputs/DATA-001-range-build-summary.json`.
- MNQ-002 baseline artifacts: `outputs/MNQ-002-open-long-baseline-summary.json`, `outputs/MNQ-002-local-clock-trades.csv`, and `outputs/MNQ-002-session-aligned-trades.csv`.
- MNQ-002 overnight filter artifacts: `outputs/MNQ-002-overnight-filter-summary.json`, `outputs/MNQ-002-overnight-negative-trades.csv`, and `outputs/MNQ-002-overnight-non-negative-trades.csv`.
- Normalized local output: `data/processed/DATA-001/ohlcv_1m.csv` (ignored by git).

## DATA-002 Notes

- Source platform: MotiveWave, inferred from user's export workflow.
- Export method: MotiveWave `Export Data` dialog, RTH-only disabled / ETH all-sessions export.
- Timezone: likely Europe/Berlin platform time. Confirm manually before time-of-day research.
- Contract handling: file name says `NQU6`; however the date range spans 2017-2026, so this is probably a continuous series or platform-adjusted symbol export rather than a single September 2026 contract. Confirm before contract-roll-sensitive research.
- Session treatment: ETH/all-sessions inferred from filename and full 1,440 local clock-minute coverage. Confirm exact MotiveWave session template.
- Known limitations: no header, no cost/slippage assumptions, no explicit timezone metadata, no explicit roll metadata.
- Dataset config: `config/datasets/DATA-002.json`.
- Profile artifact: `outputs/DATA-002-profile.json`.
- Normalized local output: `data/processed/DATA-002/ohlcv_1m.csv` (ignored by git).
- Common starts/ends: starts mostly at `00:00`; ends mostly at `22:59`, consistent with futures maintenance break behavior in platform time.
- Rows per day commonly show 1,380 and 1,365 bars, consistent with near-24h coverage minus maintenance breaks and DST/session effects.

## Required Before Backtests

- Confirm source platform and feed.
- Confirm timezone.
- Confirm whether prices are back-adjusted, merged continuous, or raw contract data.
- Confirm RTH definition and whether holidays/half-days are included.
- DATA-002 now provides ETH/all-sessions data for overnight context, but timezone/session-template confirmation is still required before final time-of-day claims.
- Define canonical imported copy path if this dataset becomes the project baseline.
- Confirm whether the normalized output should use local platform time or converted exchange time before time-of-day strategies.
- Test whether MotiveWave Big Trades study values appear as extra columns when `Export Chart Data Including Study Values` is enabled.

## Deferred Checks

- MotiveWave Big Trades study export comparison: deferred by user on 2026-07-10, maybe later or tomorrow. Reminder suggestion was created for 2026-07-11 10:00 Europe/Berlin. When ready, export a small sample once without `Export Chart Data Including Study Values` and once with it enabled, then compare columns and rows.
- MotiveWave ETH/all-sessions export for DATA-002: completed on 2026-07-11. DATA-002 is registered and normalized locally.
- MNQ-002 filter follow-up: reminder suggestion created for 2026-07-11 16:00 Europe/Berlin. After DATA-002, use MNQ-002 as a weak benchmark and test structural filters instead of optimizing the naked time-entry baseline.

## DATA-001 Descriptive Session Summary

Generated with:

```bash
python3 scripts/summarize_dataset.py --input data/processed/DATA-001/ohlcv_1m.csv --output outputs/DATA-001-session-summary.csv --min-bars 300
```

Summary:

- Sessions: 2,379.
- Sessions with at least 300 bars: 2,185.
- First session: 2017-04-17.
- Last session: 2026-07-10.
- Average full-session range: 224.34 points.
- Median full-session range: 193.25 points.
- P10/P90 full-session range: 62.5 / 413.5 points.
- Average close-to-open move: 4.45 points.
- Median close-to-open move: 13.0 points.

These numbers describe the dataset only. They are not strategy performance and still inherit DATA-001 provenance limitations.

## DATA-001 Time-Of-Day Profile

Generated with:

```bash
python3 scripts/time_of_day_profile.py --input data/processed/DATA-001/ohlcv_1m.csv --output-dir outputs --min-bars 300
```

Summary:

- Sessions used for session-aligned profiles: 2,185.
- First 60 session minutes average 15.18 points of 1m range.
- Second 60 session minutes average 11.04 points of 1m range.
- Session minute 0 averages 25.00 points of 1m range.
- Session minute 1 averages 22.42 points of 1m range.
- Session minute 30 averages 21.28 points of 1m range.

These numbers describe where movement concentrates. They are not entry rules, R:R, or strategy performance.

## DATA-001 Range-Build Profile

Generated with:

```bash
python3 scripts/range_build_profile.py --input data/processed/DATA-001/ohlcv_1m.csv --output-dir outputs --min-bars 300 --windows 5 15 30 60 120
```

Summary over 2,185 sessions:

- First 5 minutes: average 48.06 points, 24.32% average share of full RTH range.
- First 15 minutes: average 72.58 points, 36.35% average share.
- First 30 minutes: average 94.01 points, 46.35% average share.
- First 60 minutes: average 122.69 points, 59.10% average share.
- First 120 minutes: average 152.23 points, 71.80% average share.
- First 60 minutes contain one eventual session extreme on 75.93% of usable sessions.

These numbers describe range development only. They are not entry rules, R:R, or strategy performance.

## MNQ-002 Open-Long Baseline

Generated with:

```bash
python3 scripts/open_time_baseline.py --input data/processed/DATA-001/ohlcv_1m.csv --output-dir outputs --entry-time 15:30 --exit-time 16:00 --hold-minutes 30 --min-bars 300 --point-value 20 --round-turn-cost-points 0
```

Summary:

- Exact local-clock 15:30 to 16:00: 2,314 trades, gross average 1.48 points, win rate 52.42%, profit factor 1.065, max drawdown -2,170.5 points.
- Session-aligned first RTH bar plus 30m: 2,185 trades, gross average 0.98 points, win rate 52.17%, profit factor 1.04, max drawdown -2,923.5 points.
- Costs/slippage: 0.0 points round turn in first pass.

This is a weak gross baseline, not a promotable strategy. It has no stop/target, no R:R, no ETH context, and poor year-to-year consistency.

## MNQ-002 Overnight Negative Filter

Generated with:

```bash
python3 scripts/mnq002_overnight_filter.py --rth-input data/processed/DATA-001/ohlcv_1m.csv --eth-input data/processed/DATA-002/ohlcv_1m.csv --output-dir outputs --hold-minutes 30 --min-bars 300 --point-value 20 --round-turn-cost-points 0
```

Definition: overnight negative means current RTH entry open is below previous RTH session close.

Summary:

- Overnight negative: 963 trades, gross average 2.14 points, win rate 53.17%, profit factor 1.0843, max drawdown -1,367.25 points.
- Overnight non-negative: 1,221 trades, gross average 0.05 points, win rate 51.35%, profit factor 1.002, max drawdown -2,227.5 points.

This filter is directionally useful but still weak before costs/slippage and not strategy-grade.

## ETH Data Status

DATA-001 is not suitable for ETH/overnight research. DATA-002 is the registered ETH/all-sessions dataset and should be used for overnight high/low, premarket behavior, London session, Asia session, gaps, and ETH VWAP/context once timezone/session-template details are confirmed.

## Related

- [[data-execution-stack]]
- [[backtest-battery]]
