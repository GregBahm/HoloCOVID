using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace CovidDataProcessor
{
    public class CoordinatesLoader
    {
        private static string GoogleMapsKey = File.ReadAllText(Program.DirectoryRoot + @"GoogleMapsKey.txt");

        public void CreateCoordsForAllData(string[] reportPaths)
        {
            Dictionary<string, Location> dictionary = new Dictionary<string, Location>();

            foreach (string reportPath in reportPaths)
            {
                CreateCoords(reportPath, dictionary);
            }
        }

        private void CreateCoords(string reportPath, Dictionary<string, Location> dictionary)
        {
            string[] report = File.ReadAllLines(reportPath);
            StringBuilder output = new StringBuilder("Header line\n");
            foreach (string line in report.Skip(1))
            {
                string[] lineComponents = GetLineComponents(line);
                if (lineComponents.Length > 0)
                {
                    if (lineComponents.Length < 7)
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
            if (quoteSplit.Length == 1)
            {
                return line.Split(',');
            }
            else if (quoteSplit.Length == 3)
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

        private static string GetKey(string province, string country)
        {
            if (string.IsNullOrWhiteSpace(province))
            {
                return country;
            }
            return province + ", " + country;
        }

        private static Location GetNewLocation(string locationDescription)
        {
            string firstPart = @"https://maps.googleapis.com/maps/api/geocode/xml?address=";
            string keyPart = @"&key=" + GoogleMapsKey;
            string url = firstPart + locationDescription + keyPart;
            Console.WriteLine("Getting location for " + locationDescription);
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
                return new Location(locationDescription, latitude, longitude);
            }
            catch
            {
                Console.WriteLine("***");
                Console.WriteLine("Failed to get data for " + locationDescription);
                Console.WriteLine("***");
                return new Location(locationDescription, 0, 0);
            }
        }

        private static string GetProcessedReportPath(string reportPath)
        {
            throw new NotImplementedException(); 
        }
    }
}


