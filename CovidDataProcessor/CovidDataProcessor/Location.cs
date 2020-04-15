namespace CovidDataProcessor
{
    public class Location
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
}
