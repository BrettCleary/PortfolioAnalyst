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
            SetColorTheme(AppData.ColorTheme);
            //bool loaded = await AppData.LoadSettingsFromXML();
            PositionsModel = await PositionsAnalyzerModel.CreateAsync("AllTrades.csv", AppData);
            MainPageFrame.Navigate(typeof(SummaryPage), PositionsModel);
            return true;
        }

        /*protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (AppData != null)
                SetColorTheme(AppData.ColorTheme);
        }*/

        private void MainPageNavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            FrameNavigationOptions navOptions = new FrameNavigationOptions();
            //navOptions.TransitionInfoOverride = args.RecommendedNavigationTransitionInfo;
            Type pageType = typeof(SetupPage);
            if (args.IsSettingsInvoked == true)
            {
                MainPageFrame.Navigate(pageType, AppData, new DrillInNavigationTransitionInfo());
                return;
            }

            string navItemTag = args.InvokedItemContainer.Tag.ToString();
            if (navItemTag == "HomeID")
            {
                pageType = typeof(SummaryPage);
                MainPageFrame.Navigate(pageType, PositionsModel, new DrillInNavigationTransitionInfo());
            }
            else if (navItemTag == "GraphID")
            {
                pageType = typeof(PerformanceGraphPage);
                MainPageFrame.Navigate(pageType, PositionsModel, new DrillInNavigationTransitionInfo());
            }
            else if (navItemTag == "PosTableID")
            {
                pageType = typeof(PositionTablePage);
                MainPageFrame.Navigate(pageType, PositionsModel, new DrillInNavigationTransitionInfo());
            }
            /*else if (navItemTag == "SetupID")
            {
                pageType = typeof(SetupPage);
                MainPageFrame.Navigate(pageType, AppData, new DrillInNavigationTransitionInfo());
            }*/
            //MainPageFrame.Navigate(pageType, PositionsModel, navOptions.TransitionInfoOverride);
            //MainPageFrame.Navigate(pageType, PositionsModel, new DrillInNavigationTransitionInfo());
        }

        private void SetColorTheme(ColorThemeEnum colorTheme)
        {
            string navViewTextBlockID = "TextBlock" + colorTheme.ToString() + "Style";
            Style navViewTextBlockIDStyle = (Style)Application.Current.Resources[navViewTextBlockID];

            foreach (var item in MainPageNavView.MenuItems.OfType<NavigationViewItem>())
            {
                TextBlock contentBlock = (TextBlock)item.Content;
                contentBlock.Style = navViewTextBlockIDStyle;
            }

            Windows.UI.ViewManagement.ApplicationView appView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
            appView.TitleBar.BackgroundColor = Windows.UI.Colors.Black; // or {a: 255, r: 0, g: 0, b: 0}
            appView.TitleBar.InactiveBackgroundColor = Windows.UI.Colors.Black;
            appView.TitleBar.ButtonBackgroundColor = Windows.UI.Colors.Black;
            appView.TitleBar.ButtonHoverBackgroundColor = Windows.UI.Colors.Black;
            appView.TitleBar.ButtonPressedBackgroundColor = Windows.UI.Colors.Black;
            appView.TitleBar.ButtonInactiveBackgroundColor = Windows.UI.Colors.Black;
        }
    }
}
