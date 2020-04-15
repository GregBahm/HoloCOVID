public struct BufferDataPoint
{
    public float X { get; }
    public float Y { get; }
    public float Population { get; }
    public float MaxMortaility { get; }
    public int NationId { get; }

    public const int Stride = 
        sizeof(float) + // X
        sizeof(float) + // Y
        sizeof(float) + // Population
        sizeof(float) + // MaxMortality
        sizeof(int); // NationID

    public BufferDataPoint(float x, float y, float population, float maxMortality, int nationId)
    {
        X = x;
        Y = y;
        Population = population;
        MaxMortaility = maxMortality;
        NationId = nationId;
    }
}