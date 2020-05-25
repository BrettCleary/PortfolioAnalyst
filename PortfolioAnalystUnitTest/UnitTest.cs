using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PortfolioAnalyst;

namespace PortfolioAnalystUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        //List<Trade> UnitTestTrades = new List<Trade>();
        [TestMethod]
        public async Task TestMethod1()
        {
            //TradeParserModel parser = new TradeParserModel("PositionCreationUnitTest.csv", UnitTestTrades);
            //PositionsAnalyzerModel model = new PositionsAnalyzerModel("PositionCreationUnitTest.csv");
            PositionsAnalyzerModel model = await PositionsAnalyzerModel.CreateAsync("PositionCreationUnitTest.csv");
            
            Assert.AreEqual(model.Positions.Count, 1);
            Position tslaTestPosition = model.Positions[0];
            Assert.AreEqual(tslaTestPosition.Ticker, "TSLA");
            Assert.AreEqual(tslaTestPosition.GrossBuy, 702065);
            Assert.AreEqual(tslaTestPosition.GrossSell, 1137780);
            Assert.AreEqual(tslaTestPosition.MaxPosSize, 381825);
            Assert.AreEqual(tslaTestPosition.GrossProfit, tslaTestPosition.GrossSell - tslaTestPosition.GrossBuy, 0.001);
            Assert.AreEqual(tslaTestPosition.GrossPercent, tslaTestPosition.GrossProfit / tslaTestPosition.MaxPosSize * 100, 0.01);
            Assert.AreEqual(tslaTestPosition.IRR, 53.17, 0.001);
            Assert.AreEqual(tslaTestPosition.CurrentQuantity, 0);
        }
    }
}
