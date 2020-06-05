using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
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
    public sealed partial class WaitingForDataFilePage : Page
    {
        PositionsAnalyzerModel PositionsModel;
        AppSettingsModel AppData;

        public WaitingForDataFilePage()
        {
            this.InitializeComponent();
        }

        private async void BrowseCsvButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.List;
            picker.FileTypeFilter.Add(".csv");

            StorageFile file = await picker.PickSingleFileAsync();


            if (file != null)
            {
                AppData.CsvFilePath = file.Path;
            }
            else
            {

            }
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            AppData = (AppSettingsModel)e.Parameter;

            SetColorTheme(AppData.ColorTheme);
        }

        private void SetColorTheme(ColorThemeEnum colorTheme)
        {
            string textBlockID = "TextBlock" + colorTheme.ToString() + "Style";
            Style textBlockStyle = (Style)Application.Current.Resources[textBlockID];
            WaitingTextBlock.Style = textBlockStyle;

            string buttonID = "Button" + colorTheme.ToString() + "Style";
            Style buttonStyle = (Style)Application.Current.Resources[buttonID];
            BrowseCsvButton.Style = buttonStyle;
        }

        private void Page_DragEnter(object sender, DragEventArgs e)
        {

        }

        private void Page_DropCompleted(UIElement sender, DropCompletedEventArgs args)
        {
            
        }

        private void Page_Drop(object sender, DragEventArgs e)
        {

        }

        private void CsvFileLoadGrid_Drop(object sender, DragEventArgs e)
        {

        }
    }
}
