namespace CovidDataProcessor
{
    class GridCell
    {
        public int X { get; }
        public int Y { get; }

        private readonly CellDayData[] timeline;

        public GridCell(int x, int y, int daysOfData)
        {
            X = x;
            Y = y;
            timeline = CreateTimeline(daysOfData);
        }

        private CellDayData[] CreateTimeline(int daysOfData)
        {
            CellDayData[] ret = new CellDayData[daysOfData];
            for (int i = 0; i < daysOfData; i++)
            {
                ret[i] = new CellDayData();
            }
            return ret;
        }

        public void AddCaseData(int day, int confirmed, int deaths, int recovered)
        {
            CellDayData datum = timeline[day];
            datum.Confirmed += confirmed;
            datum.Deaths += deaths;
            datum.Recovered += recovered;
        }
    }
}
