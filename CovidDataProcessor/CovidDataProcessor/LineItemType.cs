namespace CovidDataProcessor
{
    public enum LineItemType
    {
        // This information is specific enough to target a single grid point
        Point,
        // This information is vague, and should be distributed by population into the nation of its position
        Nation
    }
}
