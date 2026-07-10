---
type: reference
updated: 2026-07-10
status: active
verdict: authoritative-link
---

# First Filter

The first filter decides whether a raw idea deserves build/test work.

## Required Questions

1. Structural reason: why should this edge exist?
2. Data availability: can it be tested with data the project actually has or can reasonably obtain?
3. Sample size: is there enough historical occurrence count to matter?
4. Rule type: is it price action requiring visual/contextual confirmation, or mechanical enough for indicator/event rules?
5. Risk/reward: is expected R:R at least 1.5:1? If not, say so and usually reject.
6. Invalidation: what market behavior disproves the setup?

## Verdicts

- `advance`: passes enough criteria to enter build/test.
- `needs-data`: structurally plausible, but data is missing.
- `needs-definition`: concept is plausible, but rules are too vague.
- `reject`: no structural reason, no real sample, bad R:R, or duplicated failed idea.

## Scoring

| Field | Score 0 | Score 1 | Score 2 |
| --- | --- | --- | --- |
| Structural reason | None | Plausible story | Clear market mechanism |
| Data availability | Missing | Partial/proxy | Available |
| Sample size | Tiny/unknown | Marginal | Enough to test |
| Rule clarity | Vague visual idea | Semi-defined | Mechanical/testable |
| R:R | <1.5 or unknown | Around 1.5 | Clearly >1.5 |
| Invalidation | Missing | Subjective | Explicit |

Default threshold to advance: 8+ total and no zero in structural reason, data availability, or invalidation.

## Related

- [[hypothesis-template]]
- [[backtest-battery]]
- [[../research/strategy-queue]]
