# MNQ-003 LucidFlex 50k Dynamic Sizing Simulation

Source trade list: `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-21 07-19.csv`.

Rule assumptions:

- Account: 50k LucidFlex evaluation.
- Pass target: $53,000, meaning +$3,000 from $50,000 start.
- Breach rule: simulated LucidFlex EOD MLL.
- Consistency rule: largest profitable day must be <= 50% of account profit at pass.
- Strategy source size: 1 NQ trade list, scaled to 1 MNQ or 2 MNQ.

Simulation method:

- Rolling-start test from every available trading day in the historical daily PnL path.
- Dynamic size is selected at the start of each trading day from the prior EOD account profit.
- If account profit is at or above the scale-up threshold, trade 2 MNQ.
- If account profit falls to or below the scale-down threshold, trade 1 MNQ.
- No payout, reset, or intraday risk-stop logic.

## Static Baselines

| Plan | Pass Rate | Breach Rate | Median Pass Days | P10-P90 Pass Days | Full Historical Start |
| --- | ---: | ---: | ---: | ---: | --- |
| Static 1 MNQ | 84.14% | 13.11% | 175 | 62-335 | Pass in 496 days |
| Static 2 MNQ | 55.27% | 44.61% | 58 | 18-190 | Breach in 24 days |

## Selected Dynamic Variants

| Plan | Pass Rate | Breach Rate | Median Pass Days | P10-P90 Pass Days | Avg % Days At 2 MNQ During Passes | Full Historical Start |
| --- | ---: | ---: | ---: | ---: | ---: | --- |
| Up +$500 / Down +$250 | 64.67% | 33.43% | 100 | 28-213 | 51.38% | Pass in 356 days |
| Up +$750 / Down +$400 | 64.93% | 33.14% | 103 | 31-238 | 41.86% | Pass in 387 days |
| Up +$1,000 / Down +$500 | 69.28% | 28.79% | 115 | 34-254 | 33.63% | Pass in 389 days |
| Up +$1,250 / Down +$750 | 72.44% | 25.59% | 123.5 | 38-281 | 26.44% | Pass in 390 days |
| Up +$1,500 / Down +$1,000 | 76.45% | 21.58% | 147 | 42-303 | 21.13% | Pass in 389 days |

## Readout

Dynamic sizing sits between static 1 MNQ and static 2 MNQ, exactly as expected. It improves pass speed versus static 1 MNQ, but it does not improve pass rate. The earlier the strategy scales to 2 MNQ, the more it behaves like the fragile 2 MNQ plan.

The best practical candidate from this first grid is not the aggressive +$500 or +$750 scale-up. The cleaner compromise is:

- Start with 1 MNQ.
- Scale to 2 MNQ at +$1,250 buffer.
- Cut back to 1 MNQ at +$750 buffer.

This still reduces median pass time from 175 to 123.5 trading days while keeping breach risk at 25.59%, much lower than static 2 MNQ but still materially worse than static 1 MNQ.

## Operator Conclusion

There is no free lunch in sizing. For a 50k LucidFlex evaluation, static 1 MNQ is slow but robust; static 2 MNQ is fast but too fragile; dynamic sizing is a middle path, but the threshold must be conservative enough that the account has real cushion before doubling risk.

Next test should add operational kill switches:

- Stop trading for the day after one full stop.
- No scale-up during historically weak platform hours.
- Scale back to 1 MNQ after any day worse than -$250.
- Compare pass speed and breach rate after these risk controls.
