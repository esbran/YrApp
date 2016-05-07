using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using RestSharp;

namespace YrApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string url = "http://www.yr.no/place/Norway/Oslo/Oslo/Oslo/forecast.xml";
        WeatherData weather; 
        private void button1_Click(object sender, EventArgs e)
        {

            
            richTextBox1.Text = GetWeatherXml();
            weather = GetWeatherData();
            label1.Text = string.Format("Temp: {0}", weather.WeatherItems[0].Temp);
            label2.Text = string.Format("Wind: {0}", weather.WeatherItems[0].WindSpeedName);
            label3.Text = string.Format("Precipitation: {0}", weather.WeatherItems[0].PrecipitationValue);
            label4.Text = string.Format("Time: {0}", weather.WeatherItems[0].TimeFrom);
        }
        public string GetWeatherXml()
        {
            var client = new RestClient(url);
            var request = new RestRequest(Method.GET);
            return client.Execute(request).Content;
        }

        public WeatherData GetWeatherData()
        {
            WeatherData Data = new WeatherData() { WeatherItems = new List<WeatherItem>() };

            StringBuilder sb = new StringBuilder("");

            System.Xml.XmlDocument doc = new XmlDocument();

            doc.LoadXml(GetWeatherXml());

            System.Xml.XmlNode sun = doc.SelectSingleNode("/weatherdata/sun");
            System.Xml.XmlNode location = doc.SelectSingleNode("/weatherdata/location");
            System.Xml.XmlNodeList nodelist = doc.SelectNodes("/weatherdata/forecast/tabular/time");

            Data.SunRise = sun.Attributes["rise"].Value;
            Data.SunSet = sun.Attributes["set"].Value;
            Data.IsFilled = true;

            Data.LocationName = location["name"].Value;
            Data.LocationType = location["type"].Value;
            Data.LocationCountry = location["country"].Value;

            Data.LocationLatitude = location["location"].Attributes["latitude"].Value;
            Data.LocationLongitude = location["location"].Attributes["longitude"].Value;

            for (int i = 0; i < 7; i++)
            {
                WeatherItem item = new WeatherItem();

                item.Period = nodelist[i].Attributes["period"].Value;
                item.TimeFrom = DateTime.Parse(nodelist[i].Attributes["from"].Value).ToString("yyyy-MM-dd HH:mm");
                item.TimeTo = DateTime.Parse(nodelist[i].Attributes["to"].Value).ToString("yyyy-MM-dd HH:mm");
                item.SymbolNumber = nodelist[i]["symbol"].Attributes["number"].Value.PadLeft(2, '0');
                item.SymbolName = nodelist[i]["symbol"].Attributes["name"].Value;
                item.Temp = nodelist[i]["temperature"].Attributes["value"].Value + "C";
                item.PrecipitationValue = nodelist[i]["precipitation"].Attributes["value"].Value;
                item.WindSpeedMPS = nodelist[i]["windSpeed"].Attributes["mps"].Value;
                item.WindSpeedName = nodelist[i]["windSpeed"].Attributes["name"].Value;
                item.WindDirectionCode = nodelist[i]["windDirection"].Attributes["code"].Value;
                item.WindDirectionDeg = nodelist[i]["windDirection"].Attributes["deg"].Value;
                item.WindDirectionName = nodelist[i]["windDirection"].Attributes["name"].Value;

                item.IsFilled = true;

                Data.WeatherItems.Add(item);
            }

            return Data;
        }


        public class WeatherItem
        {
            public string TimeFrom { get; set; }
            public string TimeTo { get; set; }

            public string Period { get; set; }

            public string SymbolNumber { get; set; }
            public string SymbolName { get; set; }
            public string SymbolVar { get; set; }

            public string PrecipitationValue { get; set; }

            public string WindDirectionDeg { get; set; }
            public string WindDirectionCode { get; set; }
            public string WindDirectionName { get; set; }
            public string WindSpeedMPS { get; set; }
            public string WindSpeedName { get; set; }

            public string Temp { get; set; }
            public string Pressure { get; set; }

            public bool IsFilled { get; set; }

        }

        public class WeatherData
        {
            public string LocationName { get; set; }
            public string LocationType { get; set; }
            public string LocationCountry { get; set; }
            public string LocationLatitude { get; set; }
            public string LocationLongitude { get; set; }

            public string SunRise { get; set; }
            public string SunSet { get; set; }

            public List<WeatherItem> WeatherItems { get; set; }

            public bool IsFilled { get; set; }

            public WeatherData()
            {
                WeatherItems = new List<WeatherItem>();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
