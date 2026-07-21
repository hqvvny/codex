# MNQ-003 LucidFlex Monthly Income Target

Source trade list: `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-21 07-19.csv`.

User target: about $1.5k payout per month.

This report adds monthly cashflow timing to the LucidFlex payout-farming model.

## Assumptions

- Month = 21 trading days.
- Starting bankroll per serial slot: $1,000.
- One serial slot means one account at a time: buy eval, trade until breach or payout-cycle lifecycle ends, then recycle.
- Parallel slots are modeled as independent/staggered serial engines. In live ops, copy-traded same-day starts will be more correlated.
- Trader cashflow is after 90% split.
- Payout targets are gross request targets.

## One Serial Account Is Not Enough

| Policy | Avg Monthly Trader Cashflow | Median Month | Months >= $1.5k | Zero-Cash Months | Median First Payout |
| --- | ---: | ---: | ---: | ---: | ---: |
| Eval 7 / Funded 8 / $1.5k gross | $466 | $0 | 5.4% | 71.1% | 63d |
| Eval 9 / Funded 8 / $1.5k gross | $501 | $0 | 5.8% | 69.0% | 57d |
| Eval 9 / Funded 10 / $2k gross | $543 | $0 | 25.5% | 74.5% | 62d |

Read: one account at a time can have positive annual EV, but it cannot produce a reliable $1.5k monthly payout stream. The median month is still $0.

## Parallel Slot Requirement

### Eval 9 / Funded 8 / $1.5k Gross Target

| Parallel Serial Slots | Avg Monthly Cashflow | Median Month | Months >= $1.5k | Zero-Cash Months | Avg Hit Months / Year |
| ---: | ---: | ---: | ---: | ---: | ---: |
| 1 | $512 | $0 | 6.0% | 68.4% | 0.72 |
| 2 | $1,025 | $1,350 | 18.6% | 47.2% | 2.23 |
| 3 | $1,532 | $1,350 | 32.3% | 33.2% | 3.88 |
| 4 | $2,044 | $1,350 | 45.3% | 23.6% | 5.44 |
| 5 | $2,553 | $2,700 | 56.4% | 17.1% | 6.76 |

### Eval 9 / Funded 10 / $2k Gross Target

| Parallel Serial Slots | Avg Monthly Cashflow | Median Month | Months >= $1.5k | Zero-Cash Months | Avg Hit Months / Year |
| ---: | ---: | ---: | ---: | ---: | ---: |
| 1 | $543 | $0 | 25.5% | 74.5% | 3.06 |
| 2 | $1,095 | $0 | 44.3% | 55.7% | 5.32 |
| 3 | $1,646 | $1,800 | 58.0% | 42.0% | 6.96 |
| 4 | $2,195 | $1,800 | 67.9% | 32.1% | 8.15 |
| 5 | $2,739 | $1,800 | 75.3% | 24.7% | 9.04 |

## Readout

If $1.5k/month means average monthly cashflow, then the model needs about 3 parallel serial slots.

If $1.5k/month means "most months should pay at least $1.5k", then 3 slots are not enough. The aggressive $2k gross policy with 5 slots gets to about 75.3% of months above $1.5k, but still has about 24.7% zero-cash months in the simulation.

The user's current "one account after another" plan is useful as proof-of-concept, but it is not enough for the stated monthly cashflow target.

## Operational Implication

To target $1.5k/month, change the business plan:

- Phase 1: run one account to validate fills, payout processing, and rule interpretation.
- Phase 2: scale to 3 staggered serial slots once process risk is understood.
- Phase 3: scale to 5 staggered serial slots if monthly regularity matters.

Best policy for average monthly cashflow:

`Eval 9 MNQ -> Funded 10 MNQ -> $2k gross target -> up to 5 payouts.`

Best balanced policy:

`Eval 9 MNQ -> Funded 8 MNQ -> $1.5k gross target -> up to 5 payouts.`

## Caveats

- Parallel slots must be staggered or mixed; identical same-day copy trading creates correlated dry spells.
- This still assumes MNQ-003 live behavior matches the NT8 export.
- Taxes, payout denials, platform issues, and exact FX/account costs are not included.
