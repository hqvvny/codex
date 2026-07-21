# MNQ-003 LucidFlex 50k Expectancy Sketch

Source trade list: `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-21 07-19.csv`.

Strategy/account assumptions:

- Strategy: MNQ-003 current best weak-hour variant.
- Source export size: 1 NQ.
- Scaled account size: 50k LucidFlex with 1 MNQ, equal to 0.1x source PnL.
- Cost assumption from user: 50k LucidFlex costs about 90 EUR.
- Profit split assumption from LucidFlex rule review: 90/10.
- Drawdown model: 50k LucidFlex EOD MLL simulation, no intraday mark-to-market breach, no payout withdrawals.
- Currency caveat: PnL is in USD, account fee is in EUR. Quick figures below treat 90 EUR as roughly comparable to $90-$105 depending EUR/USD; exact FX was not fetched.

## Core Expectancy

| Metric | 50k / 1 MNQ Value |
| --- | ---: |
| Trades | 4,999 |
| Trading days | 2,692 |
| Net PnL over full export | $35,141.52 |
| Avg per trade | $7.03 |
| Avg per trading day | $13.05 |
| Trader share at 90% split | $6.33/trade or $11.75/day |
| Break-even on 90 EUR fee | about 8-9 trading days |

Read: if the backtest edge survives live execution, the account fee is tiny relative to the modeled long-run expectancy. The real risk is not the purchase price; it is early-path drawdown before enough buffer is built.

## Rolling Start Simulation

Method: start a fresh 50k account on every possible trading day, trade 1 MNQ for a fixed horizon, stop if the EOD MLL is breached, and calculate average positive ending PnL times 90% profit split. This is a rough challenge/account EV proxy, not a true payout model.

| Horizon | Start Samples | Breach Rate | Mean Strategy PnL | Median Strategy PnL | 90% Share Of Avg Positive PnL | Approx EV After 90 EUR Fee |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| 20 trading days | 2,673 | 0.34% | $268.66 | $243.80 | $377.75 | about $275-$288 |
| 30 trading days | 2,663 | 1.73% | $406.42 | $342.02 | $499.92 | about $395-$410 |
| 60 trading days | 2,633 | 6.61% | $804.13 | $711.48 | $853.89 | about $749-$764 |
| 90 trading days | 2,603 | 9.26% | $1,162.68 | $1,120.26 | $1,180.48 | about $1,075-$1,090 |
| 180 trading days | 2,513 | 13.09% | $2,229.18 | $2,107.51 | $2,150.35 | about $2,045-$2,060 |
| 252 trading days | 2,441 | 13.56% | $2,997.40 | $2,805.24 | $2,845.95 | about $2,741-$2,756 |

## Interpretation

For a 50k LucidFlex account at 1 MNQ, the historical expectancy is positive enough that the 90 EUR account cost is not the bottleneck. The bottleneck is path survival and payout realization.

Do not scale to 2 MNQ on 50k from account start. The historical path breached at 2 MNQ before building enough buffer. If this account type is used, start with 1 MNQ and only discuss scaling after the EOD trail is locked and the remaining buffer is known.

## Caveats

- This is not a live guarantee; it is a historical path and rolling-start estimate.
- The simulation does not model payout timing, withdrawal effects, reset behavior, taxes, platform mistakes, rejected orders, or psychological/manual intervention.
- It assumes the NT8 fill, fee, slippage, session template, and weak-hour mapping are representative.
- It treats only end-of-day LucidFlex MLL breaches, matching the LucidFlex drawdown model recorded in the risk review.
