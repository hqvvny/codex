---
type: strategy-hypothesis
id: MNQ-004
status: example-code-created
created: 2026-07-21
---

# MNQ-004 Dual EMA Momentum Retest

## Hypothesis

A market trading on one side of a rising or falling exponential moving average may resume its prevailing direction after price returns to that average and shows renewed momentum. The EMA acts as a dynamic reference for trend and pullback location, not as guaranteed support or resistance.

## First Mechanical Version

Artifact: `outputs/MNQ004DualEmaRetestExampleStrategy.cs`.

Platform: NinjaTrader 8 Strategy Analyzer / chart strategy example.

Core rules:

- Fast EMA and slow EMA are calculated using completed bars.
- Long trend: fast EMA above slow EMA, both rising, and EMA separation above a minimum threshold.
- Short trend: fast EMA below slow EMA, both falling, and EMA separation above a minimum threshold.
- Retest begins when price touches the fast EMA tolerance zone and the bar does not close beyond the slow EMA.
- Long entry requires a completed confirmation bar closing back above the fast EMA with bullish candle body. Short entry is reversed.
- Initial stop uses retest swing plus buffer, capped by `MaxStopPoints`.
- Target uses `RiskReward` times the calculated stop distance.
- Quantity can be fixed or calculated from `AccountRiskDollars`, `PointValue`, and stop distance.
- Exits occur at stop, target, session close, or trend invalidation.
- Attempts per session can be limited.

## Default Parameters

- `DirectionMode = 0`, both sides.
- `FastEmaPeriod = 21`.
- `SlowEmaPeriod = 200`.
- `EmaSlopeLookbackBars = 10`.
- `MinFastSlopePoints = 0.5`.
- `MinSlowSlopePoints = 0.5`.
- `MinEmaSeparationPoints = 5`.
- `RetestTolerancePoints = 5`.
- `MaxRetestBars = 20`.
- `SwingBufferPoints = 2`.
- `MaxStopPoints = 50`.
- `RiskReward = 2`.
- `UseFixedQuantity = 1`.
- `FixedQuantity = 1`.
- `AccountRiskDollars = 500`.
- `PointValue = 20`.
- `MaxAttemptsPerSession = 3`.
- `UseTradeWindow = 0`.

## Notes

This is example code, not a validated candidate. First test should use one predefined instrument/session/timeframe and fixed costs/slippage. Do not compare results to MNQ-003 unless data range, commission template, slippage, fill settings, instrument, and session template match.
