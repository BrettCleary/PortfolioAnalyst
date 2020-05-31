using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PortfolioAnalyst
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        PositionsAnalyzerModel PositionsModel;
        AppSettingsModel AppData;

        public MainPage()
        {
            this.InitializeComponent();
            bool success;
            //PositionsModel = new PositionsAnalyzerModel("C:\\Users\\brett\\AppData\\Local\\Packages\\ff69270a-36c5-460e-b566-4fd9f0f640c8_sz704ddpb1q5c\\LocalState\\AllPositionData.csv");
            MainPageFrame.Navigate(typeof(LoadScreenPage));
            //InitializeMainPage();
        }

        public async Task<bool> InitializeMainPage(AppSettingsModel appData)
        {
            AppData = appData;
            //bool loaded = await AppData.LoadSettingsFromXML();
            PositionsModel = await PositionsAnalyzerModel.CreateAsync("AllTrades.csv", AppData);
            MainPageFrame.Navigate(typeof(SummaryPage), PositionsModel);
            return true;
        }

        private void MainPageNavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            FrameNavigationOptions navOptions = new FrameNavigationOptions();
            //navOptions.TransitionInfoOverride = args.RecommendedNavigationTransitionInfo;
            Type pageType = typeof(SetupPage);
            if (args.IsSettingsInvoked == true)
            {
                MainPageFrame.Navigate(pageType, PositionsModel, navOptions.TransitionInfoOverride);
                return;
            }

            string navItemTag = args.InvokedItemContainer.Tag.ToString();
            if (navItemTag == "HomeID")
            {
                pageType = typeof(SummaryPage);
            }
            else if (navItemTag == "GraphID")
            {
                pageType = typeof(PerformanceGraphPage);
            }
            else if (navItemTag == "PosTableID")
            {
                pageType = typeof(PositionTablePage);
            }
            else if (navItemTag == "SetupID")
            {
                pageType = typeof(SetupPage);
            }
            //MainPageFrame.Navigate(pageType, PositionsModel, navOptions.TransitionInfoOverride);
            MainPageFrame.Navigate(pageType, PositionsModel, new DrillInNavigationTransitionInfo());
        }
    }
}
