# MNQ-003 LucidFlex Account Slot Scaling

Source trade list: `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-21 07-19.csv`.

This extends the time-weighted payout-path model from one account slot to multiple account slots.

Important distinction:

- If multiple accounts are copied identically from the same start day, they are highly correlated. They scale payout dollars, but they do not diversify the path.
- If account starts are staggered or run as independent slots, they increase the chance that at least one slot reaches payout within a given time window.

## Per-Slot Baseline

| Plan | Joint First-Payout Rate | Avg Days To Resolution | EV / Day | EV / Year / Slot | Payouts / Year / Slot |
| --- | ---: | ---: | ---: | ---: | ---: |
| Static 2 MNQ | 42.35% | 81.9 | $1.81 | $456.15 | 1.30 |
| Dynamic +$500 / +$250 | 54.72% | 124.3 | $1.73 | $435.45 | 1.11 |
| Static 1 MNQ | 55.98% | 194.6 | $1.19 | $300.01 | 0.72 |

## Slot Scaling

Assumes each slot recycles after payout, breach, or timeout. Challenge cost is $90 per attempt.

| Plan | Slots | Upfront Cost At $90/Slot | Attempts / Year | Expected Payouts / Year | Expected Gross Trader Payout / Year | Expected EV / Year |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| Static 2 MNQ | 1 | $90 | 3.08 | 1.30 | $733 | $456 |
| Static 2 MNQ | 3 | $270 | 9.23 | 3.91 | $2,199 | $1,368 |
| Static 2 MNQ | 5 | $450 | 15.39 | 6.52 | $3,666 | $2,281 |
| Static 2 MNQ | 10 | $900 | 30.78 | 13.03 | $7,331 | $4,561 |
| Dynamic +$500 / +$250 | 1 | $90 | 2.03 | 1.11 | $618 | $435 |
| Dynamic +$500 / +$250 | 3 | $270 | 6.08 | 3.33 | $1,854 | $1,306 |
| Dynamic +$500 / +$250 | 5 | $450 | 10.14 | 5.55 | $3,089 | $2,177 |
| Dynamic +$500 / +$250 | 10 | $900 | 20.27 | 11.09 | $6,179 | $4,354 |

## Probability Of At Least One First Payout By Time Window

This table assumes independent/staggered slots. Identical copy-traded accounts from the same start day will be much more correlated.

### Static 2 MNQ

| Horizon | 1 Slot | 3 Slots | 5 Slots | 10 Slots |
| --- | ---: | ---: | ---: | ---: |
| 30 days | 3.53% | 10.22% | 16.44% | 30.18% |
| 60 days | 13.26% | 34.74% | 50.90% | 75.89% |
| 90 days | 21.10% | 50.88% | 69.42% | 90.65% |
| 120 days | 23.29% | 54.86% | 73.44% | 92.95% |
| 180 days | 28.86% | 64.00% | 81.78% | 96.68% |
| 252 days | 38.08% | 76.25% | 90.89% | 99.17% |

### Dynamic +$500 / +$250

| Horizon | 1 Slot | 3 Slots | 5 Slots | 10 Slots |
| --- | ---: | ---: | ---: | ---: |
| 30 days | 1.71% | 5.04% | 8.26% | 15.83% |
| 60 days | 9.92% | 26.90% | 40.68% | 64.81% |
| 90 days | 18.57% | 46.01% | 64.20% | 87.19% |
| 120 days | 24.52% | 56.99% | 75.50% | 94.00% |
| 180 days | 36.03% | 73.83% | 89.29% | 98.85% |
| 252 days | 47.03% | 85.14% | 95.83% | 99.83% |

## Readout

If the user has enough capital to buy multiple accounts, the objective changes again.

For one slot, static 2 MNQ is the fastest tested plan by EV/day.

For multiple staggered slots, static 2 MNQ gives the highest expected throughput per year, while dynamic +$500/+250 gives a higher probability that any given slot reaches payout. With 5 slots, static 2 MNQ projects about 6.52 first payouts/year and $2,281 EV/year after challenge costs. Dynamic +$500/+250 projects about 5.55 first payouts/year and $2,177 EV/year, but with smoother slot-level conversion.

Working portfolio plan:

- Use multiple account slots, not one slow survival account.
- Treat static 2 MNQ as the speed-maximizing baseline.
- Treat dynamic +$500/+250 as the smoother alternative if early breach clustering becomes psychologically or operationally annoying.
- Do not assume copy-traded accounts diversify each other. To diversify timing, stagger account starts or mix sizing plans.

Next model should simulate a fixed bankroll, e.g. 450 EUR for 5 accounts, with immediate recycle after breach/payout and optional staggered starts.
