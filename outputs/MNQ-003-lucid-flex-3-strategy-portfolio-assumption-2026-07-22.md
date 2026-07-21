# MNQ-003 LucidFlex Three-Strategy Portfolio Assumption

Question: if we find two more strategies with similar stats to MNQ-003, how would cashflow look for a prop portfolio?

This is an assumption test, not a discovered fact. It assumes each new strategy has roughly the same lifecycle distribution as MNQ-003 but independent timing/regime behavior.

## Base Policy

Policy used for every strategy sleeve:

`50k Eval 9 MNQ -> Funded 10 MNQ -> $2,000 gross payout target -> up to 5 payouts`.

Budget:

- Starting budget: $2,000.
- Challenge cost: $90.
- Horizon: 252 trading days.
- Trader cashflow after 90% split.
- Month = 21 trading days.

## Key Result

Three independent strategies matter because they can create multiple independent account engines. If only one account is active at a time, extra strategy labels do not help much. The benefit comes from running multiple staggered sleeves.

## Portfolio Scenarios

| Strategies | Slots Per Strategy | Total Active Slots | Mean Net 252d | Median Net | P10 Net | Profit Prob | Avg Monthly Cashflow | Median Month | Months >= $1.5k | Zero-Cash Months |
| ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| 1 | 3 | 3 | $17,939 | $18,450 | $6,570 | 94.0% | $1,792 | $1,800 | 60.5% | 39.5% |
| 3 | 1 | 3 | $18,022 | $18,450 | $6,750 | 94.1% | $1,800 | $1,800 | 60.7% | 39.3% |
| 2 | 2 | 4 | $23,673 | $24,390 | $10,620 | 94.1% | $2,368 | $1,800 | 69.2% | 30.8% |
| 3 | 2 | 6 | $35,016 | $36,270 | $18,810 | 94.3% | $3,507 | $3,600 | 79.7% | 20.3% |
| 3 | 3 | 9 | $51,724 | $53,730 | $31,770 | 94.2% | $5,186 | $5,400 | 86.6% | 13.4% |

## Readout

If the two additional strategies truly match MNQ-003's lifecycle stats and are independent enough, a three-strategy prop portfolio can reach the user's monthly objective more cleanly.

Most relevant configurations:

- 3 strategies x 1 slot each: about $1.8k average monthly cashflow and $1.8k median month.
- 3 strategies x 2 slots each: about $3.5k average monthly cashflow, $3.6k median month, and only about 20.3% zero-cash months.
- 3 strategies x 3 slots each: very strong modeled cashflow, but likely operationally unrealistic early because it requires many active attempts and much better process maturity.

Important distinction:

- `1 strategy x 3 slots` and `3 strategies x 1 slot` look similar in this bootstrap because the model assumes independence either way.
- In real life, 3 genuinely different strategies should reduce correlated dry spells more than 3 copied versions of the same strategy.

## Practical Interpretation

For two people seeking stable cashflow:

- The real target should be 3 independent strategy sleeves, not just more size on one setup.
- A practical first portfolio target is 3 sleeves x 1 slot.
- A scalable business target is 3 sleeves x 2 slots.

## Caveats

- The two extra strategies do not exist yet. This is a scenario analysis.
- The assumption that they match MNQ-003's stats is strong.
- Correlation between strategies matters more than raw backtest stats.
- Account caps, payout delays, execution mistakes, taxes, and rule changes are not included.

Next research priority: build MNQ-005 and then MNQ-006 so the project can move from one-strategy account farming to a diversified prop portfolio.
