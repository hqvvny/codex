# MNQ-003 NT8 Trade List Review - Advanced Baseline

Source export: `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-20 10-27.csv`.

Provenance:

- Platform: NinjaTrader Strategy Analyzer trade list export.
- Strategy: `MNQ003EmaLimitEntryAdvancedStrategy`.
- Instrument: `NQ 09-26`.
- Account: `Backtest`.
- Direction: long-only, inferred from all rows having market position `Kauf`.
- Quantity: 1 contract.
- Date range in trade list: 2016-01-04 20:01 to 2026-07-17 20:16.
- Parameter intent: advanced strategy with all new filters off, intended to replicate 2R / 35-trend-bars baseline.
- Fees: included; exported fee column totals $31,616.64, about $5.76 per trade.
- Slippage/fill settings: not visible in the trade-list export.

## Summary

| Metric | Value |
| --- | ---: |
| Trades | 5,489 |
| Net profit | $297,643.36 |
| Gross profit | $3,210,210.48 |
| Gross loss | -$2,912,567.12 |
| Profit factor | 1.102 |
| Win rate | 41.94% |
| Winners | 2,302 |
| Losers | 3,187 |
| Avg trade | $54.23 |
| Fees | $31,616.64 |
| Max drawdown, trade-list equity | -$48,082.40 |
| Longest closed recovery, trade-list equity | 360.8 days |

## Comparison To Prior Simple-Strategy Baseline

Prior reference: `outputs/MNQ-003-nt8-tradelist-2026-07-20-0733-report.md`.

| Metric | Simple Baseline | Advanced Baseline |
| --- | ---: | ---: |
| Strategy | `MNQ003EmaLimitEntryStrategy` | `MNQ003EmaLimitEntryAdvancedStrategy` |
| Trades | 5,489 | 5,489 |
| Net profit | $316,128.76 | $297,643.36 |
| Profit factor | 1.109 | 1.102 |
| Win rate | 42.03% | 41.94% |
| Fees | $28,323.24 | $31,616.64 |
| Avg fee/trade | about $5.16 | about $5.76 |
| Max drawdown | -$46,336.40 | -$48,082.40 |
| Longest recovery | about 360.7 days | about 360.8 days |

The advanced strategy reproduces the same number of trades as the simple baseline, which suggests the core entry logic is compatible when all new filters are disabled. The lower net profit appears driven by higher exported fees and minor fill/exit-price differences.

## Year Split

| Year | Trades | Net Profit | Win Rate | PF |
| --- | ---: | ---: | ---: | ---: |
| 2016 | 272 | $268.28 | 52.94% | 1.004 |
| 2017 | 270 | $26,514.80 | 60.00% | 1.489 |
| 2018 | 364 | $19,813.36 | 45.05% | 1.117 |
| 2019 | 344 | $34,913.56 | 52.91% | 1.280 |
| 2020 | 579 | $29,924.96 | 39.21% | 1.089 |
| 2021 | 546 | $35,225.04 | 41.94% | 1.117 |
| 2022 | 706 | $15,563.44 | 36.69% | 1.035 |
| 2023 | 559 | $68,290.16 | 42.75% | 1.224 |
| 2024 | 615 | -$28,127.40 | 36.42% | 0.925 |
| 2025 | 746 | $74,278.04 | 39.54% | 1.172 |
| 2026 | 488 | $20,979.12 | 36.27% | 1.069 |

2024 remains the only negative year. 2016 becomes nearly flat after the higher fee assumption, so this branch remains sensitive to execution costs.

## Exit Mix

| Exit | Trades | Net Profit |
| --- | ---: | ---: |
| Stop loss | 2,764 | -$2,793,015.64 |
| Profit target | 1,307 | $2,606,471.68 |
| Exit on session close | 1,418 | $484,187.32 |

Session-close exits still contribute materially, so the advanced baseline remains a 2R bracket plus session-close-behavior strategy rather than a pure fixed-bracket system.

## Interpretation

The advanced strategy passes the first compatibility check: same trade count and same broad equity behavior as the simple version. Before testing filters, keep the commission template constant across simple and advanced runs so we do not confuse filter impact with fee-template impact.
