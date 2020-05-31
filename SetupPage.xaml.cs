using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization;
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
    public sealed partial class SetupPage : Page
    {
        AppSettingsModel AppData;
        List<Language> LanguageList = new List<Language>();
        List<String> LanguageStringList = new List<String>();
        private Windows.ApplicationModel.Resources.ResourceLoader ResourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();

        public SetupPage()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            AppData = (AppSettingsModel)e.Parameter;
            InitializeAppData();
        }
        public void InitializeAppData()
        {
            SelectExistingColorTheme();
            Language english = new Language("en-US");
            //Language german = new Language("de-DE");
            LanguageList.Add(english);
            //LanguageList.Add(german);

            foreach (Language langString in LanguageList)
            {
                LanguageStringList.Add(langString.DisplayName);
            }

            if (AppData.LanguageCode == "en-US")
            {
                LanguageComboBox.SelectedIndex = 0;
            }
            else if (AppData.LanguageCode == "de-DE")
            {
                LanguageComboBox.SelectedIndex = 1;
            }
        }
        private void SelectExistingColorTheme()
        {
            if (AppData.ColorTheme == ColorThemeEnum.LIGHT)
                LightModeRadioButton.IsChecked = true;
            else if (AppData.ColorTheme == ColorThemeEnum.DARK)
                DarkModeRadioButton.IsChecked = true;

        }
        private void LightModeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            AppData.ColorTheme = ColorThemeEnum.LIGHT;
            AppData.SaveSettings();
        }
        private void DarkModeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            AppData.ColorTheme = ColorThemeEnum.DARK;
            AppData.SaveSettings();
        }
        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = LanguageComboBox.SelectedIndex;

            if (index < 0)
                return;

            AppData.LanguageCode = LanguageList[index].LanguageTag;
            AppData.SaveSettings();

            //ApplicationLanguages.PrimaryLanguageOverride = LanguageList[index].LanguageTag;
        }

    }
}
