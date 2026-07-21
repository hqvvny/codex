# MNQ-003 LucidFlex 50k 2 MNQ Expectancy Sketch

Source trade list: `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-21 07-19.csv`.

Assumptions match `outputs/MNQ-003-lucid-flex-50k-expectancy-2026-07-21.md`, except size is 2 MNQ instead of 1 MNQ.

## Core Expectancy

| Metric | 50k / 2 MNQ Value |
| --- | ---: |
| Trades | 4,999 |
| Trading days | 2,692 |
| Net PnL over full export, if no breach | $70,283.03 |
| Avg per trade | $14.06 |
| Avg per trading day | $26.11 |
| Trader share at 90% split | $12.65/trade or $23.50/day |
| Break-even on 90 EUR fee | about 4 average trading days |

Important: the full historical path breaches the 50k LucidFlex EOD MLL at 2 MNQ on 2016-02-05, before the strategy can realize the long-run expectancy.

## Full Historical Path

| Size | Breach? | Breach Date | Days Survived | Balance At Breach | Min Buffer |
| --- | --- | --- | ---: | ---: | ---: |
| 2 MNQ | Yes | 2016-02-05 | 24 | $48,024.04 | -$145.93 |

## Rolling Start Simulation

Method: start a fresh 50k LucidFlex account on every possible trading day, trade 2 MNQ for a fixed horizon, stop if EOD MLL is breached, and calculate average positive ending PnL times 90% profit split. This is a rough account EV proxy, not a payout model.

| Horizon | Start Samples | Breach Rate | Mean Strategy PnL | Median Strategy PnL | 90% Share Of Avg Positive PnL | Approx EV After 90 EUR Fee |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| 10 trading days | 2,683 | 4.70% | $263.58 | $231.68 | $484.07 | about $379-$394 |
| 20 trading days | 2,673 | 14.70% | $499.16 | $451.36 | $746.12 | about $641-$656 |
| 30 trading days | 2,663 | 23.24% | $686.58 | $587.01 | $954.86 | about $850-$865 |
| 60 trading days | 2,633 | 38.02% | $1,021.60 | $752.98 | $1,318.81 | about $1,214-$1,229 |
| 90 trading days | 2,603 | 44.26% | $1,275.35 | $799.02 | $1,556.55 | about $1,452-$1,467 |
| 180 trading days | 2,513 | 46.52% | $2,547.70 | $1,671.34 | $2,699.14 | about $2,594-$2,609 |
| 252 trading days | 2,441 | 47.69% | $3,356.30 | $2,461.31 | $3,438.68 | about $3,334-$3,349 |

## Readout

2 MNQ on a 50k LucidFlex account is positive-EV in the historical rolling-start sketch, but it is too fragile for a serious first deployment because breach probability quickly becomes large. It nearly converts the account into a high-variance reset game rather than a stable strategy operation.

Practical rule: use 1 MNQ from account start. Consider 2 MNQ only after a meaningful buffer exists or after the EOD trail is locked, and rerun the simulation using the actual live account buffer before scaling.
