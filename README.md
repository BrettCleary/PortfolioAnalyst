# PortfolioAnalyst
## A UWP app to analyze investing performance from raw data of closed trades.

### Trade Data Format
Data format is expected in the following CSV format:

Date, Ticker, Order, Price, Quantity, Total

MM/DD/YYYY, TICKER, buy/purchase/sell/dividend, double, double, double

If total is omitted, it will be calculated from price and quantity.

### Usage
Performance based on closed trades is plotted.

Current portfolio based on opened trades that have not been closed are tabulated with corresponding unrealized P/L.

Past positions are grouped by ticker and analyzed. This performance is measured in terms of gross profit, gross % P/L, gross P/L, and IRR and tabulated on the main page.
