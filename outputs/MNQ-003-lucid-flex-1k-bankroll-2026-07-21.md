# MNQ-003 LucidFlex 1k Bankroll Simulation

Source trade list: `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-21 07-19.csv`.

Official rule note checked 2026-07-21:

- Lucid maximum account page: `https://support.lucidtrading.com/en/articles/11404617-maximum-number-of-accounts`.
- Relevant limits: up to 10 active evaluation accounts per household/family, shared 10 total across evaluation and funded accounts, and up to 5 active funded accounts.

Simulation assumptions:

- Starting bankroll: $1,000 proxy for about 1k budget.
- Challenge cost: $90 per 50k attempt.
- Max active slots modeled: 10.
- Immediate recycle: after payout or account breach, buy a new challenge if bankroll >= $90 and a slot is open.
- Horizon: 252 trading days.
- Static size is used in both evaluation and funded phase.
- Payout model: first payout only, same as prior LucidFlex first-payout model.
- Monte Carlo: 20,000 bootstrapped paths from historical rolling-start lifecycle outcomes.

Important caveat: this is a throughput approximation. It models up to 10 active lifecycle slots but does not separately enforce the 5 active funded-account cap. The next model should split evaluation and funded states explicitly.

## Results

| Size | Mean Net | Median Net | P10 Net | P25 Net | P75 Net | P90 Net | Profit Probability | Bankroll >= $2k | Ruin / No Active | Mean Payouts | Median Payouts | Mean Attempts |
| ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| 2 MNQ | $2,281 | $2,252 | -$116 | $1,055 | $3,446 | $4,579 | 88.98% | 76.00% | 0.29% | 9.58 | 10 | 37.91 |
| 4 MNQ | $5,926 | $5,999 | -$472 | $3,222 | $8,612 | $11,036 | 88.98% | 86.23% | 6.49% | 16.73 | 18 | 80.15 |
| 5 MNQ | $9,891 | $10,308 | $1,420 | $6,675 | $13,590 | $16,588 | 91.19% | 90.45% | 7.54% | 22.09 | 24 | 105.58 |
| 6 MNQ | $13,075 | $13,892 | -$941 | $9,372 | $17,956 | $21,636 | 88.09% | 87.94% | 11.68% | 24.71 | 27 | 129.28 |
| 7 MNQ | $15,496 | $16,698 | -$963 | $11,483 | $21,334 | $25,368 | 86.77% | 86.67% | 13.14% | 28.36 | 32 | 151.78 |
| 8 MNQ | $17,612 | $19,304 | -$986 | $13,164 | $24,277 | $28,646 | 85.27% | 85.25% | 14.70% | 28.23 | 32 | 164.53 |
| 9 MNQ | $17,321 | $19,316 | -$990 | $12,088 | $24,778 | $29,414 | 81.85% | 81.81% | 18.12% | 27.74 | 32 | 171.88 |
| 10 MNQ | $9,773 | $10,674 | -$990 | -$972 | $17,156 | $22,127 | 65.30% | 65.06% | 34.38% | 20.65 | 27 | 168.15 |

## Readout

With about 1k budget, the model strongly prefers more size than the earlier 2 MNQ baseline.

The best region in this first bankroll simulation is 7-9 MNQ:

- 8 MNQ has the highest mean net at about $17.6k and median net at about $19.3k.
- 9 MNQ is similar but has higher ruin/no-active probability.
- 5 MNQ is the smoother compromise: lower mean but the best profit probability in this run and positive P10 net.
- 10 MNQ is too aggressive; the left tail gets ugly and expected value drops.

Working deployment read:

- Conservative business start: 5 MNQ across up to 10 slots.
- Aggressive EV start: 8 MNQ across up to 10 slots.
- Avoid 10 MNQ unless future state-machine modeling contradicts this first curve.

Next model:

- Explicitly separate evaluation slots and funded slots.
- Enforce max 10 total accounts and max 5 funded accounts.
- Simulate account starts as correlated or staggered, not purely bootstrapped independent lifecycle outcomes.
- Add actual EUR/USD and exact purchase cost once available.
