# Daily Strategy Scout - 2026-07-21

Scope: MNQ/NQ intraday strategy candidates for 1m/5m entries, 15m/1H bias, NY/London/Asia/overnight context.

Brain context read first: `wiki/index.md`, `wiki/hot.md`, `wiki/Failed Ideas/ledger.md`, `wiki/concepts/trading-analysis-profile.md`, `wiki/concepts/strategy-research-pipeline.md`, and `wiki/reference/first-filter.md`.

Important exclusion: avoid rebuilding the rejected generic MNQ-001 opening-range liquidity sweep. Opening-range ideas below must require extra filters such as VWAP retest, ATR risk, or regime state.

## Ranked Candidates

### 1. MNQ-005 Overnight Extreme Reversal With NY Confirmation

Source: SSRN "Overnight-Intraday Reversal Everywhere" documents an overnight-to-intraday reversal pattern across asset classes and frames it as liquidity provision rather than ordinary reversal.

Market structure thesis:

- If NQ has an extreme overnight move into NY open, early RTH may mean-revert as liquidity providers fade stretched overnight inventory.
- The trade should not enter blindly at 9:30 ET. Require NY confirmation: failure to extend overnight direction, reclaim/reject VWAP, or first 5m CHoCH back toward prior RTH close.

Mechanical test:

- Rank overnight return from prior RTH close to 9:30 ET.
- Test only top/bottom deciles or z-score extremes.
- Long after negative overnight extreme only if 1m/5m closes back above VWAP or opening VWAP band.
- Short after positive overnight extreme only if 1m/5m closes back below VWAP.
- Stop beyond opening swing; target prior close, VWAP extension, or 1.5R/2R.

Data:

- DATA-002 ETH/all-sessions is available.
- Needs exact RTH close/open session mapping and costs/slippage.

Expected R:R:

- Plausibly 1.5R+ if entry waits for confirmation and target is prior close or 2R.

Invalidation:

- Overnight extreme continues through first 15-30 minutes without VWAP reclaim/rejection.
- Opening drive holds above/below VWAP and does not rotate.

Overfit risks:

- Decile thresholds and VWAP confirmation can be over-optimized.
- Similar to MNQ-002 if reduced to "overnight negative then long at open"; must keep it symmetric and confirmation-based.

Rule type: mechanical indicator/event rules plus optional visual PA review.

Verdict: advance first. This is the best next build because DATA-002 directly supports it and it adds a structurally different edge from MNQ-003.

### 2. MNQ-006 Volatility-Volume-Gap Regime Classifier Overlay

Source: SSRN result found for a validated volatility-volume-gap classifier for Micro E-mini Nasdaq 100 futures.

Market structure thesis:

- NQ session behavior changes by volatility, volume participation, and overnight gap context. Trend-continuation setups should trade in expansion regimes; mean-reversion setups should trade in balanced/low-participation regimes.
- This should be tested first as a regime filter overlay, not as a standalone entry.

Mechanical test:

- Classify each day before NY open using overnight gap %, ETH realized range/ATR, premarket volume proxy if available, and prior-day range percentile.
- Apply classifier to MNQ-003 and MNQ-005 outcomes.
- Compare pass/fail by regime, not raw PnL only.

Data:

- DATA-002 has OHLCV.
- Volume reliability from MotiveWave export must be accepted as proxy.

Expected R:R:

- Overlay only; R:R inherited from child strategy. Reject if it only improves PnL by deleting losing years with no structural rule.

Invalidation:

- Regime buckets do not preserve out-of-sample behavior or collapse to tiny samples.

Overfit risks:

- Very high. Classifier thresholds can easily become disguised curve fit.

Rule type: mechanical.

Verdict: advance second, but build as diagnostic overlay after MNQ-005.

### 3. MNQ-007 ORB Breakout Pullback To VWAP/9EMA

Source: TradingView open-source VWAP ORB Pullback strategy describes opening-range breakout, VWAP/9EMA confirmation, ATR stops, and R:R targets.

Market structure thesis:

- NY open creates initial range and liquidity discovery. Better continuation entries may occur after breakout and pullback to VWAP/9EMA rather than chasing the first break.

Mechanical test:

- Define OR: first 5/15/30 minutes after NY open.
- Long only after break above OR high, price above VWAP and 9EMA, then pullback touches VWAP or VWAP band and closes back above.
- Short symmetric.
- ATR stop or OR-mid stop; target 1.5R/2R.

Data:

- DATA-001 RTH or DATA-002 full session enough.

Expected R:R:

- 1.5R/2R explicitly testable.

Invalidation:

- Price returns inside OR and closes through VWAP against direction.

Overfit risks:

- Similar to rejected MNQ-001 if reduced to generic opening range. Must keep pullback + VWAP confirmation and reject if no edge after costs.

Rule type: mechanical indicator rules.

Verdict: needs caution. Good candidate, but not first because it is adjacent to failed MNQ-001.

### 4. MNQ-008 Lunch-Hour Reversal / After-Lunch Drift

Source: QuantPedia lunch-effect article proposes shorting U.S. index exposure around 11:00-12:00 and then long into 14:00, with futures mentioned as a possible related vehicle.

Market structure thesis:

- Liquidity and participant mix change around lunch. Morning move may partially reverse or transition into lower-volatility afternoon drift.

Mechanical test:

- Platform time must be mapped to ET.
- Test 11:00-12:00 ET short, 12:00-14:00 ET long, and filtered variants by opening direction, VWAP location, and overnight gap.
- Add hard stop and R:R target; reject pure no-stop time drift for prop use.

Data:

- DATA-001 RTH enough.

Expected R:R:

- Unknown. Needs stop/target version; pure time entry has undefined R:R and should be benchmark only.

Invalidation:

- Day is trend-up above VWAP and short leg fights persistent expansion; or day is trend-down and long leg fights downside expansion.

Overfit risks:

- Time-window optimization and timezone/platform mapping.

Rule type: mechanical time/session rule.

Verdict: raw benchmark candidate, not first priority.

### 5. MNQ-009 Low-Volatility Momentum Continuation Overlay

Source: SSRN "Time Series Momentum and Volatility States" finds momentum is most profitable in futures during low or declining volatility states.

Market structure thesis:

- NQ continuation setups like MNQ-003 may work better when volatility is stable/declining, because high-volatility states produce whipsaw and trend failure.

Mechanical test:

- Compute rolling realized volatility/ATR state on 15m/1H.
- Only allow EMA/VWAP continuation entries when current realized volatility is below its rolling median or falling versus prior session.
- Compare against high-volatility-only and no-filter baselines.

Data:

- DATA-002 available.

Expected R:R:

- Overlay only; use child strategy R:R. For new standalone momentum entries, require 1.5R+.

Invalidation:

- Filter removes winners and leaves only slow, low-opportunity conditions; OOS improves less than in-sample.

Overfit risks:

- ATR lookback and percentile threshold tuning.

Rule type: mechanical.

Verdict: useful overlay for MNQ-003/MNQ-007, not standalone priority.

## Recommended Next Builds

1. Build MNQ-005 first: overnight extreme reversal with VWAP/5m confirmation. It is structurally distinct, uses existing ETH data, and can target 1.5R/2R mechanically.
2. Build MNQ-006 as a regime overlay after MNQ-005: volatility-volume-gap classifier applied to MNQ-003 and MNQ-005.

## Sources

- SSRN: Overnight-Intraday Reversal Everywhere - `https://papers.ssrn.com/sol3/papers.cfm?abstract_id=2730304`
- SSRN: Is There an Intraday Momentum Effect in Commodity Futures and Options - `https://papers.ssrn.com/sol3/papers.cfm?abstract_id=4688712`
- SSRN: Time Series Momentum and Volatility States - `https://papers.ssrn.com/sol3/papers.cfm?abstract_id=2515685`
- TradingView: VWAP ORB Pullback Strategy - `https://www.tradingview.com/script/75epRRh2-VWAP-ORB-Pullback-Strategy/`
- QuantPedia: Lunch Effect in the U.S. Stock Market Indices - `https://quantpedia.com/lunch-effect-in-the-u-s-stock-market-indices/`
