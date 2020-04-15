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
    }
}
