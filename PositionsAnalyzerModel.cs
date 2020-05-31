using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioAnalyst
{
    public class PositionsAnalyzerModel
    {
        public AppSettingsModel AppData { get; set; }
        public List<Position> Positions { get; set; } = new List<Position>();
        //public ObservableCollection<Position> Positions { get; set; } = new ObservableCollection<Position>();
        TradeParserModel TradeParser { get; set; }
        List<Trade> Trades { get; set; } = new List<Trade>();
        public List<AccountValue> CumulativeRealized { get; private set; } = new List<AccountValue>();
        public List<Position> ExitedPositions { get; set; } = new List<Position>();
        public List<Position> OpenPositions { get; set; } = new List<Position>();
        public CurrentPortfolio PortfolioStatus { get; set; } = new CurrentPortfolio();
        private bool Initialized = false;
        

        private PositionsAnalyzerModel()
        {

        }

        public static async Task<PositionsAnalyzerModel> CreateAsync(string csvPath, AppSettingsModel appData)
        {
            PositionsAnalyzerModel model = new PositionsAnalyzerModel();
            model.AppData = appData;
            bool ret = await model.GeneratePositions(csvPath);
            model.CalcCumulativeRealizedPerformance();
            model.LoadPrices();
            model.Initialized = true;
            return model;
        }

        private void LoadPrices()
        {
            foreach (Position pos_i in OpenPositions)
            {
                pos_i.Price = Math.Round(AppData.GetPositionPrice(pos_i.PositionName), 2);
            }
            UpdateCurrentPortfolio();
        }

        private void UpdateCurrentPortfolio()
        {
            PortfolioStatus.MarketValue = 0;
            PortfolioStatus.CostBasis = 0;
            PortfolioStatus.UnrealizedPL = 0;
            PortfolioStatus.UnrealizedPLPercent = 0;
            foreach (Position pos_i in OpenPositions)
            {
                PortfolioStatus.CostBasis += pos_i.CostBasis;
                PortfolioStatus.MarketValue += pos_i.MarketValue;
                PortfolioStatus.CalculatePL();
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
                        position.PropertyChanged += Position_PropertyChanged;
                        Positions.Add(position);
                    }
                    trade_iList = new List<Trade>();
                }
                trade_iList.Add(trade_i);
                tickerLast = trade_i.Ticker;
                if (trade_i == Trades[Trades.Count - 1])
                {
                    Position position = new Position(trade_iList, AppData);
                    position.PropertyChanged += Position_PropertyChanged;
                    Positions.Add(position);
                }
            }

            //AccountValues.Add(new AccountValue { time = new DateTime(2018,6,10), value = 50000 });
            //AccountValues.Add(new AccountValue { time = new DateTime(2019, 6, 10), value = 300000 });
            //AccountValues.Add(new AccountValue { time = new DateTime(2020, 6, 10), value = 400000 });
        }

        private void Position_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!Initialized)
                return;

            UpdateCurrentPortfolio();

        }
    }

    public struct AccountValue
    {
        public DateTime time { get; set; }
        public double value { get; set; }
    }

    public class CurrentPortfolio : INotifyPropertyChanged
    {
        public double MarketValue { get; set; } = 0;
        public double CostBasis { get; set; } = 0;
        public double UnrealizedPL { get; set; } = 0;
        public double UnrealizedPLPercent { get; set; } = 0;
        public event PropertyChangedEventHandler PropertyChanged;

        public void CalculatePL()
        {
            UnrealizedPL = Math.Round(MarketValue - CostBasis, 2);
            UnrealizedPLPercent = Math.Round(UnrealizedPL / CostBasis * 100, 1);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MarketValue)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CostBasis)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UnrealizedPL)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UnrealizedPLPercent)));
        }

    }
}
