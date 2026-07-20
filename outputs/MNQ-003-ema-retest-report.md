# MNQ-003 200 EMA Retest Report

Generated with:

```bash
python3 scripts/mnq003_ema_retest.py --input data/processed/DATA-001/ohlcv_1m.csv --output-dir outputs --ema-period 200 --tolerance-points 10 --trend-bars 10 --stop-buffer-points 2 --risk-reward 1.0 --same-bar-policy stop_first --point-value 20 --round-turn-cost-points 0
```

Provenance:

- Data: DATA-001 MotiveWave NQU6 1m RTH export.
- Date range in file: 2017-04-17 to 2026-07-10.
- Costs/slippage: 0 points round turn.
- Point value: $20/point, matching NQ.
- EMA: 200-period EMA on 1m close.
- Retest tolerance: maximum 10 points through or away from EMA.
- Trend confirmation: 10 prior closes on the correct side of EMA.
- Entry: next bar open after rejection bar.
- Stop: retest candle extreme plus 2 points.
- Target: 1R.
- Multiple trades: allowed, but no overlapping positions.
- Conservative fill: if stop and target are both touched inside one 1m candle, stop fills first.

## Main Result

| Side | Trades | Net Pts | Avg Pts | Win Rate | PF | Max DD Pts | Recovery Trades |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| All | 26,396 | -2,911.50 | -0.110 | 48.09% | 0.957 | -3,735.00 | 16,387 |
| Long | 14,574 | -701.50 | -0.048 | 48.44% | 0.981 | -1,704.25 | 8,909 |
| Short | 11,822 | -2,210.00 | -0.187 | 47.65% | 0.930 | -2,352.75 | 10,217 |

Interpretation:

- The first mechanical 1R version is negative before costs.
- Longs are close to breakeven before costs; shorts are clearly weaker.
- Trade count is very high, which suggests the raw rule is overactive and likely trading chop around the EMA.
- The 1R target does not meet the usual >=1.5R research standard, but it was tested because the user requested this first version.

## Year Split

| Year | Trades | Net Pts | Avg Pts | Win Rate | PF | Max DD Pts |
| ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| 2017 | 1,746 | -93.50 | -0.054 | 48.28% | 0.963 | -162.75 |
| 2018 | 2,838 | 378.00 | 0.133 | 51.66% | 1.074 | -189.75 |
| 2019 | 4,214 | 8.00 | 0.002 | 50.07% | 1.001 | -245.50 |
| 2020 | 2,920 | 224.50 | 0.077 | 48.77% | 1.029 | -353.75 |
| 2021 | 3,116 | -375.00 | -0.120 | 48.59% | 0.952 | -664.00 |
| 2022 | 2,111 | -367.25 | -0.174 | 46.52% | 0.951 | -603.25 |
| 2023 | 2,920 | -860.75 | -0.295 | 47.74% | 0.894 | -1,005.25 |
| 2024 | 2,974 | -811.75 | -0.273 | 46.47% | 0.912 | -992.50 |
| 2025 | 2,547 | -405.25 | -0.159 | 45.58% | 0.953 | -681.25 |
| 2026 | 1,010 | -608.50 | -0.603 | 41.29% | 0.849 | -693.00 |

Interpretation:

- The edge degrades badly after 2020.
- 2023-2026 are especially weak.
- This is not a robust trend-continuation signal in raw full-RTH form.

## Sensitivity Checks

### Max Risk 10 Points

Generated with `--max-risk-points 10`.

| Side | Trades | Avg Pts | PF | Max DD Pts |
| --- | ---: | ---: | ---: | ---: |
| All | 26,057 | -0.159 | 0.928 | -4,811.25 |
| Long | 14,401 | -0.144 | 0.934 | -2,805.50 |
| Short | 11,656 | -0.178 | 0.921 | -2,237.50 |

Risk-capping at 10 points does not help; it makes the result worse.

### Target-First Bound

Generated with `--same-bar-policy target_first`.

| Side | Trades | Avg Pts | PF | Max DD Pts |
| --- | ---: | ---: | ---: | ---: |
| All | 26,396 | 0.239 | 1.100 | -372.00 |
| Long | 14,574 | 0.295 | 1.127 | -313.25 |
| Short | 11,822 | 0.170 | 1.069 | -342.00 |

Interpretation:

- The target-first result is not trustworthy on 1m OHLC because it assumes the target fills before the stop when both are touched in the same candle.
- The huge difference between stop-first and target-first means this strategy is highly sensitive to intrabar fill order.
- Tick or 1-second data would be required before trusting tight 1R EMA-retest brackets.

## Verdict

The first mechanical version fails under conservative assumptions. Do not optimize this raw 1m 200 EMA retest rule directly. If the idea is worth another pass, the next version should reduce chop exposure:

- Test long-only first.
- Restrict to session windows, especially NY open and London open.
- Add EMA slope or higher-timeframe trend filter.
- Add distance-from-VWAP or prior-day range-location context.
- Require a displacement away from EMA before retest, not just any touch after 10 closes.
