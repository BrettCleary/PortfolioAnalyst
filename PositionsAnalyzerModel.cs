using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioAnalyst
{
    public class PositionsAnalyzerModel
    {
        public List<Position> Positions { get; set; } = new List<Position>();
        //public ObservableCollection<Position> Positions { get; set; } = new ObservableCollection<Position>();
        TradeParserModel TradeParser { get; set; }
        List<Trade> Trades { get; set; } = new List<Trade>();
        public List<AccountValue> CumulativeRealized { get; private set; } = new List<AccountValue>();
        

        private PositionsAnalyzerModel()
        {
            //GeneratePositions(csvPath);
        }

        public static async Task<PositionsAnalyzerModel> CreateAsync(string csvPath)
        {
            PositionsAnalyzerModel model = new PositionsAnalyzerModel();
            bool ret = await model.GeneratePositions(csvPath);
            model.CalcCumulativeRealizedPerformance();
            return model;
        }

        private void CalcCumulativeRealizedPerformance()
        {
            List<AccountValue> exitedPositions = CreateExitedPositionList();
            CalcCumulativePerformance(exitedPositions);
        }

        private List<AccountValue> CreateExitedPositionList()
        {
            List<AccountValue> exitedPositionGainLoss = new List<AccountValue>();
            foreach (Position pos_i in Positions)
            {
                bool positionIsClosed = pos_i.CurrentQuantity == 0;
                if (positionIsClosed)
                {
                    AccountValue posGrossDate = new AccountValue();
                    posGrossDate.time = pos_i.GetLastTrade().TradeDate;
                    posGrossDate.value = pos_i.GrossProfit;
                    exitedPositionGainLoss.Add(posGrossDate);
                }
            }
            return exitedPositionGainLoss;
        }

        private void CalcCumulativePerformance(List<AccountValue> exitedPositions)
        {
            double sum = 0;
            foreach (AccountValue val in exitedPositions)
            {
                sum += val.value;
                AccountValue cumVal = new AccountValue();
                cumVal.time = val.time;
                cumVal.value = sum;
                CumulativeRealized.Add(cumVal);
            }
        }

        private async Task<bool> GeneratePositions(string csvPath)
        {
            bool ret;
            TradeParser = new TradeParserModel(Trades);
            ret = await TradeParser.ReadCsvSuccess(csvPath);
            CreatePositions();
            return ret;
        }

        private void CreatePositions()
        {
            //Create Positions from Trades
            //Position position = new Position(Trades);
            //Positions.Add(position);
            string tickerLast = "noTicker";
            List<Trade> trade_iList = new List<Trade>();
            foreach (Trade trade_i in Trades)
            {
                //List<Trade> trade_iList = new List<Trade>();
                //trade_iList.Add(trade_i);
                if (trade_i.Ticker != tickerLast)
                {
                    if (trade_iList.Count > 0)
                    {
                        Position position = new Position(trade_iList);
                        Positions.Add(position);
                    }
                    trade_iList = new List<Trade>();
                }
                trade_iList.Add(trade_i);
                tickerLast = trade_i.Ticker;
                if (trade_i == Trades[Trades.Count - 1])
                {
                    Position position = new Position(trade_iList);
                    Positions.Add(position);
                }
            }

            //AccountValues.Add(new AccountValue { time = new DateTime(2018,6,10), value = 50000 });
            //AccountValues.Add(new AccountValue { time = new DateTime(2019, 6, 10), value = 300000 });
            //AccountValues.Add(new AccountValue { time = new DateTime(2020, 6, 10), value = 400000 });
        }
    }

    public struct AccountValue
    {
        public DateTime time { get; set; }
        public double value { get; set; }
    }
}
