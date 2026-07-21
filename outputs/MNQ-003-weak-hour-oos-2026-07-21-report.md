# MNQ-003 Weak-Hour OOS Check

Source baseline: `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-21 12-23.csv`.

Provenance:

- Platform/export: NinjaTrader Strategy Analyzer trade list.
- Strategy: `MNQ003EmaLimitEntryStrategy`.
- Instrument: `NQ 09-26`.
- Configuration from user: long-only, whole day, 200 EMA, entry offset 0 points, 35 trend bars, max hold bars 0, quantity 1, 50-point stop, 100-point target, NinjaTrader monthly fees, slippage 1, since 2016.
- Method: offline hour filters applied to the baseline NT trade list. This preserves baseline Ninja fills but does not rerun Ninja signal generation.
- Detailed CSV: `outputs/MNQ-003-weak-hour-oos-2026-07-21.csv`.

## Baseline By Period

| Period | Trades | Net | PF | Win Rate | Avg Trade | Max DD |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| Full | 5,494 | $298,890.96 | 1.103 | 41.92% | $54.40 | -$47,788.40 |
| 2016-2020 | 1,829 | $112,532.36 | 1.149 | 48.06% | $61.53 | -$24,038.80 |
| 2021-2023 | 1,811 | $120,165.24 | 1.115 | 40.14% | $66.35 | -$29,143.44 |
| 2024-2026 | 1,854 | $66,193.36 | 1.059 | 37.59% | $35.70 | -$47,788.40 |

The strategy degrades materially in 2024-2026. That is the real robustness problem.

## Candidate Filter OOS Splits

| Filter | IS 2016-2023 Net/PF/DD | OOS 2024-2026 Net/PF/DD | IS 2016-2021 Net/PF/DD | OOS 2022-2026 Net/PF/DD |
| --- | ---: | ---: | ---: | ---: |
| None | $232,698 / 1.129 / -$29,143 | $66,193 / 1.059 / -$47,788 | $148,085 / 1.140 / -$24,039 | $150,806 / 1.081 / -$47,788 |
| Skip 10/11/21 | $261,294 / 1.158 / -$24,824 | $77,495 / 1.078 / -$32,930 | $160,252 / 1.164 / -$19,494 | $178,537 / 1.107 / -$32,930 |
| Skip 10/11/21/2 | $240,290 / 1.164 / -$25,513 | $104,628 / 1.116 / -$24,222 | $148,197 / 1.173 / -$18,917 | $196,722 / 1.130 / -$24,222 |
| Skip 10/21 | $253,866 / 1.149 / -$25,152 | $80,086 / 1.078 / -$40,066 | $157,166 / 1.158 / -$20,398 | $176,787 / 1.103 / -$40,066 |
| Skip 21/2 | $220,742 / 1.142 / -$31,599 | $98,725 / 1.101 / -$28,780 | $133,168 / 1.147 / -$20,448 | $186,299 / 1.114 / -$28,780 |
| Skip 9/10/11 | $246,990 / 1.150 / -$27,103 | $61,586 / 1.062 / -$40,622 | $163,722 / 1.170 / -$22,203 | $144,854 / 1.086 / -$40,622 |
| Skip 20/21/22 | $240,613 / 1.144 / -$25,939 | $63,452 / 1.063 / -$46,870 | $139,366 / 1.140 / -$22,463 | $164,699 / 1.098 / -$46,870 |

## Overfit Check

I also searched all 1- to 4-hour combinations, optimized on 2016-2021, then tested on 2022-2026. The top in-sample combinations were mostly bad out-of-sample. Example:

- Skip 4/8/10/15: +$44.5k IS improvement, but -$83.2k OOS degradation.
- Skip 4/8/10/11: +$41.4k IS improvement, but -$65.2k OOS degradation.
- Skip 4/8/10: +$38.3k IS improvement, but -$66.9k OOS degradation.

This confirms the danger: blindly optimizing hours is classic overfitting.

Our tested filter `10/11/21/2` is different from those top in-sample combinations. It is not the best IS-mined set, but it improves OOS:

- 2016-2021 IS: baseline $148.1k vs filtered $148.2k, essentially unchanged.
- 2022-2026 OOS: baseline $150.8k vs filtered $196.7k, improvement of about $45.9k.
- OOS max drawdown improves from -$47.8k to -$24.2k.

## Readout

The weak-hour idea still has overfit risk because it was discovered from the same broad research process. But this OOS check reduces the concern for `10/11/21/2`: it does not merely beautify early in-sample performance; it improves the later, harder 2022-2026 period.

Best current interpretation: `10/11/21/2` is a plausible session-quality filter, not proven edge by itself.

Next validation:

1. Rerun `10/11/21/2` in NinjaTrader with exactly the same fee/slippage template as the baseline.
2. Map platform hours 10/11/21/2 to actual market sessions and explain why they should be weaker.
3. Do not add more hour filters. The next problem is 2024 regime behavior.
