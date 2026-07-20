// MNQ-003 Clean 200 EMA Limit Entry Strategy Tester for NinjaTrader 8.
//
// Create this as a fresh NinjaScript Strategy named:
// MNQ003EmaLimitEntryCleanStrategy
//
// This is the no-overload version. It keeps the proven 200 EMA limit-entry
// baseline and only exposes the filters that are useful for the next tests.
//
// Parameter conventions:
// - Switches use 0 = off, 1 = on.
// - Times use HHMMSS, for example 153000 for 15:30:00.
// - DirectionMode: 0 = both, 1 = long only, 2 = short only.

using System;
using NinjaTrader.Cbi;
using NinjaTrader.NinjaScript;
using NinjaTrader.NinjaScript.Indicators;

namespace NinjaTrader.NinjaScript.Strategies
{
    public class MNQ003EmaLimitEntryCleanStrategy : Strategy
    {
        private const string LongEntrySignal = "MNQ003 Clean EMA Long";
        private const string ShortEntrySignal = "MNQ003 Clean EMA Short";

        private EMA ema;
        private int entryBarNumber;
        private int lastExitBarNumber;
        private int tradesThisSession;
        private MarketPosition lastMarketPosition;

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = "MNQ-003 clean 200 EMA limit-entry strategy with a small set of practical filters.";
                Name = "MNQ003EmaLimitEntryCleanStrategy";
                Calculate = Calculate.OnBarClose;
                EntriesPerDirection = 1;
                EntryHandling = EntryHandling.AllEntries;
                IsExitOnSessionCloseStrategy = true;
                ExitOnSessionCloseSeconds = 30;
                IsInstantiatedOnEachOptimizationIteration = true;
                MaximumBarsLookBack = MaximumBarsLookBack.TwoHundredFiftySix;
                OrderFillResolution = OrderFillResolution.Standard;
                Slippage = 0;
                StartBehavior = StartBehavior.WaitUntilFlat;
                TimeInForce = TimeInForce.Day;
                TraceOrders = false;
                BarsRequiredToTrade = 240;

                OrderQuantity = 1;
                DirectionMode = 1;
                EmaPeriod = 200;
                TrendBars = 35;
                EntryOffsetPoints = 0.0;
                StopLossPoints = 50.0;
                TargetPoints = 100.0;
                MaxHoldBars = 0;

                UseTradeWindow = 0;
                TradeStartTime = 0;
                TradeEndTime = 235959;

                UseWeakHourFilter = 0;
                WeakHoursCsv = "10,11,21,2";

                UseEmaSlopeFilter = 0;
                EmaSlopeLookbackBars = 20;
                MinEmaSlopePoints = 5.0;

                CooldownBars = 0;
                MaxTradesPerSession = 0;
            }
            else if (State == State.DataLoaded)
            {
                ema = EMA(EmaPeriod);
                AddChartIndicator(ema);

                entryBarNumber = -1;
                lastExitBarNumber = -1000000;
                tradesThisSession = 0;
                lastMarketPosition = MarketPosition.Flat;
            }
        }

        protected override void OnBarUpdate()
        {
            int requiredBars = Math.Max(BarsRequiredToTrade, EmaPeriod + TrendBars + 2);
            requiredBars = Math.Max(requiredBars, EmaSlopeLookbackBars + 2);

            if (CurrentBar < requiredBars)
                return;

            if (Bars.IsFirstBarOfSession)
                tradesThisSession = 0;

            TrackPositionTransition();

            if (Position.MarketPosition == MarketPosition.Long && entryBarNumber < 0)
                entryBarNumber = CurrentBar;

            if (Position.MarketPosition == MarketPosition.Short && entryBarNumber < 0)
                entryBarNumber = CurrentBar;

            if (MaxHoldBars > 0
                && Position.MarketPosition != MarketPosition.Flat
                && entryBarNumber >= 0
                && CurrentBar >= entryBarNumber + MaxHoldBars)
            {
                ExitOpenPosition("MNQ003 Max Hold");
                return;
            }

            if (Position.MarketPosition == MarketPosition.Flat)
                entryBarNumber = -1;

            if (Position.MarketPosition != MarketPosition.Flat)
                return;

            if (!CanSubmitEntry())
                return;

            ConfigureRiskOrders();

            double emaPrice = ema[0];

            if (DirectionMode != 2 && LongSetupAllowed(emaPrice))
            {
                EnterLongLimit(OrderQuantity, emaPrice + EntryOffsetPoints, LongEntrySignal);
                return;
            }

            if (DirectionMode != 1 && ShortSetupAllowed(emaPrice))
                EnterShortLimit(OrderQuantity, emaPrice - EntryOffsetPoints, ShortEntrySignal);
        }

        protected override void OnExecutionUpdate(Execution execution, string executionId, double price, int quantity,
            MarketPosition marketPosition, string orderId, DateTime time)
        {
            if (execution == null || execution.Order == null)
                return;

            if (execution.Order.OrderState != OrderState.Filled && execution.Order.OrderState != OrderState.PartFilled)
                return;

            if (execution.Order.Name == LongEntrySignal || execution.Order.Name == ShortEntrySignal)
            {
                tradesThisSession++;
                entryBarNumber = CurrentBar;
            }
        }

        private bool LongSetupAllowed(double emaPrice)
        {
            return Close[0] > emaPrice
                && TrendConfirmed(1)
                && EmaSlopeAllowed(1);
        }

        private bool ShortSetupAllowed(double emaPrice)
        {
            return Close[0] < emaPrice
                && TrendConfirmed(-1)
                && EmaSlopeAllowed(-1);
        }

        private bool CanSubmitEntry()
        {
            int now = ToTime(Time[0]);

            if (UseTradeWindow == 1 && !IsInsideWindow(now, TradeStartTime, TradeEndTime))
                return false;

            if (UseWeakHourFilter == 1 && IsWeakHour(Time[0].Hour))
                return false;

            if (CooldownBars > 0 && CurrentBar < lastExitBarNumber + CooldownBars)
                return false;

            if (MaxTradesPerSession > 0 && tradesThisSession >= MaxTradesPerSession)
                return false;

            return true;
        }

        private void ConfigureRiskOrders()
        {
            int stopTicks = PointsToTicks(StopLossPoints);
            int targetTicks = PointsToTicks(TargetPoints);

            SetStopLoss(LongEntrySignal, CalculationMode.Ticks, (double)stopTicks, false);
            SetStopLoss(ShortEntrySignal, CalculationMode.Ticks, (double)stopTicks, false);
            SetProfitTarget(LongEntrySignal, CalculationMode.Ticks, (double)targetTicks);
            SetProfitTarget(ShortEntrySignal, CalculationMode.Ticks, (double)targetTicks);
        }

        private bool TrendConfirmed(int direction)
        {
            for (int barsAgo = 1; barsAgo <= TrendBars; barsAgo++)
            {
                if (direction == 1 && Close[barsAgo] <= ema[barsAgo])
                    return false;
                if (direction == -1 && Close[barsAgo] >= ema[barsAgo])
                    return false;
            }
            return true;
        }

        private bool EmaSlopeAllowed(int direction)
        {
            if (UseEmaSlopeFilter != 1)
                return true;

            double slopePoints = ema[0] - ema[EmaSlopeLookbackBars];
            if (direction == 1)
                return slopePoints >= MinEmaSlopePoints;
            return slopePoints <= -MinEmaSlopePoints;
        }

        private bool IsInsideWindow(int time, int startTime, int endTime)
        {
            if (startTime <= endTime)
                return time >= startTime && time <= endTime;
            return time >= startTime || time <= endTime;
        }

        private bool IsWeakHour(int hour)
        {
            if (string.IsNullOrEmpty(WeakHoursCsv))
                return false;

            string[] parts = WeakHoursCsv.Split(',');
            for (int i = 0; i < parts.Length; i++)
            {
                int parsedHour;
                if (int.TryParse(parts[i].Trim(), out parsedHour) && parsedHour == hour)
                    return true;
            }
            return false;
        }

        private void TrackPositionTransition()
        {
            if (lastMarketPosition != MarketPosition.Flat && Position.MarketPosition == MarketPosition.Flat)
            {
                lastExitBarNumber = CurrentBar;
                entryBarNumber = -1;
            }

            lastMarketPosition = Position.MarketPosition;
        }

        private void ExitOpenPosition(string reason)
        {
            if (Position.MarketPosition == MarketPosition.Long)
                ExitLong(reason + " Long", LongEntrySignal);
            else if (Position.MarketPosition == MarketPosition.Short)
                ExitShort(reason + " Short", ShortEntrySignal);

            lastExitBarNumber = CurrentBar;
            entryBarNumber = -1;
        }

        private int PointsToTicks(double points)
        {
            double rawTicks = points / TickSize;
            int ticks = Convert.ToInt32(Math.Round(rawTicks));
            return Math.Max(1, ticks);
        }

        [NinjaScriptProperty]
        public int OrderQuantity { get; set; }

        [NinjaScriptProperty]
        public int DirectionMode { get; set; }

        [NinjaScriptProperty]
        public int EmaPeriod { get; set; }

        [NinjaScriptProperty]
        public int TrendBars { get; set; }

        [NinjaScriptProperty]
        public double EntryOffsetPoints { get; set; }

        [NinjaScriptProperty]
        public double StopLossPoints { get; set; }

        [NinjaScriptProperty]
        public double TargetPoints { get; set; }

        [NinjaScriptProperty]
        public int MaxHoldBars { get; set; }

        [NinjaScriptProperty]
        public int UseTradeWindow { get; set; }

        [NinjaScriptProperty]
        public int TradeStartTime { get; set; }

        [NinjaScriptProperty]
        public int TradeEndTime { get; set; }

        [NinjaScriptProperty]
        public int UseWeakHourFilter { get; set; }

        [NinjaScriptProperty]
        public string WeakHoursCsv { get; set; }

        [NinjaScriptProperty]
        public int UseEmaSlopeFilter { get; set; }

        [NinjaScriptProperty]
        public int EmaSlopeLookbackBars { get; set; }

        [NinjaScriptProperty]
        public double MinEmaSlopePoints { get; set; }

        [NinjaScriptProperty]
        public int CooldownBars { get; set; }

        [NinjaScriptProperty]
        public int MaxTradesPerSession { get; set; }
    }
}
