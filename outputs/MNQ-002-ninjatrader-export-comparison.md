# MNQ-002 NinjaTrader Export Comparison

Source files:

- `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-11 12-53.csv`
- `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-11 12-54.csv`
- `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-11 12-55.csv`
- `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-11 01-00.csv`

Assumptions:

- NinjaTrader exports are gross results with 0.00 fees and 0 slippage.
- Dollar-to-point conversion below uses NQ at $20/point because the all-trades average of $29.64 equals 1.482 points, matching the local Python baseline.
- User confirmed `12-53.csv` is `All`, `12-55.csv` is `OvernightNonNegativeOnly`, and `01-00.csv` is `OvernightNegativeOnly`.
- `12-54.csv` is a duplicate export of `12-53.csv`.

| Export | Likely Role | Net Profit | Trades | Win Rate | Avg Trade | Avg Points | Profit Factor | Max DD | Max DD Points | Max Recovery |
| --- | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| `12-53.csv` | `All` | $72,725 | 2,454 | 52.49% | $29.64 | 1.482 | 1.07 | -$43,340 | -2,167.0 | 789 days |
| `12-54.csv` | Duplicate of `All` | $72,725 | 2,454 | 52.49% | $29.64 | 1.482 | 1.07 | -$43,340 | -2,167.0 | 789 days |
| `12-55.csv` | `OvernightNonNegativeOnly` | $15,100 | 1,367 | 51.06% | $11.05 | 0.553 | 1.03 | -$34,460 | -1,723.0 | 678 days |
| `01-00.csv` | `OvernightNegativeOnly` | $57,565 | 1,086 | 54.24% | $53.01 | 2.651 | 1.12 | -$22,085 | -1,104.25 | 856 days |

Interpretation:

- The `All` NinjaTrader run agrees closely with the local Python exact-clock baseline: roughly +1.48 points/trade before costs and slippage.
- The split confirms the signal direction: `OvernightNegativeOnly` carries most of the edge, while `OvernightNonNegativeOnly` is weak.
- `OvernightNegativeOnly` improves average trade, win rate, profit factor, and max drawdown versus `All`.
- The filtered runs sum to 2,453 trades versus 2,454 all trades, so one early/edge-case trade is likely excluded by previous-RTH-close initialization.
- None of these results are strategy-grade yet. The best filter still has PF only 1.12, max recovery is 856 days, and no stop/target R:R exists.

Next action:

- Do not optimize raw entry time yet. First add a risk model and robustness checks around the confirmed `OvernightNegativeOnly` filter.
- Next candidates: hold-time sensitivity around 15/20/30/45/60 minutes, stop/target brackets using RTH opening volatility, and year-by-year degradation check.
- Keep Strategy Analyzer settings identical across all future runs: instrument, date range, trading hours, commissions, slippage, fill resolution, and data series.
