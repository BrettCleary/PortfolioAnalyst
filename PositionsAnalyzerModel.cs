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
        private AppSettingsModel AppData { get; set; }
        public List<Position> Positions { get; set; } = new List<Position>();
        //public ObservableCollection<Position> Positions { get; set; } = new ObservableCollection<Position>();
        TradeParserModel TradeParser { get; set; }
        List<Trade> Trades { get; set; } = new List<Trade>();
        public List<AccountValue> CumulativeRealized { get; private set; } = new List<AccountValue>();
        public List<Position> ExitedPositions { get; set; } = new List<Position>();
        public List<Position> OpenPositions { get; set; } = new List<Position>();
        

        private PositionsAnalyzerModel()
        {

        }

        public static async Task<PositionsAnalyzerModel> CreateAsync(string csvPath, AppSettingsModel appData)
        {
            PositionsAnalyzerModel model = new PositionsAnalyzerModel();
            model.AppData = appData;
            bool ret = await model.GeneratePositions(csvPath);
            model.CalcCumulativeRealizedPerformance();
            model.UpdatePortfolioWithMarketData();
            return model;
        }

        private void UpdatePortfolioWithMarketData()
        {
            foreach (Position pos_i in OpenPositions)
            {
                //Random ran = new Random();
                //pos_i.Price = Math.Round(100.0, 2);
                pos_i.Price = Math.Round(AppData.GetPositionPrice(pos_i.PositionName), 2);
            }
        }

        private void CalcCumulativeRealizedPerformance()
        {
            CreatePositionLists();
            List<AccountValue> exitedPosRealized = CreateExitedPositionRealizedList();
            CalcCumulativePerformance(exitedPosRealized);
        }

        private void CreatePositionLists()
        {
            foreach (Position pos_i in Positions)
            {
                bool positionIsClosed = pos_i.CurrentQuantity == 0;
                if (positionIsClosed)
                {
                    ExitedPositions.Add(pos_i);
                }
                else
                {
                    OpenPositions.Add(pos_i);
                }
            }
        }

        private List<AccountValue> CreateExitedPositionRealizedList()
        {
            List<AccountValue> exitedPositionGainLoss = new List<AccountValue>();
            foreach (Position pos_i in ExitedPositions)
            {
                AccountValue posGrossDate = new AccountValue();
                posGrossDate.time = pos_i.GetLastTrade().TradeDate;
                posGrossDate.value = pos_i.GrossProfit;
                exitedPositionGainLoss.Add(posGrossDate);
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
            string tickerLast = "noTicker";
            List<Trade> trade_iList = new List<Trade>();
            foreach (Trade trade_i in Trades)
            {
                if (trade_i.Ticker != tickerLast)
                {
                    if (trade_iList.Count > 0)
                    {
                        Position position = new Position(trade_iList, AppData);
                        Positions.Add(position);
                    }
                    trade_iList = new List<Trade>();
                }
                trade_iList.Add(trade_i);
                tickerLast = trade_i.Ticker;
                if (trade_i == Trades[Trades.Count - 1])
                {
                    Position position = new Position(trade_iList, AppData);
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
