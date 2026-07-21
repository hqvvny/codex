# MNQ-003 LucidFlex All-Levers Review

Source trade list: `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-21 07-19.csv`.

Detailed grid CSV: `outputs/MNQ-003-lucid-flex-all-levers-grid-2026-07-21.csv`.

This tests the main business levers discussed after the serial 1k bankroll correction:

- Evaluation size.
- Funded size.
- Larger payout targets.
- Multiple payout cycles on the same funded account.
- Larger account sizes, using placeholder cost assumptions.

## Assumptions

- Bankroll: $1,000.
- Operating model: serial, one active account at a time.
- Horizon: 252 trading days.
- Strategy source: MNQ-003 weak-hour fee-matched NT8 export.
- Payout cycles: continue a funded account up to 5 payouts unless breached or timed out.
- After payout, payout qualification days reset.
- Trader receives 90% of gross payout.

Account assumptions:

| Account | Eval Target | MLL | Min Profit Day | Max Gross Payout | Cost Used |
| ---: | ---: | ---: | ---: | ---: | ---: |
| 50k | $3,000 | $2,000 | $150 | $2,000 | $90 |
| 100k | $6,000 | $3,000 | $200 | $2,500 | $180 placeholder |
| 150k | $9,000 | $4,500 | $250 | $3,000 | $270 placeholder |

Important: only the 50k cost is from the user's estimate. 100k and 150k costs are rough proportional placeholders and must be replaced with actual current prices before decisions.

## Best 50k Policies With Multi-Payout Cycles

| Rank | Eval Size | Funded Size | Gross Target | Mean Net | Median Net | P10 Net | Profit Prob | Ruin Prob | Paid Lifecycle Rate | Avg Trader Payout Per Paid Lifecycle | Avg Payout Count When Paid |
| ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| 1 | 9 MNQ | 12 MNQ | $2,000 | $4,158 | $2,610 | -$990 | 66.5% | 25.1% | 11.7% | $5,515 | 3.06 |
| 2 | 8 MNQ | 12 MNQ | $2,000 | $3,964 | $2,430 | -$990 | 67.0% | 24.1% | 11.9% | $5,349 | 2.97 |
| 3 | 9 MNQ | 10 MNQ | $2,000 | $3,879 | $2,340 | -$990 | 64.5% | 22.7% | 12.4% | $5,787 | 3.21 |
| 4 | 7 MNQ | 12 MNQ | $2,000 | $3,853 | $2,340 | -$990 | 69.5% | 22.4% | 12.6% | $5,188 | 2.88 |
| 5 | 9 MNQ | 12 MNQ | $1,500 | $3,850 | $4,500 | -$990 | 71.3% | 22.1% | 12.8% | $4,003 | 2.97 |
| 6 | 9 MNQ | 8 MNQ | $1,500 | $3,787 | $4,230 | -$990 | 76.7% | 14.1% | 16.3% | $4,179 | 3.10 |
| 7 | 8 MNQ | 8 MNQ | $1,500 | $3,570 | $3,600 | -$990 | 75.4% | 14.0% | 16.5% | $4,087 | 3.03 |
| 8 | 7 MNQ | 8 MNQ | $1,500 | $3,431 | $3,060 | -$990 | 77.2% | 12.1% | 17.7% | $3,846 | 2.85 |

## Larger Accounts

Larger accounts create bigger paid lifecycles, but the 1k serial bankroll gets much more fragile.

Examples:

| Account | Eval Size | Funded Size | Gross Target | Mean Net | Median Net | Profit Prob | Ruin Prob | Paid Lifecycle Rate | Avg Trader Payout Per Paid Lifecycle |
| ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| 100k | 15 MNQ | 12 MNQ | $2,500 | $3,211 | -$900 | 43.7% | 51.7% | 13.0% | $6,936 |
| 150k | 20 MNQ | 18 MNQ | $3,000 | $3,584 | -$810 | 36.0% | 62.9% | 15.8% | $8,170 |

Read: 100k/150k are not dead, but they are not the best first choice for a $1k bankroll because median results are negative and ruin probabilities are huge in this rough model.

## What Actually Moves The Needle

### 1. Multiple payout cycles

This is the biggest new lever. First-payout harvesting around $1,000-$1,500 gross was too small. Letting winners continue for several payout cycles pushes average paid lifecycle into the $4k-$5.8k trader-payout region on 50k.

### 2. Higher funded size

Funded 10-12 MNQ becomes useful once the goal is several payout cycles, not just first payout. It was not great in the first-payout-only model because it killed accounts before the first request; with multi-cycle upside, the paid winners compensate more.

### 3. Keep 50k as the main account size

For a $1k bankroll, 50k is still the best first account type. The cheap attempt cost matters more than larger max payout.

### 4. Evaluation size 7-9 MNQ

Eval 9 MNQ maximizes mean in the top aggressive configs. Eval 7 MNQ is the cleaner compromise with lower ruin and still strong upside.

## Working Policies

Aggressive EV policy:

`50k Eval 9 MNQ -> Funded 12 MNQ -> $2,000 gross payout target -> keep account for up to 5 payouts.`

- Highest tested 50k mean net.
- Mean net about $4.16k over 252 trading days in the serial 1k model.
- Median net about $2.61k.
- Ruin probability about 25.1%.
- Average paid lifecycle about $5.5k trader share.

Balanced thick-payout policy:

`50k Eval 7 MNQ -> Funded 8 MNQ -> $1,500 gross payout target -> keep account for up to 5 payouts.`

- Mean net about $3.43k.
- Profit probability about 77.2%.
- Ruin probability about 12.1%.
- Average paid lifecycle about $3.85k trader share.

Middle policy:

`50k Eval 9 MNQ -> Funded 8 MNQ -> $1,500 gross payout target -> keep account for up to 5 payouts.`

- Mean net about $3.79k.
- Median net about $4.23k.
- Profit probability about 76.7%.
- Ruin probability about 14.1%.
- Average paid lifecycle about $4.18k.

## Recommendation

For the user's current "start at 0 accounts, one at a time, about 1k budget" plan, the best serious starting policy is:

`50k Eval 9 MNQ -> Funded 8 MNQ -> $1,500 gross payout target -> keep account for up to 5 payouts.`

It captures the main upside from thick payout farming without jumping all the way to funded 12 MNQ or larger account sizes.

If the user wants lower tail risk:

`50k Eval 7 MNQ -> Funded 8 MNQ -> $1,500 gross payout target.`

If the user wants maximum aggression:

`50k Eval 9 MNQ -> Funded 12 MNQ -> $2,000 gross target.`

## Caveats

- This is still a model on one historical strategy export, not a live guarantee.
- 100k/150k account costs are placeholders and must be replaced with real current prices.
- Payout processing behavior, denial edge cases, and post-payout MLL handling need more exact operational confirmation.
- The model assumes serial immediate recycling and does not include taxes, platform mistakes, missed trades, or rule changes.
- The next truly valuable research work is to add a second independent strategy, because all of this is still one-edge regime risk.
