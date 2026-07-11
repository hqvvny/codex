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

## Overnight Negative Hold-Time Sensitivity

Source: user-provided NinjaTrader screenshots for 15, 45, and 60 hold bars plus prior 30-bar export.

Settings visible in screenshots:

- Strategy: `MNQ002OpenLongStrategy`
- Instrument: `NQ 09-26`
- Data series: 1 minute, last price
- Date range: 2017-02-01 to 2026-07-10
- Filter: `OvernightNegativeOnly`
- RTH start/end: `153000` to `230000`
- Fees and slippage: 0
- Historical fill resolution: standard/fastest

| Hold Bars | Net Profit | Trades | Win Rate | Avg Trade | Avg NQ Points | Profit Factor | Max DD | Max DD Points | Max Recovery | Avg Time |
| ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| 15 | $57,760 | 1,086 | 54.14% | $53.19 | 2.660 | 1.15 | -$26,345 | -1,317.25 | 449 days | 16.00 min |
| 30 | $57,565 | 1,086 | 54.24% | $53.01 | 2.651 | 1.12 | -$22,085 | -1,104.25 | 856 days | 31.01 min |
| 45 | $71,015 | 1,086 | 53.31% | $65.39 | 3.270 | 1.12 | -$38,935 | -1,946.75 | 1,574 days | 46.01 min |
| 60 | $82,315 | 1,086 | 53.59% | $75.80 | 3.790 | 1.13 | -$23,300 | -1,165.00 | 571 days | 61.01 min |

Interpretation:

- The edge is stable across hold times because all four runs stay positive with the same 1,086-trade sample.
- 15 bars has the cleanest quality profile: best PF, best Sharpe, best Sortino, and shortest max recovery.
- 60 bars has the highest net profit and average trade while keeping max drawdown close to the 30-bar version, but it doubles market exposure versus 30 bars and quadruples it versus 15 bars.
- 45 bars is unattractive despite higher net profit because max drawdown and max recovery deteriorate sharply.
- This does not yet solve R:R. There is still no stop/target, so this remains a timed-exit benchmark, not a tradeable strategy.

Current hold-time preference:

- Research benchmark: 15 bars, because it has the cleanest recovery and risk-adjusted profile.
- Alternative branch: 60 bars, but only if year-by-year results and drawdown duration do not reveal concentration.
