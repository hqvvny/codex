# MNQ-003 NT8 Summary Review - NY Session Only

Source export: `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-20 07-46.csv`.

Provenance:

- Platform: NinjaTrader Strategy Analyzer summary export.
- Strategy context from user: `MNQ003EmaLimitEntryStrategy`, long-only, 200 EMA limit entry, 35 trend bars, 50-point stop, 100-point target, 2:1R.
- Session context from user: NY session only.
- Instrument/date/settings visible in this summary export: not fully visible; summary date range is 2016-01-01 to 2026-07-20.
- Fees: included in net metrics; exported fees line is $14,406.72.
- Slippage: 0.
- Limitation: this is a summary export, not a trade list. It cannot verify year split, entry hours, exit mix, or trade-list equity curve.

## Summary

| Metric | Value |
| --- | ---: |
| Net profit | $124,356.28 |
| Gross profit | $1,299,554.64 |
| Gross loss | -$1,175,198.36 |
| Fees | $14,406.72 |
| Profit factor | 1.11 |
| Max drawdown | -$20,834.88 |
| Sharpe ratio | 0.22 |
| Sortino ratio | 0.36 |
| Trades | 2,792 |
| Win rate | 46.42% |
| Winners | 1,296 |
| Losers | 1,496 |
| Avg trade | $44.54 |
| Avg winner | $1,002.74 |
| Avg loser | -$785.56 |
| Avg win / avg loss | 1.28 |
| Max win streak | 12 |
| Max loss streak | 10 |
| Largest winner | $1,994.84 |
| Largest loser | -$1,006.16 |
| Avg trades/day | 1.05 |
| Avg time in market | 152.00 min |
| Profit/month | $984.90 |
| Max recovery time | 489.00 days |
| Avg MAE | $601.11 |
| Avg MFE | $817.62 |
| Avg ETD | $773.08 |

## Comparison To Broad-Session Trade List

Broad-session reference: `outputs/MNQ-003-nt8-tradelist-2026-07-20-0733-report.md`.

| Metric | Broad Session | NY Only Summary |
| --- | ---: | ---: |
| Net profit | $316,128.76 | $124,356.28 |
| Trades | 5,489 | 2,792 |
| Win rate | 42.03% | 46.42% |
| Profit factor | 1.109 | 1.11 |
| Max drawdown | -$46,336.40 | -$20,834.88 |
| Sharpe | user-reported 0.37 | 0.22 |
| Sortino | user-reported 0.72 | 0.36 |
| Max recovery | about 360.7 days / user-reported 357 days | 489 days |
| Avg trade | $57.59 | $44.54 |

## Interpretation

The NY-only version cuts trade count, net profit, and drawdown roughly in line with having less exposure. Win rate improves and profit factor is essentially unchanged, but average trade, Sharpe, Sortino, and max recovery are worse. From a prop-firm evaluation perspective, this is not a clear improvement over the broad-session 2R / 35-trend-bars branch.

The main benefit of NY-only is lower absolute drawdown. The main cost is weaker equity quality and a longer recovery period. Before promoting NY-only, it needs a trade-list export with yearly and monthly split to confirm whether it removes the 2024 weak regime or simply reduces exposure everywhere.
