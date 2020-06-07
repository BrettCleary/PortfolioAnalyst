using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Composition;
using System.Numerics;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Shapes;
using Syncfusion.UI.Xaml.Charts;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System.Drawing;
using Windows.Storage;
using Windows.Storage.Pickers;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PortfolioAnalyst
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SummaryPage : Page
    {
        PositionsAnalyzerModel PositionsModel;
        Compositor Comp = Window.Current.Compositor;
        SpringVector3NaturalMotionAnimation SpringAnimation;
        public ObservableCollection<AccountValue> AccountValues { get; set; } = new ObservableCollection<AccountValue>();
        public ObservableCollection<Position> OpenPositions { get; set; } = new ObservableCollection<Position>();

        public CurrentPortfolio PortfolioStatus;
        private AppSettingsModel AppData;

        public Style PricesTextBoxStyle;
        /*public ref double PortMarketValue;
        public ref double PortCostBasis;
        public ref double PortProfitLoss;
        public double PortProfitLossPercent;*/


        public SummaryPage()
        {
            this.InitializeComponent();
        }
        private void CreateOrUpdateSpringAnimation(float finalValue)
        {
            if (SpringAnimation == null)
            {
                SpringAnimation = Comp.CreateSpringVector3Animation();
                SpringAnimation.Target = "Scale";
            }

            SpringAnimation.FinalValue = new Vector3(finalValue);
        }
        private void AllPositionsDataGrid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            CreateOrUpdateSpringAnimation(1.0f);
            (sender as UIElement).StartAnimation(SpringAnimation);
        }
        private void AllPositionsDataGrid_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            CreateOrUpdateSpringAnimation(1.0f);
            (sender as UIElement).StartAnimation(SpringAnimation);
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            PositionsModel = (PositionsAnalyzerModel)e.Parameter;

            foreach (AccountValue accVal in PositionsModel.CumulativeRealized)
            {
                AccountValues.Add(accVal);
            }


            LineSeries series1 = new LineSeries();
            series1.XBindingPath = "time";
            series1.YBindingPath = "value";
            series1.Label = "acctvalues";
            series1.ItemsSource = AccountValues;
            DataContext = this;
            
            series1.EnableAnimation = true;
            AccountPerformanceChart.Series.Add(series1);

            foreach (Position pos_i in PositionsModel.OpenPositions)
            {
                OpenPositions.Add(pos_i);
            }

            PortfolioStatus = PositionsModel.PortfolioStatus;
            /*PortMarketValue = portStatus.MarketValue;
            PortCostBasis = portStatus.CostBasis;
            PortProfitLoss = portStatus.UnrealizedPL;
            PortProfitLossPercent = portStatus.UnrealizedPLPercent;*/

            //SummaryPagePage.Background = (AcrylicBrush) Application.Current.Resources["LightAcrylic"];
            //SummaryPagePage.Style = (Style)Application.Current.Resources["PageDarkStyle"];
            SetColorTheme(PositionsModel.AppData.ColorTheme);
        }

        private void SetColorTheme(ColorThemeEnum colorTheme)
        {
            string textBlockID = "TextBlock" + colorTheme.ToString() + "Style";
            Style textBlockStyle = (Style)Application.Current.Resources[textBlockID];
            CurrentPortolioTextBlock.Style = textBlockStyle;
            MarketValueTextBlock.Style = textBlockStyle;
            MarketValueLabelTextBlock.Style = textBlockStyle;
            CostBasisTextBlock.Style = textBlockStyle;
            CostBasisLabelTextBlock.Style = textBlockStyle;
            PLLabelTextBlock.Style = textBlockStyle;
            PLTextBlock.Style = textBlockStyle;
            UPLPercentLabelTextBlock.Style = textBlockStyle;
            UPLPercentTextBlock.Style = textBlockStyle;
            //PriceColumnTextBox.Style = textBlockStyle;
            PricesTextBoxStyle = textBlockStyle;

            /*string textBoxID = "TextBox" + colorTheme.ToString() + "Style";
            Style textBoxStyle = (Style)Application.Current.Resources[textBoxID];
            CurrentPositionsDataGrid.Columns[1].CellStyle = textBoxStyle;*/

            string dataGridID = "DataGrid" + colorTheme.ToString() + "Style";
            Style dataGridStyle = (Style)Application.Current.Resources[dataGridID];
            CurrentPositionsDataGrid.Style = dataGridStyle;
            AllPositionsDataGrid.Style = dataGridStyle;

            string sfChartID = "SfChart" + colorTheme.ToString() + "Style";
            Style sfChartStyle = (Style)Application.Current.Resources[sfChartID];
            AccountPerformanceChart.Style = sfChartStyle;

            string sfCatAxisID = "DateAxis" + colorTheme.ToString() + "Style";
            Style sfCatAxisStyle = (Style)Application.Current.Resources[sfCatAxisID];
            XAxisDateTime.Style = sfCatAxisStyle;

            string sfNumAxisID = "NumAxis" + colorTheme.ToString() + "Style";
            Style sfNumAxisStyle = (Style)Application.Current.Resources[sfNumAxisID];
            YAxisNumerical.Style = sfNumAxisStyle;

            string buttonID = "Button" + colorTheme.ToString() + "Style";
            Style buttonStyle = (Style)Application.Current.Resources[buttonID];
            BrowseCsvButton.Style = buttonStyle;
        }

        private async void BrowseCsvButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.List;
            picker.FileTypeFilter.Add(".csv");

            StorageFile file = await picker.PickSingleFileAsync();


            if (file != null)
            {
                PositionsModel.AppData.CsvFilePath = file.Path;
            }
            else
            {

            }
        }

        private void AllPositionsDataGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            
        }

        /*private void UpdatePositionsButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            int priceColIndex = 0;
            int tickerColIndex = 0;
            int colIndex = 0;
            foreach(DataGridColumn col_i in CurrentPositionsDataGrid.Columns)
            {
                if ((string)col_i.Tag == "Price")
                {
                    priceColIndex = colIndex;
                }
                if ((string)col_i.Tag == "Ticker")
                {
                    tickerColIndex = colIndex;
                }
                ++colIndex;
            }
        }*/

        private void CurrentPositionsDataGrid_CellEditEnded(object sender, DataGridCellEditEndedEventArgs e)
        {
        }

        private void CurrentPositionsDataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
        }

    }
}
