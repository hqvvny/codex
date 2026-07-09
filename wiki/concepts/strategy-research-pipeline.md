---
type: concept
updated: 2026-07-09
status: stable
verdict: current
---

# Strategy Research Pipeline

## Meaning

The project strategy workflow has two separate stages: candidate generation, then build and test. They should not blur together.

## Why It Matters

Throughput compounds when research can generate many hypotheses cheaply while build/test remains disciplined. The model can increase candidate volume and implementation speed, but the operator still decides which edges are structurally real.

## Stage One: Candidate Generation

Run scheduled research scans on a cadence, not only on demand. Sources can include forums, papers, factor research, and project notes.

Each scan produces a ranked queue of hypotheses. It does not produce finished strategies.

First-filter questions:

- Is there a structural reason this could work?
- Is it testable with data the project actually has?
- Is the sample size real enough to justify further work?

Expected output: 5 to 15 raw ideas on a good day, with 2 or 3 worth taking further. A low hit rate is normal.

## Stage Two: Build And Test

Only hypotheses that clear the first filter become prototypes.

Coding agents should use the project's helper functions, templates, architecture conventions, file layout, naming conventions, order-management pattern, and risk-check sequence. They should not start from a blank file unless explicitly asked.

Every candidate, AI-built or hand-built, must go through the same backtesting and robustness battery before capital exposure.

## Where AI Helps

- Candidate volume.
- Boilerplate and code scaffolding.
- Standard entry/exit patterns.
- Backtest harnesses.
- Red-team review of finished strategies.
- Surfacing implicit exposures and correlations.

## Where AI Fabricates

- Live-book awareness.
- Real-time correlation between strategies already running.
- Portfolio construction decisions.
- Consistent codebase conventions when unconstrained.

Portfolio construction remains a manual human decision.

## Trust Rule

Before an AI-generated strategy touches real capital, require all three gates:

1. Independent review by a different reviewer or model instance.
2. Manual review where the human can personally explain all logic.
3. Observation on a secondary account, demo or small live, using the same broker and data feed long enough to confirm real fills track the backtest.

Skipping any gate means trading code that is not fully understood.

## Related Pages

- [[trading-analysis-profile]]
- [[../work-items/strategy-research-pipeline]]
