# MNQ-003 LucidFlex Risk Review

Source trade list: `/Users/farell.trades/Downloads/NinjaTrader Grid 2026-07-21 07-19.csv`.

Strategy configuration:

- Strategy: `MNQ003EmaLimitEntryCleanStrategy`.
- Instrument: `NQ 09-26`.
- Variant: weak-hour filter `10,11,21,2`, fee-matched export.
- Direction: long-only.
- Core: 200 EMA, 35 trend bars, 50-point stop, 100-point target, full day, quantity 1 in the source export.
- Export date range: 2016-01-04 to 2026-07-20.
- Export result: 4,999 trades, $351,415.16 net, PF 1.133, 42.15% win rate, -$37,336.84 trade-list max drawdown.

LucidFlex rule assumptions from Lucid Trading Help Center, checked 2026-07-21:

- Funded account rules source: `https://support.lucidtrading.com/en/articles/12945795-lucidflex-funded-account`.
- Drawdown rules source: `https://support.lucidtrading.com/en/articles/12945815-lucidflex-drawdown`.

- LucidFlex funded accounts have no daily loss limit, no consistency rule, no payout buffer, 90/10 split, and EOD trailing drawdown.
- Max Loss Limits and max sizes:
  - $25k: $1,000 MLL, max 2 minis or 20 micros.
  - $50k: $2,000 MLL, max 4 minis or 40 micros.
  - $100k: $3,000 MLL, max 6 minis or 60 micros.
  - $150k: $4,500 MLL, max 10 minis or 100 micros.
- EOD drawdown: MLL rises with highest closing balance until it locks at initial balance + $100 once the account exceeds the initial trail balance.

Simulation assumptions:

- EOD balance simulation by trade exit date.
- No intraday mark-to-market breach modeled, because LucidFlex uses EOD drawdown.
- No payout withdrawals modeled.
- Source export is 1 NQ. Multipliers scale source PnL linearly:
  - `1.0` = 1 NQ = 10 MNQ equivalent.
  - `0.1` = 1 MNQ equivalent.
  - `0.2` = 2 MNQ equivalent.

Detailed sizing CSV: `outputs/MNQ-003-lucid-flex-risk-sizing-2026-07-21.csv`.

## Daily Risk Shape

| Metric | Value |
| --- | ---: |
| Trading days in export | 2,692 |
| Net at 1 NQ source size | $351,415.16 |
| Worst realized day | -$4,205.80 on 2024-10-03 |
| Worst 5 trading days | -$13,683.04, 2024-09-27 to 2024-10-03 |
| Worst 10 trading days | -$18,180.28, 2024-09-20 to 2024-10-03 |
| Worst 20 trading days | -$20,489.28, 2024-09-20 to 2024-10-17 |
| Worst 60 trading days | -$26,555.44, 2024-09-02 to 2024-11-22 |
| Worst daily losing streak | 13 losing days, 2022-05-02 to 2022-05-18 |
| Worst trade losing streak | 13 trades |

The strategy's LucidFlex risk is not mainly one huge day. The bigger issue is multi-day clustering before the EOD trail has enough cushion.

## LucidFlex Survival By Account Size

| Account | MLL | Smallest tested size that survived | Practical read |
| --- | ---: | ---: | --- |
| $25k | $1,000 | 0.5 MNQ theoretical | Not viable with normal 1 MNQ execution |
| $50k | $2,000 | 1 MNQ survived | Viable only at very small size |
| $100k | $3,000 | 2 MNQ survived | Best practical starting point |
| $150k | $4,500 | 3 MNQ survived | Best cushion among listed accounts |

## Size Simulation

| Account | Size | Breach? | Breach Date | Min Buffer | Locked Trail? | Final / Breach Balance |
| --- | ---: | --- | --- | ---: | --- | ---: |
| $25k | 1 NQ | Yes | 2016-01-05 | -$50.32 | No | $24,799.52 |
| $25k | 1 MNQ | Yes | 2016-02-05 | -$72.96 | No | $24,012.02 |
| $25k | 0.5 MNQ theoretical | No | | $354.48 | Yes | $42,570.76 |
| $50k | 1 NQ | Yes | 2016-01-08 | -$845.80 | No | $48,004.04 |
| $50k | 2 MNQ | Yes | 2016-02-05 | -$145.93 | No | $48,024.04 |
| $50k | 1 MNQ | No | | $708.97 | Yes | $85,141.52 |
| $100k | 1 NQ | Yes | 2016-01-14 | -$611.44 | No | $97,238.40 |
| $100k | 3 MNQ | Yes | 2016-02-05 | -$218.89 | No | $97,036.06 |
| $100k | 2 MNQ | No | | $417.94 | Yes | $170,283.03 |
| $100k | 1 MNQ | No | | $1,708.97 | Yes | $135,141.52 |
| $150k | 1 NQ | Yes | 2016-01-19 | -$101.92 | No | $146,247.92 |
| $150k | 5 MNQ | Yes | 2016-02-04 | -$359.74 | No | $145,565.18 |
| $150k | 3 MNQ | No | | $626.90 | Yes | $255,424.55 |
| $150k | 2 MNQ | No | | $1,917.94 | Yes | $220,283.03 |

## Readout

For LucidFlex, this strategy should be treated as a micro-contract strategy, not a 1 NQ strategy.

Recommended starting sizes:

- $25k: avoid, or do not trade this strategy until there is extra buffer. The historical path breaches even at 1 MNQ.
- $50k: 1 MNQ maximum starting size.
- $100k: 1-2 MNQ; 2 MNQ survived, but buffer got as low as about $418.
- $150k: 2-3 MNQ; 3 MNQ survived, but buffer got as low as about $627.

Do not run max allowed size. Lucid's max size limits are much larger than this strategy's drawdown tolerance. The account rule that matters is MLL, not max position size.

## Next Risk Controls

- Build an account-size parameter table for live use: 50k=1 MNQ, 100k=1-2 MNQ, 150k=2-3 MNQ.
- Add a daily stop for our own operation even though LucidFlex has no DLL. A reasonable first cap is 1-2 stopped trades per day depending on account size.
- Pause after payout or after trail-lock transitions until the new MLL/buffer is known.
- Before live use, rerun this exact report on any updated NT8 export with the same fees, slippage, and session template.
