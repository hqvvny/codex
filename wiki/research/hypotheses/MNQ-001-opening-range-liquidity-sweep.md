---
type: strategy-hypothesis
updated: 2026-07-10
status: needs-definition
verdict: untested
---

# MNQ-001 Opening Range Liquidity Sweep

## One-Line Thesis

After the NY open, MNQ may create a tradeable setup when price sweeps opening-range liquidity and then either reclaims the range for reversal or accepts beyond it for continuation.

## Market Structure Context

- Instrument: MNQ, Micro Nasdaq futures.
- Session: NY open, centered on 9:30 ET.
- Trend/bias context: requires 15m/1H bias before testing; not defined yet.
- Key liquidity levels: opening range high/low, premarket high/low, prior day high/low, overnight high/low.
- Price relative to key levels: must be evaluated at setup time; no historical sample has been reviewed yet.

## Edge Claim

- Structural reason: index futures often run obvious liquidity around session opens before repricing. If the sweep traps breakout traders or triggers stops, a reclaim can create forced repositioning; if price accepts beyond the swept level, continuation may follow.
- Who is likely trapped or forced: early breakout traders, traders with stops around the opening range, and participants leaning on premarket/overnight levels.
- Why this should persist: session opens concentrate volume, stop orders, and discretionary decision-making around visible liquidity.

## Setup Definition

- Entry condition: not final. Candidate definitions:
  - Reversal: sweep opening range high/low, close back inside the range, then enter on reclaim confirmation.
  - Continuation: sweep/opening range break, hold outside range for a confirmation window, then enter on pullback or continuation trigger.
- Stop condition: not final. Likely beyond sweep extreme for reversal or back inside invalidation level for continuation.
- Target condition: not final. Candidate targets include opposite side of opening range, VWAP, premarket high/low, prior day high/low, or fixed R multiple.
- Invalidation: no trade if reclaim/acceptance rule fails within the defined time window, if expected R:R is below 1.5:1, or if key event/news conditions make fills unrealistic.
- Expected R:R: unknown. Must be estimated from rule geometry before testing. If below 1.5:1, reject or redesign.

## Data Requirements

- Data source: not selected.
- Date range: target at least 1-2 years of MNQ 1m data before first serious pass.
- Timeframe: 1m for event detection, 5m for confirmation variants, 15m/1H for bias features.
- Session calendar: ETH/RTH markers, NY open at 9:30 ET, holiday/half-day handling.
- Costs: not selected; must include commission and fees for MNQ.
- Slippage: not selected; must stress test at multiple tick assumptions.
- Known gaps: no local futures dataset has been registered in the wiki yet.

## Sample Size Expectation

If 1-2 years of clean 1m MNQ data are available, NY open provides hundreds of sessions. Actual valid setup count depends on the final sweep/reclaim definition and must be measured.

## First Filter

| Field | Score | Notes |
| --- | --- | --- |
| Structural reason | 2 | Clear market mechanism around session-open liquidity, stop runs, and trapped breakout flow. |
| Data availability | 0 | No authoritative MNQ 1m data source has been registered yet. |
| Sample size | 1 | Likely sufficient with 1-2 years of data, but unproven until event rules are defined. |
| Rule clarity | 1 | Semi-defined concept; sweep/reclaim/acceptance windows need exact mechanical rules. |
| R:R | 0 | Unknown. Must be calculated before testing; if below 1.5:1, reject or redesign. |
| Invalidation | 1 | Conceptual invalidation exists, but exact timing and level rules are not final. |

Total score: 5/12.

Verdict: `needs-definition`, not ready for build/test.

## Build/Test Notes

Next definition work:

1. Define opening range window, e.g. first 5, 15, or 30 minutes after 9:30 ET.
2. Define sweep threshold, e.g. one tick beyond opening range high/low or beyond premarket/overnight level.
3. Define reclaim/acceptance rule, e.g. 1m close back inside range within N bars, or N consecutive closes outside range.
4. Define entry trigger, stop, and target so expected R:R can be measured before backtesting.
5. Register data source, date range, and cost/slippage assumptions.
6. Only then move status to `filtered`.

## Backtest Provenance

- Data: none yet.
- Date range: none yet.
- Costs: none yet.
- Slippage: none yet.
- Version/commit: pending.

## Robustness Battery

Required if promoted to build/test:

- Split reversal and continuation variants.
- NY open only versus broader RTH comparison.
- Long/short split.
- Trend-bias filter versus no-bias baseline.
- Cost/slippage stress.
- Opening range length sensitivity.
- Reclaim/acceptance window sensitivity.
- Out-of-sample or walk-forward split.

## Review Gates

- Independent review: not started.
- Manual review: not started.
- Secondary-account observation: not started.

## Outcome

Promising raw hypothesis, but not test-ready. The next move is definition and data registration, not code.

## Links

- [[../strategy-queue]]
- [[../../reference/first-filter]]
- [[../../reference/backtest-battery]]
- [[../../concepts/trading-analysis-profile]]
