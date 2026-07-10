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
| - | MNQ-001 | Opening range liquidity sweep continuation/reversal after NY open | Project seed | MNQ | NY open | Price action + mechanical candidate | Index futures often sweep opening liquidity before choosing direction; needs objective sweep and reclaim rules | MNQ 1m/5m OHLCV, ETH/RTH session markers, commission, fees, slippage | Likely enough sessions if at least 1-2 years of 1m data are available, but not proven yet | Unknown; must estimate before testing and reject if <1.5:1 | abandoned | Do not build; user rejected as low-quality/not worth pursuing | [[hypotheses/MNQ-001-opening-range-liquidity-sweep]], [[../Failed Ideas/ledger]] |
| 1 | MNQ-002 | Long at 15:30 local/RTH open, exit 16:00 | User thesis | NQ/MNQ proxy via DATA-001 | RTH open | Mechanical time-entry baseline | Possible opening drift / early-session buy pressure; currently weak structural explanation | DATA-001 RTH 1m available; ETH context missing; costs/slippage not defined | 2,314 exact local-clock trades gross; 2,185 session-aligned trades gross | Not defined because no stop/target; time-exit baseline only | testing | Do not promote; inspect filters/regimes or use as benchmark only | [[hypotheses/MNQ-002-open-long-1530-exit-1600]], [[../reference/backtest-battery]] |

## Promotion Notes

- No hypothesis is approved for capital from this queue alone.
- Promotion requires the backtest battery, independent review, manual review, and secondary-account observation.
