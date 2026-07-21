# MNQ-003 NT8 Trade List Review - Weak Hours 10/11/21/2

Source export: `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-21 07-01.csv`.

Provenance:

- Platform: NinjaTrader Strategy Analyzer trade list export.
- Strategy: `MNQ003EmaLimitEntryCleanStrategy`.
- Instrument: `NQ 09-26`.
- Account: `Backtest`.
- Direction: long-only, inferred from all rows having market position `Kauf`.
- Quantity: 1 contract.
- User context: weak-hour filter enabled with the aggressive variant, intended platform hours `10,11,21,2`.
- Date range in trade list: 2016-01-04 20:01 to 2026-07-20 18:42.
- Fees: included; exported fee column totals $21,795.64, about $4.36 per trade.
- Slippage/fill settings: not visible in this trade-list export.

## Summary

| Metric | Value |
| --- | ---: |
| Trades | 4,999 |
| Net profit | $355,414.36 |
| Gross profit | $2,997,935.52 |
| Gross loss | -$2,642,521.16 |
| Profit factor | 1.134 |
| Win rate | 42.37% |
| Winners | 2,118 |
| Losers | 2,881 |
| Avg trade | $71.10 |
| Avg winner | $1,415.46 |
| Avg loser | -$917.22 |
| Fees | $21,795.64 |
| Max drawdown, trade-list equity | -$36,977.64 |
| Longest closed recovery, trade-list equity | 355.8 days |

## Comparison

Baseline reference: `outputs/MNQ-003-nt8-slippage1-tradelist-2026-07-21-1223-report.md`.
Conservative weak-hour reference: `outputs/MNQ-003-nt8-weak-hours-10-11-21-tradelist-2026-07-21-0649-report.md`.

| Metric | Baseline | Weak 10/11/21 | Weak 10/11/21/2 |
| --- | ---: | ---: | ---: |
| Trades | 5,494 | 5,104 | 4,999 |
| Net profit | $298,890.96 | $329,006.56 | $355,414.36 |
| Profit factor | 1.103 | 1.121 | 1.134 |
| Win rate | 41.92% | 42.07% | 42.37% |
| Avg trade | $54.40 | $64.46 | $71.10 |
| Max drawdown | -$47,788.40 | -$37,140.72 | -$36,977.64 |
| Longest recovery | 360.8 days | 354.7 days | 355.8 days |
| 2024 net | -$27,758.40 | -$18,990.20 | -$19,152.12 |
| Fees | $28,349.04 | $22,253.44 | $21,795.64 |
| Avg fee/trade | about $5.16 | about $4.36 | about $4.36 |

The aggressive weak-hour variant has the strongest overall headline so far among these three: highest net profit, PF, win rate, and average trade, with max drawdown similar to the conservative weak-hour variant. It does not improve 2024 versus the conservative weak-hour variant.

## Year Split

| Year | Trades | Net Profit | Win Rate | PF |
| --- | ---: | ---: | ---: | ---: |
| 2016 | 262 | $292.68 | 53.82% | 1.004 |
| 2017 | 268 | $25,521.52 | 60.07% | 1.471 |
| 2018 | 338 | $15,306.32 | 44.08% | 1.096 |
| 2019 | 320 | $32,169.80 | 52.81% | 1.269 |
| 2020 | 531 | $41,634.84 | 39.74% | 1.136 |
| 2021 | 480 | $50,037.20 | 42.08% | 1.188 |
| 2022 | 637 | $31,967.68 | 37.05% | 1.081 |
| 2023 | 515 | $70,044.60 | 43.30% | 1.250 |
| 2024 | 567 | -$19,152.12 | 36.51% | 0.945 |
| 2025 | 653 | $79,222.92 | 39.97% | 1.209 |
| 2026 | 428 | $28,368.92 | 36.92% | 1.107 |

Only 2024 is negative. Unlike the conservative weak-hour variant, 2016 stays barely positive.

## 2024 Month Split

| Month | Trades | Net Profit | Win Rate | PF |
| --- | ---: | ---: | ---: | ---: |
| Jan | 40 | -$5,839.40 | 32.50% | 0.778 |
| Feb | 46 | $6,919.44 | 45.65% | 1.281 |
| Mar | 37 | -$5,646.32 | 35.14% | 0.760 |
| Apr | 47 | -$12,474.92 | 23.40% | 0.631 |
| May | 40 | $8,010.60 | 42.50% | 1.389 |
| Jun | 42 | $2,801.88 | 42.86% | 1.120 |
| Jul | 47 | -$1,179.92 | 34.04% | 0.961 |
| Aug | 55 | $3,605.20 | 38.18% | 1.110 |
| Sep | 58 | -$10,417.88 | 32.76% | 0.729 |
| Oct | 57 | -$10,718.52 | 33.33% | 0.714 |
| Nov | 53 | -$3,441.08 | 35.85% | 0.894 |
| Dec | 45 | $9,228.80 | 44.44% | 1.388 |

2024 remains a regime problem. April, September, and October are still the main weak clusters.

## Exit Mix

| Exit | Trades | Net Profit |
| --- | ---: | ---: |
| Stop loss | 2,523 | -$2,545,920.28 |
| Exit on session close | 1,244 | $442,706.16 |
| Profit target | 1,232 | $2,458,628.48 |

Session-close exits remain a large positive contributor.

## Hour Check

Hours 10 and 11 are absent. Hour 2 still has 5 entries for -$976.80, and hour 21 still has 7 entries for -$5,145.52. This suggests the weak-hour filter is mostly working, but not perfectly excluding edge cases around timestamp/session handling.

## Interpretation

This is the best headline variant so far for MNQ-003 among the tested weak-hour filters. It improves the equity-quality metrics without materially reducing trade count, and 2016 stays barely positive. The unresolved issue is 2024: the aggressive weak-hour filter does not improve 2024 versus the conservative weak-hour filter, so the next research target should be 2024 regime/session behavior rather than more hour pruning.
