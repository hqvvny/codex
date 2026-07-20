# MNQ-003 NT8 Trade List Review - 2R / 35 Trend Bars

Source export: `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-20 07-33.csv`.

Provenance:

- Platform: NinjaTrader Strategy Analyzer trade list export.
- Strategy: `MNQ003EmaLimitEntryStrategy`.
- Instrument: `NQ 09-26`.
- Account: `Backtest`.
- Direction: long-only, inferred from all rows having market position `Kauf`.
- Quantity: 1 contract.
- Date range in trade list: 2016-01-04 20:01 to 2026-07-17 20:16.
- Fees: included; exported fee column totals $28,323.24, about $5.16 per trade.
- Parameter context from user: 200 EMA limit entry, 35 trend bars, 50-point stop, 100-point target, 2:1R.
- Slippage/fill settings: not visible in the trade-list export.

## Summary

| Metric | Value |
| --- | ---: |
| Trades | 5,489 |
| Net profit | $316,128.76 |
| Gross profit | $3,214,918.88 |
| Gross loss | -$2,898,790.12 |
| Profit factor | 1.109 |
| Win rate | 42.03% |
| Winners | 2,307 |
| Losers | 3,182 |
| Avg trade | $57.59 |
| Avg winner | $1,393.55 |
| Avg loser | -$911.00 |
| Max drawdown, trade-list equity | -$46,336.40 |
| Longest closed recovery, trade-list equity | 360.7 days |

The exported trade list matches the user-reported headline closely: about $316.1k net profit and about 42% win rate after fees. Profit factor is positive but modest at 1.109, so the edge is real in this sample but not huge.

## Year Split

| Year | Trades | Net Profit | Win Rate | PF |
| --- | ---: | ---: | ---: | ---: |
| 2016 | 272 | $1,255.48 | 53.31% | 1.017 |
| 2017 | 270 | $27,508.80 | 60.37% | 1.511 |
| 2018 | 364 | $21,091.76 | 45.05% | 1.126 |
| 2019 | 344 | $36,183.96 | 52.91% | 1.292 |
| 2020 | 579 | $31,844.36 | 39.21% | 1.095 |
| 2021 | 546 | $37,068.64 | 41.94% | 1.124 |
| 2022 | 706 | $17,839.04 | 36.69% | 1.041 |
| 2023 | 559 | $70,109.56 | 42.93% | 1.231 |
| 2024 | 615 | -$25,974.40 | 36.75% | 0.930 |
| 2025 | 746 | $76,649.64 | 39.54% | 1.178 |
| 2026 | 488 | $22,551.92 | 36.27% | 1.075 |

2024 is the only negative calendar year in the export. 2016 and 2022 are positive but thin: PF 1.017 and 1.041 leave little room for additional slippage or weaker fills.

## 2024 Month Split

| Month | Trades | Net Profit | Win Rate | PF |
| --- | ---: | ---: | ---: | ---: |
| Jan | 43 | -$5,012.88 | 34.88% | 0.816 |
| Feb | 48 | $8,195.32 | 47.92% | 1.334 |
| Mar | 41 | -$9,588.56 | 31.71% | 0.651 |
| Apr | 50 | -$12,148.00 | 24.00% | 0.646 |
| May | 46 | $9,110.64 | 45.65% | 1.404 |
| Jun | 44 | $5,934.96 | 45.45% | 1.255 |
| Jul | 52 | $894.68 | 34.62% | 1.028 |
| Aug | 64 | -$2,390.24 | 35.94% | 0.940 |
| Sep | 63 | -$15,314.08 | 30.16% | 0.647 |
| Oct | 61 | -$10,787.76 | 32.79% | 0.728 |
| Nov | 55 | -$4,710.80 | 36.36% | 0.863 |
| Dec | 48 | $9,842.32 | 45.83% | 1.398 |

The weak 2024 regime is not one isolated trade. It clusters in March-April and September-November, which makes this a regime/filter problem rather than a single outlier problem.

## Exit Mix

| Exit | Trades | Net Profit |
| --- | ---: | ---: |
| Stop loss | 2,764 | -$2,780,881.24 |
| Profit target | 1,307 | $2,607,255.88 |
| Exit on session close | 1,418 | $489,754.12 |

Session-close exits contribute materially. This means the strategy is not a pure 2R bracket system in practice; a large share of edge comes from open trades being closed at the session boundary before either bracket is hit.

## Notes

- The headline is promising: positive in 10 of 11 calendar years, fees included, and 2R/35 trend bars remains the best reported variant so far.
- The edge is still modest by profit factor. More realistic slippage or different fill assumptions could matter.
- The 2024 loss cluster should be the next research target. Do not optimize targets further until 2024 is explained.
- The hour-of-day split suggests possible filters, with entry hours 10, 11, and 21 local platform time weak in this export, but this needs timezone/session confirmation before use.
