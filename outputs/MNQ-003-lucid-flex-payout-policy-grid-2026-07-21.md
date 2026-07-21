# MNQ-003 LucidFlex Payout Policy Grid

Source trade list: `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-21 07-19.csv`.

Detailed grid CSV: `outputs/MNQ-003-lucid-flex-payout-policy-grid-2026-07-21.csv`.

This tests how to farm larger LucidFlex payouts instead of requesting the first minimum payout.

## Assumptions

- Starting bankroll: $1,000.
- Operating model: serial, one account active at a time.
- Challenge cost: $90 per 50k attempt.
- Account: 50k LucidFlex.
- Evaluation target: $53,000.
- Evaluation MLL: $2,000 EOD.
- Evaluation consistency: largest winning day <= 50% of profit at pass.
- Funded phase starts fresh after pass.
- Payout targets are gross request targets: $500, $1,000, $1,500, or $2,000.
- Trader receives 90% of gross payout.
- 50k max gross payout is $2,000.
- First-payout model only; no repeated payout cycles yet.
- Monte Carlo: 6,000 serial bankroll paths per configuration.

Grid:

- Evaluation size: 5, 7, 8, 9 MNQ.
- Funded size: 7, 8, 10, 12 MNQ.
- Gross payout target: $500, $1,000, $1,500, $2,000.

## Best 252-Day Mean Net

| Rank | Eval Size | Funded Size | Gross Target | Mean Net 252d | Median Net 252d | Profit Prob | Ruin Prob | 90d Mean Net | Any Payout By 60d | Avg Trader Payout | Median Paid Days |
| ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| 1 | 9 MNQ | 8 MNQ | $1,500 | $2,523 | $2,660 | 80.3% | 14.6% | $751 | 45.5% | $1,599 | 26 |
| 2 | 7 MNQ | 8 MNQ | $1,500 | $2,462 | $2,250 | 83.3% | 11.6% | $706 | 40.8% | $1,589 | 29 |
| 3 | 8 MNQ | 8 MNQ | $1,500 | $2,413 | $2,404 | 80.6% | 13.8% | $694 | 42.8% | $1,599 | 27 |
| 4 | 9 MNQ | 7 MNQ | $1,500 | $2,331 | $2,317 | 80.2% | 14.8% | $649 | 42.8% | $1,580 | 28 |
| 5 | 9 MNQ | 8 MNQ | $1,000 | $2,327 | $2,277 | 81.9% | 13.6% | $721 | 54.0% | $1,320 | 23 |

## Smoother Candidates

Sorted by better downside profile rather than raw mean.

| Candidate | Eval Size | Funded Size | Gross Target | Mean Net 252d | Median Net 252d | P10 Net 252d | Profit Prob | Ruin Prob | Any Payout By 60d | Avg Trader Payout |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| Smoother 1 | 5 MNQ | 8 MNQ | $1,000 | $1,740 | $1,576 | -$499 | 83.7% | 7.1% | 33.0% | $1,327 |
| Smoother 2 | 5 MNQ | 7 MNQ | $1,000 | $1,624 | $1,479 | -$551 | 82.5% | 7.1% | 32.9% | $1,258 |
| Smoother 3 | 7 MNQ | 7 MNQ | $1,000 | $2,094 | $1,995 | -$633 | 85.0% | 8.5% | 47.6% | $1,241 |
| Smoother 4 | 5 MNQ | 7 MNQ | $500 | $1,476 | $1,281 | -$574 | 80.6% | 5.6% | 40.4% | $1,021 |

## Readout

The user's instinct is correct: requesting the first minimum payout was leaving money on the table.

The best tested larger-payout policy is:

`Eval 9 MNQ -> Funded 8 MNQ -> wait for $1,500 gross payout target.`

Why:

- It has the highest 252-day mean net in the grid.
- It raises average trader payout to about $1,599 instead of about $500-$600.
- Median paid lifecycle is still fast at about 26 days.
- 90-day mean net is the best tested at about $751.

The best balanced policy is:

`Eval 7 MNQ -> Funded 8 MNQ -> wait for $1,500 gross payout target.`

Why:

- It is close to the top mean net.
- It has better profit probability and lower ruin probability than the 9 MNQ eval variant.
- It still farms large payouts around $1.6k trader share when successful.

The smoother "learn the process" policy is:

`Eval 7 MNQ -> Funded 7 MNQ -> wait for $1,000 gross payout target.`

Why:

- It has the best profit probability among the highlighted candidates.
- Ruin probability is only about 8.5%.
- It still more than doubles the average payout versus minimum-payout harvesting.

## Working Decision

To farm thicker payouts, change the operating policy:

- Stop targeting the first eligible payout.
- Keep evaluation aggressive enough to pass quickly: 7-9 MNQ.
- Trade funded at 7-8 MNQ, not necessarily larger.
- Request around $1,500 gross for the aggressive policy, or $1,000 gross for the smoother policy.

Do not blindly push funded size to 10-12 MNQ. In this grid, higher funded size did not dominate. The sweet spot was mostly funded 7-8 MNQ.

## Next Test

Model repeated payout cycles on the same funded account:

- Payout 1 target: $1,000 or $1,500.
- After payout withdrawal, continue to payout 2/3/4/5.
- Apply Lucid's payout-cycle reset: 5 profitable days required again after every approved payout.
- Apply MLL adjustment after payout.

This decides whether the business should harvest first payout and recycle, or keep successful accounts alive for multiple payout cycles.
