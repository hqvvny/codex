# MNQ-003 NT8 Trade List Review - Weak Hours 10/11/21/2 Fee-Matched

Source export: `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-21 07-19.csv`.

Provenance:

- Platform: NinjaTrader Strategy Analyzer trade list export.
- Strategy: `MNQ003EmaLimitEntryCleanStrategy`.
- Instrument: `NQ 09-26`.
- Account: `Backtest`.
- Direction: long-only, inferred from all rows having market position `Kauf`.
- Quantity: 1 contract.
- User context: aggressive weak-hour filter, intended platform hours `10,11,21,2`.
- Date range in trade list: 2016-01-04 20:01 to 2026-07-20 18:42.
- Fees: included; exported fee column totals $25,794.84, about $5.16 per trade.
- Slippage/fill settings: not visible in this trade-list export.

## Summary

| Metric | Value |
| --- | ---: |
| Trades | 4,999 |
| Net profit | $351,415.16 |
| Gross profit | $2,996,242.88 |
| Gross loss | -$2,644,827.72 |
| Profit factor | 1.133 |
| Win rate | 42.15% |
| Winners | 2,107 |
| Losers | 2,892 |
| Avg trade | $70.30 |
| Avg winner | $1,422.04 |
| Avg loser | -$914.53 |
| Fees | $25,794.84 |
| Max drawdown, trade-list equity | -$37,336.84 |
| Longest closed recovery, trade-list equity | 355.8 days |

## Comparison To Baseline

Baseline reference: `outputs/MNQ-003-nt8-slippage1-tradelist-2026-07-21-1223-report.md`.

| Metric | Baseline | Weak 10/11/21/2 Fee-Matched |
| --- | ---: | ---: |
| Trades | 5,494 | 4,999 |
| Net profit | $298,890.96 | $351,415.16 |
| Profit factor | 1.103 | 1.133 |
| Win rate | 41.92% | 42.15% |
| Avg trade | $54.40 | $70.30 |
| Max drawdown | -$47,788.40 | -$37,336.84 |
| Longest recovery | 360.8 days | 355.8 days |
| 2024 net | -$27,758.40 | -$19,605.72 |
| Fees | $28,349.04 | $25,794.84 |
| Avg fee/trade | about $5.16 | about $5.16 |

The aggressive weak-hour filter remains a clear improvement with the same approximate per-trade fee template as the baseline.

## Comparison To Lower-Fee Aggressive Run

Previous aggressive weak-hour reference: `outputs/MNQ-003-nt8-weak-hours-10-11-21-2-tradelist-2026-07-21-0701-report.md`.

| Metric | Lower-Fee Run | Fee-Matched Run |
| --- | ---: | ---: |
| Trades | 4,999 | 4,999 |
| Net profit | $355,414.36 | $351,415.16 |
| Profit factor | 1.134 | 1.133 |
| Win rate | 42.37% | 42.15% |
| Avg trade | $71.10 | $70.30 |
| Max drawdown | -$36,977.64 | -$37,336.84 |
| 2024 net | -$19,152.12 | -$19,605.72 |
| Avg fee/trade | about $4.36 | about $5.16 |

Higher fees reduce net profit by about $4.0k but do not change the strategic readout.

## Year Split

| Year | Trades | Net Profit | Win Rate | PF |
| --- | ---: | ---: | ---: | ---: |
| 2016 | 262 | $83.08 | 53.05% | 1.001 |
| 2017 | 268 | $25,307.12 | 59.33% | 1.466 |
| 2018 | 338 | $15,035.92 | 43.79% | 1.094 |
| 2019 | 320 | $31,913.80 | 52.81% | 1.267 |
| 2020 | 531 | $41,210.04 | 39.74% | 1.134 |
| 2021 | 480 | $49,653.20 | 41.67% | 1.187 |
| 2022 | 637 | $31,458.08 | 37.05% | 1.079 |
| 2023 | 515 | $69,632.60 | 42.91% | 1.248 |
| 2024 | 567 | -$19,605.72 | 36.16% | 0.944 |
| 2025 | 653 | $78,700.52 | 39.97% | 1.207 |
| 2026 | 428 | $28,026.52 | 36.92% | 1.106 |

Only 2024 is negative. 2016 is effectively flat after fees, so the filter is sensitive to early-year execution assumptions.

## 2024 Month Split

| Month | Trades | Net Profit | Win Rate | PF |
| --- | ---: | ---: | ---: | ---: |
| Jan | 40 | -$5,871.40 | 32.50% | 0.777 |
| Feb | 46 | $6,882.64 | 45.65% | 1.280 |
| Mar | 37 | -$5,675.92 | 32.43% | 0.759 |
| Apr | 47 | -$12,512.52 | 23.40% | 0.631 |
| May | 40 | $7,978.60 | 42.50% | 1.387 |
| Jun | 42 | $2,768.28 | 42.86% | 1.119 |
| Jul | 47 | -$1,217.52 | 31.91% | 0.960 |
| Aug | 55 | $3,561.20 | 38.18% | 1.109 |
| Sep | 58 | -$10,464.28 | 32.76% | 0.728 |
| Oct | 57 | -$10,764.12 | 33.33% | 0.713 |
| Nov | 53 | -$3,483.48 | 35.85% | 0.893 |
| Dec | 45 | $9,192.80 | 44.44% | 1.386 |

2024 remains a regime problem, concentrated in April, September, and October.

## Exit Mix

| Exit | Trades | Net Profit |
| --- | ---: | ---: |
| Stop loss | 2,523 | -$2,547,938.68 |
| Exit on session close | 1,244 | $441,710.96 |
| Profit target | 1,232 | $2,457,642.88 |

Session-close exits remain a large positive contributor.

## Hour Check

Hours 10 and 11 are absent. Hour 2 still has 5 entries for -$980.80, and hour 21 still has 7 entries for -$5,151.12. The filter mostly works but still lets through edge cases around platform-hour/session-boundary behavior.

## Interpretation

This run removes the biggest caveat from the previous aggressive weak-hour export: the per-trade fee template now matches the baseline. The improvement remains intact. `10/11/21/2` is the current best MNQ-003 production candidate configuration, pending 2024 regime validation and exact session-hour mapping.
