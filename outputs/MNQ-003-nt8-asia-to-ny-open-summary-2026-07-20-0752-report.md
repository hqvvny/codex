# MNQ-003 NT8 Summary Review - Asia To NY Open

Source export: `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-20 07-52.csv`.

Provenance:

- Platform: NinjaTrader Strategy Analyzer summary export.
- Strategy context from user: `MNQ003EmaLimitEntryStrategy`, long-only, 200 EMA limit entry, 35 trend bars, 50-point stop, 100-point target, 2:1R.
- Session context from user: Asia through NY open.
- Instrument/date/settings visible in this summary export: not fully visible; summary date range is 2016-01-01 to 2026-07-20.
- Fees: included in net metrics; exported fees line is $19,169.40.
- Slippage: 0.
- Limitation: this is a summary export, not a trade list. It cannot verify year split, entry hours, exit mix, or trade-list equity curve.

## Summary

| Metric | Value |
| --- | ---: |
| Net profit | $201,095.60 |
| Gross profit | $2,266,168.72 |
| Gross loss | -$2,065,073.12 |
| Fees | $19,169.40 |
| Profit factor | 1.10 |
| Max drawdown | -$40,737.80 |
| Sharpe ratio | 0.27 |
| Sortino ratio | 0.52 |
| Trades | 3,715 |
| Win rate | 40.59% |
| Winners | 1,508 |
| Losers | 2,207 |
| Avg trade | $54.13 |
| Avg winner | $1,502.76 |
| Avg loser | -$935.69 |
| Avg win / avg loss | 1.61 |
| Max win streak | 10 |
| Max loss streak | 13 |
| Largest winner | $1,994.84 |
| Largest loser | -$1,006.16 |
| Avg trades/day | 1.40 |
| Avg time in market | 497.61 min |
| Profit/month | $1,593.10 |
| Max recovery time | 531.98 days |
| Avg MAE | $727.44 |
| Avg MFE | $1,010.61 |
| Avg ETD | $956.48 |

## Comparison

Broad-session reference: `outputs/MNQ-003-nt8-tradelist-2026-07-20-0733-report.md`.
NY-only reference: `outputs/MNQ-003-nt8-ny-session-summary-2026-07-20-0746-report.md`.

| Metric | Broad Session | Asia To NY Open | NY Only |
| --- | ---: | ---: | ---: |
| Net profit | $316,128.76 | $201,095.60 | $124,356.28 |
| Trades | 5,489 | 3,715 | 2,792 |
| Win rate | 42.03% | 40.59% | 46.42% |
| Profit factor | 1.109 | 1.10 | 1.11 |
| Max drawdown | -$46,336.40 | -$40,737.80 | -$20,834.88 |
| Sharpe | user-reported 0.37 | 0.27 | 0.22 |
| Sortino | user-reported 0.72 | 0.52 | 0.36 |
| Max recovery | about 360.7 days / user-reported 357 days | 531.98 days | 489 days |
| Avg trade | $57.59 | $54.13 | $44.54 |

## Interpretation

Asia-to-NY-open keeps more of the broad-session profit than NY-only, and its average trade is close to broad session. However, max drawdown remains close to broad session while recovery is worse than both broad and NY-only. Profit factor is essentially unchanged around 1.10.

This session slice is not a clear improvement. It suggests the pre-NY period contributes meaningful profit, but by itself it carries too much stagnation/recovery risk. The broad-session branch remains the best overall reported variant unless a later trade-list export shows the Asia-to-NY slice removes the 2024 weak regime.
