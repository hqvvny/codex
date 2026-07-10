---
type: reference
updated: 2026-07-10
status: active
verdict: authoritative-link
---

# Backtest Battery

This is the minimum robustness battery before a candidate can move toward capital exposure.

## Provenance Required

Never present a backtest number without:

- Data source.
- Date range.
- Instrument and contract handling.
- Timeframe.
- Session calendar.
- Commission, fees, spread, and slippage assumptions.
- Code version or commit.

## Core Metrics

- Net PnL and expectancy.
- Win rate and average win/loss.
- R multiple distribution.
- Max drawdown and drawdown duration.
- Profit factor.
- Trade count and trades per session.
- Session split: London, NY open, NY midday, NY close if relevant.
- Long/short split.
- Regime split: trend, chop, volatility expansion/contraction when definable.

## Prop-Firm Review Lens

- Is drawdown smooth or clustered?
- Does the edge depend on one week, month, session, or volatility regime?
- Does performance degrade over time?
- Are losses recoverable under realistic daily/weekly limits?
- Does the strategy duplicate exposure already in the book?

## Robustness Tests

- Costs and slippage stress.
- Parameter sensitivity.
- Walk-forward or train/test split.
- Out-of-sample period where possible.
- Time-of-day split.
- News/event exclusion or explicit handling.
- Entry/exit delay test.
- Stop/target execution realism.

## Trust Gates

No AI-generated strategy touches real capital until:

1. Independent review is complete.
2. Manual review is complete and the human can explain every rule.
3. Secondary-account observation confirms fills track backtest assumptions.

## Related

- [[first-filter]]
- [[hypothesis-template]]
- [[../concepts/strategy-research-pipeline]]
