"""Small stdlib helpers for normalized OHLCV CSV data."""

from __future__ import annotations

import csv
from dataclasses import dataclass
from datetime import date, datetime, timedelta, time
from pathlib import Path
from typing import Iterable, Iterator


@dataclass(frozen=True)
class Bar:
    timestamp: datetime
    open: float
    high: float
    low: float
    close: float
    volume: int


@dataclass(frozen=True)
class SessionStats:
    session_date: date
    start: datetime
    end: datetime
    bars: int
    open: float
    high: float
    low: float
    close: float
    volume: int
    range_points: float
    close_to_open_points: float


def load_bars(path: Path | str) -> list[Bar]:
    """Load normalized OHLCV bars sorted by timestamp.

    Expected normalized format:
    timestamp,open,high,low,close,volume
    """

    bars: list[Bar] = []
    with Path(path).open("r", encoding="utf-8", newline="") as handle:
        reader = csv.DictReader(handle)
        required = {"timestamp", "open", "high", "low", "close", "volume"}
        if set(reader.fieldnames or []) != required:
            missing = required - set(reader.fieldnames or [])
            extra = set(reader.fieldnames or []) - required
            raise ValueError(f"unexpected columns: missing={sorted(missing)} extra={sorted(extra)}")
        for row in reader:
            bars.append(
                Bar(
                    timestamp=datetime.fromisoformat(row["timestamp"]),
                    open=float(row["open"]),
                    high=float(row["high"]),
                    low=float(row["low"]),
                    close=float(row["close"]),
                    volume=int(float(row["volume"])),
                )
            )
    bars.sort(key=lambda bar: bar.timestamp)
    return bars


def iter_bars(path: Path | str) -> Iterator[Bar]:
    """Stream normalized bars without materializing the whole file."""

    with Path(path).open("r", encoding="utf-8", newline="") as handle:
        reader = csv.DictReader(handle)
        for row in reader:
            yield Bar(
                timestamp=datetime.fromisoformat(row["timestamp"]),
                open=float(row["open"]),
                high=float(row["high"]),
                low=float(row["low"]),
                close=float(row["close"]),
                volume=int(float(row["volume"])),
            )


def bars_for_date(bars: Iterable[Bar], session_date: date) -> list[Bar]:
    return [bar for bar in bars if bar.timestamp.date() == session_date]


def bars_between_times(bars: Iterable[Bar], start: time, end: time) -> list[Bar]:
    if start <= end:
        return [bar for bar in bars if start <= bar.timestamp.time() <= end]
    return [bar for bar in bars if bar.timestamp.time() >= start or bar.timestamp.time() <= end]


def bars_between_datetimes(bars: Iterable[Bar], start: datetime, end: datetime) -> list[Bar]:
    return [bar for bar in bars if start <= bar.timestamp <= end]


def group_by_date(bars: Iterable[Bar]) -> dict[date, list[Bar]]:
    grouped: dict[date, list[Bar]] = {}
    for bar in bars:
        grouped.setdefault(bar.timestamp.date(), []).append(bar)
    return grouped


def session_stats(session_date: date, bars: list[Bar]) -> SessionStats:
    if not bars:
        raise ValueError("cannot compute stats for empty session")
    ordered = sorted(bars, key=lambda bar: bar.timestamp)
    high = max(bar.high for bar in ordered)
    low = min(bar.low for bar in ordered)
    volume = sum(bar.volume for bar in ordered)
    return SessionStats(
        session_date=session_date,
        start=ordered[0].timestamp,
        end=ordered[-1].timestamp,
        bars=len(ordered),
        open=ordered[0].open,
        high=high,
        low=low,
        close=ordered[-1].close,
        volume=volume,
        range_points=high - low,
        close_to_open_points=ordered[-1].close - ordered[0].open,
    )


def summarize_sessions(bars: Iterable[Bar]) -> list[SessionStats]:
    return [session_stats(session_date, day_bars) for session_date, day_bars in sorted(group_by_date(bars).items())]


def opening_window(bars: Iterable[Bar], minutes: int) -> list[Bar]:
    ordered = sorted(bars, key=lambda bar: bar.timestamp)
    if not ordered:
        return []
    start = ordered[0].timestamp
    cutoff = start + timedelta(minutes=minutes - 1)
    return [bar for bar in ordered if start <= bar.timestamp <= cutoff]
