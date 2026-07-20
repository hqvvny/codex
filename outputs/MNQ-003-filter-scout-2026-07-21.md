# MNQ-003 Filter Scout - Slippage 1 Baseline

Source baseline: `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-21 12-23.csv`.

Baseline provenance:

- Platform/export: NinjaTrader Strategy Analyzer trade list.
- Strategy: `MNQ003EmaLimitEntryStrategy`.
- Instrument: `NQ 09-26`.
- Configuration from user: long-only, whole day, 200 EMA, entry offset 0 points, 35 trend bars, max hold bars 0, quantity 1, 50-point stop, 100-point target, NinjaTrader monthly fees, slippage 1, since 2016.
- Trade-list date range: 2016-01-04 20:01 to 2026-07-20 18:42.
- Baseline: 5,494 trades, $298,890.96 net profit, PF 1.103, 41.92% win rate, -$47,788.40 max drawdown, about 360.8-day closed recovery.

Important limitation: most tests below are offline filters applied to the already exported NinjaTrader trade list. That means fills are the real Ninja fills from the baseline export, but skipped trades do not change later signal generation or pending-order behavior. Treat this as a fast scout, not final Strategy Analyzer proof.

## Offline NT Trade-List Filter Results

| Variant | Trades | Net Profit | PF | Win Rate | Avg Trade | Max DD | Recovery | 2024 Net | Negative Years |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | --- |
| Baseline | 5,494 | $298,890.96 | 1.103 | 41.92% | $54.40 | -$47,788.40 | 360.8d | -$27,758.40 | 2024 |
| Skip hours 10,11,21,2 | 4,478 | $344,918.52 | 1.146 | 42.32% | $77.03 | -$25,512.60 | 406.0d | -$4,445.48 | 2016, 2024 |
| Skip hours 10,11,21 | 4,948 | $338,788.32 | 1.128 | 42.06% | $68.47 | -$32,929.92 | 350.7d | -$18,078.00 | 2024 |
| Skip hours 10,21 | 5,080 | $333,952.20 | 1.123 | 41.89% | $65.74 | -$40,066.36 | 355.8d | -$24,229.76 | 2024 |
| Skip hour 10 | 5,367 | $319,506.28 | 1.113 | 42.20% | $59.53 | -$45,671.16 | 357.7d | -$27,636.00 | 2024 |
| Skip hour 21 | 5,207 | $313,336.88 | 1.112 | 41.60% | $60.18 | -$42,183.60 | 357.0d | -$24,352.16 | 2024 |
| Skip hour 2 | 5,024 | $305,021.16 | 1.115 | 42.14% | $60.71 | -$35,707.76 | 410.0d | -$14,125.88 | 2016, 2024 |
| Cooldown 30m | 5,341 | $287,730.44 | 1.102 | 42.00% | $53.87 | -$48,294.40 | 429.7d | -$34,836.00 | 2024 |
| Cooldown 60m | 5,140 | $286,402.60 | 1.106 | 42.16% | $55.72 | -$46,858.68 | 536.4d | -$34,991.84 | 2016, 2024 |
| Cooldown 120m | 4,610 | $245,797.40 | 1.101 | 42.17% | $53.32 | -$41,478.52 | 466.0d | -$23,097.24 | 2024 |
| Max 1 trade/day proxy | 2,700 | $98,558.00 | 1.069 | 41.74% | $36.50 | -$42,066.80 | 1147.3d | -$19,111.28 | 2016, 2021, 2022, 2024, 2026 |
| Max 2 trades/day proxy | 4,266 | $260,077.44 | 1.115 | 42.33% | $60.97 | -$43,654.36 | 471.9d | -$23,879.88 | 2016, 2024 |
| Max 3 trades/day proxy | 5,043 | $284,238.12 | 1.106 | 42.00% | $56.36 | -$44,841.84 | 357.7d | -$25,541.36 | 2024 |
| Skip 10,11,21,2 + cooldown 60m | 4,219 | $340,239.96 | 1.154 | 42.66% | $80.64 | -$26,595.20 | 406.0d | -$7,185.36 | 2016, 2024 |

## EMA Slope Scout

EMA slope was approximated using `DATA-002` 1m all-sessions bars. Because DATA-002 starts later than the NT export's 2016 start, 499 NT trades had no matching local bar and were skipped in these slope checks. Treat this as directional only.

| Variant | Trades | Net Profit | PF | Win Rate | Avg Trade | Max DD | Recovery | 2024 Net | Negative Years |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | --- |
| EMA slope 10 / 2.5 pts | 603 | $43,143.52 | 1.119 | 38.81% | $71.55 | -$15,304.32 | 940.8d | -$1,981.36 | 2019, 2020, 2022, 2024 |
| EMA slope 20 / 5 pts | 1,154 | $105,795.36 | 1.156 | 39.86% | $91.68 | -$27,748.92 | 1163.9d | -$4,793.04 | 2019, 2020, 2024 |
| EMA slope 40 / 10 pts | 1,713 | $109,445.92 | 1.110 | 39.58% | $63.89 | -$28,181.76 | 1334.9d | -$9,671.48 | 2020, 2024 |
| EMA slope 20 / 2.5 pts | 2,609 | $143,047.56 | 1.095 | 39.82% | $54.83 | -$35,138.96 | 782.0d | -$11,752.64 | 2017, 2018, 2020, 2024 |
| EMA slope 40 / 5 pts | 3,122 | $195,145.48 | 1.109 | 39.88% | $62.51 | -$37,427.32 | 782.0d | -$18,800.76 | 2017, 2018, 2024 |

## Readout

Best next NinjaTrader test: `UseWeakHourFilter = 1` with `WeakHoursCsv = 10,11,21`.

Why not include hour 2 immediately: removing hour 2 improves drawdown and 2024 materially, but it makes 2016 negative and worsens recovery in the offline scout. The broader `10,11,21,2` version has the best-looking headline, but it may be more overfit. Test both, but trust `10,11,21` first.

Cooldown is not attractive as a standalone filter. It reduces trade count and often worsens 2024/recovery. Max trades per day is also not attractive in this offline pass.

EMA slope can raise PF and average trade, but it removes too many trades and creates multi-year weakness / very long recovery in this rough check. Do not prioritize EMA slope until weak-hour filters are confirmed in real NinjaTrader Strategy Analyzer runs.

Recommended next tests in NT8 Clean Strategy:

1. `UseWeakHourFilter = 1`, `WeakHoursCsv = 10,11,21`.
2. `UseWeakHourFilter = 1`, `WeakHoursCsv = 10,11,21,2`.
3. If one of those confirms, combine with `CooldownBars = 0` only; do not add cooldown yet.
4. Only after weak-hour confirmation, test EMA slope gently: `UseEmaSlopeFilter = 1`, `EmaSlopeLookbackBars = 40`, `MinEmaSlopePoints = 5`.
