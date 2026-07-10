# MNQ-001 Definition Worksheet

Purpose: fill this out so MNQ-001 can move from `needs-definition` toward `filtered`. Unknown is allowed, but any unknown keeps the idea out of build/test.

## 1. Core Variant

Choose one primary variant first:

- [ ] Reversal after sweep and reclaim
- [ ] Continuation after sweep and acceptance
- [ ] Test both as separate variants

Notes:

```text

```

## 2. Opening Range

Opening range starts at NY open, 9:30 ET.

Range length:

- [ ] First 5 minutes, 9:30-9:35 ET
- [ ] First 15 minutes, 9:30-9:45 ET
- [ ] First 30 minutes, 9:30-10:00 ET
- [ ] Other:

If other:

```text

```

## 3. Liquidity Levels

Which levels count as valid sweep targets?

- [ ] Opening range high/low only
- [ ] Premarket high/low
- [ ] Overnight high/low
- [ ] Prior day high/low
- [ ] VWAP
- [ ] Other:

Priority order if multiple levels are selected:

```text
1.
2.
3.
```

## 4. Sweep Definition

Minimum sweep threshold:

- [ ] 1 tick beyond level
- [ ] 2 ticks beyond level
- [ ] 4 ticks beyond level
- [ ] Must close beyond level
- [ ] Other:

Maximum time after NY open for a valid sweep:

- [ ] Before 10:00 ET
- [ ] Before 10:30 ET
- [ ] Before 11:00 ET
- [ ] No limit inside RTH
- [ ] Other:

Notes:

```text

```

## 5. Reclaim Rule

For reversal variant only.

What confirms reclaim?

- [ ] 1m close back inside swept level
- [ ] 5m close back inside swept level
- [ ] Close back inside opening range
- [ ] Break of the candle that caused the sweep
- [ ] Other:

How fast must reclaim happen?

- [ ] Within 1 bar
- [ ] Within 3 bars
- [ ] Within 5 bars
- [ ] Within 10 bars
- [ ] Other:

## 6. Acceptance Rule

For continuation variant only.

What confirms acceptance beyond the level?

- [ ] 1m close beyond swept level
- [ ] 5m close beyond swept level
- [ ] 2 consecutive 1m closes beyond level
- [ ] Pullback holds swept level as support/resistance
- [ ] Other:

Confirmation window:

- [ ] Within 5 minutes
- [ ] Within 15 minutes
- [ ] Within 30 minutes
- [ ] Other:

## 7. Entry Trigger

Entry style:

- [ ] Market on confirmation close
- [ ] Limit order on retest of reclaimed/swept level
- [ ] Stop order beyond confirmation candle
- [ ] Other:

Entry timeframe:

- [ ] 1m
- [ ] 5m
- [ ] Both variants

Notes:

```text

```

## 8. Stop

Stop location:

- [ ] Beyond sweep extreme
- [ ] Beyond opening range opposite side
- [ ] Beyond confirmation candle
- [ ] Fixed points/ticks
- [ ] ATR-based
- [ ] Other:

Buffer:

- [ ] 1 tick
- [ ] 2 ticks
- [ ] 4 ticks
- [ ] No buffer
- [ ] Other:

## 9. Target

Primary target:

- [ ] Opposite side of opening range
- [ ] VWAP
- [ ] Premarket high/low
- [ ] Overnight high/low
- [ ] Prior day high/low
- [ ] Fixed R multiple
- [ ] Other:

Minimum required planned R:R:

- [ ] 1.5:1
- [ ] 2:1
- [ ] 3:1
- [ ] Other:

Management:

- [ ] All out at target
- [ ] Partial at 1R, rest at target
- [ ] Trail after 1R
- [ ] Move stop to breakeven after 1R
- [ ] Other:

## 10. Bias Filter

Use higher-timeframe bias?

- [ ] No, baseline first
- [ ] 15m trend filter
- [ ] 1H trend filter
- [ ] VWAP bias
- [ ] Prior day range bias
- [ ] Other:

Bias rule in plain language:

```text

```

## 11. No-Trade Conditions

Skip trade when:

- [ ] Major scheduled news within +/- 15 minutes
- [ ] Major scheduled news within +/- 30 minutes
- [ ] Opening range is too large
- [ ] Opening range is too small
- [ ] Spread/liquidity abnormal
- [ ] Price already hit major HTF target before setup
- [ ] Other:

Define thresholds if selected:

```text

```

## 12. Data Source

What data can we actually use?

Provider/source:

```text

```

Available date range:

```text

```

Timeframe:

- [ ] 1m OHLCV
- [ ] Tick data
- [ ] 5m only
- [ ] Other:

Includes ETH/RTH session info?

- [ ] Yes
- [ ] No
- [ ] Unknown

## 13. Cost Assumptions

Commission + fees per MNQ round trip:

```text

```

Slippage assumption:

- [ ] 1 tick round trip
- [ ] 2 ticks round trip
- [ ] 4 ticks round trip
- [ ] Stress all of the above
- [ ] Other:

## 14. Invalidation

The setup is invalid if:

```text

```

The hypothesis should be abandoned if backtest shows:

```text

```

## 15. Human Notes

Anything discretionary you visually care about, even if it is not mechanical yet:

```text

```
