using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class SourceData
{
    public int Width { get; }
    public int Height { get; }
    private readonly ReadOnlyDictionary<int, Nation> nationTable;
    public Nation[] Nations { get; }

    public ReadOnlyCollection<CellData> CellsToRender { get; }

    public int DaysOfData { get; }

    public float MaxCellPopulation { get; }
    public float MaxCellMortality { get; }

    private readonly CellData[,] grid;

    public SourceData(int width,
        int height,
        string populationMapSource,
        string[] ageMapsSource,
        string nationIdsMapSource,
        string nationIdsTableSource,
        string covidReportSource)
    {
        IEnumerable<ReportLineItem> reportItems = LoadReportItems(covidReportSource).ToArray();
        DaysOfData = reportItems.Max(item => item.Day) + 1;
        Width = width;
        Height = height;
        nationTable = GetNationsTable(nationIdsTableSource);
        float[,] populationData = LoadFloats(populationMapSource);
        AgeData[,] ageData = GetAgeData(ageMapsSource);
        int[,] nationData = LoadInts(nationIdsMapSource);
        grid = GetGridData(populationData, ageData, nationData);
        CellsToRender = GetCellsToRender().ToList().AsReadOnly();
        MaxCellPopulation = CellsToRender.Max(item => item.Population);
        MaxCellMortality = CellsToRender.Max(item => item.AgeData.TotalMaxMortality);
        Nations = nationTable.Values.ToArray();

        FilterReportItemsIntoData(reportItems);
    }

    private void FilterReportItemsIntoData(IEnumerable<ReportLineItem> reportItems)
    {
        foreach (ReportLineItem item in reportItems)
        {
            int cellX = GetIndexFromLongitude(item.Longitude);
            int cellY = GetIndexFromLatitude(item.Latitude);

            CellData cell = grid[cellX, cellY];
            //TODO: Handle the populaion filtering
            cell.AddCaseData(item.Day, item.Confirmed, item.Deaths, item.Recovered);
        }
    }

    private IEnumerable<CellData> GetCellsToRender()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                CellData item = this.grid[x, y];
                if(item.Nation != null && item.Nation.Id != 32767) // water
                {
                    yield return item;
                }
            }
        }
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

    private IEnumerable<ReportLineItem> LoadReportItems(string covidReportSource)
    {
        foreach (string line in covidReportSource.Split('\n'))
        {
            yield return ReportLineItem.LoadFromLine(line);
        }
    }

    private float[,] LoadFloats(string dataTable)
    {
        float[,] ret = new float[Width, Height];
        string[] lines = dataTable.Split('\n');
        for (int y = 0; y < Height; y++)
        {
            string[] cells = lines[y].Split(' ');
            for (int x = 0; x < Width; x++)
            {
                ret[x, y] = Convert.ToSingle(cells[x]);
            }
        }
        return ret;
    }

    private AgeData[,] GetAgeData(string[] ageMapsSource)
    {
        AgeData[,] ret = new AgeData[Width, Height];
        float[][,] items = ageMapsSource.Select(item => LoadFloats(item)).ToArray();
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                IEnumerable<float> ageDatum = items.Select(item => item[x, y]);
                ret[x, y] = new AgeData(ageDatum);
            }
        }
        return ret;
    }
    
    private int[,] LoadInts(string nationIdsMapSource)
    {
        int[,] ret = new int[Width, Width];
        string[] lines = nationIdsMapSource.Split('\n');
        for (int y = 0; y < Height; y++)
        {
            string[] cells = lines[y].Split(' ');
            for (int x = 0; x < Width; x++)
            {
                ret[x, y] = Convert.ToInt32(cells[x]);
            }
        }
        return ret;
    }

    private CellData[,] GetGridData(float[,] populationData, AgeData[,] ageData, int[,] nationData)
    {
        CellData[,] ret = new CellData[Width, Height];

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                int nationalId = nationData[x, y];
                float population = Mathf.Max(0, populationData[x, y]);
                Nation nation = null;
                if (nationTable.ContainsKey(nationalId))
                {
                    nation = nationTable[nationalId];
                }
                AgeData age = ageData[x, y];
                float xVal = (float)x / Width;
                float yVal = (float)y / Height;
                ret[x,y] = new CellData(xVal, yVal, population, nation, age, DaysOfData);
            }
        }
        return ret;
    }

    private ReadOnlyDictionary<int, Nation> GetNationsTable(string nationIdsTableSource)
    {
        IEnumerable<Nation> nations = GetNations(nationIdsTableSource);
        Dictionary<int, Nation> dictionary = nations.ToDictionary(item => item.Id, item => item);
        return new ReadOnlyDictionary<int, Nation>(dictionary);
    }

    private List<Nation> GetNations(string nationIdsTable)
    {
        List<Nation> ret = new List<Nation>();
        string[] lines = nationIdsTable.Split('\n');
        foreach (string line in lines.Skip(1))
        {
            string[] cells = line.Split('\t');
            int id  = Convert.ToInt32(cells[0]);
            string name = cells[3];
            ret.Add(new Nation(name, id));
        }
        return ret;
    }
}
