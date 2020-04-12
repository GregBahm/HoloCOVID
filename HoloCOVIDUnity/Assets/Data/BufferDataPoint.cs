public struct BufferDataPoint
{
    public float X { get; }
    public float Y { get; }
    public float Population { get; }

    public const int Stride = sizeof(float) +
        sizeof(float) +
        sizeof(float);

    public BufferDataPoint(float x, float y, float population)
    {
        X = x;
        Y = y;
        Population = population;
    }
}