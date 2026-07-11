// MNQ-002 Open Long R:R Strategy Tester for NinjaTrader 8.
//
// Use this as a fresh NinjaScript Strategy named MNQ002OpenLongRRStrategy.
// It avoids custom enums and Range/Display attributes to reduce NT8 compile
// friction across installations.
//
// Parameter codes:
// - FilterMode: 0 = All, 1 = OvernightNegativeOnly, 2 = OvernightNonNegativeOnly
// - ExitMode: 0 = TimedOnly, 1 = BracketOnly, 2 = BracketWithTimeStop
//
// First R:R grid:
// - FilterMode = 1
// - ExitMode = 1 or 2
// - HoldBars = 15 or 60 when ExitMode = 2
// - StopLossPoints = 10, 15, 20, 25, 30
// - RiskReward = 1.5, 2.0, 2.5

using System;
using NinjaTrader.Cbi;
using NinjaTrader.NinjaScript;

namespace NinjaTrader.NinjaScript.Strategies
{
    public class MNQ002OpenLongRRStrategy : Strategy
    {
        private const string LongSignalName = "MNQ002 Long";

        private double previousRthClose;
        private int entryBarNumber;
        private bool hasPreviousRthClose;
        private bool activeTrade;

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = "MNQ-002 open long with overnight filter and timed or R:R bracket exits.";
                Name = "MNQ002OpenLongRRStrategy";
                Calculate = Calculate.OnBarClose;
                EntriesPerDirection = 1;
                EntryHandling = EntryHandling.AllEntries;
                IsExitOnSessionCloseStrategy = false;
                IsInstantiatedOnEachOptimizationIteration = true;
                MaximumBarsLookBack = MaximumBarsLookBack.TwoHundredFiftySix;
                OrderFillResolution = OrderFillResolution.Standard;
                Slippage = 0;
                StartBehavior = StartBehavior.WaitUntilFlat;
                TimeInForce = TimeInForce.Day;
                TraceOrders = false;
                BarsRequiredToTrade = 2;

                OrderQuantity = 1;
                HoldBars = 15;
                FilterMode = 1;
                ExitMode = 2;
                StopLossPoints = 20.0;
                RiskReward = 2.0;
                RthStartTime = 153000;
                RthEndTime = 230000;
            }
            else if (State == State.DataLoaded)
            {
                previousRthClose = 0.0;
                entryBarNumber = -1;
                hasPreviousRthClose = false;
                activeTrade = false;
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

            if (Bars.IsFirstBarOfSession && inRth && CurrentBar > 0)
            {
                previousRthClose = Close[1];
                hasPreviousRthClose = true;
            }

            if (isRthEnd)
            {
                previousRthClose = Close[1];
                hasPreviousRthClose = true;
            }

            if (isRthStart && Position.MarketPosition == MarketPosition.Flat && IsFilterAllowed())
            {
                ConfigureBracketOrders();
                EnterLong(OrderQuantity, LongSignalName);
                activeTrade = true;
            }

            if (Position.MarketPosition == MarketPosition.Long && entryBarNumber < 0)
                entryBarNumber = CurrentBar;

            if (UsesTimeExit()
                && activeTrade
                && Position.MarketPosition == MarketPosition.Long
                && entryBarNumber >= 0
                && CurrentBar >= entryBarNumber + HoldBars)
            {
                ExitLong("MNQ002 Time Exit", LongSignalName);
                activeTrade = false;
                entryBarNumber = -1;
            }

            if (Position.MarketPosition == MarketPosition.Flat && entryBarNumber >= 0)
            {
                activeTrade = false;
                entryBarNumber = -1;
            }
        }

        private bool IsInsideRth(int time)
        {
            return time >= RthStartTime && time <= RthEndTime;
        }

        private bool IsFilterAllowed()
        {
            if (FilterMode == 0)
                return true;

            if (!hasPreviousRthClose)
                return false;

            bool overnightNegative = Open[0] < previousRthClose;

            if (FilterMode == 1)
                return overnightNegative;

            if (FilterMode == 2)
                return !overnightNegative;

            return false;
        }

        private void ConfigureBracketOrders()
        {
            if (!UsesBracketExit())
                return;

            int stopTicks = PointsToTicks(StopLossPoints);
            int targetTicks = PointsToTicks(StopLossPoints * RiskReward);

            SetStopLoss(LongSignalName, CalculationMode.Ticks, (double)stopTicks, false);
            SetProfitTarget(LongSignalName, CalculationMode.Ticks, (double)targetTicks);
        }

        private int PointsToTicks(double points)
        {
            double rawTicks = points / TickSize;
            int ticks = Convert.ToInt32(Math.Round(rawTicks));
            return Math.Max(1, ticks);
        }

        private bool UsesBracketExit()
        {
            return ExitMode == 1 || ExitMode == 2;
        }

        private bool UsesTimeExit()
        {
            return ExitMode == 0 || ExitMode == 2;
        }

        [NinjaScriptProperty]
        public int OrderQuantity { get; set; }

        [NinjaScriptProperty]
        public int HoldBars { get; set; }

        [NinjaScriptProperty]
        public int FilterMode { get; set; }

        [NinjaScriptProperty]
        public int ExitMode { get; set; }

        [NinjaScriptProperty]
        public double StopLossPoints { get; set; }

        [NinjaScriptProperty]
        public double RiskReward { get; set; }

        [NinjaScriptProperty]
        public int RthStartTime { get; set; }

        [NinjaScriptProperty]
        public int RthEndTime { get; set; }
    }
}
