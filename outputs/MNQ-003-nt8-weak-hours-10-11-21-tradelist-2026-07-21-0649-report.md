# MNQ-003 NT8 Trade List Review - Weak Hours 10/11/21

Source export: `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-21 06-49.csv`.

Provenance:

- Platform: NinjaTrader Strategy Analyzer trade list export.
- Strategy: `MNQ003EmaLimitEntryCleanStrategy`.
- Instrument: `NQ 09-26`.
- Account: `Backtest`.
- Direction: long-only, inferred from all rows having market position `Kauf`.
- Quantity: 1 contract.
- User context: weak-hour filter enabled with the first/conservative variant, intended as platform hours `10,11,21`.
- Date range in trade list: 2016-01-04 20:01 to 2026-07-20 18:42.
- Fees: included; exported fee column totals $22,253.44, about $4.36 per trade.
- Slippage/fill settings: user context from prior baseline was slippage 1, but this export itself does not expose slippage.

## Summary

| Metric | Value |
| --- | ---: |
| Trades | 5,104 |
| Net profit | $329,006.56 |
| Gross profit | $3,052,664.08 |
| Gross loss | -$2,723,657.52 |
| Profit factor | 1.121 |
| Win rate | 42.07% |
| Winners | 2,147 |
| Losers | 2,957 |
| Avg trade | $64.46 |
| Avg winner | $1,421.83 |
| Avg loser | -$921.09 |
| Fees | $22,253.44 |
| Max drawdown, trade-list equity | -$37,140.72 |
| Longest closed recovery, trade-list equity | 354.7 days |

## Comparison To Slippage-1 Baseline

Baseline reference: `outputs/MNQ-003-nt8-slippage1-tradelist-2026-07-21-1223-report.md`.

| Metric | Baseline | Weak Hours 10/11/21 |
| --- | ---: | ---: |
| Trades | 5,494 | 5,104 |
| Net profit | $298,890.96 | $329,006.56 |
| Profit factor | 1.103 | 1.121 |
| Win rate | 41.92% | 42.07% |
| Avg trade | $54.40 | $64.46 |
| Max drawdown | -$47,788.40 | -$37,140.72 |
| Longest recovery | 360.8 days | 354.7 days |
| 2024 net | -$27,758.40 | -$18,990.20 |
| Fees | $28,349.04 | $22,253.44 |
| Avg fee/trade | about $5.16 | about $4.36 |

The weak-hour filter improves the headline: higher net profit, PF, win rate, average trade, lower max drawdown, slightly shorter recovery, and less 2024 damage. However, the fee template appears different from the slippage-1 baseline, so this comparison is not perfectly apples-to-apples.

## Year Split

| Year | Trades | Net Profit | Win Rate | PF |
| --- | ---: | ---: | ---: | ---: |
| 2016 | 266 | -$799.76 | 53.76% | 0.989 |
| 2017 | 268 | $26,391.52 | 60.45% | 1.488 |
| 2018 | 343 | $15,829.52 | 43.73% | 1.097 |
| 2019 | 326 | $35,953.64 | 53.37% | 1.297 |
| 2020 | 543 | $33,287.52 | 39.04% | 1.105 |
| 2021 | 495 | $42,596.80 | 41.21% | 1.153 |
| 2022 | 650 | $21,851.00 | 36.46% | 1.054 |
| 2023 | 526 | $74,816.64 | 43.54% | 1.261 |
| 2024 | 570 | -$18,990.20 | 36.49% | 0.946 |
| 2025 | 672 | $77,845.08 | 39.73% | 1.198 |
| 2026 | 445 | $20,224.80 | 36.18% | 1.073 |

2016 turns slightly negative after this filter, while 2024 remains negative but improves by about $8.8k versus the slippage-1 baseline.

## 2024 Month Split

| Month | Trades | Net Profit | Win Rate | PF |
| --- | ---: | ---: | ---: | ---: |
| Jan | 40 | -$5,844.40 | 32.50% | 0.778 |
| Feb | 46 | $6,729.44 | 45.65% | 1.274 |
| Mar | 38 | -$6,655.68 | 34.21% | 0.728 |
| Apr | 47 | -$12,474.92 | 23.40% | 0.631 |
| May | 40 | $8,005.60 | 42.50% | 1.388 |
| Jun | 42 | $5,871.88 | 45.24% | 1.263 |
| Jul | 47 | -$1,179.92 | 34.04% | 0.961 |
| Aug | 55 | $3,600.20 | 38.18% | 1.110 |
| Sep | 60 | -$12,436.60 | 31.67% | 0.693 |
| Oct | 57 | -$10,723.52 | 33.33% | 0.714 |
| Nov | 53 | -$3,111.08 | 35.85% | 0.904 |
| Dec | 45 | $9,228.80 | 44.44% | 1.388 |

The filter helps 2024 but does not solve it. April, September, and October remain the main problem clusters.

## Exit Mix

| Exit | Trades | Net Profit |
| --- | ---: | ---: |
| Stop loss | 2,603 | -$2,626,684.08 |
| Profit target | 1,260 | $2,514,506.40 |
| Exit on session close | 1,241 | $441,184.24 |

Session-close exits remain a large positive contributor.

## Hour Check

Entry hours 10 and 11 are absent in this export. Hour 21 still has 7 entries for -$5,145.52, so the run appears to have mostly but not perfectly removed hour 21. This may be due to platform time/session boundary details or how NinjaTrader timestamps entries around the hour filter.

## Interpretation

The first weak-hour filter variant is a confirmed improvement direction, but not yet a solved strategy. It improves risk/reward quality and reduces 2024 damage, while keeping a high trade count. Next checks should keep fees/slippage identical and test:

1. The same filter with the same commission/slippage template as the baseline.
2. Aggressive weak hours `10,11,21,2`.
3. A focused 2024-only review to see whether April, September, and October need a regime/session filter rather than more hour filtering.
