using Syncfusion.UI.Xaml.Charts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PortfolioAnalyst
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PerformanceGraphPage : Page
    {
        PositionsAnalyzerModel PositionsModel;
        public ObservableCollection<AccountValue> AccountValues { get; set; } = new ObservableCollection<AccountValue>();
        public PerformanceGraphPage()
        {
            this.InitializeComponent();
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

            SetColorTheme(PositionsModel.AppData.ColorTheme);
        }

        private void SetColorTheme(ColorThemeEnum colorTheme)
        {
            string sfChartID = "SfChart" + colorTheme.ToString() + "Style";
            Style sfChartStyle = (Style)Application.Current.Resources[sfChartID];
            AccountPerformanceChart.Style = sfChartStyle;

            string sfCatAxisID = "DateAxis" + colorTheme.ToString() + "Style";
            Style sfCatAxisStyle = (Style)Application.Current.Resources[sfCatAxisID];
            XAxisDateTime.Style = sfCatAxisStyle;

            string sfNumAxisID = "NumAxis" + colorTheme.ToString() + "Style";
            Style sfNumAxisStyle = (Style)Application.Current.Resources[sfNumAxisID];
            YAxisNumerical.Style = sfNumAxisStyle;
        }
    }
}
