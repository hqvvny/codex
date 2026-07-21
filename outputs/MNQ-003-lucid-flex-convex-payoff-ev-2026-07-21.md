# MNQ-003 LucidFlex Convex Payoff EV

Source trade list: `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-21 07-19.csv`.

Rule sources checked 2026-07-21:

- LucidFlex evaluation: `https://support.lucidtrading.com/en/articles/12945790-lucidflex-evaluation-account`.
- LucidFlex funded account: `https://support.lucidtrading.com/en/articles/12945795-lucidflex-funded-account`.
- LucidFlex drawdown: `https://support.lucidtrading.com/en/articles/12945815-lucidflex-drawdown`.
- LucidFlex payouts: `https://support.lucidtrading.com/en/articles/12945796-lucidflex-payouts`.

Rule assumptions:

- Evaluation account: 50k, pass target $53,000, MLL $2,000, 50% consistency.
- Challenge cost: user estimate about 90 EUR. Tables use a simple $90-$105 cost band instead of fetching live FX.
- Funded account: no daily loss limit, no consistency rule, no payout buffer, EOD MLL.
- Payout rule modeled: first payout once there are 5 qualifying profitable days, simulated using $150 minimum day threshold for 50k, total profit is positive, and requestable payout is at least $500.
- Payout amount model: trader receives 90% of `min(50% of account profit, $2,000)`.
- Funded phase model: after evaluation pass, start a fresh 50k funded account and trade 2 MNQ until first payout, funded breach, or 252 trading days.

## Simple Convex EV Matrix

Formula:

`EV = pass_rate * conditional_payout_probability * payout_amount - challenge_cost`

The table below uses $100 challenge cost for a clean midpoint. `q100` means every passed account eventually realizes that payout. `q50` means only half of passed accounts do.

| Plan | Eval Pass Rate | $500 q100 | $500 q50 | $1,000 q100 | $1,000 q50 | Break-Even q at $500 |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| Static 1 MNQ | 84.14% | $320.69 | $110.35 | $741.38 | $320.69 | 23.8% |
| Static 2 MNQ | 55.27% | $176.37 | $38.19 | $452.75 | $176.37 | 36.2% |
| Dynamic +$500 / +$250 | 64.67% | $223.37 | $61.68 | $546.73 | $223.37 | 30.9% |
| Dynamic +$1,250 / +$750 | 72.44% | $262.18 | $81.09 | $624.37 | $262.18 | 27.6% |
| Dynamic +$1,500 / +$1,000 | 76.45% | $282.24 | $91.12 | $664.49 | $282.24 | 26.2% |

Read: the user is right that a 55.27% pass rate is attractive under a capped-loss, convex-payout structure. Even static 2 MNQ only needs about 36.2% of passed accounts to realize a $500 payout to break even before FX and operational frictions.

## Lifecycle EV Sketch

This version models both stages:

1. Evaluation attempt.
2. If passed, funded account trades 2 MNQ until first payout, funded breach, or 252 trading days.

| Plan | Eval Pass Rate | Eval Breach Rate | Joint First-Payout Rate | Avg Trader Payout When Paid | EV After $90 Cost | EV After $105 Cost | Avg Days To Resolution | EV / Day at $90 Cost |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| Static 1 MNQ | 84.14% | 13.11% | 55.98% | $574.63 | $231.68 | $216.68 | 194.6 | $1.19 |
| Static 2 MNQ | 55.27% | 44.61% | 42.35% | $562.52 | $148.21 | $133.21 | 81.9 | $1.81 |
| Dynamic +$500 / +$250 | 64.67% | 33.43% | 54.72% | $557.05 | $214.81 | $199.81 | 124.3 | $1.73 |
| Dynamic +$1,250 / +$750 | 72.44% | 25.59% | 56.02% | $554.68 | $220.72 | $205.72 | 153.3 | $1.44 |
| Dynamic +$1,500 / +$1,000 | 76.45% | 21.58% | 57.73% | $547.47 | $226.03 | $211.03 | 164.1 | $1.38 |

## Interpretation

The convex payoff changes the conclusion.

From a risk-manager lens, static 2 MNQ looked too fragile because evaluation breach risk is 44.61%. From an option-value lens, the downside is approximately the fixed challenge cost, while a successful pass can plausibly lead to a $500-$2,000 payout. Under that structure, static 2 MNQ remains positive EV in both the simple payout matrix and the rough lifecycle simulation.

However, static 2 MNQ is not the highest expected payout per attempt in the lifecycle sketch. It is the fastest plan, not the highest raw EV plan:

- Static 1 MNQ has the highest pass rate and strong per-attempt EV, but it is slow.
- Static 2 MNQ has worse per-attempt EV, but much better capital/time velocity.
- Dynamic sizing sits between them; aggressive dynamic sizing looks nearly as time-efficient as static 2 MNQ while keeping higher joint payout probability.

## Working Decision

If the objective is "maximize chance that one account survives and passes", static 1 MNQ is best.

If the objective is "maximize convex attempt throughput with capped downside", static 2 MNQ is defensible and likely positive EV, assuming passed accounts can convert to first payouts at even moderate rates.

If the objective is a balanced first deployment, dynamic +$500/+250 or +$1,250/+750 are the most interesting candidates to refine with operational kill switches.

Next test: add a funded-stage simulator with repeated payout cycles and payout withdrawals, because the real edge of LucidFlex may be account-farm portfolio EV, not single-account survival.
