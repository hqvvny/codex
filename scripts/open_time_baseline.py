#!/usr/bin/env python3
"""Test simple time-entry/time-exit baseline hypotheses."""

from __future__ import annotations

import argparse
import csv
from datetime import time
import json
from pathlib import Path
import sys
from collections import defaultdict

ROOT = Path(__file__).resolve().parents[1]
if str(ROOT) not in sys.path:
    sys.path.insert(0, str(ROOT))

from lib.market_data import group_by_date, iter_bars, median  # noqa: E402


def average(values: list[float]) -> float:
    if not values:
        raise ValueError("empty values")
    return sum(values) / len(values)


def max_drawdown(cumulative: list[float]) -> float:
    peak = 0.0
    worst = 0.0
    for value in cumulative:
        peak = max(peak, value)
        worst = min(worst, value - peak)
    return worst


def profit_factor(trades: list[float]) -> float | None:
    wins = sum(value for value in trades if value > 0)
    losses = -sum(value for value in trades if value < 0)
    if losses == 0:
        return None
    return wins / losses


def parse_hhmm(value: str) -> time:
    hour, minute = value.split(":", 1)
    return time(int(hour), int(minute))


def run_local_clock(sessions, entry_time: time, exit_time: time):
    rows = []
    for session_date, bars in sorted(sessions.items()):
        by_time = {bar.timestamp.time(): bar for bar in bars}
        entry = by_time.get(entry_time)
        exit_ = by_time.get(exit_time)
        if not entry or not exit_:
            continue
        pnl_points = exit_.close - entry.open
        rows.append(
            {
                "session_date": session_date.isoformat(),
                "entry_timestamp": entry.timestamp.isoformat(sep=" "),
                "exit_timestamp": exit_.timestamp.isoformat(sep=" "),
                "entry_price": entry.open,
                "exit_price": exit_.close,
                "pnl_points": pnl_points,
            }
        )
    return rows


def run_session_aligned(sessions, hold_minutes: int, min_bars: int):
    rows = []
    for session_date, bars in sorted(sessions.items()):
        ordered = sorted(bars, key=lambda bar: bar.timestamp)
        if len(ordered) < max(min_bars, hold_minutes + 1):
            continue
        entry = ordered[0]
        exit_ = ordered[hold_minutes]
        pnl_points = exit_.close - entry.open
        rows.append(
            {
                "session_date": session_date.isoformat(),
                "entry_timestamp": entry.timestamp.isoformat(sep=" "),
                "exit_timestamp": exit_.timestamp.isoformat(sep=" "),
                "entry_price": entry.open,
                "exit_price": exit_.close,
                "pnl_points": pnl_points,
            }
        )
    return rows


def summarize(rows, point_value: float, round_turn_cost_points: float):
    gross = [row["pnl_points"] for row in rows]
    net = [value - round_turn_cost_points for value in gross]
    cumulative = []
    running = 0.0
    for value in net:
        running += value
        cumulative.append(running)
    winners = [value for value in net if value > 0]
    losers = [value for value in net if value < 0]
    return {
        "trades": len(net),
        "gross_total_points": round(sum(gross), 2),
        "gross_avg_points": round(average(gross), 4) if gross else None,
        "net_total_points": round(sum(net), 2),
        "net_avg_points": round(average(net), 4) if net else None,
        "net_total_dollars": round(sum(net) * point_value, 2),
        "win_rate": round(len(winners) / len(net), 4) if net else None,
        "avg_win_points": round(average(winners), 4) if winners else None,
        "avg_loss_points": round(average(losers), 4) if losers else None,
        "median_net_points": round(median(net), 4) if net else None,
        "profit_factor": round(profit_factor(net), 4) if profit_factor(net) is not None else None,
        "max_drawdown_points": round(max_drawdown(cumulative), 2),
        "max_drawdown_dollars": round(max_drawdown(cumulative) * point_value, 2),
    }


def summarize_by_year(rows, round_turn_cost_points: float):
    grouped = defaultdict(list)
    for row in rows:
        grouped[row["session_date"][:4]].append(row["pnl_points"] - round_turn_cost_points)
    summary = {}
    for year, values in sorted(grouped.items()):
        winners = [value for value in values if value > 0]
        summary[year] = {
            "trades": len(values),
            "net_total_points": round(sum(values), 2),
            "net_avg_points": round(average(values), 4),
            "win_rate": round(len(winners) / len(values), 4),
        }
    return summary


def write_trades(path: Path, rows) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    with path.open("w", encoding="utf-8", newline="") as handle:
        writer = csv.DictWriter(
            handle,
            fieldnames=[
                "session_date",
                "entry_timestamp",
                "exit_timestamp",
                "entry_price",
                "exit_price",
                "pnl_points",
            ],
        )
        writer.writeheader()
        writer.writerows(rows)


def main() -> int:
    parser = argparse.ArgumentParser(description="Test simple open-time long baselines.")
    parser.add_argument("--input", required=True, type=Path)
    parser.add_argument("--output-dir", required=True, type=Path)
    parser.add_argument("--entry-time", default="15:30")
    parser.add_argument("--exit-time", default="16:00")
    parser.add_argument("--hold-minutes", type=int, default=30)
    parser.add_argument("--min-bars", type=int, default=300)
    parser.add_argument("--point-value", type=float, default=20.0)
    parser.add_argument("--round-turn-cost-points", type=float, default=0.0)
    args = parser.parse_args()

    sessions = group_by_date(iter_bars(args.input))
    local_rows = run_local_clock(sessions, parse_hhmm(args.entry_time), parse_hhmm(args.exit_time))
    aligned_rows = run_session_aligned(sessions, args.hold_minutes, args.min_bars)

    args.output_dir.mkdir(parents=True, exist_ok=True)
    write_trades(args.output_dir / "MNQ-002-local-clock-trades.csv", local_rows)
    write_trades(args.output_dir / "MNQ-002-session-aligned-trades.csv", aligned_rows)

    summary = {
        "input": str(args.input),
        "data_source": "DATA-001 MotiveWave NQU6 1m RTH export",
        "date_range": "2017-04-17 to 2026-07-10",
        "cost_assumption": f"{args.round_turn_cost_points} points round turn",
        "point_value": args.point_value,
        "variants": {
            "local_clock_1530_to_1600": {
                "definition": f"Long at {args.entry_time} bar open, exit at {args.exit_time} bar close.",
                **summarize(local_rows, args.point_value, args.round_turn_cost_points),
                "by_year": summarize_by_year(local_rows, args.round_turn_cost_points),
            },
            "session_first_bar_plus_30m": {
                "definition": f"Long at first RTH bar open, exit at bar index {args.hold_minutes} close.",
                **summarize(aligned_rows, args.point_value, args.round_turn_cost_points),
                "by_year": summarize_by_year(aligned_rows, args.round_turn_cost_points),
            },
        },
        "caveats": [
            "Gross/simple baseline unless round_turn_cost_points is set.",
            "DATA-001 is RTH-only and likely Europe/Berlin platform time.",
            "No stop, target, slippage, commissions, news filter, or ETH context.",
            "This is a baseline test, not a capital-ready strategy.",
        ],
    }
    (args.output_dir / "MNQ-002-open-long-baseline-summary.json").write_text(
        json.dumps(summary, indent=2) + "\n",
        encoding="utf-8",
    )
    print(json.dumps(summary, indent=2))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
