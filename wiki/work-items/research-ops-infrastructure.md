---
type: work-item
updated: 2026-07-10
status: complete
verdict: useful
---

# Research Ops Infrastructure

## Goal

Create the first operational layer for the strategy-research pipeline: queue, first filter, hypothesis template, candidate-generation workflow, and backtest battery.

## Context

The project already had standing market-analysis defaults and a two-stage strategy pipeline. The next step was to make those rules actionable so future work starts with structured hypotheses instead of scattered chat notes.

## Decisions

- Put active hypotheses in `wiki/research/strategy-queue.md`.
- Put process documents in `wiki/reference/` so they route to authoritative operating rules.
- Seed the queue with one raw MNQ opening-range liquidity-sweep hypothesis as a starter candidate, not an approved strategy.
- Require data/date/cost provenance before any backtest number is treated as real.

## Outcome

The project now has a practical Stage 1 to Stage 2 workflow: capture ideas, first-filter them, rank the queue, then only build/test candidates that pass.

## Links

- [[../research/strategy-queue]]
- [[../research/candidate-generation-workflow]]
- [[../reference/hypothesis-template]]
- [[../reference/first-filter]]
- [[../reference/backtest-battery]]
