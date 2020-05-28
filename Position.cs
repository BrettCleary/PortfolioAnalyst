using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioAnalyst
{
    public class Position : INotifyPropertyChanged
    {
        List<Trade> Trades;
        public string Ticker;
        InvestmentType Investment;
        public double GrossBuy = 0;
        public double GrossSell = 0;
        public double MaxPosSize = 0;
        public double GrossProfit = 0;
        public double GrossPercent = 0;
        public double IRR = 0;
        public double CurrentQuantity = 0;

        private double _Price = -1;
        public double Price { get { return _Price;  } 
            set 
            { 
                _Price = value;
                OnPriceChanged();
            } 
        }
        public double MarketValue = -1;
        public double ProfitLoss = -1;
        public double ProfitLossPercent = -1;
        public double CostBasis = -1;
        public event PropertyChangedEventHandler PropertyChanged;

        public Position(List<Trade> trades)
        {
            Trades = trades;
            InitializePosition();
        }

        private void OnPriceChanged()
        {
            MarketValue = _Price * CurrentQuantity;
            CostBasis = GrossBuy - GrossSell;
            ProfitLoss = MarketValue - CostBasis;
            ProfitLossPercent = Math.Round(ProfitLoss / CostBasis * 100, 2);
            EventArgs e = new EventArgs();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MarketValue)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProfitLoss)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProfitLossPercent)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CostBasis)));

        }

        public Trade GetLastTrade()
        {
            return Trades[Trades.Count - 1];
        }

        private void InitializePosition()
        {
            Ticker = Trades[0].Ticker;
            //sort trades by tradeDate
            Trades.Sort();
            foreach (Trade trade in Trades)
            {
                if (Ticker != trade.Ticker)
                    throw new Exception("Trade tickers not equal.");
                if (trade.BuyOrSell == Order.BUY)
                {
                    GrossBuy += Math.Abs(trade.Total);
                    CurrentQuantity += trade.Quantity;
                }
                else if (trade.BuyOrSell == Order.SELL)
                {
                    GrossSell += Math.Abs(trade.Total);
                    CurrentQuantity += trade.Quantity;
                }
                else if (trade.BuyOrSell == Order.DIVIDEND)
                {
                    GrossSell += Math.Abs(trade.Total);
                }
                double currentPosSize = 0;
                if (trade.BuyOrSell == Order.BUY || trade.BuyOrSell == Order.SELL)
                    currentPosSize = (GrossBuy - GrossSell);
                if (currentPosSize > MaxPosSize)
                    MaxPosSize = currentPosSize;
            }
            MaxPosSize = Math.Round(MaxPosSize, 2);
            GrossBuy = Math.Round(GrossBuy, 2);
            GrossSell = Math.Round(GrossSell, 2);
            bool positionExited = CurrentQuantity == 0;
            if (positionExited)
                GrossProfit = Math.Round(GrossSell - GrossBuy, 2);
            GrossPercent = Math.Round(GrossProfit / MaxPosSize * 100, 2);

            //discount each trade value to npv for irr calc
            //converge on npv = 0
            if (positionExited)
                CalcIRR(0.10);
        }

        private bool CalcIRR(double irrGuessFraction)
        {
            double maxError = 1;
            double error = 10;
            double dx = 0.0001;
            double x = irrGuessFraction;
            ulong triesLeft = 10000;
            while (Math.Abs(error) > maxError && triesLeft > 0)
            {
                double xa = x - dx;
                double xb = x + dx;
                double ya = CalcNPV(xa);
                double yb = CalcNPV(xb);
                double m = (yb - ya) / (2 * dx);
                double y = CalcNPV(x);
                if (m == 0)
                {
                    IRR = 0;
                    return true;
                }
                x = -y / m + x;
                error = y;
                --triesLeft;
            }
            if (error < maxError)
            {
                IRR = Math.Round(x * 100, 2);
                return true;
            }
            else
            {
                IRR = 0;
                return false;
            }
        }

        private double CalcNPV(double irrFraction)
        {
            DateTime initialDate = Trades[0].TradeDate;
            double npvSum = 0;
            foreach (Trade trade in Trades)
            {
                TimeSpan time_i = trade.TradeDate.Subtract(initialDate);

                npvSum += trade.Total / Math.Pow((1 + irrFraction), time_i.TotalDays / 365.2422);
            }
            return npvSum;
        }
    }

    public enum InvestmentType
    {
        STOCK,
        OPTION
    }
}
