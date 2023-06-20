using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NinjaTrader.NinjaScript.Indicators;
using NinjaTrader.NinjaScript.MarketAnalyzerColumns;
using NinjaTrader.NinjaScript.Strategies;
using NinjaTrader.NinjaScript;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;
using System.Xml.Linq;

namespace NinjaTrader.Custom.Strategies
{
    public class MyStrategy : Strategy
    {
        private Series<double> vwap;
        private SMA sma14;
        private bool longPosition;
        private bool shortPosition;

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = "Example strategy for VWAP-SMA crossover";
                Name = "VWAP-SMA Crossover";
                Calculate = Calculate.OnBarClose;
                EntriesPerDirection = 1;
                EntryHandling = EntryHandling.AllEntries;
                IsExitOnSessionCloseStrategy = true;
                ExitOnSessionCloseSeconds = 30;
                BarsRequiredToTrade = 30;
            }
            else if (State == State.Configure)
            {
                AddDataSeries(Data.BarsPeriodType.Minute, 1);
                sma14 = SMA(Close, 14);
            }
            else if (State == State.DataLoaded)
            {
                vwap = new Series<double>(this);
            }
        }

        protected override void OnBarUpdate()
        {
            if (CurrentBar < BarsRequiredToTrade)
                return;

            if (CurrentBars[1] == 0)
            {
                vwap[0] = (High[0] + Low[0] + Close[0]) / 3.0;
            }
            else
            {
                double cumVolume = (Volume[0] + Volume[1]);
                vwap[0] = ((High[0] + Low[0] + Close[0]) / 3.0) * (Volume[0] / cumVolume) + vwap[1] * (cumVolume - Volume[0]) / cumVolume;
            }

            if (CrossAbove(vwap, sma14, 1))
            {
                longPosition = true;
                shortPosition = false;
            }

            if (CrossBelow(vwap, sma14, 1))
            {
                longPosition = false;
                shortPosition = true;
            }

            if (longPosition)
                EnterLong("Long Entry");

            if (shortPosition)
                EnterShort("Short Entry");
        }
    }
}

