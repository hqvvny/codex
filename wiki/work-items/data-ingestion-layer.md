---
type: work-item
updated: 2026-07-10
status: complete
verdict: useful
---

# Data Ingestion Layer

## Goal

Create a reusable local ingestor for zero-budget OHLCV CSV exports and run it on DATA-001.

## Context

The project now has a local NQU6/NQ 1m RTH CSV export spanning 2017-2026. The raw file lives outside the repo and should remain the source of truth. Generated processed data should not be committed.

## Decisions

- Add dataset metadata at `config/datasets/DATA-001.json`.
- Add stdlib-only ingestor at `scripts/ingest_ohlcv_csv.py`.
- Write normalized generated data to ignored `data/processed/DATA-001/ohlcv_1m.csv`.
- Write a small committed profile artifact to `outputs/DATA-001-profile.json`.
- Keep large market data ignored via `.gitignore`.

## Outcome

The ingestor normalized DATA-001 and produced a profile with row counts, date range, session-start/end summaries, gap summaries, and OHLCV sanity checks. The dataset remains research-grade until source platform, timezone, session template, and contract-roll handling are confirmed.

## Links

- [[../reference/data-inventory]]
- [[../reference/data-execution-stack]]
