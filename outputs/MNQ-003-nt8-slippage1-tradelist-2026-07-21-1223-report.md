# MNQ-003 NT8 Trade List Review - Slippage 1 Baseline

Source export: `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-21 12-23.csv`.

Provenance:

- Platform: NinjaTrader Strategy Analyzer trade list export.
- Strategy: `MNQ003EmaLimitEntryStrategy`.
- Instrument: `NQ 09-26`.
- Account: `Backtest`.
- Direction: long-only, inferred from all rows having market position `Kauf`.
- Quantity: 1 contract.
- Date range in trade list: 2016-01-04 20:01 to 2026-07-20 18:42.
- User-stated configuration: whole day, 200 EMA, entry offset 0 points, 35 trend bars, max hold bars 0, 50-point stop, 100-point target, NinjaTrader monthly fees, slippage 1.
- Fees: included; exported fee column totals $28,349.04, about $5.16 per trade.
- Slippage: user-stated 1.

## Summary

| Metric | Value |
| --- | ---: |
| Trades | 5,494 |
| Net profit | $298,890.96 |
| Gross profit | $3,213,586.52 |
| Gross loss | -$2,914,695.56 |
| Profit factor | 1.103 |
| Win rate | 41.92% |
| Winners | 2,303 |
| Losers | 3,191 |
| Avg trade | $54.40 |
| Avg winner | $1,395.39 |
| Avg loser | -$913.41 |
| Fees | $28,349.04 |
| Avg bars in trade | 364.26 |
| Avg MAE | $704.50 |
| Avg MFE | $986.07 |
| Avg ETD | $931.66 |
| Max drawdown, trade-list equity | -$47,788.40 |
| Longest closed recovery, trade-list equity | 360.8 days |

## Comparison To Previous No-Slippage Trade List

Previous reference: `outputs/MNQ-003-nt8-tradelist-2026-07-20-0733-report.md`.

| Metric | Prior Export | Slippage 1 Export |
| --- | ---: | ---: |
| Trades | 5,489 | 5,494 |
| Net profit | $316,128.76 | $298,890.96 |
| Profit factor | 1.109 | 1.103 |
| Win rate | 42.03% | 41.92% |
| Avg trade | $57.59 | $54.40 |
| Fees | $28,323.24 | $28,349.04 |
| Max drawdown | -$46,336.40 | -$47,788.40 |
| Longest recovery | about 360.7 days | about 360.8 days |

The strategy remains profitable after slippage 1, but expectancy and drawdown both degrade modestly. The trade count differs by 5 trades because this export runs through 2026-07-20, while the previous trade-list export ended on 2026-07-17.

## Year Split

| Year | Trades | Net Profit | Win Rate | PF |
| --- | ---: | ---: | ---: | ---: |
| 2016 | 272 | $431.48 | 52.94% | 1.006 |
| 2017 | 270 | $26,676.80 | 60.00% | 1.493 |
| 2018 | 364 | $20,031.76 | 45.05% | 1.119 |
| 2019 | 344 | $35,119.96 | 52.91% | 1.282 |
| 2020 | 579 | $30,272.36 | 39.21% | 1.090 |
| 2021 | 546 | $35,552.64 | 41.94% | 1.119 |
| 2022 | 706 | $15,987.04 | 36.69% | 1.036 |
| 2023 | 559 | $68,625.56 | 42.75% | 1.225 |
| 2024 | 615 | -$27,758.40 | 36.42% | 0.926 |
| 2025 | 746 | $74,725.64 | 39.54% | 1.173 |
| 2026 | 493 | $19,226.12 | 36.11% | 1.063 |

2024 remains the only negative year. 2016 and 2022 remain thin after realistic friction, with PF close to 1.

## 2024 Month Split

| Month | Trades | Net Profit | Win Rate | PF |
| --- | ---: | ---: | ---: | ---: |
| Jan | 43 | -$5,136.88 | 34.88% | 0.812 |
| Feb | 48 | $8,067.32 | 47.92% | 1.328 |
| Mar | 41 | -$9,716.56 | 29.27% | 0.647 |
| Apr | 50 | -$12,308.00 | 24.00% | 0.643 |
| May | 46 | $8,982.64 | 45.65% | 1.397 |
| Jun | 44 | $5,822.96 | 45.45% | 1.250 |
| Jul | 52 | $746.68 | 32.69% | 1.023 |
| Aug | 64 | -$2,570.24 | 35.94% | 0.936 |
| Sep | 63 | -$15,510.08 | 30.16% | 0.644 |
| Oct | 61 | -$10,979.76 | 32.79% | 0.724 |
| Nov | 55 | -$4,878.80 | 36.36% | 0.858 |
| Dec | 48 | $9,722.32 | 45.83% | 1.392 |

The weak 2024 clusters remain March-April and September-November.

## Exit Mix

| Exit | Trades | Net Profit |
| --- | ---: | ---: |
| Stop loss | 2,768 | -$2,795,397.88 |
| Profit target | 1,308 | $2,609,250.72 |
| Exit on session close | 1,418 | $485,038.12 |

Session-close exits still contribute materially. The strategy remains a 2R bracket plus session-close-behavior setup.

## Hour Diagnostic

Weak platform entry hours in this export:

- Hour 10: 127 trades, -$20,615.32.
- Hour 21: 287 trades, -$14,445.92.
- Hour 2: 470 trades, -$6,130.20.
- Hour 11: 132 trades, -$4,836.12.

Strong platform entry hours include 19, 17, 4, 13, and 7. Treat these as diagnostic only until timezone/session mapping is locked.

## Interpretation

This is a healthier baseline than the zero-slippage test because it includes more realistic friction. It remains a promising candidate: positive in 10 of 11 calendar years, about 42% win rate at 2R, and average trade above $50 after fees and slippage. The weak points remain unchanged: profit factor is only about 1.10, recovery is about one year, 2024 is a real regime failure, and the edge depends materially on session-close exits.
