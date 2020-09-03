# PortfolioAnalyst
## A UWP app to analyze investing performance from raw data of closed trades.

<!-- ABOUT THE PROJECT -->
## About The Project
[Product Name Screen Shot][acctPerfDark]

### Trade Data Format
Data format is expected in the following CSV format:

Date, Ticker, Order, Price, Quantity, Total

MM/DD/YYYY, TICKER, buy/purchase/sell/dividend, double, double, double

If total is omitted, it will be calculated from price and quantity.

### Usage
Performance based on closed trades is plotted.

Current portfolio based on opened trades that have not been closed are tabulated with corresponding unrealized P/L.

Past positions are grouped by ticker and analyzed. This performance is measured in terms of gross profit, gross % P/L, gross P/L, and IRR and tabulated on the main page.

The app must be restarted to use the newly selected language.

The trades.csv file must be located inside the documents folder or the application's local folder.




[acctPerfDark]: Images/AcctPerfDark.jpg
[acctPerfLight]: Images/AcctPerfLight.jpg
[allPosDark]: Images/AllPosDark.jpg
[allPosLight]: Images/AllPosLight.jpg
[sumPageDark]: Images/SummaryPageDark.jpg
[sumPageLight]: Images/SummaryPageLight.jpg
