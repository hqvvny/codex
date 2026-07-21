# MNQ-003 LucidFlex 2k Budget Monthly Target

Source trade list: `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-21 07-19.csv`.

Question: how does the LucidFlex account-farming model look with about $2k budget, targeting around $1.5k monthly cashflow for two people?

## Assumptions

- Starting budget: $2,000.
- Challenge cost: $90 per 50k attempt.
- Account: 50k LucidFlex.
- Month: 21 trading days.
- Horizon: 252 trading days.
- Slots: 1-5 active serial account engines.
- Each slot recycles after the account lifecycle ends.
- Payout model: continue funded account for up to 5 payout cycles.
- Trader cashflow is after 90% split.
- Parallel slots are modeled as independent/staggered. Same-day copy-traded starts will be more correlated.

Tested policies:

- Balanced: Eval 7 MNQ / Funded 8 MNQ / $1,500 gross payout target.
- Middle: Eval 9 MNQ / Funded 8 MNQ / $1,500 gross payout target.
- Cashflow: Eval 9 MNQ / Funded 10 MNQ / $2,000 gross payout target.
- Aggressive: Eval 9 MNQ / Funded 12 MNQ / $2,000 gross payout target.

## Best Monthly Cashflow Policy

Policy: `Eval 9 MNQ -> Funded 10 MNQ -> $2,000 gross payout target -> up to 5 payouts`.

| Slots | Mean Net 252d | Median Net 252d | P10 Net | Profit Prob | Ruin Prob | Avg Monthly Cashflow | Median Month | Months >= $1.5k | Zero-Cash Months |
| ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| 1 | $5,490 | $6,120 | -$270 | 87.8% | 4.0% | $562 | $0 | 26.4% | 73.6% |
| 2 | $10,747 | $11,205 | $2,790 | 93.3% | 5.6% | $1,098 | $0 | 44.3% | 55.7% |
| 3 | $15,958 | $16,560 | $5,850 | 93.9% | 5.8% | $1,628 | $1,800 | 57.2% | 42.8% |
| 4 | $21,359 | $22,140 | $9,810 | 94.6% | 5.3% | $2,177 | $1,800 | 66.7% | 33.3% |
| 5 | $26,227 | $27,180 | $13,410 | 94.2% | 5.8% | $2,680 | $1,800 | 72.9% | 27.1% |

## Balanced Policy

Policy: `Eval 9 MNQ -> Funded 8 MNQ -> $1,500 gross payout target -> up to 5 payouts`.

| Slots | Mean Net 252d | Median Net 252d | P10 Net | Profit Prob | Ruin Prob | Avg Monthly Cashflow | Median Month | Months >= $1.5k | Zero-Cash Months |
| ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| 1 | $4,828 | $4,860 | $720 | 92.0% | 1.6% | $497 | $0 | 5.7% | 69.1% |
| 2 | $9,576 | $9,720 | $3,960 | 97.3% | 2.1% | $984 | $1,350 | 17.5% | 48.8% |
| 3 | $14,270 | $14,535 | $7,470 | 97.7% | 2.3% | $1,467 | $1,350 | 30.8% | 35.2% |
| 4 | $19,113 | $19,440 | $11,340 | 97.8% | 2.1% | $1,962 | $1,350 | 43.3% | 25.6% |
| 5 | $23,681 | $24,120 | $14,670 | 97.7% | 2.3% | $2,434 | $2,700 | 53.6% | 19.5% |

## Readout

With $2k budget, the model changes materially:

- One account is still not enough for stable monthly cashflow.
- Two slots are better but still below the $1.5k/month target on average.
- Three slots under the cashflow policy are the first configuration that clears the average target: about $1,628/month, $1,800 median month, and $15.96k mean net over 252 trading days.
- Four to five slots are where the monthly target starts to feel more like a business, but zero-cash months still exist.

Best fit for the user's goal:

`3 staggered slots using Eval 9 MNQ / Funded 10 MNQ / $2k gross target`.

Why:

- Average monthly cashflow clears $1.5k.
- Median month is $1.8k.
- P10 annual net is still positive at about $5.85k in the simulation.
- Profit probability is about 93.9%.

If smoother downside is more important:

`3-4 slots using Eval 9 MNQ / Funded 8 MNQ / $1.5k gross target`.

Why:

- Lower ruin risk than the cashflow policy.
- Higher profit probability.
- But monthly target is weaker: 3 slots average only $1.47k/month and median month is $1.35k.

## Practical Plan

Phase 1:

- Run 1 slot live as process validation.
- Use the balanced or cashflow policy depending on comfort.

Phase 2:

- Move to 3 staggered slots.
- This is the first serious configuration for the $1.5k/month objective.

Phase 3:

- Move to 4-5 slots only after payout processing and live fills are verified.

## Caveats

- This still uses one strategy only: MNQ-003. Strategy diversification is required for truly stable two-person cashflow.
- Parallel slots are assumed staggered/independent enough for modeling; identical copy-trading creates correlated dry months.
- Account costs, FX, taxes, payout delays, rule changes, and operational errors are not modeled.
