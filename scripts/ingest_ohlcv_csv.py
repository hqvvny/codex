#!/usr/bin/env python3
"""Profile and normalize simple OHLCV CSV exports.

Expected raw format for DATA-001:
YYYYMMDD HHMMSS,open,high,low,close,volume
"""

from __future__ import annotations

import argparse
import csv
from collections import Counter
from dataclasses import dataclass
from datetime import datetime
import json
from pathlib import Path
from typing import Any


@dataclass
class DatasetConfig:
    dataset_id: str
    raw_path: Path
    timestamp_format: str
    timezone: str
    timezone_status: str
    instrument_label: str
    instrument_family: str
    timeframe: str
    session_label: str
    source_platform: str
    export_method: str
    contract_handling: str
    notes: list[str]


def load_config(path: Path) -> DatasetConfig:
    data = json.loads(path.read_text(encoding="utf-8"))
    return DatasetConfig(
        dataset_id=data["dataset_id"],
        raw_path=Path(data["raw_path"]).expanduser(),
        timestamp_format=data["timestamp_format"],
        timezone=data["timezone"],
        timezone_status=data["timezone_status"],
        instrument_label=data["instrument_label"],
        instrument_family=data["instrument_family"],
        timeframe=data["timeframe"],
        session_label=data["session_label"],
        source_platform=data["source_platform"],
        export_method=data["export_method"],
        contract_handling=data["contract_handling"],
        notes=list(data.get("notes", [])),
    )


def parse_row(row: list[str], timestamp_format: str) -> tuple[datetime, float, float, float, float, int]:
    if len(row) != 6:
        raise ValueError(f"expected 6 columns, got {len(row)}")
    ts = datetime.strptime(row[0], timestamp_format)
    open_ = float(row[1])
    high = float(row[2])
    low = float(row[3])
    close = float(row[4])
    volume = int(float(row[5]))
    return ts, open_, high, low, close, volume


def profile_dataset(config: DatasetConfig, normalized_output: Path | None) -> dict[str, Any]:
    if not config.raw_path.exists():
        raise FileNotFoundError(config.raw_path)

    rows = 0
    parse_errors = 0
    duplicate_timestamps = 0
    high_below_low = 0
    open_outside_range = 0
    close_outside_range = 0
    negative_volume = 0
    first_ts: datetime | None = None
    last_ts: datetime | None = None
    previous_ts: datetime | None = None
    previous_seen: datetime | None = None
    gap_minutes = Counter()
    rows_per_date = Counter()
    starts = Counter()
    ends = Counter()
    current_date = None
    current_start = None
    current_end = None

    output_handle = None
    writer = None
    if normalized_output:
        normalized_output.parent.mkdir(parents=True, exist_ok=True)
        output_handle = normalized_output.open("w", encoding="utf-8", newline="")
        writer = csv.writer(output_handle)
        writer.writerow(["timestamp", "open", "high", "low", "close", "volume"])

    try:
        with config.raw_path.open("r", encoding="utf-8", newline="") as raw_file:
            reader = csv.reader(raw_file)
            for raw_row in reader:
                if not raw_row:
                    continue
                try:
                    ts, open_, high, low, close, volume = parse_row(raw_row, config.timestamp_format)
                except Exception:
                    parse_errors += 1
                    continue

                rows += 1
                if first_ts is None:
                    first_ts = ts
                last_ts = ts

                if previous_seen == ts:
                    duplicate_timestamps += 1
                previous_seen = ts

                if previous_ts is not None:
                    delta = int((ts - previous_ts).total_seconds() / 60)
                    if delta != 1:
                        gap_minutes[str(delta)] += 1
                previous_ts = ts

                if high < low:
                    high_below_low += 1
                if not low <= open_ <= high:
                    open_outside_range += 1
                if not low <= close <= high:
                    close_outside_range += 1
                if volume < 0:
                    negative_volume += 1

                rows_per_date[ts.date().isoformat()] += 1
                date_key = ts.date().isoformat()
                time_key = ts.strftime("%H:%M")
                if current_date is None:
                    current_date = date_key
                    current_start = time_key
                    current_end = time_key
                elif current_date != date_key:
                    starts[str(current_start)] += 1
                    ends[str(current_end)] += 1
                    current_date = date_key
                    current_start = time_key
                    current_end = time_key
                else:
                    current_end = time_key

                if writer:
                    writer.writerow([ts.isoformat(sep=" "), open_, high, low, close, volume])
    finally:
        if current_date is not None:
            starts[str(current_start)] += 1
            ends[str(current_end)] += 1
        if output_handle:
            output_handle.close()

    counts = list(rows_per_date.values())
    profile = {
        "dataset_id": config.dataset_id,
        "raw_path": str(config.raw_path),
        "normalized_output": str(normalized_output) if normalized_output else None,
        "instrument_label": config.instrument_label,
        "instrument_family": config.instrument_family,
        "timeframe": config.timeframe,
        "session_label": config.session_label,
        "timezone": config.timezone,
        "timezone_status": config.timezone_status,
        "source_platform": config.source_platform,
        "export_method": config.export_method,
        "contract_handling": config.contract_handling,
        "notes": config.notes,
        "rows": rows,
        "parse_errors": parse_errors,
        "first_timestamp": first_ts.isoformat(sep=" ") if first_ts else None,
        "last_timestamp": last_ts.isoformat(sep=" ") if last_ts else None,
        "unique_dates": len(rows_per_date),
        "rows_per_day_min": min(counts) if counts else None,
        "rows_per_day_max": max(counts) if counts else None,
        "rows_per_day_avg": round(sum(counts) / len(counts), 2) if counts else None,
        "top_rows_per_day_counts": Counter(counts).most_common(10),
        "common_session_starts": starts.most_common(10),
        "common_session_ends": ends.most_common(10),
        "top_gap_minutes": gap_minutes.most_common(20),
        "sanity": {
            "duplicate_timestamps": duplicate_timestamps,
            "high_below_low": high_below_low,
            "open_outside_range": open_outside_range,
            "close_outside_range": close_outside_range,
            "negative_volume": negative_volume,
        },
    }
    return profile


def main() -> int:
    parser = argparse.ArgumentParser(description="Profile and normalize OHLCV CSV data.")
    parser.add_argument("--config", required=True, type=Path)
    parser.add_argument("--profile-output", required=True, type=Path)
    parser.add_argument("--normalized-output", type=Path)
    args = parser.parse_args()

    config = load_config(args.config)
    profile = profile_dataset(config, args.normalized_output)
    args.profile_output.parent.mkdir(parents=True, exist_ok=True)
    args.profile_output.write_text(json.dumps(profile, indent=2) + "\n", encoding="utf-8")
    print(json.dumps(profile, indent=2))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
