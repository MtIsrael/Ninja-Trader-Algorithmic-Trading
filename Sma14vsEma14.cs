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
    public class SMA14vsEMA14 : Strategy
    {
        private SMA sma14;
        private EMA ema14;
        private bool longPosition;
        private bool shortPosition;

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = "Example strategy for SMA-EMA crossover";
                Name = "SMA-EMA Crossover";
                Calculate = Calculate.OnBarClose;
                EntriesPerDirection = 1;
                EntryHandling = EntryHandling.AllEntries;
                IsExitOnSessionCloseStrategy = true;
                ExitOnSessionCloseSeconds = 30;
                BarsRequiredToTrade = 30;
            }
            else if (State == State.Configure)
            {
                sma14 = SMA(Close, 14);
                ema14 = EMA(Close, 14);
            }
        }

        protected override void OnBarUpdate()
        {
            if (CurrentBar < BarsRequiredToTrade)
                return;

            if (CrossAbove(sma14, ema14, 1))
            {
                longPosition = true;
                shortPosition = false;
            }

            if (CrossBelow(sma14, ema14, 1))
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

