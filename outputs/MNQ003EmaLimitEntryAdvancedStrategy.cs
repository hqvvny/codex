// MNQ-003 Advanced 200 EMA Limit Entry Strategy Tester for NinjaTrader 8.
//
// Create this as a fresh NinjaScript Strategy named:
// MNQ003EmaLimitEntryAdvancedStrategy
//
// It preserves the simple EMA-limit version's core parameters and adds
// optional filters for session windows, EMA slope, higher-timeframe trend,
// ATR regime, cooldown, trade limits, weak-hour exclusion, and time exits.
//
// Keep parameters as plain int/double/string values to avoid NinjaTrader
// enum/display attribute compile issues.

using System;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.NinjaScript.Indicators;

namespace NinjaTrader.NinjaScript.Strategies
{
    public class MNQ003EmaLimitEntryAdvancedStrategy : Strategy
    {
        private const string LongEntrySignal = "MNQ003 Adv EMA Long";
        private const string ShortEntrySignal = "MNQ003 Adv EMA Short";

        private EMA ema;
        private EMA htfEma;
        private ATR atr;

        private int entryBarNumber;
        private int lastExitBarNumber;
        private int tradesThisSession;
        private int tradesToday;
        private DateTime currentTradeDate;
        private MarketPosition lastMarketPosition;

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = "MNQ-003 advanced 200 EMA limit-entry strategy with optional regime/session filters.";
                Name = "MNQ003EmaLimitEntryAdvancedStrategy";
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
                BarsRequiredToTrade = 220;

                OrderQuantity = 1;
                EmaPeriod = 200;
                TrendBars = 35;
                EntryOffsetPoints = 0.0;
                StopLossPoints = 50.0;
                UseProfitTarget = 1;
                TargetPoints = 100.0;
                MaxHoldBars = 0;
                DirectionMode = 1;

                UseSessionWindows = 0;
                Session1StartTime = 0;
                Session1EndTime = 235959;
                UseSession2 = 0;
                Session2StartTime = 0;
                Session2EndTime = 0;
                UseSession3 = 0;
                Session3StartTime = 0;
                Session3EndTime = 0;

                UseWeakHourFilter = 0;
                WeakHoursCsv = "10,11,21";

                UseEmaSlopeFilter = 0;
                EmaSlopeLookbackBars = 20;
                MinEmaSlopePoints = 5.0;

                UseHtfTrendFilter = 0;
                HtfBarsPeriodMinutes = 15;
                HtfEmaPeriod = 200;
                HtfSlopeLookbackBars = 4;
                MinHtfSlopePoints = 0.0;

                UseAtrFilter = 0;
                AtrPeriod = 14;
                MinAtrPoints = 0.0;
                MaxAtrPoints = 9999.0;

                CooldownBars = 0;
                MaxTradesPerSession = 0;
                MaxTradesPerDay = 0;

                UseTimeExitBeforeSessionEnd = 0;
                TimeExitTime = 225500;
            }
            else if (State == State.Configure)
            {
                if (UseHtfTrendFilter == 1)
                    AddDataSeries(BarsPeriodType.Minute, Math.Max(1, HtfBarsPeriodMinutes));
            }
            else if (State == State.DataLoaded)
            {
                ema = EMA(EmaPeriod);
                atr = ATR(AtrPeriod);

                if (UseHtfTrendFilter == 1 && BarsArray.Length > 1)
                    htfEma = EMA(Closes[1], HtfEmaPeriod);

                AddChartIndicator(ema);

                entryBarNumber = -1;
                lastExitBarNumber = -1000000;
                tradesThisSession = 0;
                tradesToday = 0;
                currentTradeDate = DateTime.MinValue;
                lastMarketPosition = MarketPosition.Flat;
            }
        }

        protected override void OnBarUpdate()
        {
            if (BarsInProgress != 0)
                return;

            int requiredBars = Math.Max(BarsRequiredToTrade, EmaPeriod + TrendBars + 2);
            requiredBars = Math.Max(requiredBars, AtrPeriod + 2);
            requiredBars = Math.Max(requiredBars, EmaSlopeLookbackBars + 2);

            if (CurrentBar < requiredBars)
                return;

            if (UseHtfTrendFilter == 1)
            {
                if (BarsArray.Length < 2 || CurrentBars[1] < Math.Max(HtfEmaPeriod + HtfSlopeLookbackBars + 2, 5))
                    return;
            }

            if (Bars.IsFirstBarOfSession)
                tradesThisSession = 0;

            if (currentTradeDate != Time[0].Date)
            {
                currentTradeDate = Time[0].Date;
                tradesToday = 0;
            }

            TrackPositionTransition();

            if (UseTimeExitBeforeSessionEnd == 1 && Position.MarketPosition != MarketPosition.Flat && ToTime(Time[0]) >= TimeExitTime)
            {
                ExitOpenPosition("MNQ003 Time Exit");
                return;
            }

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

            if (!CanSubmitNewEntry())
                return;

            ConfigureRiskOrders();

            double emaPrice = ema[0];
            if (DirectionMode != 2 && LongSetupAllowed(emaPrice))
            {
                double limitPrice = emaPrice + EntryOffsetPoints;
                EnterLongLimit(OrderQuantity, limitPrice, LongEntrySignal);
                return;
            }

            if (DirectionMode != 1 && ShortSetupAllowed(emaPrice))
            {
                double limitPrice = emaPrice - EntryOffsetPoints;
                EnterShortLimit(OrderQuantity, limitPrice, ShortEntrySignal);
            }
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
                tradesToday++;
                entryBarNumber = CurrentBar;
            }
        }

        private bool LongSetupAllowed(double emaPrice)
        {
            if (Close[0] <= emaPrice)
                return false;
            if (!TrendConfirmed(1))
                return false;
            if (!EmaSlopeAllowed(1))
                return false;
            if (!HtfTrendAllowed(1))
                return false;
            if (!AtrAllowed())
                return false;
            return true;
        }

        private bool ShortSetupAllowed(double emaPrice)
        {
            if (Close[0] >= emaPrice)
                return false;
            if (!TrendConfirmed(-1))
                return false;
            if (!EmaSlopeAllowed(-1))
                return false;
            if (!HtfTrendAllowed(-1))
                return false;
            if (!AtrAllowed())
                return false;
            return true;
        }

        private bool CanSubmitNewEntry()
        {
            int now = ToTime(Time[0]);

            if (UseSessionWindows == 1 && !IsInsideAnySessionWindow(now))
                return false;

            if (UseWeakHourFilter == 1 && IsWeakHour(Time[0].Hour))
                return false;

            if (CooldownBars > 0 && CurrentBar < lastExitBarNumber + CooldownBars)
                return false;

            if (MaxTradesPerSession > 0 && tradesThisSession >= MaxTradesPerSession)
                return false;

            if (MaxTradesPerDay > 0 && tradesToday >= MaxTradesPerDay)
                return false;

            return true;
        }

        private void ConfigureRiskOrders()
        {
            int stopTicks = PointsToTicks(StopLossPoints);
            SetStopLoss(LongEntrySignal, CalculationMode.Ticks, (double)stopTicks, false);
            SetStopLoss(ShortEntrySignal, CalculationMode.Ticks, (double)stopTicks, false);

            if (UseProfitTarget == 1)
            {
                int targetTicks = PointsToTicks(TargetPoints);
                SetProfitTarget(LongEntrySignal, CalculationMode.Ticks, (double)targetTicks);
                SetProfitTarget(ShortEntrySignal, CalculationMode.Ticks, (double)targetTicks);
            }
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

        private bool HtfTrendAllowed(int direction)
        {
            if (UseHtfTrendFilter != 1)
                return true;

            if (htfEma == null)
                return false;

            double htfClose = Closes[1][0];
            double htfEmaNow = htfEma[0];
            double htfSlope = htfEma[0] - htfEma[HtfSlopeLookbackBars];

            if (direction == 1)
                return htfClose > htfEmaNow && htfSlope >= MinHtfSlopePoints;
            return htfClose < htfEmaNow && htfSlope <= -MinHtfSlopePoints;
        }

        private bool AtrAllowed()
        {
            if (UseAtrFilter != 1)
                return true;

            double atrPoints = atr[0];
            return atrPoints >= MinAtrPoints && atrPoints <= MaxAtrPoints;
        }

        private bool IsInsideAnySessionWindow(int time)
        {
            if (IsInsideWindow(time, Session1StartTime, Session1EndTime))
                return true;
            if (UseSession2 == 1 && IsInsideWindow(time, Session2StartTime, Session2EndTime))
                return true;
            if (UseSession3 == 1 && IsInsideWindow(time, Session3StartTime, Session3EndTime))
                return true;
            return false;
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
        public int EmaPeriod { get; set; }

        [NinjaScriptProperty]
        public int TrendBars { get; set; }

        [NinjaScriptProperty]
        public double EntryOffsetPoints { get; set; }

        [NinjaScriptProperty]
        public double StopLossPoints { get; set; }

        [NinjaScriptProperty]
        public int UseProfitTarget { get; set; }

        [NinjaScriptProperty]
        public double TargetPoints { get; set; }

        [NinjaScriptProperty]
        public int MaxHoldBars { get; set; }

        [NinjaScriptProperty]
        public int DirectionMode { get; set; }

        [NinjaScriptProperty]
        public int UseSessionWindows { get; set; }

        [NinjaScriptProperty]
        public int Session1StartTime { get; set; }

        [NinjaScriptProperty]
        public int Session1EndTime { get; set; }

        [NinjaScriptProperty]
        public int UseSession2 { get; set; }

        [NinjaScriptProperty]
        public int Session2StartTime { get; set; }

        [NinjaScriptProperty]
        public int Session2EndTime { get; set; }

        [NinjaScriptProperty]
        public int UseSession3 { get; set; }

        [NinjaScriptProperty]
        public int Session3StartTime { get; set; }

        [NinjaScriptProperty]
        public int Session3EndTime { get; set; }

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
        public int UseHtfTrendFilter { get; set; }

        [NinjaScriptProperty]
        public int HtfBarsPeriodMinutes { get; set; }

        [NinjaScriptProperty]
        public int HtfEmaPeriod { get; set; }

        [NinjaScriptProperty]
        public int HtfSlopeLookbackBars { get; set; }

        [NinjaScriptProperty]
        public double MinHtfSlopePoints { get; set; }

        [NinjaScriptProperty]
        public int UseAtrFilter { get; set; }

        [NinjaScriptProperty]
        public int AtrPeriod { get; set; }

        [NinjaScriptProperty]
        public double MinAtrPoints { get; set; }

        [NinjaScriptProperty]
        public double MaxAtrPoints { get; set; }

        [NinjaScriptProperty]
        public int CooldownBars { get; set; }

        [NinjaScriptProperty]
        public int MaxTradesPerSession { get; set; }

        [NinjaScriptProperty]
        public int MaxTradesPerDay { get; set; }

        [NinjaScriptProperty]
        public int UseTimeExitBeforeSessionEnd { get; set; }

        [NinjaScriptProperty]
        public int TimeExitTime { get; set; }
    }
}
