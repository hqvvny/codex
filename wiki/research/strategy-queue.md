---
type: research
updated: 2026-07-21
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
| 2 | MNQ-003 | 1m 200 EMA retest continuation long/short | User thesis | NQ/MNQ proxy via DATA-001/DATA-002 | Full RTH first pass; session split pending | Indicator + mechanical retest candidate | 200 EMA may act as dynamic trend boundary; continuation traders may defend retests while countertrend traders get trapped | DATA-001 RTH 1m used; DATA-002 and session-window split still available for next pass | First pass produced 26,396 trades, enough sample but likely overactive/choppy | User requested 1R target; below normal >=1.5R standard | testing | Raw full-RTH version failed under stop-first; only continue with structural filters such as long-only/session-window/EMA-slope/displacement | [[hypotheses/MNQ-003-200ema-retest-continuation]], [[../reference/first-filter]] |
| 3 | MNQ-005 | Overnight extreme reversal with NY VWAP/5m confirmation | Daily scout 2026-07-21; SSRN overnight-intraday reversal | MNQ/NQ | Overnight into NY open | Mechanical event + VWAP confirmation | Extreme overnight inventory can reverse intraday as liquidity providers fade stretched moves; NY confirmation avoids blind open fade | DATA-002 ETH 1m, RTH close/open mapping, VWAP, costs/slippage | Likely enough daily sessions from 2017-2026; extreme deciles reduce sample but should remain testable | Target 1.5R/2R or prior close; must verify | raw | Build first as Python backtest using DATA-002 | outputs/daily-strategy-scout-2026-07-21.md |
| 4 | MNQ-006 | Volatility-volume-gap day classifier overlay | Daily scout 2026-07-21; MNQ VVG classifier reference | MNQ/NQ | Pre-NY regime | Mechanical regime filter | NQ strategy behavior changes by overnight gap, volatility expansion, and volume participation | DATA-002 OHLCV; volume reliability; child strategy trade lists | Enough daily sessions, but buckets can get small | Overlay only; inherits child strategy R:R | raw | Build after MNQ-005 as overlay diagnostic for MNQ-003/MNQ-005 | outputs/daily-strategy-scout-2026-07-21.md |
| 5 | MNQ-007 | ORB breakout pullback to VWAP/9EMA | Daily scout 2026-07-21; TradingView community script | MNQ/NQ | NY open | Mechanical ORB + VWAP pullback | Opening range defines early auction; pullback to VWAP/EMA after breakout may improve entry quality versus chasing | DATA-001/002 1m/5m, VWAP, ATR, costs/slippage | Likely enough NY sessions | 1.5R/2R ATR target explicitly testable | raw | Keep behind MNQ-005 because it is adjacent to abandoned MNQ-001 | outputs/daily-strategy-scout-2026-07-21.md |
| 6 | MNQ-008 | Lunch-hour reversal / after-lunch drift | Daily scout 2026-07-21; QuantPedia lunch effect | MNQ/NQ | Midday RTH | Mechanical time/session rule | Liquidity and participant mix change around lunch; morning pressure may reverse or transition into afternoon drift | DATA-001 RTH, exact ET mapping, stop/target costs/slippage | Enough RTH days | Unknown until stop/target added; pure time rule rejected for R:R | raw | Benchmark only; add VWAP/open-direction filters before any serious test | outputs/daily-strategy-scout-2026-07-21.md |
| 7 | MNQ-009 | Low-volatility momentum continuation overlay | Daily scout 2026-07-21; SSRN TSMOM volatility states | MNQ/NQ | Any, especially NY/London | Mechanical regime overlay | Futures momentum may perform better in low or declining volatility states; high-volatility states can whipsaw continuation setups | DATA-002, rolling ATR/realized vol, child strategy outputs | Enough bars and sessions; threshold buckets need OOS check | Overlay only; inherits child R:R | raw | Test as overlay for MNQ-003 and MNQ-007 after MNQ-005 | outputs/daily-strategy-scout-2026-07-21.md |

## Promotion Notes

- No hypothesis is approved for capital from this queue alone.
- Promotion requires the backtest battery, independent review, manual review, and secondary-account observation.
