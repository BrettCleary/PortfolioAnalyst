using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

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

        public async Task<bool> ReadCsvSuccess(string csvPath)
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await storageFolder.GetFileAsync(csvPath);
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
            foreach(var line in lines)
            {
                if (line == "")
                    continue;
                if (skipFirstLine)
                {
                    skipFirstLine = false;
                    continue;
                }
                var data = line.Split(',');
                Trade trade_i = new Trade();
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
                if (trade_i.BuyOrSell == Order.BUY)
                    trade_i.Total = -1 * Math.Abs(Convert.ToDouble(data[5]));
                else
                    trade_i.Total = Math.Abs(Convert.ToDouble(data[5]));

                Trades.Add(trade_i);
            }
            return true;
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
