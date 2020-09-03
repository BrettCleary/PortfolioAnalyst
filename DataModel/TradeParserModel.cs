using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Composition;
using Windows.UI.Xaml.Media;

namespace PortfolioAnalyst
{
    public class TradeParserModel
    {
        List<Trade> Trades { get; set; }
        public TradeParserModel(List<Trade> trades)
        {
            Trades = trades;
            //ReadCsvSuccess(csvPath);
        }

        public async Task<bool> CreateRandomTrades()
        {
            string[] randTickers = {"TSLA", "F", "AER", "MU", "LFIN", "NKLA", "SPCE", "KODK", "AAL",
            "AMZN", "AAPL", "MSFT", "AMD", "NVDA", "MMM", "ABT", "ABBV", "ATVI", "ADBE",
            "ALB", "AZO", "BKR", "BLL", "BAC", "CSCO", "CTXS", "CI", "KO", "CERN", "CVS",
            "DAL"};
            string[] randOrder = { "buy", "sell" };
            int numTickers = 10;
            for (int i = 0; i < numTickers; ++i)
            {
                Random rnd = new Random();
                string ticker_i = randTickers[rnd.Next(0, randTickers.Length)];

                int numHolding = rnd.Next(100, 10000);
                int numTradesPerTicker = 10;
                int maxYear = 1980;
                double totalSum = 0;

                for (int j = 0; j < numTradesPerTicker; ++j)
                {
                    int quantity = rnd.Next(0, numHolding);
                    int isSell = rnd.Next(0, 2);
                    string[] data = new string[6];
                    int month = rnd.Next(1, 13);
                    int day = rnd.Next(1, 29);
                    int year = rnd.Next(1980 + j, 2021 - numTickers + i);
                    int price = rnd.Next(1, 100);
                    if (year > maxYear)
                        maxYear = year;
                    data[0] = Convert.ToString(month) + "/" + Convert.ToString(day) + "/" + Convert.ToString(year);
                    data[1] = ticker_i;
                    if (j == 0)
                        data[2] = "buy";
                    else
                        data[2] = randOrder[isSell];
                    data[3] = Convert.ToString(price);
                    if (j == 0)
                        data[4] = Convert.ToString(numHolding);
                    else
                        data[4] = Convert.ToString(quantity);
                    data[5] = "";
                    if (isSell == 0)
                    {
                        quantity = rnd.Next(100, 10000);
                        data[4] = Convert.ToString(quantity);
                    }

                    if (isSell == 1 && j != numTradesPerTicker - 1)
                        totalSum += quantity * price;
                    else if (j != numTradesPerTicker - 1)
                        totalSum += -1 * quantity * price;

                    if (j == numTradesPerTicker - 1 && rnd.Next(0,2) == 1)
                    {
                        data[4] = Convert.ToString(numHolding);
                        quantity = numHolding;
                        data[2] = "sell";
                        isSell = 1;
                        data[0] = "1/1/" + Convert.ToString(maxYear + 1);
                        int priceForProfitMin = (int)(2 * (Math.Abs(totalSum) + 1) / quantity + 1);
                        int priceForProfitMax = (int)(3 * Math.Abs(totalSum) / quantity + 1);
                        int priceForProfit = rnd.Next(priceForProfitMin, priceForProfitMax) + 1;
                        data[3] = Convert.ToString(priceForProfit);
                    }

                    if (isSell == 1 && j > 0)
                        numHolding -= quantity;
                    else if (j > 0)
                        numHolding += quantity;

                    if (isSell == 1 && j != numTradesPerTicker - 1)
                        totalSum += quantity * price;
                    else if (j != numTradesPerTicker - 1)
                        totalSum += -1 * quantity * price;

                    ReadDataIntoTrade(data);
                }
            }
            return true;
        }
        
        public async Task<bool> ReadCsv(string csvPath)
        {
            //StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await StorageFile.GetFileFromPathAsync(csvPath);

            //StorageFile file = await storageFolder.GetFileAsync(csvPath);
            IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
            if (!stream.CanRead)
                return false;
            ulong size = stream.Size;
            string text;
            using (var inputStream = stream.GetInputStreamAt(0))
            {
                using (var dataReader = new DataReader(inputStream))
                {
                    uint numBytesLoaded = await dataReader.LoadAsync((uint)size);
                    text = dataReader.ReadString(numBytesLoaded);
                }
            }

            var lines = text.Split('\n');
            bool skipFirstLine = true;
            foreach (var line in lines)
            {
                if (line == "")
                    continue;
                if (skipFirstLine)
                {
                    skipFirstLine = false;
                    continue;
                }
                string[] data = line.Split(',');
                for (int i = 0; i < data.Length; ++i)
                {
                    data[i] = data[i].Trim('"');
                    data[i] = data[i].Trim(' ');
                    data[i] = data[i].Trim('\r');
                    data[i] = data[i].Trim('\\');
                    data[i] = data[i].Trim('(');
                    data[i] = data[i].Trim(')');
                    data[i] = data[i].Trim(' ');
                    data[i] = data[i].Trim('$');
                }
                ReadDataIntoTrade(data);
            }
            return true;
        }

        public void ReadDataIntoTrade(string[] data)
        {
            Trade trade_i = new Trade();
            var date = data[0].Split('/');
            trade_i.TradeDate = new DateTime(Convert.ToInt32(date[2]), Convert.ToInt32(date[0]), Convert.ToInt32(date[1]));

            trade_i.Ticker = data[1];

            data[2] = data[2].ToLower();
            if (data[2] == "buy" || data[2] == "purchase")
                trade_i.BuyOrSell = Order.BUY;
            else if (data[2] == "sell")
                trade_i.BuyOrSell = Order.SELL;
            else if (data[2] == "dividend")
                trade_i.BuyOrSell = Order.DIVIDEND;
            else if (trade_i.Total > 0)
                trade_i.BuyOrSell = Order.SELL;
            else
                trade_i.BuyOrSell = Order.BUY;

            if (data[3] == "")
                trade_i.Price = 0;
            else
                trade_i.Price = Convert.ToDouble(data[3]);
            if (data[4] == "")
                trade_i.Quantity = 0;
            else
            {
                if (trade_i.BuyOrSell == Order.BUY)
                    trade_i.Quantity = Math.Abs(Convert.ToDouble(data[4]));
                else
                    trade_i.Quantity = -1 * Math.Abs(Convert.ToDouble(data[4]));
            }
            if (data[5] == "")
                trade_i.Total = trade_i.Quantity * trade_i.Price;
            else if (trade_i.BuyOrSell == Order.BUY)
                trade_i.Total = -1 * Math.Abs(Convert.ToDouble(data[5]));
            else
                trade_i.Total = Math.Abs(Convert.ToDouble(data[5]));

            Trades.Add(trade_i);
        }

        public async Task<bool> ReadCsvSuccess(string csvPath)
        {
            if (csvPath == "generateRandom")
            {
                return await CreateRandomTrades();
            }
            else
            {
                return await ReadCsv(csvPath);
            }
        }
    }

    public class Trade : IComparable<Trade>
    {
        public DateTime TradeDate;
        public String Ticker;
        public InvestmentType Investment;
        public Order BuyOrSell;
        public double Price;
        public double Quantity;
        public double Total;

        public string GetDisplayName()
        {
            return Ticker;
        }

        public int CompareTo(Trade compareTrade)
        {
            return TradeDate.CompareTo(compareTrade.TradeDate);
        }
    }

    public class OptionTrade : Trade
    {
        DateTime ExpDate;

        new public string GetDisplayName()
        {
            return Ticker + " " + ExpDate.ToString();
        }
    }

    public enum Order
    {
        BUY,
        SELL,
        DIVIDEND
    }

}
