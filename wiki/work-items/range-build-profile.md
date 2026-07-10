---
type: work-item
updated: 2026-07-10
status: complete
verdict: useful
---

# Range Build Profile

## Goal

Measure how much of the RTH session range is built after the first 5, 15, 30, 60, and 120 session minutes.

## Context

The prior time-of-day profile measured average one-minute candle range by session minute. This work measures cumulative window range as a share of the full RTH session range, which better answers how quickly the day develops.

## Decisions

- Add `scripts/range_build_profile.py`.
- Use DATA-001 normalized 1m RTH data.
- Use sessions with at least 300 bars.
- Treat the output as descriptive market structure, not strategy performance.

## Outcome

Generated artifacts:

- `outputs/DATA-001-range-build-detail.csv`
- `outputs/DATA-001-range-build-summary.csv`
- `outputs/DATA-001-range-build-summary.json`

Summary over 2,185 sessions:

| Window | Avg Range | Median Range | Avg Share Of Full RTH Range | Median Share | Contains One Session Extreme |
| --- | ---: | ---: | ---: | ---: | ---: |
| 5 min | 48.06 pts | 42.00 pts | 24.32% | 22.49% | 30.94% |
| 15 min | 72.58 pts | 62.25 pts | 36.35% | 34.30% | 47.05% |
| 30 min | 94.01 pts | 81.25 pts | 46.35% | 44.21% | 60.14% |
| 60 min | 122.69 pts | 106.25 pts | 59.10% | 57.87% | 75.93% |
| 120 min | 152.23 pts | 132.75 pts | 71.80% | 72.50% | 88.79% |

Interpretation:

- DATA-001 strongly supports focusing research on early-session range development.
- The first 30 minutes build nearly half the average RTH range.
- The first 60 minutes contain at least one eventual session extreme on roughly 76% of usable sessions.
- This does not imply a breakout edge; it only describes where the session range tends to form.

## Links

- [[../reference/data-inventory]]
- [[time-of-day-profile]]
- [[../reference/backtest-battery]]
