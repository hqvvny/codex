# MNQ-003 LucidFlex Time-Weighted Payout Path

Source trade list: `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-21 07-19.csv`.

Official LucidFlex sources checked 2026-07-21:

- Evaluation rules: `https://support.lucidtrading.com/en/articles/12945790-lucidflex-evaluation-account`.
- Payout rules: `https://support.lucidtrading.com/en/articles/12945796-lucidflex-payouts`.
- Drawdown rules: `https://support.lucidtrading.com/en/articles/12945815-lucidflex-drawdown`.

Rule assumptions:

- 50k evaluation target: $53,000.
- Evaluation MLL: $2,000 EOD.
- Evaluation consistency: largest winning day <= 50% of profit at pass.
- Funded account starts fresh at $50,000 after evaluation pass.
- Funded first payout requires 5 qualifying days of at least $150 profit, positive net profit in the cycle, and a payout request of at least $500.
- Payout model: trader receives 90% of `min(50% of funded account profit, $2,000)`.
- Challenge cost band: $90-$105, approximating the user's 90 EUR cost without fetching live FX.
- Funded phase trades 2 MNQ until first payout, funded breach, or 252 trading days.

## Payout Path

For each attempt:

1. Buy 50k LucidFlex evaluation.
2. Trade selected evaluation sizing plan until pass, evaluation breach, or timeout.
3. If evaluation passes, upgrade to funded.
4. In funded, trade 2 MNQ.
5. Track qualifying profitable days: each day with at least $150 profit counts.
6. Once 5 qualifying days exist and account profit is high enough to request at least $500, request the first payout.
7. If EOD MLL is breached before payout, the attempt ends at zero payout.

This is a first-payout model, not a repeated payout-cycle model.

## Time-Weighted Results

`Account-slot` means one serial seat: after a breach or payout, start a new attempt. It does not assume multiple parallel accounts. Multiply the annual numbers by the number of accounts/challenge slots if run in parallel.

| Plan | Eval Pass Rate | Joint First-Payout Rate | Avg Trader Payout When Paid | EV After $90 Cost | Avg Days To Resolution | Attempts / Year / Slot | Payouts / Year / Slot | EV / Trading Day | EV / Year / Slot |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| Static 1 MNQ | 84.14% | 55.98% | $574.63 | $231.68 | 194.6 | 1.29 | 0.72 | $1.19 | $300.01 |
| Static 2 MNQ | 55.27% | 42.35% | $562.52 | $148.21 | 81.9 | 3.08 | 1.30 | $1.81 | $456.15 |
| Dynamic +$500 / +$250 | 64.67% | 54.72% | $557.05 | $214.81 | 124.3 | 2.03 | 1.11 | $1.73 | $435.45 |
| Dynamic +$750 / +$400 | 64.93% | 52.56% | $562.93 | $205.90 | 132.5 | 1.90 | 1.00 | $1.55 | $391.62 |
| Dynamic +$1,250 / +$750 | 72.44% | 56.02% | $554.68 | $220.72 | 153.3 | 1.64 | 0.92 | $1.44 | $362.85 |
| Dynamic +$1,500 / +$1,000 | 76.45% | 57.73% | $547.47 | $226.03 | 164.1 | 1.54 | 0.89 | $1.38 | $347.03 |

## Conditional Successful Payout Timing

This table only looks at attempts that actually reach first payout.

| Plan | Median Total Days To Payout | P10-P90 Total Days | Median Eval Days | Median Funded Days | Median Trader Payout |
| --- | ---: | ---: | ---: | ---: | ---: |
| Static 1 MNQ | 251 | 95-383 | 236 | 19 | $520.71 |
| Static 2 MNQ | 91 | 32-253 | 65 | 30 | $501.22 |
| Dynamic +$500 / +$250 | 135 | 44-276 | 111 | 20 | $495.78 |
| Dynamic +$750 / +$400 | 142 | 46-291 | 120 | 23 | $499.49 |
| Dynamic +$1,250 / +$750 | 163 | 55-336 | 143 | 19 | $498.40 |
| Dynamic +$1,500 / +$1,000 | 203 | 58-348 | 169 | 16 | $488.05 |

## Readout

When time is the dominant variable, static 2 MNQ is the best tested plan in this model.

It has worse pass rate and worse per-attempt EV than static 1 MNQ, but it resolves much faster. The faster resolution creates more attempts per year and more first-payout chances per year. On one serial account slot, static 2 MNQ produces about:

- 3.08 attempts per trading year.
- 1.30 first payouts per trading year.
- $456 expected value per trading year after $90 challenge cost.
- $1.81 expected value per trading day.

Dynamic +$500/+250 is the closest compromise:

- Higher joint first-payout rate than static 2 MNQ.
- Lower EV per day than static 2 MNQ, but close.
- Less evaluation breach risk than static 2 MNQ.

## Working Decision

If the objective is to exploit the convex payoff structure with time as the primary constraint, the current leader is:

`Static 2 MNQ during evaluation, then 2 MNQ in funded until first payout.`

The more balanced alternative is:

`Dynamic +$500/+250 during evaluation, then 2 MNQ in funded until first payout.`

Next model should simulate repeated payout cycles after the first payout, including payout withdrawals and the MLL adjustment after payout. That will tell whether the correct objective is first-payout harvesting or multi-payout account farming.
