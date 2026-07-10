#!/usr/bin/env python3
"""Summarize normalized OHLCV sessions."""

from __future__ import annotations

import argparse
import csv
from pathlib import Path
import statistics
import sys

ROOT = Path(__file__).resolve().parents[1]
if str(ROOT) not in sys.path:
    sys.path.insert(0, str(ROOT))
from lib.market_data import iter_bars, summarize_sessions


def percentile(values: list[float], pct: float) -> float:
    if not values:
        raise ValueError("empty values")
    ordered = sorted(values)
    index = round((len(ordered) - 1) * pct)
    return ordered[index]


def main() -> int:
    parser = argparse.ArgumentParser(description="Summarize normalized OHLCV sessions.")
    parser.add_argument("--input", required=True, type=Path)
    parser.add_argument("--output", required=True, type=Path)
    parser.add_argument("--min-bars", type=int, default=300)
    args = parser.parse_args()

    sessions = summarize_sessions(iter_bars(args.input))
    full_sessions = [session for session in sessions if session.bars >= args.min_bars]
    ranges = [session.range_points for session in full_sessions]
    moves = [session.close_to_open_points for session in full_sessions]

    args.output.parent.mkdir(parents=True, exist_ok=True)
    with args.output.open("w", encoding="utf-8", newline="") as handle:
        writer = csv.writer(handle)
        writer.writerow(
            [
                "session_date",
                "start",
                "end",
                "bars",
                "open",
                "high",
                "low",
                "close",
                "volume",
                "range_points",
                "close_to_open_points",
            ]
        )
        for session in sessions:
            writer.writerow(
                [
                    session.session_date.isoformat(),
                    session.start.isoformat(sep=" "),
                    session.end.isoformat(sep=" "),
                    session.bars,
                    session.open,
                    session.high,
                    session.low,
                    session.close,
                    session.volume,
                    session.range_points,
                    session.close_to_open_points,
                ]
            )

    summary = {
        "sessions": len(sessions),
        "sessions_with_min_bars": len(full_sessions),
        "min_bars": args.min_bars,
        "first_session": sessions[0].session_date.isoformat() if sessions else None,
        "last_session": sessions[-1].session_date.isoformat() if sessions else None,
    }
    if ranges:
        summary.update(
            {
                "range_avg": round(statistics.fmean(ranges), 2),
                "range_median": round(statistics.median(ranges), 2),
                "range_p10": round(percentile(ranges, 0.10), 2),
                "range_p90": round(percentile(ranges, 0.90), 2),
                "close_to_open_avg": round(statistics.fmean(moves), 2),
                "close_to_open_median": round(statistics.median(moves), 2),
            }
        )
    print(summary)
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
