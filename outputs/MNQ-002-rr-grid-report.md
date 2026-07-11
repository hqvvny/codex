# MNQ-002 R:R Grid Report

Generated with:

```bash
python3 scripts/mnq002_rr_grid.py --rth-input data/processed/DATA-001/ohlcv_1m.csv --output-dir outputs --hold-bars 15,60 --stop-points 10,15,20,25,30 --risk-rewards 1.5,2.0,2.5 --exit-modes bracket_only,bracket_with_time_stop --filter-mode negative --same-bar-policy stop_first --point-value 20 --round-turn-cost-points 0
```

Provenance:

- Data: DATA-001 MotiveWave NQU6 1m RTH export.
- Date range in file: 2017-04-17 to 2026-07-10.
- Strategy sample: 963 overnight-negative sessions after first-session/coverage filters.
- Costs/slippage: 0 points round turn.
- Point value: $20/point, matching NQ.
- Entry: first RTH bar open.
- Same-bar policy: `stop_first`, because 1m OHLC bars do not reveal whether stop or target was hit first inside the same candle.

## Conservative Stop-First Results

Top variants by profit factor:

| Rank | Exit Mode | Hold | Stop | R | Trades | Avg Pts | PF | Max DD Pts | Recovery Trades |
| ---: | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| 1 | `bracket_only` | 15 | 30 | 2.5 | 963 | 2.841 | 1.107 | -2,012.25 | 320 |
| 2 | `bracket_only` | 60 | 30 | 2.5 | 963 | 1.861 | 1.088 | -1,120.50 | 324 |
| 3 | `bracket_with_time_stop` | 15 | 20 | 2.0 | 963 | 0.984 | 1.087 | -560.75 | 257 |
| 4 | `bracket_with_time_stop` | 15 | 30 | 2.5 | 963 | 1.215 | 1.081 | -911.50 | 207 |
| 5 | `bracket_with_time_stop` | 15 | 15 | 2.5 | 963 | 0.763 | 1.080 | -620.75 | 257 |

Interpretation:

- Fixed R:R does not materially improve the edge under conservative fills.
- The best PF is only 1.107, below the prior NT8 timed-exit `OvernightNegativeOnly` PF region around 1.12-1.15.
- The highest-PF conservative run has a very large drawdown of -2,012.25 points and negative median trade of -30 points.
- Bracket-with-time-stop variants reduce drawdown but also reduce average trade and remain low-PF.

## Year Split For Best Conservative Variant

Best conservative variant: `bracket_only`, hold 15, stop 30, R 2.5.

| Year | Trades | Net Pts | Avg Pts | Win Rate | PF | Max DD Pts |
| ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| 2017 | 43 | -198.75 | -4.622 | 51.16% | 0.740 | -321.25 |
| 2018 | 96 | 329.25 | 3.430 | 50.00% | 1.133 | -703.00 |
| 2019 | 100 | -181.75 | -1.818 | 48.00% | 0.924 | -887.25 |
| 2020 | 99 | 1,122.50 | 11.338 | 40.40% | 1.394 | -766.25 |
| 2021 | 103 | 1,756.50 | 17.053 | 43.69% | 1.864 | -321.75 |
| 2022 | 142 | -179.50 | -1.264 | 28.87% | 0.954 | -709.50 |
| 2023 | 119 | 387.25 | 3.254 | 35.29% | 1.102 | -1,152.00 |
| 2024 | 102 | -3.75 | -0.037 | 33.33% | 0.999 | -1,103.00 |
| 2025 | 104 | 100.25 | 0.964 | 30.77% | 1.039 | -753.50 |
| 2026 | 55 | -396.50 | -7.209 | 27.27% | 0.734 | -772.00 |

Interpretation:

- The best conservative R:R variant is not stable enough year-by-year.
- 2020 and 2021 carry much of the result.
- 2026 is negative so far, which is a serious degradation warning.

## Optimistic Target-First Bound

An additional run was generated with `same_bar_policy=target_first` under `outputs/MNQ-002-rr-target-first/`.

Top target-first variant:

| Exit Mode | Hold | Stop | R | Trades | Avg Pts | PF | Max DD Pts | Recovery Trades |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| `bracket_with_time_stop` | 15 | 10 | 1.5 | 963 | 2.220 | 1.445 | -155.00 | 86 |

Interpretation:

- The target-first result is likely too optimistic for 1m OHLC data.
- It proves the test is highly sensitive to intrabar fill assumptions.
- Before trusting tight 10-point stops/targets, use tick or 1-second data, or verify in NT8 with high-order-fill resolution.

## Verdict

R:R brackets are useful as a control, but they do not yet improve the strategy under conservative assumptions. The timed-exit edge is still the stronger baseline. The next structural improvement should not be simple fixed R:R alone; it should test whether opening volatility, prior-day range location, or a higher-timeframe trend filter can remove bad years before tuning exits.
