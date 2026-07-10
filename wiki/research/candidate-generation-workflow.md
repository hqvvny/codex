---
type: research
updated: 2026-07-10
status: active
verdict: useful
---

# Candidate Generation Workflow

## Source Link

- Internal workflow. See [[../concepts/strategy-research-pipeline]].

## Cadence

Default cadence: scheduled scan on trading/research days, with ad hoc scans allowed only when explicitly requested.

## Inputs

- Forums and trader discussions.
- Papers and factor research.
- Prior project notes.
- Failed Ideas ledger.
- Existing strategy queue.

## Procedure

1. Read [[../Failed Ideas/ledger]] and [[strategy-queue]].
2. Scan sources for raw strategy ideas, anomalies, or repeatable market behaviors.
3. Normalize each idea into the fields from [[../reference/hypothesis-template]].
4. Apply [[../reference/first-filter]].
5. Add candidates to [[strategy-queue]] as `raw`, `needs-data`, or `filtered`.
6. Rank by structural plausibility, data availability, sample size, and implementation simplicity.
7. Output 5 to 15 raw ideas when enough source material exists; only 2 or 3 should normally advance.

## Output Format

Each scan should produce:

- Date and source set.
- New candidates added.
- Candidates promoted to `filtered`.
- Candidates rejected or abandoned, with Failed Ideas rows.
- Open data requirements.

## Caveats

- Do not invent backtest stats during candidate generation.
- Do not build strategy code during candidate generation.
- Do not approve capital exposure from this stage.

## Used In

- [[strategy-queue]]
- [[../reference/first-filter]]
- [[../reference/backtest-battery]]
