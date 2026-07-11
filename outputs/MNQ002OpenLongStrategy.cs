// MNQ-002 Open Long Strategy Tester for NinjaTrader 8.
//
// Install:
// 1. NinjaTrader 8 -> New -> NinjaScript Editor.
// 2. Right-click Strategies -> New Strategy, create any placeholder.
// 3. Replace the generated file contents with this file, then compile.
//
// Best use:
// - 1 minute NQ/MNQ chart.
// - ETH/all-session data if you want the previous RTH close to be tracked
//   naturally across the overnight session.
// - RTH window set to 09:30:00-16:00:00 New York/exchange time.
// - For R:R tests, use ExitMode = BracketOnly or BracketWithTimeStop,
//   then optimize StopLossPoints and RiskReward.
//
// Caveat:
// The local Python baseline entered at the first RTH bar open. A NinjaTrader
// Strategy Analyzer run on 1 minute OHLC bars generally cannot prove the exact
// same open fill unless the order is submitted before the bar or the test uses
// more granular fill data. Use this as a Strategy Analyzer and chart-review
// version of the idea, not as live execution logic.

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;

namespace NinjaTrader.NinjaScript.Strategies
{
    public enum MNQ002FilterMode
    {
        All,
        OvernightNegativeOnly,
        OvernightNonNegativeOnly
    }

    public enum MNQ002ExitMode
    {
        TimedOnly,
        BracketOnly,
        BracketWithTimeStop
    }

    public class MNQ002OpenLongStrategy : Strategy
    {
        private const string LongSignalName = "MNQ-002 Long";

        private double previousRthClose;
        private int entryBarNumber;
        private bool hasPreviousRthClose;
        private bool activeTimedTrade;

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = "MNQ-002 baseline: long first RTH bar, optional overnight-negative filter, timed or R:R bracket exits.";
                Name = "MNQ002OpenLongStrategy";
                Calculate = Calculate.OnBarClose;
                EntriesPerDirection = 1;
                EntryHandling = EntryHandling.AllEntries;
                IsExitOnSessionCloseStrategy = false;
                ExitOnSessionCloseSeconds = 30;
                IsInstantiatedOnEachOptimizationIteration = true;
                MaximumBarsLookBack = MaximumBarsLookBack.TwoHundredFiftySix;
                OrderFillResolution = OrderFillResolution.Standard;
                Slippage = 0;
                StartBehavior = StartBehavior.WaitUntilFlat;
                TimeInForce = TimeInForce.Day;
                TraceOrders = false;
                BarsRequiredToTrade = 2;

                OrderQuantity = 1;
                HoldBars = 30;
                ExitMode = MNQ002ExitMode.BracketWithTimeStop;
                StopLossPoints = 20.0;
                RiskReward = 2.0;
                RthStartTime = 93000;
                RthEndTime = 160000;
                FilterMode = MNQ002FilterMode.OvernightNegativeOnly;
            }
            else if (State == State.DataLoaded)
            {
                previousRthClose = 0.0;
                entryBarNumber = -1;
                hasPreviousRthClose = false;
                activeTimedTrade = false;
            }
        }

        protected override void OnBarUpdate()
        {
            if (CurrentBar < BarsRequiredToTrade)
                return;

            int now = ToTime(Time[0]);
            int previous = ToTime(Time[1]);

            bool inRth = IsInsideRth(now);
            bool wasInRth = IsInsideRth(previous);
            bool isRthStart = inRth && !wasInRth;
            bool isRthEnd = !inRth && wasInRth;

            // On RTH-only charts, the first bar of the new session can be the
            // cleanest place to capture the prior RTH close.
            if (Bars.IsFirstBarOfSession && inRth && CurrentBar > 0)
            {
                previousRthClose = Close[1];
                hasPreviousRthClose = true;
            }

            // On ETH/all-session charts, update after the RTH window ends.
            if (isRthEnd)
            {
                previousRthClose = Close[1];
                hasPreviousRthClose = true;
            }

            if (isRthStart && Position.MarketPosition == MarketPosition.Flat && IsFilterAllowed())
            {
                ConfigureBracketOrders();
                EnterLong(OrderQuantity, LongSignalName);
                activeTimedTrade = true;
            }

            if (Position.MarketPosition == MarketPosition.Long && entryBarNumber < 0)
                entryBarNumber = CurrentBar;

            if (UsesTimeExit()
                && activeTimedTrade
                && Position.MarketPosition == MarketPosition.Long
                && entryBarNumber >= 0
                && CurrentBar >= entryBarNumber + HoldBars)
            {
                ExitLong("MNQ-002 Timed Exit", LongSignalName);
                activeTimedTrade = false;
                entryBarNumber = -1;
            }

            if (Position.MarketPosition == MarketPosition.Flat && entryBarNumber >= 0)
            {
                activeTimedTrade = false;
                entryBarNumber = -1;
            }
        }

        private bool IsInsideRth(int time)
        {
            return time >= RthStartTime && time <= RthEndTime;
        }

        private bool IsFilterAllowed()
        {
            if (FilterMode == MNQ002FilterMode.All)
                return true;

            if (!hasPreviousRthClose)
                return false;

            bool overnightNegative = Open[0] < previousRthClose;

            if (FilterMode == MNQ002FilterMode.OvernightNegativeOnly)
                return overnightNegative;

            return !overnightNegative;
        }

        private void ConfigureBracketOrders()
        {
            if (!UsesBracketExit())
                return;

            int stopTicks = PointsToTicks(StopLossPoints);
            int targetTicks = PointsToTicks(StopLossPoints * RiskReward);

            if (stopTicks < 1 || targetTicks < 1)
                return;

            SetStopLoss(LongSignalName, CalculationMode.Ticks, stopTicks, false);
            SetProfitTarget(LongSignalName, CalculationMode.Ticks, targetTicks);
        }

        private int PointsToTicks(double points)
        {
            return Math.Max(1, (int)Math.Round(points / TickSize, MidpointRounding.AwayFromZero));
        }

        private bool UsesBracketExit()
        {
            return ExitMode == MNQ002ExitMode.BracketOnly || ExitMode == MNQ002ExitMode.BracketWithTimeStop;
        }

        private bool UsesTimeExit()
        {
            return ExitMode == MNQ002ExitMode.TimedOnly || ExitMode == MNQ002ExitMode.BracketWithTimeStop;
        }

        [NinjaScriptProperty]
        [Range(1, int.MaxValue)]
        [Display(Name = "Order Quantity", GroupName = "Parameters", Order = 0)]
        public int OrderQuantity { get; set; }

        [NinjaScriptProperty]
        [Range(1, int.MaxValue)]
        [Display(Name = "Hold Bars", Description = "Number of bars to hold after the strategy position is detected. Used by TimedOnly and BracketWithTimeStop.", GroupName = "Parameters", Order = 1)]
        public int HoldBars { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Exit Mode", GroupName = "Parameters", Order = 2)]
        public MNQ002ExitMode ExitMode { get; set; }

        [NinjaScriptProperty]
        [Range(0.25, double.MaxValue)]
        [Display(Name = "Stop Loss Points", Description = "Stop distance in index points. Target = Stop Loss Points * Risk Reward.", GroupName = "Parameters", Order = 3)]
        public double StopLossPoints { get; set; }

        [NinjaScriptProperty]
        [Range(0.25, double.MaxValue)]
        [Display(Name = "Risk Reward", Description = "Target multiple of stop distance. Values below 1.5 are below the normal research standard.", GroupName = "Parameters", Order = 4)]
        public double RiskReward { get; set; }

        [NinjaScriptProperty]
        [Range(0, 235959)]
        [Display(Name = "RTH Start Time", Description = "HHmmss, normally 093000 for index futures RTH.", GroupName = "Parameters", Order = 5)]
        public int RthStartTime { get; set; }

        [NinjaScriptProperty]
        [Range(0, 235959)]
        [Display(Name = "RTH End Time", Description = "HHmmss, normally 160000 for index futures RTH.", GroupName = "Parameters", Order = 6)]
        public int RthEndTime { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Filter Mode", GroupName = "Parameters", Order = 7)]
        public MNQ002FilterMode FilterMode { get; set; }
    }
}
