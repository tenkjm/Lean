using QuantConnect.Brokerages;
using QuantConnect.Data;
using QuantConnect.Data.Consolidators;
using QuantConnect.Data.Market;
using QuantConnect.Indicators;
using QuantConnect.Orders.Fees;
using QuantConnect.Securities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optimization.Example
{
    public class BitfinexSuperTrend : QCAlgorithm
    {
        private Security alt;
        private SuperTrend superTrendInd;
        private BollingerBands bbInd;
        private decimal cash = 100000;

        // -- STRATEGY INPUT PARAMETERS --
        private TimeSpan consolidatorPeriod = TimeSpan.FromMinutes(278);
        private int supertrendPeriod = 7;
        private decimal supertrendMultiplier = 2.5m;
        private int bollingerPeriod = 12;
        private decimal bollingerMultiplier = 2m;

        public override void Initialize()
        {
            SetStartDate(2018, 02, 10);
            //SetEndDate(2019, 01, 01);
            SetTimeZone(TimeZones.Utc);

            SetCash(cash);
            //SetWarmUp(TimeSpan.FromDays(1));

            SetBrokerageModel(BrokerageName.Bitfinex, AccountType.Margin);
            alt = AddSecurity(SecurityType.Crypto, "BTCUSD", Resolution.Minute);
            alt.BuyingPowerModel = new SecurityMarginModel(2m);

            // consolidator
            var consolidator = new TradeBarConsolidator(consolidatorPeriod);
            SubscriptionManager.AddConsolidator(alt.Symbol, consolidator);

            // Super Trend
            superTrendInd = new SuperTrend(supertrendPeriod, supertrendMultiplier);
            RegisterIndicator(alt.Symbol, superTrendInd, consolidator);

            // Bollinger Bands
            bbInd = new BollingerBands(bollingerPeriod, bollingerMultiplier);
            RegisterIndicator(alt.Symbol, bbInd, consolidator);

            consolidator.DataConsolidated += EveryConsolidatedPeriodALT;
        }

        public void EveryConsolidatedPeriodALT(object sender, TradeBar consolidated)
        {
            var close = consolidated.Close;

            Log("," + close);

            if (!superTrendInd.IsReady)
            {
                Log("zero");
                return;
            }

            if (IsWarmingUp)
                return;

            if (close > superTrendInd)
            {
                if (Portfolio[alt.Symbol].IsShort)
                {
                    Liquidate(alt.Symbol);
                }

                if (!Portfolio[alt.Symbol].HoldStock
                    && consolidated.Close > bbInd.UpperBand
                    )
                {
                    Execution(1);
                }
            }

            if (close < superTrendInd)
            {
                if (Portfolio[alt.Symbol].IsLong)
                {
                    Liquidate(alt.Symbol);
                }

                if (!Portfolio[alt.Symbol].HoldStock
                    && consolidated.Close < bbInd.LowerBand
                    )
                {
                    Execution(-1);

                }
            }

        }

        // == EXECUTION ==
        private void Execution(int direction)
        {
            //decimal funds = Math.Min(moneyforTrade, Portfolio.TotalPortfolioValue);
            decimal funds = Portfolio.TotalPortfolioValue;
            decimal quantity = Math.Round(funds / alt.Price, 1);
            var ticket = MarketOrder(alt.Symbol, direction * quantity);
        }

        // == ON-DATA ==
        public override void OnData(Slice data)
        {
            if (IsWarmingUp || data.Ticks.Count == 0)
                return;

        }
    }
}
