# PortfolioAnalyst
## A UWP app to analyze investing performance from raw data of closed trades.

## Table of Contents

* [About the Project](#about-the-project)
  * [Built With](#built-with)
  * [Trade Data Format](#trade-data-format)
* [Usage](#usage)
* [Contributing](#contributing)
* [App Pages](#app-pages)
* [License](#license)

## About The Project

### Built With
* [UWP](https://docs.microsoft.com/en-us/windows/uwp/)
* [Syncfusion](https://help.syncfusion.com/uwp/overview)

### Trade Data Format
Data format is expected in the following CSV format:

Date, Ticker, Order, Price, Quantity, Total

MM/DD/YYYY, TICKER, buy/purchase/sell/dividend, double, double, double

If total is omitted, it will be calculated from price and quantity.

### Usage
This UWP app requires access to documents to load trades. 

It is also built for desktop PC's. It has not been extended to the full family of Windows devices.

Performance based on closed trades is plotted.

Current portfolio based on opened trades that have not been closed are tabulated with corresponding unrealized P/L.

Past positions are grouped by ticker and analyzed. This performance is measured in terms of gross profit, gross % P/L, gross P/L, and IRR and tabulated on the main page.

The app must be restarted to use the newly selected language.

The trades.csv file must be located inside the documents folder or the application's local folder.

## App Pages
![Summary Page Dark][sumPageDark]
![Summary Page Light][sumPageLight]
![Account Performance Dark][acctPerfDark]
![Account Performance Light][acctPerfLight]
![All Positions Dark][allPosDark]
![All Positions Dark][allPosLight]

[acctPerfDark]: Images/AcctPerfDark.JPG
[acctPerfLight]: Images/AcctPerfLight.JPG
[allPosDark]: Images/AllPosDark.JPG
[allPosLight]: Images/AllPosLight.JPG
[sumPageDark]: Images/SummaryPageDark.JPG
[sumPageLight]: Images/SummaryPageLight.JPG

## Contributing

Contributions are what make the open source community such an amazing place to be learn, inspire, and create. Any contributions you make are **greatly appreciated**.

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE` for more information.
