# MNQ-003 LucidFlex 1k Bankroll Time Profile

Source trade list: `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-21 07-19.csv`.

This extends `outputs/MNQ-003-lucid-flex-1k-bankroll-2026-07-21.md` by adding time-to-cashflow metrics.

Simulation assumptions:

- Starting bankroll: $1,000.
- Challenge cost: $90 per 50k attempt.
- Max active lifecycle slots modeled: 10.
- Immediate recycle after payout or account breach.
- Static size used in evaluation and funded phase.
- First-payout model only.
- Monte Carlo: 12,000 bootstrapped bankroll paths from historical lifecycle outcomes.

Important caveat: this version still does not separately enforce the 5 active funded-account cap. Treat it as a speed/throughput sketch, not final operations code.

## First Payout Timing

| Size | Payout By 30d | Payout By 60d | Payout By 90d | Payout By 252d | Median First Payout Day | P25-P75 First Payout Day | P90 First Payout Day |
| ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| 4 MNQ | 65.88% | 82.58% | 93.16% | 96.26% | 22 | 16-37 | 69 |
| 5 MNQ | 70.43% | 89.21% | 94.98% | 95.95% | 21 | 15-32 | 56 |
| 6 MNQ | 75.25% | 92.51% | 93.42% | 93.42% | 19 | 15-28 | 42 |
| 7 MNQ | 79.15% | 92.08% | 92.76% | 92.76% | 18 | 14-25 | 36 |
| 8 MNQ | 78.26% | 89.62% | 90.07% | 90.07% | 17 | 14-24 | 34 |
| 9 MNQ | 78.59% | 88.45% | 88.69% | 88.69% | 17 | 14-23 | 32 |
| 10 MNQ | 70.32% | 79.60% | 79.60% | 79.60% | 17 | 14-24 | 31 |

## Net Bankroll By Horizon

Mean net profit after starting with $1,000. P10 is included to show the left tail.

| Size | 30d Mean / P10 | 60d Mean / P10 | 90d Mean / P10 | 180d Mean / P10 | 252d Mean / P10 |
| ---: | ---: | ---: | ---: | ---: | ---: |
| 4 MNQ | -$380 / -$990 | $199 / -$990 | $958 / -$984 | $3,632 / -$940 | $5,894 / -$687 |
| 5 MNQ | -$198 / -$990 | $789 / -$990 | $2,043 / -$975 | $6,281 / -$913 | $9,859 / $1,470 |
| 6 MNQ | $12 / -$990 | $1,475 / -$990 | $3,155 / -$977 | $8,614 / -$944 | $13,102 / -$938 |
| 7 MNQ | $39 / -$990 | $1,815 / -$988 | $3,829 / -$978 | $10,349 / -$957 | $15,568 / -$954 |
| 8 MNQ | $152 / -$990 | $2,192 / -$990 | $4,496 / -$990 | $11,685 / -$990 | $17,465 / -$990 |
| 9 MNQ | $88 / -$990 | $2,143 / -$990 | $4,460 / -$990 | $11,550 / -$990 | $17,350 / -$990 |
| 10 MNQ | -$320 / -$990 | $901 / -$990 | $2,218 / -$990 | $6,329 / -$990 | $9,631 / -$990 |

## Profit Probability By Horizon

| Size | 30d | 60d | 90d | 120d | 180d | 252d |
| ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| 4 MNQ | 24.5% | 42.7% | 58.4% | 70.2% | 82.6% | 88.6% |
| 5 MNQ | 29.9% | 53.8% | 70.2% | 79.5% | 88.1% | 91.2% |
| 6 MNQ | 35.7% | 63.0% | 76.3% | 82.9% | 87.5% | 88.4% |
| 7 MNQ | 36.5% | 65.1% | 77.5% | 82.9% | 86.6% | 87.3% |
| 8 MNQ | 38.6% | 66.8% | 77.9% | 81.8% | 84.0% | 84.4% |
| 9 MNQ | 37.0% | 65.2% | 75.9% | 79.8% | 81.8% | 82.2% |
| 10 MNQ | 24.2% | 45.3% | 55.4% | 60.2% | 63.5% | 64.5% |

## Readout

Adding time changes the 1k bankroll read:

- Fastest first-payout profile: 7-9 MNQ. Median first payout arrives around day 17-18, with 60-day payout probability around 88-92%.
- Best 60-90 day net growth: 8 MNQ edges out the field on mean net, but with a persistent -$990 P10 tail.
- Best smoother business start: 5 MNQ. It has slightly slower first payouts but the only tested size with positive 252-day P10 net.
- Too aggressive: 10 MNQ. It has fast wins, but too many paths burn the bankroll.

Working recommendation with 1k budget:

- Start research/ops plan around 5 MNQ if the first goal is staying alive and learning the process.
- Use 7-8 MNQ if the first goal is maximizing cashflow speed and accepting that the 1k bankroll can be lost.
- Avoid 10 MNQ for now.

Next exact model should enforce both account caps: 10 total accounts and 5 active funded accounts.
