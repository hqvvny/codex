---
type: work-item
updated: 2026-07-10
status: complete
verdict: useful
---

# Time Of Day Profile

## Goal

Build descriptive time-of-day profiles for DATA-001 so future candidate generation can start from observed session structure instead of generic open/close assumptions.

## Context

The project uses a session-first trading lens. DATA-001 has likely Europe/Berlin platform timestamps and RTH-only export settings, so the profile was built two ways:

- Session-aligned buckets from each day's first RTH bar.
- Local clock-time buckets for platform-time inspection.

## Decisions

- Add `scripts/time_of_day_profile.py`.
- Extend `lib/market_data.py` with bucket statistics.
- Output profile artifacts to `outputs/`.
- Use sessions with at least 300 bars for session-aligned profiles.
- Treat all results as descriptive market-profile statistics, not strategy performance.

## Outcome

Generated artifacts:

- `outputs/DATA-001-time-of-day-session-minute.csv`
- `outputs/DATA-001-time-of-day-session-hour.csv`
- `outputs/DATA-001-time-of-day-local-time.csv`
- `outputs/DATA-001-time-of-day-summary.json`

Summary:

- Sessions: 2,379.
- Sessions used for session-aligned profiles: 2,185.
- Session-minute buckets: 870.
- Local-time buckets: 930.
- The first 60 session minutes show the highest average 1m range: 15.18 points.
- The second 60 session minutes remain active: 11.04 points average 1m range.
- Later core RTH hours compress materially, mostly around 7.8-9.0 points average 1m range.
- Session minute 0 has the highest average 1m range: 25.00 points.

Interpretation:

- The data supports prioritizing NY/RTH open research over midday ideas.
- It does not, by itself, justify an opening-range or breakout strategy.
- Because timezone and session template are still not fully confirmed, session-aligned buckets are safer than local clock-time buckets for first-pass research.

## Links

- [[../reference/data-inventory]]
- [[../concepts/trading-analysis-profile]]
- [[../reference/backtest-battery]]
