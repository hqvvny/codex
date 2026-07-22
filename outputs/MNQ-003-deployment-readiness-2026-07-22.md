# MNQ-003 Deployment Readiness

Question: should the current EMA strategy be treated as ready for deployment, or should it stay in research while the next strategy is built?

Verdict: not deployment ready yet. MNQ-003 is a promising prop-strategy candidate, but it still needs promotion checks before funded execution.

## Current Candidate

- Strategy: `MNQ003EmaLimitEntryCleanStrategy`.
- Instrument/export: `NQ 09-26`, NinjaTrader Strategy Analyzer trade list.
- Source export: `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-21 07-19.csv`.
- Date range in trade list: 2016-01-04 20:01 to 2026-07-20 18:42.
- Core configuration: long-only, 1m 200 EMA limit retest, 35 trend bars, 50-point stop, 100-point target, full day, weak-hour filter intended as `10,11,21,2`.
- Costs: fees included at about $5.16 per trade; slippage/fill settings are not fully visible in the exported trade list.
- Result summary: 4,999 trades, $351,415.16 net, PF 1.133, 42.15% win rate, $70.30 average trade at 1 NQ source sizing.

## What Already Looks Good

- Large sample size across multiple market regimes.
- Long-only branch is materially stronger than the raw mixed long/short branch.
- 2R target improves expectancy quality versus lower R:R versions.
- Weak-hour filter `10/11/21/2` improves the later 2022-2026 OOS split in the offline check rather than only improving early in-sample performance.
- Only 2024 is negative in the fee-matched weak-hour export.
- For LucidFlex-style prop math, the edge is interesting when scaled through micros and account lifecycles rather than traded as full NQ risk.

## Current Red Flags

- Profit factor is still modest at 1.133. The edge exists in the export, but it is not wide.
- 2024 remains negative and concentrated in April, September, and October.
- Longest closed recovery is about 356 days, which is too long for psychological and prop-business comfort.
- Session-close exits are a large positive contributor, so target/stop-only behavior is not the whole edge.
- Some intended weak-hour exclusions still leak entries around platform-hour/session-boundary behavior.
- The exact NinjaTrader fill/slippage template must be locked down before any real deployment interpretation.
- The weak-hour filter is plausible but still needs a structural explanation; otherwise it can become disguised overfitting.

## Required Promotion Gates

| Gate | Status | Pass Condition |
| --- | --- | --- |
| Provenance locked | Partial | Export must include exact instrument, contract, trading-hours template, commission template, slippage, fill mode, strategy version, and commit. |
| Parameter robustness | Missing | EMA, trend bars, SL, TP, entry offset, weak-hour list, and session settings must stay profitable when shifted modestly. |
| Walk-forward/OOS | Partial | Existing weak-hour OOS smoke test is useful, but a proper rolling train/test or anchored walk-forward is still needed. |
| 2024 regime review | Missing | Identify why 2024 failed and whether the same conditions can be detected before or during trading without curve-fitting. |
| Cost stress | Missing | Rerun with worse slippage and higher fees. Candidate should not collapse under small execution stress. |
| Execution realism | Partial | NinjaTrader backtest exists, but market/limit fill behavior, stop behavior, session-close exits, and ambiguous fills still need inspection. |
| Risk layer | Missing | Add/account for daily max loss, max trades/day, pause-after-losses, news shutoff, and account-level MLL guardrails. |
| Manual review | Missing | Human operator must be able to explain every rule and every kill switch. |
| Observation run | Missing | Run SIM/playback or secondary account long enough to compare fills against Strategy Analyzer assumptions. |

## Minimal Next Test Plan

1. Freeze current best baseline.
   - Name: `MNQ-003-B1`.
   - Config: long-only, EMA 200, TrendBars 35, SL 50, TP 100, weak hours `10,11,21,2`, full day, fees/slippage locked.

2. Run parameter sensitivity grid.
   - EMA: 180, 200, 220.
   - TrendBars: 25, 35, 45.
   - Stop: 40, 50, 60.
   - Target R: 1.5, 2.0, 2.5.
   - Entry offset: 0, 1, 2 points.
   - Weak hours: none, `10/11/21`, `10/11/21/2`.

3. Run period splits.
   - 2016-2020.
   - 2021-2023.
   - 2024 only.
   - 2025-2026.
   - Rolling 12-month and 24-month windows.

4. Inspect 2024.
   - Month split: April, September, October are the priority.
   - Compare ATR, gap size, VWAP position, session, and trend/chop condition.
   - Do not add a 2024-specific filter unless it also has a pre-defined structural reason.

5. Add deployment risk shell.
   - Max trades/day.
   - Max daily realized loss.
   - Pause after N consecutive losses.
   - Optional no-news window.
   - Disable if account EOD trailing drawdown buffer is below a fixed threshold.

6. Only then decide:
   - If robust: forward-observation candidate.
   - If fragile: keep as research component or prop-only high-variance strategy.

## Practical Decision

Do not keep optimizing MNQ-003 blindly right now. Freeze the current best version, run the promotion tests above, and start building MNQ-005 in parallel as the next independent edge source.

MNQ-003 is useful enough to protect. That means no more random filter stacking until the robustness battery tells us exactly where the edge is real and where it is just curve-fit noise.
