using System;
using System.Collections.Generic;
using System.Linq;

public class Nation
{
    public string Name { get; }
    public int Id { get; }

    public Nation(string name, int id)
    {
        Name = name;
        Id = id;
    }
}

public class NationDistributionMap
{
    Dictionary<Nation, IEnumerable<DistributionMapItem>> map;

    public NationDistributionMap(IEnumerable<Nation> nations, IEnumerable<CellData> cellsSource)
    {
        Dictionary<Nation, List<CellData>> sortedCells = GetSortedCells(nations, cellsSource);
        map = CreateMap(sortedCells);
    }

    public void DistributItem(Nation nation, ReportLineItem reportLine)
    {
        IEnumerable<DistributionMapItem> items = map[nation];
        foreach (DistributionMapItem item in items)
        {
            float confirmed = item.PopWeight * reportLine.Confirmed;
            float death = item.PopWeight * reportLine.Deaths;
            float recovered = item.PopWeight * reportLine.Recovered;
            item.Cell.AddCaseData(reportLine.Day, confirmed, death, recovered);
        }
    }

    private Dictionary<Nation, IEnumerable<DistributionMapItem>> CreateMap(Dictionary<Nation, List<CellData>> sortedCells)
    {
        Dictionary<Nation, IEnumerable<DistributionMapItem>> ret = new Dictionary<Nation, IEnumerable<DistributionMapItem>>();
        foreach (KeyValuePair<Nation, List<CellData>> item in sortedCells)
        {
            IEnumerable<DistributionMapItem> mapItems = CreateMapItems(item.Value);
            ret.Add(item.Key, mapItems);
        }
        return ret;
    }

    private IEnumerable<DistributionMapItem> CreateMapItems(IEnumerable<CellData> cells)
    {
        float totalPop = cells.Sum(item => item.Population);
        foreach (CellData item in cells)
        {
            float popWeight = item.Population / totalPop;
            yield return new DistributionMapItem(popWeight, item);
        }
    }

    private Dictionary<Nation, List<CellData>> GetSortedCells(IEnumerable<Nation> nations, IEnumerable<CellData> cellsSource)
    {
        List<CellData> remainingCells = new List<CellData>();
        Dictionary<Nation, List<CellData>> ret = new Dictionary<Nation, List<CellData>>();
        foreach (Nation nation in nations)
        {
            List<CellData> myCells = new List<CellData>();
            foreach (CellData cell in cellsSource)
            {
                if (nation == cell.Nation)
                {
                    myCells.Add(cell);
                }
                else
                {
                    remainingCells.Add(cell);
                }
            }
            ret.Add(nation, myCells);
            cellsSource = remainingCells;
            remainingCells = new List<CellData>();
        }
        return ret;
    }

    private class DistributionMapItem
    {
        public float PopWeight { get; }
        public CellData Cell { get; }

        public DistributionMapItem(float popWeight, CellData cell)
        {
            PopWeight = popWeight;
            Cell = cell;
        }
    }
}