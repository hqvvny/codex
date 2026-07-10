---
type: research
updated: 2026-07-10
status: active
verdict: useful
---

# Strategy Queue

This is the ranked queue of hypotheses. It is not a list of approved strategies.

## Queue Rules

- Raw ideas enter here before code is written.
- Every candidate must pass the first filter before entering build/test.
- Keep rejected or abandoned ideas linked to [[../Failed Ideas/ledger]].
- Do not treat rank as approval. Rank only decides what gets reviewed first.

## Status Values

- `raw`: captured, not filtered.
- `needs-data`: structurally interesting but missing required data.
- `filtered`: passed first filter and can enter build/test.
- `building`: prototype in progress.
- `testing`: backtest or robustness battery in progress.
- `review`: awaiting independent or manual review.
- `observe`: running on secondary/demo/small-live account.
- `promoted`: eligible for main-book consideration.
- `abandoned`: rejected; must have a Failed Ideas row.

## Ranked Queue

| Rank | ID | Idea | Source | Market | Session | Type | Structural Reason | Data Needed | Sample Size Check | Expected R:R | Status | Next Action | Links |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 1 | MNQ-001 | Opening range liquidity sweep continuation/reversal after NY open | Project seed | MNQ | NY open | Price action + mechanical candidate | Index futures often sweep opening liquidity before choosing direction; needs objective sweep and reclaim rules | MNQ 1m/5m OHLCV, regular + ETH session markers, costs/slippage | Needs count of NY open sessions over available history | Unknown; must estimate before testing and reject if <1.5:1 | raw | Run first filter and define mechanical event rules | [[candidate-generation-workflow]], [[../reference/first-filter]] |

## Promotion Notes

- No hypothesis is approved for capital from this queue alone.
- Promotion requires the backtest battery, independent review, manual review, and secondary-account observation.
