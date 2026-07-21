# MNQ-003 LucidFlex 1k Serial Bankroll

Source trade list: `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-21 07-19.csv`.

This corrects the prior 1k bankroll model for the user's intended operating plan:

- Start with 0 accounts.
- Buy one 50k LucidFlex evaluation.
- Trade it until breach or first payout.
- Only then buy the next account if bankroll still has at least $90.
- No parallel accounts.

Simulation assumptions:

- Starting bankroll: $1,000.
- Challenge cost: $90 per 50k attempt.
- Active accounts: exactly 1 at a time.
- Horizon: 252 trading days.
- Static size in both evaluation and funded phase.
- First-payout model only.
- Monte Carlo: 20,000 bootstrapped serial bankroll paths from historical lifecycle outcomes.

## Serial Results

| Size | Mean Net | Median Net | P10 Net | P25 Net | P75 Net | P90 Net | Profit Probability | Ruin Probability | Mean Payouts | Median Payouts |
| ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| 4 MNQ | $699 | $518 | -$507 | -$72 | $1,311 | $2,153 | 72.1% | 2.3% | 1.95 | 2 |
| 5 MNQ | $1,150 | $972 | -$473 | $145 | $1,974 | $2,996 | 79.2% | 4.2% | 2.54 | 3 |
| 6 MNQ | $1,516 | $1,370 | -$752 | $263 | $2,583 | $3,774 | 80.0% | 8.0% | 2.85 | 3 |
| 7 MNQ | $1,801 | $1,681 | -$923 | $408 | $3,010 | $4,295 | 81.3% | 9.8% | 3.28 | 3 |
| 8 MNQ | $2,043 | $1,936 | -$982 | $441 | $3,432 | $4,856 | 80.5% | 12.4% | 3.26 | 3 |
| 9 MNQ | $2,059 | $1,954 | -$990 | $353 | $3,543 | $4,987 | 78.6% | 15.1% | 3.27 | 3 |
| 10 MNQ | $1,337 | $1,098 | -$990 | -$946 | $2,776 | $4,301 | 65.0% | 27.4% | 2.77 | 3 |

## First Payout Timing

| Size | Payout By 30d | Payout By 60d | Payout By 90d | Payout By 180d | Payout By 252d | Median First Payout Day |
| ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| 4 MNQ | 12.4% | 28.7% | 47.7% | 81.6% | 91.0% | 87 |
| 5 MNQ | 15.4% | 37.8% | 61.3% | 90.0% | 95.0% | 71 |
| 6 MNQ | 18.0% | 47.8% | 68.5% | 90.9% | 93.0% | 59 |
| 7 MNQ | 22.5% | 55.9% | 75.1% | 91.9% | 92.7% | 51 |
| 8 MNQ | 23.9% | 56.5% | 75.0% | 90.0% | 90.4% | 49 |
| 9 MNQ | 24.6% | 57.5% | 75.2% | 88.4% | 88.6% | 47 |
| 10 MNQ | 23.4% | 54.6% | 70.3% | 80.1% | 80.2% | 45 |

## Horizon Net Profile

| Size | 60d Mean / Profit % | 90d Mean / Profit % | 180d Mean / Profit % | 252d Mean / Profit % |
| ---: | ---: | ---: | ---: | ---: |
| 5 MNQ | $112 / 37.3% | $279 / 57.0% | $767 / 71.7% | $1,154 / 79.3% |
| 6 MNQ | $210 / 46.9% | $419 / 61.8% | $1,039 / 74.3% | $1,520 / 80.0% |
| 7 MNQ | $276 / 53.4% | $521 / 63.6% | $1,249 / 76.9% | $1,807 / 81.4% |
| 8 MNQ | $336 / 54.5% | $623 / 65.4% | $1,445 / 77.1% | $2,049 / 80.6% |
| 9 MNQ | $340 / 54.4% | $624 / 64.0% | $1,448 / 75.6% | $2,066 / 78.7% |
| 10 MNQ | $196 / 49.5% | $415 / 56.9% | $962 / 63.6% | $1,343 / 65.0% |

## Readout

This serial model is much more conservative than the prior 10-active-slot bankroll model and matches the user's intended operating plan better.

Main conclusions:

- The previous multi-slot five-figure annual expectation does not apply if only one account runs at a time.
- 8-9 MNQ still gives the highest expected net after 252 trading days, around $2.0k mean net, but with larger ruin/tail risk.
- 7 MNQ is the cleanest compromise: strong first-payout speed, best profit probability among the aggressive sizes, and lower ruin risk than 8-9 MNQ.
- 5 MNQ is smoother but meaningfully slower.
- 10 MNQ remains too aggressive.

Working serial plan:

- Start one account at a time.
- Use 7 MNQ as the first serious candidate.
- Use 8 MNQ only if maximizing speed matters more than the higher ruin probability.
- Avoid 10 MNQ.

Next model should test a serial adaptive plan: start 7 MNQ, drop to 5 MNQ after bankroll falls below $500, and increase to 8 MNQ after bankroll exceeds $1,500.
