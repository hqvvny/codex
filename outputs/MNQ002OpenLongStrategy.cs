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
//
// Caveat:
// The local Python baseline entered at the first RTH bar open. A NinjaTrader
// Strategy Analyzer run on 1 minute OHLC bars generally cannot prove the exact
// same open fill unless the order is submitted before the bar or the test uses
// more granular fill data. Use this as a Strategy Analyzer and chart-review
// version of the idea, not as live execution logic.

#region Using declarations
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
#endregion

namespace NinjaTrader.NinjaScript.Strategies
{
    public enum MNQ002FilterMode
    {
        All,
        OvernightNegativeOnly,
        OvernightNonNegativeOnly
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
                Description = "MNQ-002 baseline: long first RTH bar, exit after N bars, optional overnight-negative filter.";
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
                EnterLong(OrderQuantity, LongSignalName);
                activeTimedTrade = true;
            }

            if (Position.MarketPosition == MarketPosition.Long && entryBarNumber < 0)
                entryBarNumber = CurrentBar;

            if (activeTimedTrade
                && Position.MarketPosition == MarketPosition.Long
                && entryBarNumber >= 0
                && CurrentBar >= entryBarNumber + HoldBars)
            {
                ExitLong("MNQ-002 Timed Exit", LongSignalName);
                activeTimedTrade = false;
                entryBarNumber = -1;
            }

            if (Position.MarketPosition == MarketPosition.Flat && !activeTimedTrade)
                entryBarNumber = -1;
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

        #region Properties
        [NinjaScriptProperty]
        [Range(1, int.MaxValue)]
        [Display(Name = "Order Quantity", GroupName = "Parameters", Order = 0)]
        public int OrderQuantity { get; set; }

        [NinjaScriptProperty]
        [Range(1, int.MaxValue)]
        [Display(Name = "Hold Bars", Description = "Number of bars to hold after the strategy position is detected.", GroupName = "Parameters", Order = 1)]
        public int HoldBars { get; set; }

        [NinjaScriptProperty]
        [Range(0, 235959)]
        [Display(Name = "RTH Start Time", Description = "HHmmss, normally 093000 for index futures RTH.", GroupName = "Parameters", Order = 2)]
        public int RthStartTime { get; set; }

        [NinjaScriptProperty]
        [Range(0, 235959)]
        [Display(Name = "RTH End Time", Description = "HHmmss, normally 160000 for index futures RTH.", GroupName = "Parameters", Order = 3)]
        public int RthEndTime { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Filter Mode", GroupName = "Parameters", Order = 4)]
        public MNQ002FilterMode FilterMode { get; set; }
        #endregion
    }
}
