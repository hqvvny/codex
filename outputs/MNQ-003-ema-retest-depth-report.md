# MNQ-003 EMA Retest Depth Report

Source:

- `outputs/MNQ-003-ema-retest-trades.csv`
- Generated from the first MNQ-003 Python backtest using DATA-001 RTH 1m data.

Definitions:

- Long retest depth uses `signal_low - EMA`.
  - Negative means the signal candle low pushed below the EMA.
  - Positive means the low stayed above the EMA.
- Short retest depth uses `signal_high - EMA`.
  - Positive means the signal candle high pushed above the EMA.
  - Negative means the high stayed below the EMA.
- `penetration_points` means only the amount through the EMA in the wrong direction.
- `abs_distance_to_ema_points` means absolute distance from the retest extreme to the EMA, whether it crossed or not.

## Summary

| Side | Signals | Avg Abs Distance | Median Abs Distance | P75 | P90 | P95 | Crossed EMA | Avg Penetration When Crossed |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| All | 26,396 | 5.36 pts | 5.53 pts | 8.12 pts | 9.37 pts | 9.70 pts | 8.74% | 1.87 pts |
| Long | 14,574 | 5.43 pts | 5.62 pts | 8.17 pts | 9.41 pts | 9.72 pts | 8.52% | 1.89 pts |
| Short | 11,822 | 5.27 pts | 5.39 pts | 8.06 pts | 9.32 pts | 9.65 pts | 9.02% | 1.85 pts |

## Interpretation

- The current 10-point tolerance is wide enough to include almost all selected retests near its upper bound; P95 is around 9.7 points because the rule caps inclusion at 10 points.
- Most selected retests do not actually pierce the EMA. They remain on the trend side and come within the tolerance zone.
- When price does pierce the EMA, the average penetration is small: about 1.9 points.
- This suggests a tighter first retest definition may be worth testing, such as `MaxRetestPoints = 5`, or separating "touch from trend side" from "pierce and reclaim/reject".

## Next Test Ideas

- Compare `MaxRetestPoints` values: 2, 5, 7.5, 10.
- Split retests into:
  - trend-side touch only, no EMA cross
  - EMA pierce and reclaim/reject
- Test long-only first because the first MNQ-003 backtest showed shorts were weaker.
