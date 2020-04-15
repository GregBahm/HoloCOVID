using System;

namespace CovidDataProcessor
{
    class GriddedMap
    {
        public GridCell[,] Cells { get; }

        public GriddedMap(int width, int height, int daysOfData)
        {
            Cells = CreateCells(width, height, daysOfData);
        }

        private GridCell[,] CreateCells(int width, int height, int daysOfData)
        {
            GridCell[,] ret = new GridCell[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    ret[x, y] = new GridCell(x, y, daysOfData);
                }
            }
            return ret;
        }

        internal void LoadNations(string nationIdsPath, string populationsPath)
        {
            throw new NotImplementedException();
        }

        internal void AddData(ReportLineItem item)
        {
            throw new NotImplementedException();
        }

        public GridCell GetCell(float latitude, float longitude)
        {
            int y = GetIndexFromLatitude(latitude);
            int x = GetIndexFromLongitude(longitude);
            return Cells[x, y];
        }

        private int GetIndexFromLongitude(float longitude)
        {
            int ret = (int)(longitude + 180);
            return ret % 360;
        }

        private int GetIndexFromLatitude(float latitude)
        {
            int ret = (int)(latitude + 90);
            return ret % 180;
        }

        internal void AddData(float latitude, float longitude, int day, int confirmed, int deaths, int recovered)
        {
            int x = GetIndexFromLongitude(longitude);
            int y = GetIndexFromLatitude(latitude);
            Cells[x, y].AddCaseData(day, confirmed, deaths, recovered);
        }
    }
}
