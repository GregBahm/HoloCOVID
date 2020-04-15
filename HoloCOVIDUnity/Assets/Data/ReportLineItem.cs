using System;

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
    public enum LineItemType
    {
        // This information is specific enough to target a single grid point
        Point,
        // This information is vague, and should be distributed by population into the nation of its position
        Nation
    }

    internal static ReportLineItem LoadFromLine(string line)
    {
        string[] splitLine = line.Split(',');
        int day = Convert.ToInt32(splitLine[0]);
        float latitude = Convert.ToSingle(splitLine[1]);
        float longitude = Convert.ToSingle(splitLine[2]);
        int confirmed = Convert.ToInt32(splitLine[3]);
        int deaths = Convert.ToInt32(splitLine[4]);
        int recovered = Convert.ToInt32(splitLine[5]);
        bool isPoint = Convert.ToBoolean(splitLine[6]);
        return new ReportLineItem(day,
            latitude,
            longitude,
            confirmed,
            deaths,
            recovered,
            isPoint ? LineItemType.Point : LineItemType.Nation);
    }
}