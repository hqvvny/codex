# MNQ-003 LucidFlex 50k Evaluation Pass Simulation

Source trade list: `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-21 07-19.csv`.

Rule assumptions checked from Lucid Trading Help Center on 2026-07-21:

- 50k LucidFlex evaluation profit target: $3,000, meaning account must reach $53,000.
- 50k Max Loss Limit: $2,000.
- 50% consistency requirement applies during evaluation.
- EOD drawdown model applies to evaluation and funded accounts.

Simulation assumptions:

- Start balance: $50,000.
- Pass condition: balance >= $53,000 and largest profitable day <= 50% of total account profit.
- Breach condition: EOD balance reaches the simulated LucidFlex MLL.
- No payout or reset logic.
- Daily PnL generated from the fee-matched MNQ-003 trade list, scaled from 1 NQ source PnL to 1 MNQ or 2 MNQ.

## Results

| Size | Full Historical Start Status | Full Historical Start Days | Rolling Pass Rate | Rolling Breach Rate | Median Pass Days | P10-P90 Pass Days |
| --- | --- | ---: | ---: | ---: | ---: | ---: |
| 1 MNQ | Pass | 496 | 84.14% | 13.11% | 175 | 62-335 |
| 2 MNQ | Breach | 24 | 55.27% | 44.61% | 58 | 18-190 |

## Readout

The user's correction is right: the relevant evaluation target is $53,000, not merely building enough survival buffer. At 1 MNQ, the account is much more survivable, but too slow as a standalone evaluation-pass engine. At 2 MNQ, the median pass time is much faster, but breach risk is extremely high from a fresh 50k account.

Practical implication: a static 1 MNQ plan is slow; a static 2 MNQ plan is fragile. The next useful test is a dynamic sizing plan such as 1 MNQ from account start, scaling to 2 MNQ only after a defined buffer, then cutting back to 1 MNQ after drawdown.
