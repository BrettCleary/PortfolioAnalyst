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

            //AccountValues = new ObservableCollection<AccountValue>(PositionsModel.AccountValues);
            foreach (AccountValue accVal in PositionsModel.CumulativeRealized)
                AccountValues.Add(accVal);

            LineSeries series1 = new LineSeries();
            series1.XBindingPath = "time";
            series1.YBindingPath = "value";
            series1.Label = "acctvalues";
            series1.ItemsSource = AccountValues;
            DataContext = this;
            
            series1.EnableAnimation = true;
            AccountPerformanceChart.Series.Add(series1);
            //Positions = new ObservableCollection<Position>(PositionsModel.Positions);
        }

        private void AllPositionsDataGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }
    }
}
