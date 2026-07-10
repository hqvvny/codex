---
type: reference
updated: 2026-07-10
status: active
verdict: authoritative-link
---

# Hypothesis Template

Use this template for any strategy hypothesis that deserves its own page.

```markdown
---
type: strategy-hypothesis
updated: YYYY-MM-DD
status: raw | needs-data | filtered | building | testing | review | observe | promoted | abandoned
verdict: untested | rejected | promising | robust | failed | superseded
---

# <Hypothesis Name>

## One-Line Thesis

## Market Structure Context

- Instrument:
- Session:
- Trend/bias context:
- Key liquidity levels:
- Price relative to key levels:

## Edge Claim

- Structural reason:
- Who is likely trapped or forced:
- Why this should persist:

## Setup Definition

- Entry condition:
- Stop condition:
- Target condition:
- Invalidation:
- Expected R:R:

## Data Requirements

- Data source:
- Date range:
- Timeframe:
- Session calendar:
- Costs:
- Slippage:
- Known gaps:

## Sample Size Expectation

## First Filter

- Structural reason:
- Testable with available data:
- Real sample size:
- PA or mechanical:
- R:R >= 1.5:
- Verdict:

## Build/Test Notes

## Backtest Provenance

- Data:
- Date range:
- Costs:
- Slippage:
- Version/commit:

## Robustness Battery

## Review Gates

- Independent review:
- Manual review:
- Secondary-account observation:

## Outcome

## Links
```

## Related

- [[first-filter]]
- [[backtest-battery]]
- [[../research/strategy-queue]]
