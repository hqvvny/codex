// MNQ-004 Dual EMA Retest Example Strategy for NinjaTrader 8.
//
// Create this as a fresh NinjaScript Strategy named:
// MNQ004DualEmaRetestExampleStrategy
//
// This is a first mechanical example of the hypothesis:
// - Fast EMA and slow EMA define trend.
// - Price retests the fast EMA without closing beyond the slow EMA.
// - Entry requires a completed confirmation bar back across the fast EMA.
// - Stop is based on retest swing, capped by a maximum stop distance.
// - Target is calculated from actual stop distance and RiskReward.
//
// Parameter conventions:
// - Switches use 0 = off, 1 = on.
// - Times use HHMMSS, for example 153000.
// - DirectionMode: 0 = both, 1 = long only, 2 = short only.

using System;
using NinjaTrader.Cbi;
using NinjaTrader.NinjaScript;
using NinjaTrader.NinjaScript.Indicators;

namespace NinjaTrader.NinjaScript.Strategies
{
    public class MNQ004DualEmaRetestExampleStrategy : Strategy
    {
        private const string LongEntrySignal = "MNQ004 EMA Retest Long";
        private const string ShortEntrySignal = "MNQ004 EMA Retest Short";

        private EMA fastEma;
        private EMA slowEma;

        private bool longRetestActive;
        private bool shortRetestActive;
        private double longRetestLow;
        private double shortRetestHigh;
        private int longRetestBars;
        private int shortRetestBars;
        private int attemptsThisSession;
        private MarketPosition lastMarketPosition;

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = "MNQ-004 example dual EMA retest strategy with confirmation bar and capped swing stop.";
                Name = "MNQ004DualEmaRetestExampleStrategy";
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

                DirectionMode = 0;
                FastEmaPeriod = 21;
                SlowEmaPeriod = 200;
                EmaSlopeLookbackBars = 10;
                MinFastSlopePoints = 0.5;
                MinSlowSlopePoints = 0.5;
                MinEmaSeparationPoints = 5.0;
                RetestTolerancePoints = 5.0;
                MaxRetestBars = 20;
                SwingBufferPoints = 2.0;
                MaxStopPoints = 50.0;
                RiskReward = 2.0;
                UseFixedQuantity = 1;
                FixedQuantity = 1;
                AccountRiskDollars = 500.0;
                PointValue = 20.0;
                MaxAttemptsPerSession = 3;
                UseTradeWindow = 0;
                TradeStartTime = 0;
                TradeEndTime = 235959;
            }
            else if (State == State.DataLoaded)
            {
                fastEma = EMA(FastEmaPeriod);
                slowEma = EMA(SlowEmaPeriod);
                AddChartIndicator(fastEma);
                AddChartIndicator(slowEma);

                ResetRetests();
                attemptsThisSession = 0;
                lastMarketPosition = MarketPosition.Flat;
            }
        }

        protected override void OnBarUpdate()
        {
            int requiredBars = Math.Max(BarsRequiredToTrade, SlowEmaPeriod + EmaSlopeLookbackBars + 2);
            requiredBars = Math.Max(requiredBars, FastEmaPeriod + EmaSlopeLookbackBars + 2);

            if (CurrentBar < requiredBars)
                return;

            if (Bars.IsFirstBarOfSession)
            {
                attemptsThisSession = 0;
                ResetRetests();
            }

            TrackPositionTransition();

            if (Position.MarketPosition != MarketPosition.Flat)
            {
                ExitIfTrendInvalidated();
                return;
            }

            if (UseTradeWindow == 1 && !IsInsideWindow(ToTime(Time[0]), TradeStartTime, TradeEndTime))
                return;

            if (MaxAttemptsPerSession > 0 && attemptsThisSession >= MaxAttemptsPerSession)
                return;

            UpdateRetestState();
            TryEnterAfterConfirmation();
        }

        private void UpdateRetestState()
        {
            bool longTrend = IsLongTrendValid();
            bool shortTrend = IsShortTrendValid();

            if (!longTrend)
            {
                longRetestActive = false;
                longRetestBars = 0;
            }

            if (!shortTrend)
            {
                shortRetestActive = false;
                shortRetestBars = 0;
            }

            if (DirectionMode != 2 && longTrend && LongRetestBeginsOrContinues())
            {
                if (!longRetestActive)
                {
                    longRetestActive = true;
                    longRetestLow = Low[0];
                    longRetestBars = 1;
                }
                else
                {
                    longRetestLow = Math.Min(longRetestLow, Low[0]);
                    longRetestBars++;
                }
            }

            if (DirectionMode != 1 && shortTrend && ShortRetestBeginsOrContinues())
            {
                if (!shortRetestActive)
                {
                    shortRetestActive = true;
                    shortRetestHigh = High[0];
                    shortRetestBars = 1;
                }
                else
                {
                    shortRetestHigh = Math.Max(shortRetestHigh, High[0]);
                    shortRetestBars++;
                }
            }

            if (longRetestBars > MaxRetestBars)
                longRetestActive = false;

            if (shortRetestBars > MaxRetestBars)
                shortRetestActive = false;
        }

        private void TryEnterAfterConfirmation()
        {
            if (DirectionMode != 2 && longRetestActive && Close[0] > fastEma[0] && Close[0] > Open[0])
            {
                double entryPrice = Close[0];
                double swingStop = longRetestLow - SwingBufferPoints;
                double maxStop = entryPrice - MaxStopPoints;
                double stopPrice = Math.Max(swingStop, maxStop);
                double riskPoints = entryPrice - stopPrice;

                if (riskPoints > 0 && SubmitLong(entryPrice, riskPoints))
                {
                    longRetestActive = false;
                    attemptsThisSession++;
                }
                return;
            }

            if (DirectionMode != 1 && shortRetestActive && Close[0] < fastEma[0] && Close[0] < Open[0])
            {
                double entryPrice = Close[0];
                double swingStop = shortRetestHigh + SwingBufferPoints;
                double maxStop = entryPrice + MaxStopPoints;
                double stopPrice = Math.Min(swingStop, maxStop);
                double riskPoints = stopPrice - entryPrice;

                if (riskPoints > 0 && SubmitShort(entryPrice, riskPoints))
                {
                    shortRetestActive = false;
                    attemptsThisSession++;
                }
            }
        }

        private bool SubmitLong(double entryPrice, double riskPoints)
        {
            int quantity = CalculateQuantity(riskPoints);
            if (quantity < 1)
                return false;

            SetStopLoss(LongEntrySignal, CalculationMode.Ticks, (double)PointsToTicks(riskPoints), false);
            SetProfitTarget(LongEntrySignal, CalculationMode.Ticks, (double)PointsToTicks(riskPoints * RiskReward));
            EnterLong(quantity, LongEntrySignal);
            return true;
        }

        private bool SubmitShort(double entryPrice, double riskPoints)
        {
            int quantity = CalculateQuantity(riskPoints);
            if (quantity < 1)
                return false;

            SetStopLoss(ShortEntrySignal, CalculationMode.Ticks, (double)PointsToTicks(riskPoints), false);
            SetProfitTarget(ShortEntrySignal, CalculationMode.Ticks, (double)PointsToTicks(riskPoints * RiskReward));
            EnterShort(quantity, ShortEntrySignal);
            return true;
        }

        private int CalculateQuantity(double riskPoints)
        {
            if (UseFixedQuantity == 1)
                return Math.Max(1, FixedQuantity);

            double dollarsPerContract = riskPoints * PointValue;
            if (dollarsPerContract <= 0)
                return 0;

            int quantity = Convert.ToInt32(Math.Floor(AccountRiskDollars / dollarsPerContract));
            return Math.Max(0, quantity);
        }

        private bool IsLongTrendValid()
        {
            return fastEma[0] > slowEma[0]
                && fastEma[0] - fastEma[EmaSlopeLookbackBars] >= MinFastSlopePoints
                && slowEma[0] - slowEma[EmaSlopeLookbackBars] >= MinSlowSlopePoints
                && fastEma[0] - slowEma[0] >= MinEmaSeparationPoints;
        }

        private bool IsShortTrendValid()
        {
            return fastEma[0] < slowEma[0]
                && fastEma[EmaSlopeLookbackBars] - fastEma[0] >= MinFastSlopePoints
                && slowEma[EmaSlopeLookbackBars] - slowEma[0] >= MinSlowSlopePoints
                && slowEma[0] - fastEma[0] >= MinEmaSeparationPoints;
        }

        private bool LongRetestBeginsOrContinues()
        {
            bool touchesFastZone = Low[0] <= fastEma[0] + RetestTolerancePoints;
            bool doesNotCloseBeyondSlow = Close[0] >= slowEma[0];
            return touchesFastZone && doesNotCloseBeyondSlow;
        }

        private bool ShortRetestBeginsOrContinues()
        {
            bool touchesFastZone = High[0] >= fastEma[0] - RetestTolerancePoints;
            bool doesNotCloseBeyondSlow = Close[0] <= slowEma[0];
            return touchesFastZone && doesNotCloseBeyondSlow;
        }

        private void ExitIfTrendInvalidated()
        {
            if (Position.MarketPosition == MarketPosition.Long && !IsLongTrendValid())
                ExitLong("MNQ004 Trend Invalid Long", LongEntrySignal);

            if (Position.MarketPosition == MarketPosition.Short && !IsShortTrendValid())
                ExitShort("MNQ004 Trend Invalid Short", ShortEntrySignal);
        }

        private void TrackPositionTransition()
        {
            if (lastMarketPosition != MarketPosition.Flat && Position.MarketPosition == MarketPosition.Flat)
                ResetRetests();

            lastMarketPosition = Position.MarketPosition;
        }

        private void ResetRetests()
        {
            longRetestActive = false;
            shortRetestActive = false;
            longRetestLow = 0.0;
            shortRetestHigh = 0.0;
            longRetestBars = 0;
            shortRetestBars = 0;
        }

        private bool IsInsideWindow(int time, int startTime, int endTime)
        {
            if (startTime <= endTime)
                return time >= startTime && time <= endTime;
            return time >= startTime || time <= endTime;
        }

        private int PointsToTicks(double points)
        {
            double rawTicks = points / TickSize;
            int ticks = Convert.ToInt32(Math.Round(rawTicks));
            return Math.Max(1, ticks);
        }

        [NinjaScriptProperty]
        public int DirectionMode { get; set; }

        [NinjaScriptProperty]
        public int FastEmaPeriod { get; set; }

        [NinjaScriptProperty]
        public int SlowEmaPeriod { get; set; }

        [NinjaScriptProperty]
        public int EmaSlopeLookbackBars { get; set; }

        [NinjaScriptProperty]
        public double MinFastSlopePoints { get; set; }

        [NinjaScriptProperty]
        public double MinSlowSlopePoints { get; set; }

        [NinjaScriptProperty]
        public double MinEmaSeparationPoints { get; set; }

        [NinjaScriptProperty]
        public double RetestTolerancePoints { get; set; }

        [NinjaScriptProperty]
        public int MaxRetestBars { get; set; }

        [NinjaScriptProperty]
        public double SwingBufferPoints { get; set; }

        [NinjaScriptProperty]
        public double MaxStopPoints { get; set; }

        [NinjaScriptProperty]
        public double RiskReward { get; set; }

        [NinjaScriptProperty]
        public int UseFixedQuantity { get; set; }

        [NinjaScriptProperty]
        public int FixedQuantity { get; set; }

        [NinjaScriptProperty]
        public double AccountRiskDollars { get; set; }

        [NinjaScriptProperty]
        public double PointValue { get; set; }

        [NinjaScriptProperty]
        public int MaxAttemptsPerSession { get; set; }

        [NinjaScriptProperty]
        public int UseTradeWindow { get; set; }

        [NinjaScriptProperty]
        public int TradeStartTime { get; set; }

        [NinjaScriptProperty]
        public int TradeEndTime { get; set; }
    }
}
