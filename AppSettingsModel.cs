using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.Storage;
using Windows.Storage.Streams;

namespace PortfolioAnalyst
{
    public class AppSettingsModel
    {
        public ColorThemeEnum ColorTheme { get; set; } = ColorThemeEnum.LIGHT;
        public string LanguageCode { get; set; } = "en-US";

        public SortedList<string, double> PositionPriceList { get; set; } = new SortedList<string, double>();


        private string SaveFileName { get; set; } = "AppSettings.xml";
        async public Task<bool> LoadSettingsFromXML()
        {
            StorageFile file = await OpenStorageFile(SaveFileName);

            XmlDocument xDoc = await LoadXMLDoc(file);

            XmlNode xAppSettings = GetAppSettingsNode(ref xDoc, file);

            XmlNode xColorTheme = GetColorThemeNode(ref xDoc, file);

            if (xColorTheme.InnerText == ColorThemeEnum.DARK.ToString())
                ColorTheme = ColorThemeEnum.DARK;
            else
                ColorTheme = ColorThemeEnum.LIGHT;

            XmlNode xLangCode = GetLangCodeNode(ref xDoc, file);
            LanguageCode = xLangCode.InnerText;

            ReadAllPositionPrices(xDoc, xAppSettings, file);


            return true;
        }

        public double GetPositionPrice(string positionName)
        {
            if (!PositionPriceList.ContainsKey(positionName))
                return 100;

            return PositionPriceList[positionName];
        }

        public void SetPositionPrice(string positionName, double price)
        {
            PositionPriceList[positionName] = price;
            SaveSettings();
        }

        async public void SaveSettings()
        {
            StorageFile file = await OpenStorageFile(SaveFileName);

            XmlDocument xdoc = await LoadXMLDoc(file);

            XmlNode xAppSettingsNode = GetAppSettingsNode(ref xdoc, file);

            while (xAppSettingsNode.HasChildNodes)
            {
                xAppSettingsNode.RemoveChild(xAppSettingsNode.FirstChild);
            }

            AppendAppSettingsElements(xdoc, ref xAppSettingsNode);

            SaveXMLDoc(xdoc, file);
        }

        private void ReadAllPositionPrices(XmlDocument xDoc, XmlNode xAppSettings, StorageFile file)
        {
            XmlNode xPriceList = GetPriceListNode(xDoc, xAppSettings, file);
            if (xPriceList.ChildNodes == null)
                return;

            foreach (XmlNode pos_i in xPriceList.ChildNodes)
            {
                string positionName = pos_i.Attributes["PositionName"].Value;
                double price = Convert.ToDouble(pos_i.InnerText);
                PositionPriceList.Add(positionName, price);
            }

        }

        private XmlNode GetPriceListNode(XmlDocument xDoc, XmlNode xAppSettings, StorageFile file)
        {
            XmlNode xPriceList;
            bool needToSave = false;
            
            xPriceList = xAppSettings.SelectSingleNode("child::PositionPrices[position()=1]");
            if (xPriceList == null)
            {
                xPriceList = xDoc.CreateElement("PositionPrices");
                xAppSettings.AppendChild(xPriceList);
                needToSave = true;
            }
            if (needToSave)
            {
                SaveXMLDoc(xDoc, file);
            }
            return xPriceList;
        }

        private void AppendAppSettingsElements(XmlDocument xdoc, ref XmlNode xAppSettingsNode)
        {
            //App Settings Elements
            XmlElement xColorTheme = xdoc.CreateElement("ColorTheme");
            xColorTheme.InnerText = ColorTheme.ToString();
            xAppSettingsNode.AppendChild(xColorTheme);

            XmlElement xElement = xdoc.CreateElement("LanguageCode");
            xElement.InnerText = LanguageCode;
            xAppSettingsNode.AppendChild(xElement);

            XmlElement xPriceList = xdoc.CreateElement("PositionPrices");
            AppendAllPositionPrices(xdoc, xPriceList, xAppSettingsNode);
            xAppSettingsNode.AppendChild(xPriceList);
        }

        private void AppendAllPositionPrices(XmlDocument xDoc, XmlElement xPriceList, XmlNode xAppSettingsNode)
        {
            foreach (KeyValuePair<string, double> posPrice in PositionPriceList)
            {
                XmlNode xPosPriceNode = xDoc.CreateElement("Position");
                xPosPriceNode.InnerText = Math.Round(posPrice.Value, 2).ToString();
                XmlAttribute xAttr = xDoc.CreateAttribute("PositionName");
                xAttr.Value = posPrice.Key;
                xPosPriceNode.Attributes.Append(xAttr);
                xPriceList.AppendChild(xPosPriceNode);
            }
        }

        /*static public void FindChannelNode(string channelName, string boardName, int canID, ref XmlDocument xDoc, ref XmlNode xChannelNode)
       {
           XmlNode xPortfolioAnalystNode, xAppSettings;


           try
           {
               xPortfolioAnalystNode = xDoc.SelectSingleNode("child::PortfolioAnalyst[position()=1]");
           }
           catch
           {
               xPortfolioAnalystNode = CreateRootNode(ref xDoc);
           }

           try
           {
               xAppSettings = xPortfolioAnalystNode.SelectSingleNode("child::AppSettings[position()=1]");
           }
           catch
           {
               xAppSettings = CreateAppSettingsNode(ref xDoc, ref xPortfolioAnalystNode);
           }


       }*/

        private XmlNode CreateRootNode(ref XmlDocument xDoc)
        {
            XmlNode xRoot = xDoc.CreateElement("PortfolioAnalyst");
            xDoc.AppendChild(xRoot);
            return xRoot;
        }

        private XmlNode CreateAppSettingsNode(ref XmlDocument xDoc, ref XmlNode xRootNode)
        {
            XmlNode xAppSettings = xDoc.CreateElement("AppSettings");
            xRootNode.AppendChild(xAppSettings);
            return xAppSettings;
        }

        private XmlNode CreateChannelSettingsNode(ref XmlDocument xDoc, ref XmlNode xRootNode)
        {
            XmlNode xChannelSettings = xDoc.CreateElement("ChannelSettings");
            xRootNode.AppendChild(xChannelSettings);
            return xChannelSettings;
        }

        async private Task<XmlDocument> LoadXMLDoc(StorageFile file)
        {
            XmlDocument xDoc = new XmlDocument();
            try
            {
                using (StreamReader sr = new StreamReader(file.Path, true))
                {
                    xDoc.Load(sr);
                }
                return xDoc;
            }
            catch
            {
                xDoc = new XmlDocument();
                XmlNode xRoot = CreateRootNode(ref xDoc);
                CreateAppSettingsNode(ref xDoc, ref xRoot);
                return xDoc;
            }
        }

        async private Task<int> SaveXMLDoc(XmlDocument xdoc, StorageFile file)
        {
            using (var stream = await file.OpenStreamForWriteAsync())
            {
                stream.SetLength(0);
            }

            IRandomAccessStream existingFileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);

            using (IOutputStream outputStream = existingFileStream.GetOutputStreamAt(0))
            {
                using (DataWriter dataWriter = new Windows.Storage.Streams.DataWriter(outputStream))
                {


                    dataWriter.WriteString(CreateXMLStringWithNewlines(ref xdoc));


                    await dataWriter.StoreAsync();
                    await outputStream.FlushAsync();
                }
            }
            existingFileStream.Dispose();
            return 0;
        }

        private string CreateXMLStringWithNewlines(ref XmlDocument doc)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace
            };
            using (XmlWriter writer = XmlWriter.Create(sb, settings))
            {
                doc.Save(writer);
            }
            return sb.ToString();
        }

        async private Task<StorageFile> OpenStorageFile(string fileName)
        {
            StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
            return file;
        }

        private XmlNode GetAppSettingsNode(ref XmlDocument xDoc, StorageFile file)
        {
            XmlNode xRoot, xAppSettings;
            bool needToSave = false;

            try
            {
                xRoot = xDoc.SelectSingleNode("child::PortfolioAnalyst[position()=1]");
            }
            catch
            {
                xRoot = xDoc.CreateElement("PortfolioAnalyst");
                xDoc.AppendChild(xRoot);
                needToSave = true;
            }


            xAppSettings = xRoot.SelectSingleNode("child::AppSettings[position()=1]");
            if (xAppSettings == null)
            {
                xAppSettings = xDoc.CreateElement("AppSettings");
                xRoot.AppendChild(xAppSettings);
                needToSave = true;
            }

            if (needToSave)
                SaveXMLDoc(xDoc, file);

            return xAppSettings;
        }

        /*static public XmlNode GetChannelSettingsNode(ref XmlDocument xDoc, StorageFile file)
        {
            XmlNode xJetINXAppNode, xChannelSettings;
            bool needToSave = false;

            try
            {
                xJetINXAppNode = xDoc.SelectSingleNode("child::JetINXApp[position()=1]");
            }
            catch
            {
                xJetINXAppNode = xDoc.CreateElement("JetINXApp");
                xDoc.AppendChild(xJetINXAppNode);
                needToSave = true;
            }

            try
            {
                xChannelSettings = xJetINXAppNode.SelectSingleNode("child::ChannelSettings[position()=1]");
            }
            catch
            {
                xChannelSettings = xDoc.CreateElement("ChannelSettings");
                xJetINXAppNode.AppendChild(xChannelSettings);
                needToSave = true;
            }

            if (needToSave)
                SaveXMLDoc(xDoc, file);

            return xChannelSettings;
        }*/

        private XmlNode GetColorThemeNode(ref XmlDocument xDoc, StorageFile file)
        {
            XmlNode xAppSettings = GetAppSettingsNode(ref xDoc, file);
            XmlNode xColorTheme;
            xColorTheme = xAppSettings.SelectSingleNode("child::ColorTheme[position()=1]");
            if (xColorTheme == null)
            {
                xColorTheme = xDoc.CreateElement("ColorTheme");
                xColorTheme.InnerText = ColorThemeEnum.LIGHT.ToString();
                xAppSettings.AppendChild(xColorTheme);
                SaveXMLDoc(xDoc, file);
            }

            return xColorTheme;
        }

        private XmlNode GetLangCodeNode(ref XmlDocument xDoc, StorageFile file)
        {
            XmlNode xAppSettings = GetAppSettingsNode(ref xDoc, file);
            XmlNode xLangCode = xAppSettings.SelectSingleNode("child::LanguageCode[position()=1]");
            if (xLangCode == null)
            {
                xLangCode = xDoc.CreateElement("LanguageCode");
                xLangCode.InnerText = "en-US";
                xAppSettings.AppendChild(xLangCode);
                SaveXMLDoc(xDoc, file);
            }

            return xLangCode;
        }


    }

    public enum ColorThemeEnum
    {
        LIGHT,
        DARK
    }
}
