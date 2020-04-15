using System;

namespace CovidDataProcessor
{
    public class ReportLineItem
    {
        public int Day { get; }

        public float Latitude { get; }
        public float Longitude { get; }

        public int Confirmed { get; }
        public int Deaths { get; }
        public int Recovered { get; }

        public LineItemType LineType { get; }

        public ReportLineItem(int day, 
            float latitude, 
            float longitude, 
            int confirmed, 
            int deaths, 
            int recovered,
            LineItemType lineType)
        {
            Day = day;
            Latitude = latitude;
            Longitude = longitude;
            Confirmed = confirmed;
            Deaths = deaths;
            Recovered = recovered;
            LineType = lineType;
        }

        public string GetReportLine()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6}", Day, Latitude, Longitude, Confirmed, Deaths, Recovered, LineType == LineItemType.Point);
        }
    }
}
