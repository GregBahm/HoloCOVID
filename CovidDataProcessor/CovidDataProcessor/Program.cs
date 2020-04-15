using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace CovidDataProcessor
{
    class Program
    {
        private static string GoogleMapsKey = File.ReadAllText(@"C:\Users\Lisa\Documents\HoloCOVID\CovidDataProcessor\CovidDataProcessor\GoogleMapsKey.txt");

        static void Main(string[] args)
        {
            string[] earlyReportPaths = Directory.GetFiles(@"C:\Users\Lisa\Documents\HoloCOVID\CovidDataProcessor\CovidDataProcessor\ProcessedReports\EarlyReports");
            string[] lateReportPaths = Directory.GetFiles(@"C:\Users\Lisa\Documents\HoloCOVID\CovidDataProcessor\CovidDataProcessor\ProcessedReports\LaterReports");
            //CreateCoordsForAllData(reportPaths);
            GriddedMap map = CreateGriddedMapData(earlyReportPaths);
        }

        private class DataItem
        {

        }

        private class Location
        {
            public string Key { get; }
            public float Latitude { get; }
            public float Longitude { get; }

            public Location(string key, float latitude, float longitude)
            {
                Key = key;
                Latitude = latitude;
                Longitude = longitude;
            }
        }

        private static void CreateCoordsForAllData(string[] reportPaths)
        {
            Dictionary<string, Location> dictionary = new Dictionary<string, Location>();

            foreach (string reportPath in reportPaths)
            {
                CreateCoords(reportPath, dictionary);
            }
        }

        private static void CreateCoords(string reportPath, Dictionary<string, Location> dictionary)
        {
            string[] report = File.ReadAllLines(reportPath);
            StringBuilder output = new StringBuilder("Header line\n");
            foreach (string line in report.Skip(1))
            {
                string[] lineComponents = GetLineComponents(line);
                if (lineComponents.Length > 0)
                {
                    if(lineComponents.Length < 7)
                    {
                        string key = GetKey(lineComponents[0], lineComponents[1]);
                        if (!dictionary.ContainsKey(key))
                        {
                            Location newLocation = GetNewLocation(key);
                            dictionary.Add(key, newLocation);
                        }

                        Location location = dictionary[key];
                        string outputLine = line + "," + location.Latitude + "," + location.Longitude;
                        output.AppendLine(outputLine);
                    }
                    else
                    {
                        output.AppendLine(line);
                    }
                }
            }
            string outputPath = GetProcessedReportPath(reportPath);
            File.WriteAllText(outputPath, output.ToString());
        }

        private static string[] GetLineComponents(string line)
        {
            //US cities are "City, State" obnoxiously
            string[] quoteSplit = line.Split('"');
            if(quoteSplit.Length == 1)
            {
                return line.Split(',');
            }
            else if(quoteSplit.Length == 3)
            {
                List<string> ret = new List<string>() { quoteSplit[1] };
                ret.AddRange(quoteSplit[2].Split(',').Skip(1));
                return ret.ToArray();
            }
            else
            {
                throw new Exception("This data is weird: " + line);
            }
        }

        private static string GetProcessedReportPath(string reportPath)
        {
            return reportPath.Replace("csse_covid_19_daily_reports", "ProcessedReports");
        }

        private static Location GetNewLocation(string location)
        {
            string firstPart = @"https://maps.googleapis.com/maps/api/geocode/xml?address=";
            string keyPart = @"&key=" + GoogleMapsKey;
            string url = firstPart + location + keyPart;
            Console.WriteLine("Getting location for " + location);
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            Stream data = response.GetResponseStream();
            StreamReader reader = new StreamReader(data);

            string responseFromServer = reader.ReadToEnd();
            response.Close();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(responseFromServer);
            try
            {
                XmlNode locationNode = doc.DocumentElement.SelectSingleNode("result").SelectSingleNode("geometry").SelectSingleNode("location");
                string latText = locationNode.SelectSingleNode("lat").InnerText;
                string longText = locationNode.SelectSingleNode("lng").InnerText;
                float latitude = Convert.ToSingle(latText);
                float longitude = Convert.ToSingle(longText);

                Console.WriteLine("Location is " + latitude.ToString() + "," + longitude.ToString());
                return new Location(location, latitude, longitude);
            }
            catch
            {
                Console.WriteLine("***");
                Console.WriteLine("Failed to get data for " + location);
                Console.WriteLine("***");
                return new Location(location, 0, 0);
            }
        }

        private static string GetKey(string province, string country)
        {
            if(string.IsNullOrWhiteSpace(province))
            {
                return country;
            }
            return province + ", " + country;
        }

        private static GriddedMap CreateGriddedMapData(string[] reportPaths)
        {
            GriddedMap map = new GriddedMap(360, 180, reportPaths.Length);
            for (int i = 0; i < reportPaths.Length; i++)
            {
                string outputPath = GetProcessedReportPath(reportPaths[i]);
                IngestReport(map, outputPath, i);
            }
            return map;
        }

        private static void IngestReport(GriddedMap map, string reportPath, int day)
        {
            string[] report = File.ReadAllLines(reportPath);
            foreach (string line in report.Skip(1))
            {
                IngestReportLine(map, line, day);
            }
        }

        private static void IngestReportLine(GriddedMap map, string line, int day)
        {
            string[] lineComponents = GetLineComponents(line);
            if(lineComponents.Length > 0)
            {
                string rawConfirmed = lineComponents[3];
                int confirmed = LoadInt(rawConfirmed);

                string rawDeaths = lineComponents[4];
                int deaths = LoadInt(rawDeaths);

                string rawRecovered = lineComponents[5];
                int recovered = LoadInt(rawRecovered);

                string rawLatitude = lineComponents[6];
                float latitude = LoadFloat(rawLatitude);

                string rawLongitude = lineComponents[7];
                float longitude = LoadFloat(rawLongitude);

                map.AddData(latitude, longitude, day, confirmed, deaths, recovered);
            }
        }

        private static int LoadInt(string val)
        {
            if(string.IsNullOrWhiteSpace(val))
            {
                return 0;
            }
            return Convert.ToInt32(val);
        }

        private static float LoadFloat(string val)
        {
            if (string.IsNullOrWhiteSpace(val))
            {
                return 0;
            }
            return Convert.ToSingle(val);
        }
    }

    class GriddedMap
    {
        public GridCell[,] Cells { get; }

        public GriddedMap(int width, int height, int daysOfData)
        {
            Cells = CreateCells(width, height, daysOfData);
        }

        private GridCell[,] CreateCells(int width, int height, int daysOfData)
        {
            GridCell[,] ret = new GridCell[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    ret[x, y] = new GridCell(x, y, daysOfData);
                }
            }
            return ret;
        }

        public GridCell GetCell(float latitude, float longitude)
        {
            int y = GetIndexFromLatitude(latitude);
            int x = GetIndexFromLongitude(longitude);
            return Cells[x, y];
        }

        private int GetIndexFromLongitude(float longitude)
        {
            int ret = (int)(longitude + 180);
            return ret % 360;
        }

        private int GetIndexFromLatitude(float latitude)
        {
            int ret = (int)(latitude + 90);
            return ret % 180;
        }

        internal void AddData(float latitude, float longitude, int day, int confirmed, int deaths, int recovered)
        {
            int x = GetIndexFromLongitude(longitude);
            int y = GetIndexFromLatitude(latitude);
            Cells[x, y].AddCaseData(day, confirmed, deaths, recovered);
        }
    }

    class GridCell
    {
        public int X { get; }
        public int Y { get; }

        private readonly CellDayData[] timeline;

        public GridCell(int x, int y, int daysOfData)
        {
            X = x;
            Y = y;
            timeline = CreateTimeline(daysOfData);
        }

        private CellDayData[] CreateTimeline(int daysOfData)
        {
            CellDayData[] ret = new CellDayData[daysOfData];
            for (int i = 0; i < daysOfData; i++)
            {
                ret[i] = new CellDayData();
            }
            return ret;
        }

        public void AddCaseData(int day, int confirmed, int deaths, int recovered)
        {
            CellDayData datum = timeline[day];
            datum.Confirmed += confirmed;
            datum.Deaths += deaths;
            datum.Recovered += recovered;
        }
    }

    class CellDayData
    {
        public int Confirmed { get; set; }
        public int Deaths { get; set; }
        public int Recovered { get; set; }
    }
}
