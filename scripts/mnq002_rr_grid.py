#!/usr/bin/env python3
"""Run MNQ-002 overnight-negative R:R bracket tests on normalized RTH data."""

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


def max_drawdown(values: list[float]) -> float:
    peak = 0.0
    worst = 0.0
    running = 0.0
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
    in_drawdown_since: int | None = None
    worst = 0
    for index, value in enumerate(values):
        running += value
        if running >= peak:
            if in_drawdown_since is not None:
                worst = max(worst, index - in_drawdown_since + 1)
                in_drawdown_since = None
            peak = running
        elif in_drawdown_since is None:
            in_drawdown_since = index
    if in_drawdown_since is not None:
        worst = max(worst, len(values) - in_drawdown_since)
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
        by_year.setdefault(row["session_date"][:4], []).append(row["pnl_points"] - round_turn_cost_points)
    output: dict[str, dict] = {}
    for year, values in sorted(by_year.items()):
        output[year] = {
            "trades": len(values),
            "net_total_points": round(sum(values), 2),
            "net_avg_points": round(average(values), 4),
            "win_rate": round(sum(1 for value in values if value > 0) / len(values), 4),
            "profit_factor": round(profit_factor(values), 4) if profit_factor(values) is not None else None,
            "max_drawdown_points": round(max_drawdown(values), 2),
        }
    return output


def exit_trade(
    bars: list[Bar],
    entry_price: float,
    hold_bars: int,
    stop_points: float,
    risk_reward: float,
    exit_mode: str,
    same_bar_policy: str,
) -> tuple[int, str, float, float]:
    stop_price = entry_price - stop_points
    target_price = entry_price + stop_points * risk_reward
    max_index = min(hold_bars, len(bars) - 1)

    for index, bar in enumerate(bars[: max_index + 1]):
        stop_hit = bar.low <= stop_price
        target_hit = bar.high >= target_price

        if exit_mode in {"bracket_only", "bracket_with_time_stop"} and (stop_hit or target_hit):
            if stop_hit and target_hit:
                if same_bar_policy == "target_first":
                    return index, "target_same_bar", target_price, target_price - entry_price
                return index, "stop_same_bar", stop_price, stop_price - entry_price
            if stop_hit:
                return index, "stop", stop_price, stop_price - entry_price
            return index, "target", target_price, target_price - entry_price

    if exit_mode == "bracket_only":
        exit_bar = bars[-1]
        return len(bars) - 1, "end_of_session", exit_bar.close, exit_bar.close - entry_price

    exit_bar = bars[max_index]
    return max_index, "time", exit_bar.close, exit_bar.close - entry_price


def build_trades(
    rth_sessions: dict,
    hold_bars: int,
    stop_points: float,
    risk_reward: float,
    exit_mode: str,
    filter_mode: str,
    same_bar_policy: str,
    min_bars: int,
) -> list[dict]:
    rows: list[dict] = []
    previous_close = None
    previous_date = None

    for session_date, bars in sorted(rth_sessions.items()):
        ordered = sorted(bars, key=lambda bar: bar.timestamp)
        if len(ordered) < min_bars:
            continue

        if previous_close is None:
            previous_close = ordered[-1].close
            previous_date = session_date
            continue

        entry = ordered[0]
        overnight_points = entry.open - previous_close
        overnight_direction = "negative" if overnight_points < 0 else "non_negative"
        if filter_mode != "all" and overnight_direction != filter_mode:
            previous_close = ordered[-1].close
            previous_date = session_date
            continue

        exit_index, exit_reason, exit_price, pnl_points = exit_trade(
            ordered,
            entry.open,
            hold_bars,
            stop_points,
            risk_reward,
            exit_mode,
            same_bar_policy,
        )
        exit_bar = ordered[exit_index]
        rows.append(
            {
                "session_date": session_date.isoformat(),
                "previous_session_date": previous_date.isoformat() if previous_date else "",
                "previous_rth_close": previous_close,
                "entry_timestamp": entry.timestamp.isoformat(sep=" "),
                "exit_timestamp": exit_bar.timestamp.isoformat(sep=" "),
                "entry_price": entry.open,
                "exit_price": exit_price,
                "exit_reason": exit_reason,
                "bars_held": exit_index + 1,
                "overnight_points": overnight_points,
                "overnight_direction": overnight_direction,
                "stop_points": stop_points,
                "risk_reward": risk_reward,
                "target_points": stop_points * risk_reward,
                "pnl_points": pnl_points,
            }
        )

        previous_close = ordered[-1].close
        previous_date = session_date

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


def parse_float_grid(value: str) -> list[float]:
    return [float(item.strip()) for item in value.split(",") if item.strip()]


def parse_int_grid(value: str) -> list[int]:
    return [int(item.strip()) for item in value.split(",") if item.strip()]


def main() -> int:
    parser = argparse.ArgumentParser(description="Run MNQ-002 R:R bracket grid.")
    parser.add_argument("--rth-input", required=True, type=Path)
    parser.add_argument("--output-dir", required=True, type=Path)
    parser.add_argument("--hold-bars", default="15,60")
    parser.add_argument("--stop-points", default="10,15,20,25,30")
    parser.add_argument("--risk-rewards", default="1.5,2.0,2.5")
    parser.add_argument("--exit-modes", default="bracket_only,bracket_with_time_stop")
    parser.add_argument("--filter-mode", default="negative", choices=["all", "negative", "non_negative"])
    parser.add_argument("--same-bar-policy", default="stop_first", choices=["stop_first", "target_first"])
    parser.add_argument("--min-bars", type=int, default=300)
    parser.add_argument("--point-value", type=float, default=20.0)
    parser.add_argument("--round-turn-cost-points", type=float, default=0.0)
    args = parser.parse_args()

    sessions = group_by_date(iter_bars(args.rth_input))
    hold_grid = parse_int_grid(args.hold_bars)
    stop_grid = parse_float_grid(args.stop_points)
    rr_grid = parse_float_grid(args.risk_rewards)
    exit_modes = [item.strip() for item in args.exit_modes.split(",") if item.strip()]

    args.output_dir.mkdir(parents=True, exist_ok=True)
    variants = []
    best_rows: list[dict] = []
    best_key = None

    for exit_mode in exit_modes:
        for hold_bars in hold_grid:
            for stop_points in stop_grid:
                for risk_reward in rr_grid:
                    rows = build_trades(
                        sessions,
                        hold_bars,
                        stop_points,
                        risk_reward,
                        exit_mode,
                        args.filter_mode,
                        args.same_bar_policy,
                        args.min_bars,
                    )
                    summary = summarize(rows, args.point_value, args.round_turn_cost_points)
                    variant = {
                        "exit_mode": exit_mode,
                        "hold_bars": hold_bars,
                        "stop_points": stop_points,
                        "risk_reward": risk_reward,
                        **summary,
                    }
                    variants.append(variant)
                    score = (
                        summary["profit_factor"] or 0,
                        summary["net_avg_points"] or -999,
                        summary["max_drawdown_points"],
                    )
                    if best_key is None or score > best_key:
                        best_key = score
                        best_rows = rows

    variants.sort(
        key=lambda row: (
            row["profit_factor"] or 0,
            row["net_avg_points"] or -999,
            row["net_total_points"],
        ),
        reverse=True,
    )

    summary = {
        "definition": "MNQ-002 first RTH open long with previous-RTH-close overnight filter and explicit stop/target R:R exits.",
        "rth_input": str(args.rth_input),
        "date_range": "2017-04-17 to 2026-07-10",
        "filter_mode": args.filter_mode,
        "same_bar_policy": args.same_bar_policy,
        "cost_assumption": f"{args.round_turn_cost_points} points round turn",
        "slippage_assumption": "0 points unless included in round_turn_cost_points",
        "point_value": args.point_value,
        "caveats": [
            "Uses 1m OHLC data; if stop and target are both touched in one bar, same_bar_policy decides fill order.",
            "Default same-bar policy is stop_first, a conservative assumption.",
            "Entry is modeled at the first RTH bar open from DATA-001, matching the local baseline rather than NT8 order-fill mechanics exactly.",
            "This is a research backtest, not live execution logic.",
        ],
        "variants": variants,
        "top_10": variants[:10],
        "best_by_profit_factor_by_year": summarize_by_year(best_rows, args.round_turn_cost_points) if best_rows else {},
    }

    (args.output_dir / "MNQ-002-rr-grid-summary.json").write_text(
        json.dumps(summary, indent=2) + "\n",
        encoding="utf-8",
    )

    with (args.output_dir / "MNQ-002-rr-grid-summary.csv").open("w", encoding="utf-8", newline="") as handle:
        fieldnames = list(variants[0].keys()) if variants else []
        writer = csv.DictWriter(handle, fieldnames=fieldnames)
        if variants:
            writer.writeheader()
            writer.writerows(variants)

    write_csv(args.output_dir / "MNQ-002-rr-grid-best-trades.csv", best_rows)
    print(json.dumps({"top_10": variants[:10]}, indent=2))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
