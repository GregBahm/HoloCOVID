public class CellData
{
    public float X { get; }
    public float Y { get; }
    public float Population { get; }
    public Nation Nation { get; }
    public AgeData AgeData { get; }

    public CovidDayData[] CovidTimeline { get; }

    public CellData(float x,
        float y,
        float population,
        Nation nation,
        AgeData ageData,
        int daysOfData)
    {
        X = x;
        Y = y;
        Population = population;
        Nation = nation;
        AgeData = ageData;
        CovidTimeline = new CovidDayData[daysOfData];
    }

    public BufferDataPoint GetBufferDatum()
    {
        return new BufferDataPoint(X, Y, Population, AgeData.TotalMaxMortality, Nation.Id);
    }

    public void AddCaseData(int day, int confirmed, int deaths, int recovered)
    {
        CovidTimeline[day].Confirmed += confirmed;
        CovidTimeline[day].Deaths += deaths;
        CovidTimeline[day].Recovered += recovered;
    }
}
