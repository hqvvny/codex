---
type: work-item
updated: 2026-07-11
status: complete
verdict: useful
---

# DATA-002 ETH Registration

## Goal

Register and normalize the MotiveWave NQU6 1m ETH/all-sessions export so overnight context can be used in research.

## Context

DATA-001 is RTH-only and cannot support overnight, premarket, Asia/London, gap, or prior overnight high/low research. The user exported `/Users/farell.trades/NQU6 - 1 min - ETH.csv` as the ETH/all-sessions counterpart.

## Decisions

- Register the new file as DATA-002.
- Add dataset metadata at `config/datasets/DATA-002.json`.
- Normalize to `data/processed/DATA-002/ohlcv_1m.csv`, ignored by git.
- Commit the small profile artifact `outputs/DATA-002-profile.json`.
- Keep timezone/session-template and contract-roll handling as open provenance checks.

## Outcome

DATA-002 profile:

- Rows: 3,106,544.
- First timestamp: 2017-04-17 17:55.
- Last timestamp: 2026-07-10 22:59.
- Unique dates: 2,418.
- No parse errors.
- No duplicate timestamps.
- No OHLC sanity violations.
- Full 1,440 local clock-minute coverage appears present.

## Links

- [[../reference/data-inventory]]
- [[../reference/data-execution-stack]]
