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

## DATA-001 Notes

- Source platform: unknown from file alone; likely MotiveWave or another chart export. Confirm manually.
- Export method: unknown from file alone. Confirm manually.
- Timezone: likely Europe/Berlin platform time, inferred from common session starts at 15:30/14:30 and ends at 22:59/21:59. Confirm manually before time-of-day research.
- Contract handling: file name says `NQU6`; however the date range spans 2017-2026, so this is probably a continuous series or platform-adjusted symbol export rather than a single September 2026 contract. Confirm before contract-roll-sensitive research.
- Session treatment: labelled RTH in filename. Row counts commonly show 450 or 435 minutes per day, with some holiday/partial days. Confirm session template.
- Known limitations: no header, no cost/slippage assumptions, no explicit timezone metadata, no explicit roll metadata.
- Dataset config: `config/datasets/DATA-001.json`.
- Ingestor: `scripts/ingest_ohlcv_csv.py`.
- Loader: `lib/market_data.py`.
- Session summarizer: `scripts/summarize_dataset.py`.
- Profile artifact: `outputs/DATA-001-profile.json`.
- Session summary artifact: `outputs/DATA-001-session-summary.csv`.
- Normalized local output: `data/processed/DATA-001/ohlcv_1m.csv` (ignored by git).

## Required Before Backtests

- Confirm source platform and feed.
- Confirm timezone.
- Confirm whether prices are back-adjusted, merged continuous, or raw contract data.
- Confirm RTH definition and whether holidays/half-days are included.
- Define canonical imported copy path if this dataset becomes the project baseline.
- Confirm whether the normalized output should use local platform time or converted exchange time before time-of-day strategies.

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

## Related

- [[data-execution-stack]]
- [[backtest-battery]]
