from datetime import date, datetime, time
import unittest

from lib.market_data import (
    Bar,
    bars_between_times,
    bars_for_date,
    buckets_by_session_hour,
    buckets_by_session_minute,
    opening_window,
    session_stats,
    time_bucket_stats,
)


def sample_bars():
    return [
        Bar(datetime(2026, 7, 10, 15, 30), 100.0, 101.0, 99.5, 100.5, 10),
        Bar(datetime(2026, 7, 10, 15, 31), 100.5, 102.0, 100.0, 101.5, 20),
        Bar(datetime(2026, 7, 10, 15, 32), 101.5, 101.75, 100.75, 101.0, 30),
        Bar(datetime(2026, 7, 11, 15, 30), 200.0, 201.0, 199.0, 200.5, 40),
    ]


class MarketDataTest(unittest.TestCase):
    def test_bars_for_date(self):
        bars = bars_for_date(sample_bars(), date(2026, 7, 10))
        self.assertEqual(len(bars), 3)

    def test_bars_between_times(self):
        bars = bars_between_times(sample_bars(), time(15, 31), time(15, 32))
        self.assertEqual([bar.close for bar in bars], [101.5, 101.0])

    def test_session_stats(self):
        bars = bars_for_date(sample_bars(), date(2026, 7, 10))
        stats = session_stats(date(2026, 7, 10), bars)
        self.assertEqual(stats.bars, 3)
        self.assertEqual(stats.high, 102.0)
        self.assertEqual(stats.low, 99.5)
        self.assertEqual(stats.range_points, 2.5)
        self.assertEqual(stats.volume, 60)

    def test_opening_window(self):
        bars = opening_window(bars_for_date(sample_bars(), date(2026, 7, 10)), 2)
        self.assertEqual(len(bars), 2)
        self.assertEqual(bars[-1].timestamp, datetime(2026, 7, 10, 15, 31))

    def test_session_minute_buckets(self):
        session = bars_for_date(sample_bars(), date(2026, 7, 10))
        buckets = buckets_by_session_minute([session], min_bars=1)
        self.assertEqual(len(buckets["0"]), 1)
        self.assertEqual(buckets["1"][0].timestamp, datetime(2026, 7, 10, 15, 31))

    def test_session_hour_buckets(self):
        session = bars_for_date(sample_bars(), date(2026, 7, 10))
        buckets = buckets_by_session_hour([session], min_bars=1)
        self.assertEqual(len(buckets["000-059"]), 3)

    def test_time_bucket_stats(self):
        session = bars_for_date(sample_bars(), date(2026, 7, 10))
        stats = time_bucket_stats({"x": session})
        self.assertEqual(stats[0].bucket, "x")
        self.assertEqual(stats[0].observations, 3)
        self.assertEqual(stats[0].median_range_points, 1.5)


if __name__ == "__main__":
    unittest.main()
