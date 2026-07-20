// MNQ-003 200 EMA Retest Strategy Tester for NinjaTrader 8.
//
// Use this as a fresh NinjaScript Strategy named MNQ003EmaRetestStrategy.
// It is intentionally simple: numeric mode parameters, no custom enums, no
// Display/Range attributes.
//
// Rule:
// - 1m 200 EMA.
// - Long: prior trend closes above EMA, signal candle retests EMA from above
//   within MaxRetestPoints, closes back above EMA.
// - Short: prior trend closes below EMA, signal candle retests EMA from below
//   within MaxRetestPoints, closes back below EMA.
// - Entry: signal on close; NinjaTrader historical fill is normally next bar.
// - Stop: signal candle extreme plus StopBufferPoints.
// - Target: 1R by default, calculated from actual entry fill price.
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
    public class MNQ003EmaRetestStrategy : Strategy
    {
        private const string LongEntrySignal = "MNQ003 Long";
        private const string ShortEntrySignal = "MNQ003 Short";

        private EMA ema;
        private double pendingStopPrice;
        private int pendingDirection;
        private bool waitingForEntryFill;

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = "MNQ-003 1m 200 EMA retest continuation with candle-extreme stop and 1R target.";
                Name = "MNQ003EmaRetestStrategy";
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
                BarsRequiredToTrade = 220;

                OrderQuantity = 1;
                EmaPeriod = 200;
                MaxRetestPoints = 10.0;
                TrendBars = 10;
                StopBufferPoints = 2.0;
                RiskReward = 1.0;
                MinRiskPoints = 0.25;
                MaxRiskPoints = 0.0;
                DirectionMode = 0;
                TradeStartTime = 0;
                TradeEndTime = 235959;
            }
            else if (State == State.DataLoaded)
            {
                ema = EMA(EmaPeriod);
                AddChartIndicator(ema);
                pendingStopPrice = 0.0;
                pendingDirection = 0;
                waitingForEntryFill = false;
            }
        }

        protected override void OnBarUpdate()
        {
            if (CurrentBar < Math.Max(BarsRequiredToTrade, EmaPeriod + TrendBars + 2))
                return;

            if (!IsInsideTradeWindow(ToTime(Time[0])))
                return;

            if (Position.MarketPosition != MarketPosition.Flat || waitingForEntryFill)
                return;

            if (DirectionMode != 2 && IsLongSignal())
            {
                double stopPrice = Low[0] - StopBufferPoints;
                double assumedRisk = Close[0] - stopPrice;
                if (!RiskAllowed(assumedRisk))
                    return;

                pendingStopPrice = stopPrice;
                pendingDirection = 1;
                waitingForEntryFill = true;
                EnterLong(OrderQuantity, LongEntrySignal);
                return;
            }

            if (DirectionMode != 1 && IsShortSignal())
            {
                double stopPrice = High[0] + StopBufferPoints;
                double assumedRisk = stopPrice - Close[0];
                if (!RiskAllowed(assumedRisk))
                    return;

                pendingStopPrice = stopPrice;
                pendingDirection = -1;
                waitingForEntryFill = true;
                EnterShort(OrderQuantity, ShortEntrySignal);
            }
        }

        protected override void OnExecutionUpdate(
            Execution execution,
            string executionId,
            double price,
            int quantity,
            MarketPosition marketPosition,
            string orderId,
            DateTime time)
        {
            if (execution == null || execution.Order == null || execution.Order.OrderState != OrderState.Filled)
                return;

            if (execution.Order.Name == LongEntrySignal && pendingDirection == 1)
            {
                double riskPoints = price - pendingStopPrice;
                if (!RiskAllowed(riskPoints))
                {
                    ExitLong("MNQ003 Invalid Risk Exit", LongEntrySignal);
                    ResetPending();
                    return;
                }

                double targetPrice = price + riskPoints * RiskReward;
                ExitLongStopMarket(0, true, quantity, pendingStopPrice, "MNQ003 Long Stop", LongEntrySignal);
                ExitLongLimit(0, true, quantity, targetPrice, "MNQ003 Long Target", LongEntrySignal);
                ResetPending();
                return;
            }

            if (execution.Order.Name == ShortEntrySignal && pendingDirection == -1)
            {
                double riskPoints = pendingStopPrice - price;
                if (!RiskAllowed(riskPoints))
                {
                    ExitShort("MNQ003 Invalid Risk Exit", ShortEntrySignal);
                    ResetPending();
                    return;
                }

                double targetPrice = price - riskPoints * RiskReward;
                ExitShortStopMarket(0, true, quantity, pendingStopPrice, "MNQ003 Short Stop", ShortEntrySignal);
                ExitShortLimit(0, true, quantity, targetPrice, "MNQ003 Short Target", ShortEntrySignal);
                ResetPending();
            }
        }

        private bool IsLongSignal()
        {
            if (!TrendConfirmed(1))
                return false;

            double value = ema[0];
            bool touched = Low[0] <= value + MaxRetestPoints;
            bool notTooDeep = Low[0] >= value - MaxRetestPoints;
            bool rejected = Close[0] > value;
            return touched && notTooDeep && rejected;
        }

        private bool IsShortSignal()
        {
            if (!TrendConfirmed(-1))
                return false;

            double value = ema[0];
            bool touched = High[0] >= value - MaxRetestPoints;
            bool notTooDeep = High[0] <= value + MaxRetestPoints;
            bool rejected = Close[0] < value;
            return touched && notTooDeep && rejected;
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

        private bool RiskAllowed(double riskPoints)
        {
            if (riskPoints < MinRiskPoints)
                return false;
            if (MaxRiskPoints > 0 && riskPoints > MaxRiskPoints)
                return false;
            return true;
        }

        private bool IsInsideTradeWindow(int time)
        {
            if (TradeStartTime <= TradeEndTime)
                return time >= TradeStartTime && time <= TradeEndTime;
            return time >= TradeStartTime || time <= TradeEndTime;
        }

        private void ResetPending()
        {
            pendingStopPrice = 0.0;
            pendingDirection = 0;
            waitingForEntryFill = false;
        }

        [NinjaScriptProperty]
        public int OrderQuantity { get; set; }

        [NinjaScriptProperty]
        public int EmaPeriod { get; set; }

        [NinjaScriptProperty]
        public double MaxRetestPoints { get; set; }

        [NinjaScriptProperty]
        public int TrendBars { get; set; }

        [NinjaScriptProperty]
        public double StopBufferPoints { get; set; }

        [NinjaScriptProperty]
        public double RiskReward { get; set; }

        [NinjaScriptProperty]
        public double MinRiskPoints { get; set; }

        [NinjaScriptProperty]
        public double MaxRiskPoints { get; set; }

        [NinjaScriptProperty]
        public int DirectionMode { get; set; }

        [NinjaScriptProperty]
        public int TradeStartTime { get; set; }

        [NinjaScriptProperty]
        public int TradeEndTime { get; set; }
    }
}
