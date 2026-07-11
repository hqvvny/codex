# MNQ-002 NinjaTrader Export Comparison

Source files:

- `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-11 12-53.csv`
- `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-11 12-54.csv`
- `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-11 12-55.csv`

Assumptions:

- NinjaTrader exports are gross results with 0.00 fees and 0 slippage.
- Dollar-to-point conversion below uses NQ at $20/point because the all-trades average of $29.64 equals 1.482 points, matching the local Python baseline.
- The first two CSV files are byte-for-byte identical.
- The exports do not include the selected `FilterMode`, so labels must be confirmed from the Strategy Analyzer settings or file names.

| Export | Likely Role | Net Profit | Trades | Win Rate | Avg Trade | Avg Points | Profit Factor | Max DD | Max DD Points | Max Recovery |
| --- | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| `12-53.csv` | Unconfirmed, likely `All` | $72,725 | 2,454 | 52.49% | $29.64 | 1.482 | 1.07 | -$43,340 | -2,167.0 | 789 days |
| `12-54.csv` | Duplicate of `12-53.csv` | $72,725 | 2,454 | 52.49% | $29.64 | 1.482 | 1.07 | -$43,340 | -2,167.0 | 789 days |
| `12-55.csv` | Unconfirmed filtered run | $15,100 | 1,367 | 51.06% | $11.05 | 0.553 | 1.03 | -$34,460 | -1,723.0 | 678 days |

Interpretation:

- The `All`-style NinjaTrader run agrees closely with the local Python exact-clock baseline: roughly +1.48 points/trade before costs and slippage.
- The duplicate export means the three-filter comparison is incomplete. One of `All`, `OvernightNegativeOnly`, or `OvernightNonNegativeOnly` is missing or was exported twice with unchanged settings.
- The filtered run in `12-55.csv` is worse than the all-trades baseline: lower profit factor, lower win rate, lower average trade, and still large drawdown.
- None of these results are strategy-grade yet. Profit factor is close to 1, max recovery is measured in years, and no stop/target R:R exists.

Next action:

- Re-export the missing filter run with a visible label in the filename, ideally `MNQ-002_NT8_All.csv`, `MNQ-002_NT8_OvernightNegativeOnly.csv`, and `MNQ-002_NT8_OvernightNonNegativeOnly.csv`.
- Keep Strategy Analyzer settings identical across all three runs: instrument, date range, trading hours, commissions, slippage, fill resolution, and data series.
