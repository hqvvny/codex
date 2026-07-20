// MNQ-003 200 EMA Limit Entry Strategy Tester for NinjaTrader 8.
//
// Use this as a fresh NinjaScript Strategy named MNQ003EmaLimitEntryStrategy.
// This version enters directly at the 200 EMA using limit orders.
//
// Rule:
// - Long: trend is above the 200 EMA, place a buy limit at the EMA.
// - Short: trend is below the 200 EMA, place a sell short limit at the EMA.
// - Stop: fixed StopLossPoints from entry, default 50 points.
// - Target: optional fixed TargetPoints from entry, default enabled at 50 points
//   so Strategy Analyzer trades close cleanly. Set UseProfitTarget = 0 to disable.
//
// DirectionMode:
// - 0 = both long and short
// - 1 = long only
// - 2 = short only

using System;
using NinjaTrader.Cbi;
using NinjaTrader.NinjaScript;
using NinjaTrader.NinjaScript.Indicators;

namespace NinjaTrader.NinjaScript.Strategies
{
    public class MNQ003EmaLimitEntryStrategy : Strategy
    {
        private const string LongEntrySignal = "MNQ003 EMA Long";
        private const string ShortEntrySignal = "MNQ003 EMA Short";

        private EMA ema;
        private int entryBarNumber;

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = "MNQ-003 200 EMA limit-entry retest strategy with fixed 50-point stop.";
                Name = "MNQ003EmaLimitEntryStrategy";
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
                TrendBars = 10;
                EntryOffsetPoints = 0.0;
                StopLossPoints = 50.0;
                UseProfitTarget = 1;
                TargetPoints = 50.0;
                MaxHoldBars = 0;
                DirectionMode = 0;
                TradeStartTime = 0;
                TradeEndTime = 235959;
            }
            else if (State == State.DataLoaded)
            {
                ema = EMA(EmaPeriod);
                AddChartIndicator(ema);
                entryBarNumber = -1;
            }
        }

        protected override void OnBarUpdate()
        {
            if (CurrentBar < Math.Max(BarsRequiredToTrade, EmaPeriod + TrendBars + 2))
                return;

            if (!IsInsideTradeWindow(ToTime(Time[0])))
                return;

            if (Position.MarketPosition == MarketPosition.Long && entryBarNumber < 0)
                entryBarNumber = CurrentBar;

            if (Position.MarketPosition == MarketPosition.Short && entryBarNumber < 0)
                entryBarNumber = CurrentBar;

            if (MaxHoldBars > 0
                && Position.MarketPosition == MarketPosition.Long
                && entryBarNumber >= 0
                && CurrentBar >= entryBarNumber + MaxHoldBars)
            {
                ExitLong("MNQ003 Max Hold Long", LongEntrySignal);
                entryBarNumber = -1;
                return;
            }

            if (MaxHoldBars > 0
                && Position.MarketPosition == MarketPosition.Short
                && entryBarNumber >= 0
                && CurrentBar >= entryBarNumber + MaxHoldBars)
            {
                ExitShort("MNQ003 Max Hold Short", ShortEntrySignal);
                entryBarNumber = -1;
                return;
            }

            if (Position.MarketPosition == MarketPosition.Flat)
                entryBarNumber = -1;

            if (Position.MarketPosition != MarketPosition.Flat)
                return;

            ConfigureRiskOrders();

            double emaPrice = ema[0];
            if (DirectionMode != 2 && TrendConfirmed(1) && Close[0] > emaPrice)
            {
                double limitPrice = emaPrice + EntryOffsetPoints;
                EnterLongLimit(OrderQuantity, limitPrice, LongEntrySignal);
                return;
            }

            if (DirectionMode != 1 && TrendConfirmed(-1) && Close[0] < emaPrice)
            {
                double limitPrice = emaPrice - EntryOffsetPoints;
                EnterShortLimit(OrderQuantity, limitPrice, ShortEntrySignal);
            }
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

        private bool IsInsideTradeWindow(int time)
        {
            if (TradeStartTime <= TradeEndTime)
                return time >= TradeStartTime && time <= TradeEndTime;
            return time >= TradeStartTime || time <= TradeEndTime;
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
        public int TradeStartTime { get; set; }

        [NinjaScriptProperty]
        public int TradeEndTime { get; set; }
    }
}
