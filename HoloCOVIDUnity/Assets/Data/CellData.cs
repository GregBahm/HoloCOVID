public class CellData
{
    public float X { get; }
    public float Y { get; }
    public float Population { get; }
    public Nation Nation { get; }
    public AgeData AgeData { get; }

    public CellData(float x,
        float y,
        float population,
        Nation nation,
        AgeData ageData)
    {
        X = x;
        Y = y;
        Population = population;
        Nation = nation;
        AgeData = ageData;
    }

    public BufferDataPoint GetBufferDatum()
    {
        return new BufferDataPoint(X, Y, Population, Nation.Id);
    }
}
