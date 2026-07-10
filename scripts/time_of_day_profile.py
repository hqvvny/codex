#!/usr/bin/env python3
"""Create time-of-day profile artifacts from normalized OHLCV data."""

from __future__ import annotations

import argparse
import csv
import json
from pathlib import Path
import sys

ROOT = Path(__file__).resolve().parents[1]
if str(ROOT) not in sys.path:
    sys.path.insert(0, str(ROOT))

from lib.market_data import (  # noqa: E402
    TimeBucketStats,
    buckets_by_local_time,
    buckets_by_session_hour,
    buckets_by_session_minute,
    group_by_date,
    iter_bars,
    time_bucket_stats,
)


def write_stats(path: Path, stats: list[TimeBucketStats]) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    with path.open("w", encoding="utf-8", newline="") as handle:
        writer = csv.writer(handle)
        writer.writerow(
            [
                "bucket",
                "observations",
                "avg_range_points",
                "median_range_points",
                "avg_abs_close_to_open_points",
                "median_abs_close_to_open_points",
                "avg_volume",
            ]
        )
        for item in stats:
            writer.writerow(
                [
                    item.bucket,
                    item.observations,
                    round(item.avg_range_points, 4),
                    round(item.median_range_points, 4),
                    round(item.avg_abs_close_to_open_points, 4),
                    round(item.median_abs_close_to_open_points, 4),
                    round(item.avg_volume, 2),
                ]
            )


def top_buckets(stats: list[TimeBucketStats], field: str, count: int = 10) -> list[dict[str, float | int | str]]:
    ranked = sorted(stats, key=lambda item: getattr(item, field), reverse=True)
    output = []
    for item in ranked[:count]:
        output.append(
            {
                "bucket": item.bucket,
                "observations": item.observations,
                field: round(getattr(item, field), 4),
            }
        )
    return output


def main() -> int:
    parser = argparse.ArgumentParser(description="Create time-of-day profile artifacts.")
    parser.add_argument("--input", required=True, type=Path)
    parser.add_argument("--output-dir", required=True, type=Path)
    parser.add_argument("--min-bars", type=int, default=300)
    args = parser.parse_args()

    bars = list(iter_bars(args.input))
    sessions_by_date = group_by_date(bars)
    sessions = [session for _, session in sorted(sessions_by_date.items())]
    full_sessions = [session for session in sessions if len(session) >= args.min_bars]

    minute_stats = time_bucket_stats(buckets_by_session_minute(sessions, args.min_bars))
    hour_stats = time_bucket_stats(buckets_by_session_hour(sessions, args.min_bars))
    local_time_stats = time_bucket_stats(buckets_by_local_time(bars))

    args.output_dir.mkdir(parents=True, exist_ok=True)
    write_stats(args.output_dir / "DATA-001-time-of-day-session-minute.csv", minute_stats)
    write_stats(args.output_dir / "DATA-001-time-of-day-session-hour.csv", hour_stats)
    write_stats(args.output_dir / "DATA-001-time-of-day-local-time.csv", local_time_stats)

    summary = {
        "input": str(args.input),
        "min_bars": args.min_bars,
        "sessions": len(sessions),
        "sessions_used_for_session_aligned_profiles": len(full_sessions),
        "session_minute_buckets": len(minute_stats),
        "session_hour_buckets": len(hour_stats),
        "local_time_buckets": len(local_time_stats),
        "top_session_minutes_by_avg_range": top_buckets(minute_stats, "avg_range_points"),
        "top_session_hours_by_avg_range": top_buckets(hour_stats, "avg_range_points"),
        "top_local_times_by_avg_range": top_buckets(local_time_stats, "avg_range_points"),
        "caveat": "Descriptive market-profile stats only; not strategy performance.",
    }
    (args.output_dir / "DATA-001-time-of-day-summary.json").write_text(
        json.dumps(summary, indent=2) + "\n",
        encoding="utf-8",
    )
    print(json.dumps(summary, indent=2))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
