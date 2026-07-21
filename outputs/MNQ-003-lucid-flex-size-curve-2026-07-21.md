# MNQ-003 LucidFlex 50k Size Curve

Source trade list: `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-21 07-19.csv`.

This extends the LucidFlex convex payoff model by testing static 1-10 MNQ size on a 50k account.

Assumptions:

- Evaluation: 50k LucidFlex, pass at $53,000, $2,000 EOD MLL, 50% consistency.
- Funded phase: after pass, trade the same MNQ size until first payout, funded breach, or 252 trading days.
- Payout rule: 5 qualifying funded profit days, $150 minimum day for 50k, positive cycle profit, minimum payout request $500.
- Payout amount: trader receives 90% of `min(50% of funded profit, $2,000)`.
- Cost: $90 per challenge attempt.
- No repeated payout cycles yet.

## Static Size Curve

| Size | Eval Pass Rate | Eval Breach Rate | Joint First-Payout Rate | Avg Days To Resolution | Median Days When Paid | Avg Payout When Paid | EV / Attempt | EV / Day | EV / Year / Slot | Payouts / Year / Slot |
| ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| 1 MNQ | 84.14% | 13.11% | 73.37% | 235.4 | 273 | $515.50 | $288.20 | $1.22 | $308.52 | 0.79 |
| 2 MNQ | 55.27% | 44.61% | 42.35% | 81.9 | 91 | $562.52 | $148.21 | $1.81 | $456.15 | 1.30 |
| 3 MNQ | 51.34% | 48.59% | 35.10% | 48.4 | 66 | $671.12 | $145.59 | $3.01 | $758.70 | 1.83 |
| 4 MNQ | 46.10% | 53.86% | 26.19% | 30.9 | 45 | $751.89 | $106.91 | $3.46 | $872.78 | 2.14 |
| 5 MNQ | 43.31% | 56.69% | 25.11% | 22.7 | 38 | $848.12 | $122.98 | $5.43 | $1,368.07 | 2.79 |
| 6 MNQ | 41.20% | 58.80% | 21.81% | 17.6 | 28 | $980.14 | $123.72 | $7.02 | $1,769.48 | 3.12 |
| 7 MNQ | 38.30% | 61.70% | 21.03% | 14.6 | 25 | $1,010.29 | $122.42 | $8.38 | $2,112.46 | 3.63 |
| 8 MNQ | 37.44% | 62.56% | 19.09% | 13.2 | 23 | $1,132.36 | $126.21 | $9.53 | $2,402.71 | 3.63 |
| 9 MNQ | 36.74% | 63.26% | 17.83% | 12.1 | 22 | $1,169.16 | $118.47 | $9.77 | $2,462.08 | 3.71 |
| 10 MNQ | 32.02% | 67.98% | 13.45% | 10.0 | 21 | $1,196.47 | $70.89 | $7.11 | $1,790.54 | 3.40 |

## Readout

Increasing size changes the model materially. The best EV/day in this first curve is around 8-9 MNQ, not 2 MNQ.

The reason is the convex payoff structure:

- Challenge loss remains capped near the account fee.
- Higher size resolves attempts much faster.
- Successful payout size rises with funded profit.
- Breach rate rises, but not fast enough to offset the speed and payout-size gains until around 10 MNQ in this sample.

Important caveat: this is a highly aggressive account-farming model, not a conservative trading plan. Evaluation breach rates above 60% are normal in the top EV/day region. This only makes sense if the operator is intentionally buying optionality and can tolerate frequent failed accounts.

Working leader from this first size curve:

- Speed/EV leader: 8-9 MNQ static.
- Less violent compromise: 5-6 MNQ static.
- Prior 2 MNQ plan now looks too conservative if the objective is purely time-weighted convex EV.

Next test should compare:

- Eval size 5-9 MNQ, funded size 2-9 MNQ separately.
- Immediate first-payout harvesting versus repeated payout cycles.
- Parallel slots with capped total spend, e.g. 5 accounts at 5 MNQ or 8 MNQ.
