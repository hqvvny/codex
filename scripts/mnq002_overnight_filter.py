#!/usr/bin/env python3
"""Apply overnight-direction filters to the MNQ-002 open-long baseline."""

from __future__ import annotations

import argparse
import csv
import json
from pathlib import Path
import sys

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


def profit_factor(values: list[float]) -> float | None:
    wins = sum(value for value in values if value > 0)
    losses = -sum(value for value in values if value < 0)
    if losses == 0:
        return None
    return wins / losses


def summarize(rows: list[dict], point_value: float, round_turn_cost_points: float) -> dict:
    gross = [row["pnl_points"] for row in rows]
    net = [value - round_turn_cost_points for value in gross]
    winners = [value for value in net if value > 0]
    losers = [value for value in net if value < 0]
    cumulative = []
    running = 0.0
    for value in net:
        running += value
        cumulative.append(running)
    pf = profit_factor(net)
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
        "profit_factor": round(pf, 4) if pf is not None else None,
        "max_drawdown_points": round(max_drawdown(cumulative), 2),
        "max_drawdown_dollars": round(max_drawdown(cumulative) * point_value, 2),
    }


def summarize_by_year(rows: list[dict], round_turn_cost_points: float) -> dict:
    by_year: dict[str, list[float]] = {}
    for row in rows:
        by_year.setdefault(row["session_date"][:4], []).append(row["pnl_points"] - round_turn_cost_points)
    output = {}
    for year, values in sorted(by_year.items()):
        output[year] = {
            "trades": len(values),
            "net_total_points": round(sum(values), 2),
            "net_avg_points": round(average(values), 4),
            "win_rate": round(sum(1 for value in values if value > 0) / len(values), 4),
        }
    return output


def build_rows(rth_sessions, hold_minutes: int, min_bars: int):
    rows = []
    previous_close = None
    previous_date = None
    for session_date, bars in sorted(rth_sessions.items()):
        ordered = sorted(bars, key=lambda bar: bar.timestamp)
        if len(ordered) < max(min_bars, hold_minutes + 1):
            continue
        entry = ordered[0]
        exit_ = ordered[hold_minutes]
        if previous_close is None:
            previous_close = ordered[-1].close
            previous_date = session_date
            continue
        overnight_points = entry.open - previous_close
        rows.append(
            {
                "session_date": session_date.isoformat(),
                "previous_session_date": previous_date.isoformat() if previous_date else "",
                "previous_rth_close": previous_close,
                "entry_timestamp": entry.timestamp.isoformat(sep=" "),
                "exit_timestamp": exit_.timestamp.isoformat(sep=" "),
                "entry_price": entry.open,
                "exit_price": exit_.close,
                "overnight_points": overnight_points,
                "overnight_direction": "negative" if overnight_points < 0 else "non_negative",
                "pnl_points": exit_.close - entry.open,
            }
        )
        previous_close = ordered[-1].close
        previous_date = session_date
    return rows


def write_rows(path: Path, rows: list[dict]) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    with path.open("w", encoding="utf-8", newline="") as handle:
        writer = csv.DictWriter(handle, fieldnames=list(rows[0].keys()) if rows else [])
        if rows:
            writer.writeheader()
            writer.writerows(rows)


def main() -> int:
    parser = argparse.ArgumentParser(description="Apply overnight filters to MNQ-002.")
    parser.add_argument("--rth-input", required=True, type=Path)
    parser.add_argument("--eth-input", type=Path)
    parser.add_argument("--output-dir", required=True, type=Path)
    parser.add_argument("--hold-minutes", type=int, default=30)
    parser.add_argument("--min-bars", type=int, default=300)
    parser.add_argument("--point-value", type=float, default=20.0)
    parser.add_argument("--round-turn-cost-points", type=float, default=0.0)
    args = parser.parse_args()

    rth_sessions = group_by_date(iter_bars(args.rth_input))
    all_rows = build_rows(rth_sessions, args.hold_minutes, args.min_bars)
    negative_rows = [row for row in all_rows if row["overnight_direction"] == "negative"]
    non_negative_rows = [row for row in all_rows if row["overnight_direction"] == "non_negative"]

    args.output_dir.mkdir(parents=True, exist_ok=True)
    write_rows(args.output_dir / "MNQ-002-overnight-negative-trades.csv", negative_rows)
    write_rows(args.output_dir / "MNQ-002-overnight-non-negative-trades.csv", non_negative_rows)

    summary = {
        "definition": "Overnight negative means current RTH entry open is below the previous RTH session close.",
        "rth_input": str(args.rth_input),
        "eth_input": str(args.eth_input) if args.eth_input else None,
        "data_sources": [
            "DATA-001 RTH bars for prior RTH close, current RTH open, and 30m exit",
            "DATA-002 ETH/all-sessions available for future richer overnight features, not required for this net overnight direction pass",
        ],
        "date_range": "2017-04-17 to 2026-07-10",
        "cost_assumption": f"{args.round_turn_cost_points} points round turn",
        "point_value": args.point_value,
        "variants": {
            "overnight_negative": {
                **summarize(negative_rows, args.point_value, args.round_turn_cost_points),
                "by_year": summarize_by_year(negative_rows, args.round_turn_cost_points),
            },
            "overnight_non_negative": {
                **summarize(non_negative_rows, args.point_value, args.round_turn_cost_points),
                "by_year": summarize_by_year(non_negative_rows, args.round_turn_cost_points),
            },
        },
        "caveats": [
            "Gross/simple baseline unless round_turn_cost_points is set.",
            "No stop, target, slippage, commissions, news filter, or risk sizing.",
            "Filter uses net overnight direction only; it does not yet use overnight range, overnight high/low location, or London/Asia structure.",
            "This is a filter diagnostic, not a capital-ready strategy.",
        ],
    }
    (args.output_dir / "MNQ-002-overnight-filter-summary.json").write_text(
        json.dumps(summary, indent=2) + "\n",
        encoding="utf-8",
    )
    print(json.dumps(summary, indent=2))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
