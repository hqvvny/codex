#!/usr/bin/env python3
"""Profile how much session range is built by early-session windows."""

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


def percentile(values: list[float], pct: float) -> float:
    if not values:
        raise ValueError("empty values")
    ordered = sorted(values)
    index = round((len(ordered) - 1) * pct)
    return ordered[index]


def average(values: list[float]) -> float:
    if not values:
        raise ValueError("empty values")
    return sum(values) / len(values)


def profile_window(session_bars, window_minutes: int):
    ordered = sorted(session_bars, key=lambda bar: bar.timestamp)
    if len(ordered) < window_minutes:
        return None
    window = ordered[:window_minutes]
    session_high = max(bar.high for bar in ordered)
    session_low = min(bar.low for bar in ordered)
    window_high = max(bar.high for bar in window)
    window_low = min(bar.low for bar in window)
    session_range = session_high - session_low
    window_range = window_high - window_low
    if session_range <= 0:
        return None
    return {
        "window_minutes": window_minutes,
        "window_range_points": window_range,
        "session_range_points": session_range,
        "range_share": window_range / session_range,
        "window_high_is_session_high": window_high == session_high,
        "window_low_is_session_low": window_low == session_low,
        "window_contains_one_extreme": window_high == session_high or window_low == session_low,
        "window_contains_both_extremes": window_high == session_high and window_low == session_low,
    }


def summarize(rows: list[dict]) -> dict:
    window_ranges = [row["window_range_points"] for row in rows]
    shares = [row["range_share"] for row in rows]
    return {
        "observations": len(rows),
        "avg_window_range_points": round(average(window_ranges), 2),
        "median_window_range_points": round(median(window_ranges), 2),
        "p10_window_range_points": round(percentile(window_ranges, 0.10), 2),
        "p90_window_range_points": round(percentile(window_ranges, 0.90), 2),
        "avg_session_range_share": round(average(shares), 4),
        "median_session_range_share": round(median(shares), 4),
        "p10_session_range_share": round(percentile(shares, 0.10), 4),
        "p90_session_range_share": round(percentile(shares, 0.90), 4),
        "pct_window_high_is_session_high": round(
            sum(1 for row in rows if row["window_high_is_session_high"]) / len(rows),
            4,
        ),
        "pct_window_low_is_session_low": round(
            sum(1 for row in rows if row["window_low_is_session_low"]) / len(rows),
            4,
        ),
        "pct_window_contains_one_extreme": round(
            sum(1 for row in rows if row["window_contains_one_extreme"]) / len(rows),
            4,
        ),
        "pct_window_contains_both_extremes": round(
            sum(1 for row in rows if row["window_contains_both_extremes"]) / len(rows),
            4,
        ),
    }


def main() -> int:
    parser = argparse.ArgumentParser(description="Profile early-session range build.")
    parser.add_argument("--input", required=True, type=Path)
    parser.add_argument("--output-dir", required=True, type=Path)
    parser.add_argument("--min-bars", type=int, default=300)
    parser.add_argument("--windows", nargs="+", type=int, default=[5, 15, 30, 60, 120])
    args = parser.parse_args()

    sessions = group_by_date(iter_bars(args.input))
    rows = []
    by_window = {window: [] for window in args.windows}
    for session_date, session_bars in sorted(sessions.items()):
        if len(session_bars) < args.min_bars:
            continue
        for window in args.windows:
            result = profile_window(session_bars, window)
            if result is None:
                continue
            result["session_date"] = session_date.isoformat()
            by_window[window].append(result)
            rows.append(result)

    args.output_dir.mkdir(parents=True, exist_ok=True)
    detail_path = args.output_dir / "DATA-001-range-build-detail.csv"
    with detail_path.open("w", encoding="utf-8", newline="") as handle:
        fieldnames = [
            "session_date",
            "window_minutes",
            "window_range_points",
            "session_range_points",
            "range_share",
            "window_high_is_session_high",
            "window_low_is_session_low",
            "window_contains_one_extreme",
            "window_contains_both_extremes",
        ]
        writer = csv.DictWriter(handle, fieldnames=fieldnames)
        writer.writeheader()
        for row in rows:
            writer.writerow(row)

    summary_rows = []
    for window in args.windows:
        window_rows = by_window[window]
        if not window_rows:
            continue
        summary = summarize(window_rows)
        summary["window_minutes"] = window
        summary_rows.append(summary)

    summary_csv = args.output_dir / "DATA-001-range-build-summary.csv"
    with summary_csv.open("w", encoding="utf-8", newline="") as handle:
        fieldnames = [
            "window_minutes",
            "observations",
            "avg_window_range_points",
            "median_window_range_points",
            "p10_window_range_points",
            "p90_window_range_points",
            "avg_session_range_share",
            "median_session_range_share",
            "p10_session_range_share",
            "p90_session_range_share",
            "pct_window_high_is_session_high",
            "pct_window_low_is_session_low",
            "pct_window_contains_one_extreme",
            "pct_window_contains_both_extremes",
        ]
        writer = csv.DictWriter(handle, fieldnames=fieldnames)
        writer.writeheader()
        for row in summary_rows:
            writer.writerow(row)

    summary_json = {
        "input": str(args.input),
        "min_bars": args.min_bars,
        "windows": args.windows,
        "sessions_used": len(next(iter(by_window.values()), [])),
        "summary": summary_rows,
        "caveat": "Descriptive range-build profile only; not strategy performance.",
    }
    (args.output_dir / "DATA-001-range-build-summary.json").write_text(
        json.dumps(summary_json, indent=2) + "\n",
        encoding="utf-8",
    )
    print(json.dumps(summary_json, indent=2))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
