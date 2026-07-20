#!/usr/bin/env python3
"""Backtest MNQ-003 1m 200 EMA retest continuation."""

from __future__ import annotations

import argparse
import csv
import json
from pathlib import Path
import sys

ROOT = Path(__file__).resolve().parents[1]
if str(ROOT) not in sys.path:
    sys.path.insert(0, str(ROOT))

from lib.market_data import Bar, average, group_by_date, iter_bars, median  # noqa: E402


def ema_values(bars: list[Bar], period: int) -> list[float | None]:
    values: list[float | None] = [None] * len(bars)
    if len(bars) < period:
        return values
    seed = sum(bar.close for bar in bars[:period]) / period
    values[period - 1] = seed
    multiplier = 2 / (period + 1)
    previous = seed
    for index in range(period, len(bars)):
        previous = bars[index].close * multiplier + previous * (1 - multiplier)
        values[index] = previous
    return values


def max_drawdown(values: list[float]) -> float:
    peak = 0.0
    running = 0.0
    worst = 0.0
    for value in values:
        running += value
        peak = max(peak, running)
        worst = min(worst, running - peak)
    return worst


def profit_factor(values: list[float]) -> float | None:
    wins = sum(value for value in values if value > 0)
    losses = -sum(value for value in values if value < 0)
    if losses == 0:
        return None
    return wins / losses


def max_recovery_trades(values: list[float]) -> int:
    peak = 0.0
    running = 0.0
    start: int | None = None
    worst = 0
    for index, value in enumerate(values):
        running += value
        if running >= peak:
            if start is not None:
                worst = max(worst, index - start + 1)
                start = None
            peak = running
        elif start is None:
            start = index
    if start is not None:
        worst = max(worst, len(values) - start)
    return worst


def summarize(rows: list[dict], point_value: float, round_turn_cost_points: float) -> dict:
    net = [row["pnl_points"] - round_turn_cost_points for row in rows]
    winners = [value for value in net if value > 0]
    losers = [value for value in net if value < 0]
    pf = profit_factor(net)
    return {
        "trades": len(net),
        "net_total_points": round(sum(net), 2),
        "net_total_dollars": round(sum(net) * point_value, 2),
        "net_avg_points": round(average(net), 4) if net else None,
        "net_avg_dollars": round(average(net) * point_value, 2) if net else None,
        "win_rate": round(len(winners) / len(net), 4) if net else None,
        "avg_win_points": round(average(winners), 4) if winners else None,
        "avg_loss_points": round(average(losers), 4) if losers else None,
        "median_net_points": round(median(net), 4) if net else None,
        "profit_factor": round(pf, 4) if pf is not None else None,
        "max_drawdown_points": round(max_drawdown(net), 2),
        "max_drawdown_dollars": round(max_drawdown(net) * point_value, 2),
        "max_recovery_trades": max_recovery_trades(net),
    }


def summarize_by_year(rows: list[dict], round_turn_cost_points: float) -> dict[str, dict]:
    by_year: dict[str, list[float]] = {}
    for row in rows:
        by_year.setdefault(row["entry_timestamp"][:4], []).append(row["pnl_points"] - round_turn_cost_points)
    output: dict[str, dict] = {}
    for year, values in sorted(by_year.items()):
        pf = profit_factor(values)
        output[year] = {
            "trades": len(values),
            "net_total_points": round(sum(values), 2),
            "net_avg_points": round(average(values), 4),
            "win_rate": round(sum(1 for value in values if value > 0) / len(values), 4),
            "profit_factor": round(pf, 4) if pf is not None else None,
            "max_drawdown_points": round(max_drawdown(values), 2),
        }
    return output


def confirmed_trend(emas: list[float | None], bars: list[Bar], index: int, direction: str, trend_bars: int) -> bool:
    start = index - trend_bars
    if start < 0:
        return False
    for prior in range(start, index):
        ema = emas[prior]
        if ema is None:
            return False
        if direction == "long" and bars[prior].close <= ema:
            return False
        if direction == "short" and bars[prior].close >= ema:
            return False
    return True


def signal_direction(
    bars: list[Bar],
    emas: list[float | None],
    index: int,
    tolerance_points: float,
    trend_bars: int,
) -> str | None:
    ema = emas[index]
    if ema is None:
        return None
    bar = bars[index]

    if confirmed_trend(emas, bars, index, "long", trend_bars):
        touched = bar.low <= ema + tolerance_points
        not_too_deep = bar.low >= ema - tolerance_points
        rejected = bar.close > ema
        if touched and not_too_deep and rejected:
            return "long"

    if confirmed_trend(emas, bars, index, "short", trend_bars):
        touched = bar.high >= ema - tolerance_points
        not_too_deep = bar.high <= ema + tolerance_points
        rejected = bar.close < ema
        if touched and not_too_deep and rejected:
            return "short"

    return None


def exit_trade(
    bars: list[Bar],
    start_index: int,
    direction: str,
    entry_price: float,
    stop_price: float,
    target_price: float,
    same_bar_policy: str,
) -> tuple[int, str, float, float]:
    for index in range(start_index, len(bars)):
        bar = bars[index]
        if direction == "long":
            stop_hit = bar.low <= stop_price
            target_hit = bar.high >= target_price
            if stop_hit or target_hit:
                if stop_hit and target_hit:
                    if same_bar_policy == "target_first":
                        return index, "target_same_bar", target_price, target_price - entry_price
                    return index, "stop_same_bar", stop_price, stop_price - entry_price
                if stop_hit:
                    return index, "stop", stop_price, stop_price - entry_price
                return index, "target", target_price, target_price - entry_price
        else:
            stop_hit = bar.high >= stop_price
            target_hit = bar.low <= target_price
            if stop_hit or target_hit:
                if stop_hit and target_hit:
                    if same_bar_policy == "target_first":
                        return index, "target_same_bar", target_price, entry_price - target_price
                    return index, "stop_same_bar", stop_price, entry_price - stop_price
                if stop_hit:
                    return index, "stop", stop_price, entry_price - stop_price
                return index, "target", target_price, entry_price - target_price

    last_index = len(bars) - 1
    last_close = bars[last_index].close
    pnl = last_close - entry_price if direction == "long" else entry_price - last_close
    return last_index, "end_of_session", last_close, pnl


def build_trades(
    bars: list[Bar],
    ema_period: int,
    tolerance_points: float,
    trend_bars: int,
    stop_buffer_points: float,
    risk_reward: float,
    same_bar_policy: str,
    min_risk_points: float,
    max_risk_points: float | None,
) -> list[dict]:
    sessions = group_by_date(bars)
    rows: list[dict] = []

    for session_date, session_bars in sorted(sessions.items()):
        ordered = sorted(session_bars, key=lambda bar: bar.timestamp)
        if len(ordered) <= ema_period + trend_bars + 2:
            continue
        emas = ema_values(ordered, ema_period)
        index = ema_period + trend_bars
        while index < len(ordered) - 1:
            direction = signal_direction(ordered, emas, index, tolerance_points, trend_bars)
            if direction is None:
                index += 1
                continue

            signal = ordered[index]
            entry_bar = ordered[index + 1]
            entry_price = entry_bar.open

            if direction == "long":
                stop_price = signal.low - stop_buffer_points
                risk_points = entry_price - stop_price
                target_price = entry_price + risk_points * risk_reward
            else:
                stop_price = signal.high + stop_buffer_points
                risk_points = stop_price - entry_price
                target_price = entry_price - risk_points * risk_reward

            if risk_points < min_risk_points or (max_risk_points is not None and risk_points > max_risk_points):
                index += 1
                continue

            exit_index, exit_reason, exit_price, pnl_points = exit_trade(
                ordered,
                index + 1,
                direction,
                entry_price,
                stop_price,
                target_price,
                same_bar_policy,
            )
            exit_bar = ordered[exit_index]
            ema = emas[index]
            rows.append(
                {
                    "session_date": session_date.isoformat(),
                    "direction": direction,
                    "signal_timestamp": signal.timestamp.isoformat(sep=" "),
                    "entry_timestamp": entry_bar.timestamp.isoformat(sep=" "),
                    "exit_timestamp": exit_bar.timestamp.isoformat(sep=" "),
                    "ema": round(ema, 4) if ema is not None else "",
                    "signal_open": signal.open,
                    "signal_high": signal.high,
                    "signal_low": signal.low,
                    "signal_close": signal.close,
                    "entry_price": entry_price,
                    "stop_price": stop_price,
                    "target_price": target_price,
                    "risk_points": risk_points,
                    "risk_reward": risk_reward,
                    "exit_price": exit_price,
                    "exit_reason": exit_reason,
                    "bars_held": exit_index - index,
                    "pnl_points": pnl_points,
                }
            )
            index = exit_index + 1

    return rows


def write_csv(path: Path, rows: list[dict]) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    if not rows:
        path.write_text("", encoding="utf-8")
        return
    with path.open("w", encoding="utf-8", newline="") as handle:
        writer = csv.DictWriter(handle, fieldnames=list(rows[0].keys()))
        writer.writeheader()
        writer.writerows(rows)


def main() -> int:
    parser = argparse.ArgumentParser(description="Run MNQ-003 EMA retest backtest.")
    parser.add_argument("--input", required=True, type=Path)
    parser.add_argument("--output-dir", required=True, type=Path)
    parser.add_argument("--ema-period", type=int, default=200)
    parser.add_argument("--tolerance-points", type=float, default=10.0)
    parser.add_argument("--trend-bars", type=int, default=10)
    parser.add_argument("--stop-buffer-points", type=float, default=2.0)
    parser.add_argument("--risk-reward", type=float, default=1.0)
    parser.add_argument("--same-bar-policy", choices=["stop_first", "target_first"], default="stop_first")
    parser.add_argument("--min-risk-points", type=float, default=0.25)
    parser.add_argument("--max-risk-points", type=float)
    parser.add_argument("--point-value", type=float, default=20.0)
    parser.add_argument("--round-turn-cost-points", type=float, default=0.0)
    args = parser.parse_args()

    bars = list(iter_bars(args.input))
    rows = build_trades(
        bars,
        args.ema_period,
        args.tolerance_points,
        args.trend_bars,
        args.stop_buffer_points,
        args.risk_reward,
        args.same_bar_policy,
        args.min_risk_points,
        args.max_risk_points,
    )

    longs = [row for row in rows if row["direction"] == "long"]
    shorts = [row for row in rows if row["direction"] == "short"]
    summary = {
        "definition": "MNQ-003 1m 200 EMA retest continuation; max 10pt default EMA overshoot/undershoot; 1R target default.",
        "input": str(args.input),
        "ema_period": args.ema_period,
        "tolerance_points": args.tolerance_points,
        "trend_bars": args.trend_bars,
        "stop_buffer_points": args.stop_buffer_points,
        "risk_reward": args.risk_reward,
        "same_bar_policy": args.same_bar_policy,
        "min_risk_points": args.min_risk_points,
        "max_risk_points": args.max_risk_points,
        "cost_assumption": f"{args.round_turn_cost_points} points round turn",
        "slippage_assumption": "0 points unless included in round_turn_cost_points",
        "variants": {
            "all": {
                **summarize(rows, args.point_value, args.round_turn_cost_points),
                "by_year": summarize_by_year(rows, args.round_turn_cost_points),
            },
            "long": summarize(longs, args.point_value, args.round_turn_cost_points),
            "short": summarize(shorts, args.point_value, args.round_turn_cost_points),
        },
        "caveats": [
            "Stop uses retest candle extreme plus buffer because target is defined as 1R.",
            "Multiple trades are allowed, but positions do not overlap.",
            "Uses 1m OHLC data; same-bar stop/target order is unknown and controlled by same_bar_policy.",
            "This first pass uses full sessions in the input file; session-window splits still need to be tested.",
        ],
    }

    args.output_dir.mkdir(parents=True, exist_ok=True)
    write_csv(args.output_dir / "MNQ-003-ema-retest-trades.csv", rows)
    (args.output_dir / "MNQ-003-ema-retest-summary.json").write_text(
        json.dumps(summary, indent=2) + "\n",
        encoding="utf-8",
    )
    print(json.dumps(summary, indent=2))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
