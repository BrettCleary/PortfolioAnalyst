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
using System.ComponentModel;
using Windows.UI;

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
            //PositionsModel = new PositionsAnalyzerModel("C:\\Users\\brett\\AppData\\Local\\Packages\\ff69270a-36c5-460e-b566-4fd9f0f640c8_sz704ddpb1q5c\\LocalState\\AllPositionData.csv");
            
            //InitializeMainPage();
        }

        public async Task<bool> InitializeMainPage(AppSettingsModel appData)
        {
            AppData = appData;
            SetColorTheme(AppData.ColorTheme);
            AppData.PropertyChanged += AppDataPropertyChangedHandler;

            //bool loaded = await AppData.LoadSettingsFromXML();
            EnableNavViewItems(false);
            MainPageFrame.Navigate(typeof(WaitingForDataFilePage), appData, new DrillInNavigationTransitionInfo());
            return true;

            //PositionsModel = await PositionsAnalyzerModel.CreateAsync("AllTrades.csv", AppData);
            //MainPageFrame.Navigate(typeof(SummaryPage), PositionsModel);
            //return true;
        }

        private void EnableNavViewItems(bool enable)
        {
            MainPageNavView.IsSettingsVisible = enable;
            foreach (var navViewObj in MainPageNavView.MenuItems)
            {
                NavigationViewItem navViewItem = (NavigationViewItem)navViewObj;
                navViewItem.IsEnabled = enable;
            }
        }

        public async void AppDataPropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ColorTheme")
                SetColorTheme(AppData.ColorTheme);

            if (e.PropertyName == "CsvFilePath")
            {
                MainPageFrame.Navigate(typeof(LoadScreenPage), AppData, new DrillInNavigationTransitionInfo());
                try
                {
                    PositionsModel = await PositionsAnalyzerModel.CreateAsync(AppData);
                    EnableNavViewItems(true);
                    SetSettingsColor();
                    MainPageFrame.Navigate(typeof(SummaryPage), PositionsModel, new DrillInNavigationTransitionInfo());
                }
                catch
                {
                    MainPageFrame.Navigate(typeof(WaitingForDataFilePage),AppData, new DrillInNavigationTransitionInfo());
                }
                
                

            }
        }

        private void SetSettingsColor()
        {
            //set foreground color of settings navigation view item now that it is instantiated
            string iconElementID = "IconElement" + AppData.ColorTheme.ToString() + "Style";
            Style iconElementIDStyle = (Style)Application.Current.Resources[iconElementID];
            NavigationViewItem navSettingsItem = (NavigationViewItem)MainPageNavView.SettingsItem;

            if (navSettingsItem != null)
            {
                navSettingsItem.Icon.Style = iconElementIDStyle;
            }
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
            SetSettingsColor();

            string navViewTextBlockID = "TextBlock" + colorTheme.ToString() + "Style";
            Style navViewTextBlockIDStyle = (Style)Application.Current.Resources[navViewTextBlockID];

            string pageID = "Page" + colorTheme.ToString() + "Style";
            MainPagePage.Style = (Style)Application.Current.Resources[pageID];

            foreach (var item in MainPageNavView.MenuItems.OfType<NavigationViewItem>())
            {
                TextBlock contentBlock = (TextBlock)item.Content;
                contentBlock.Style = navViewTextBlockIDStyle;
            }

            /*string navViewSepID = "NavigationViewItemSeparator" + colorTheme.ToString() + "Style";
            Style navViewSepIDStyle = (Style)Application.Current.Resources[navViewSepID];
            foreach (var item in MainPageNavView.MenuItems.OfType<NavigationViewItemSeparator>())
            {
                NavigationViewItemSeparator separator = (NavigationViewItemSeparator)item;
                //separator.Style = navViewSepIDStyle;
                separator.Foreground = new SolidColorBrush(Colors.Red);
            }*/
            /*string iconElementID = "IconElement" + colorTheme.ToString() + "Style";
            Style iconElementIDStyle = (Style)Application.Current.Resources[iconElementID];
            NavigationViewItem navSettingsItem = (NavigationViewItem)MainPageNavView.SettingsItem;
            
            navSettingsItem.Icon.Style = iconElementIDStyle;*/

            string navViewBrushID = colorTheme.ToString() + "Acrylic";
            AcrylicBrush acrylicBrush = (AcrylicBrush)Application.Current.Resources[navViewBrushID];
            NavViewBackgroundBrush.TintColor = acrylicBrush.TintColor;

            string appColorBrushID = "AltLow" + colorTheme.ToString();
            SolidColorBrush solidBrush = (SolidColorBrush)Application.Current.Resources[appColorBrushID];

            Color appColor = solidBrush.Color;
            PointerOverNavViewBrush.Color = appColor;

            Windows.UI.ViewManagement.ApplicationView appView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
            appView.TitleBar.BackgroundColor = appColor; // or {a: 255, r: 0, g: 0, b: 0}
            appView.TitleBar.InactiveBackgroundColor = appColor;
            appView.TitleBar.ButtonBackgroundColor = appColor;
            appView.TitleBar.ButtonHoverBackgroundColor = appColor;
            appView.TitleBar.ButtonPressedBackgroundColor = appColor;
            appView.TitleBar.ButtonInactiveBackgroundColor = appColor;
        }
    }
}
