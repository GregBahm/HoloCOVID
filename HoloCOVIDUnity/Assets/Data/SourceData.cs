using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class SourceData
{
    public int Rows { get; }
    public int Columns { get; }
    private readonly ReadOnlyDictionary<int, Nation> nationTable;
    public Nation[] Nations { get; }

    public ReadOnlyCollection<CellData> CellsToRender { get; }

    public float PeakCellPopulation { get; }

    public SourceData(int rows,
        int columns,
        string populationMapSource,
        string[] ageMapsSource,
        string nationIdsMapSource,
        string nationIdsTableSource)
    {
        Rows = rows;
        Columns = columns;
        nationTable = GetNationsTable(nationIdsTableSource);
        float[,] populationData = LoadFloats(populationMapSource);
        AgeData[,] ageData = GetAgeData(ageMapsSource);
        int[,] nationData = LoadInts(nationIdsMapSource);
        CellsToRender = GetGridData(populationData, ageData, nationData).ToList().AsReadOnly();
        PeakCellPopulation = CellsToRender.Max(item => item.Population);
        Nations = nationTable.Values.ToArray();
    }

    private float[,] LoadFloats(string dataTable)
    {
        float[,] ret = new float[Columns, Rows];
        string[] lines = dataTable.Split('\n');
        for (int y = 0; y < Rows; y++)
        {
            string[] cells = lines[y].Split(' ');
            for (int x = 0; x < Columns; x++)
            {
                ret[x, y] = Convert.ToSingle(cells[x]);
            }
        }
        return ret;
    }

    private AgeData[,] GetAgeData(string[] ageMapsSource)
    {
        AgeData[,] ret = new AgeData[Columns, Rows];
        float[][,] items = ageMapsSource.Select(item => LoadFloats(item)).ToArray();
        for (int x = 0; x < Columns; x++)
        {
            for (int y = 0; y < Rows; y++)
            {
                IEnumerable<float> ageDatum = items.Select(item => item[x, y]);
                ret[x, y] = new AgeData(ageDatum);
            }
        }
        return ret;
    }
    
    private int[,] LoadInts(string nationIdsMapSource)
    {
        int[,] ret = new int[Columns, Rows];
        string[] lines = nationIdsMapSource.Split('\n');
        for (int y = 0; y < Rows; y++)
        {
            string[] cells = lines[y].Split(' ');
            for (int x = 0; x < Columns; x++)
            {
                ret[x, y] = Convert.ToInt32(cells[x]);
            }
        }
        return ret;
    }

    private IEnumerable<CellData> GetGridData(float[,] populationData, AgeData[,] ageData, int[,] nationData)
    {
        for (int x = 0; x < Columns; x++)
        {
            for (int y = 0; y < Rows; y++)
            {
                float population = populationData[x, y];
                if(population > 0)
                {
                    int nationalId = nationData[x, y];
                    Nation nation = nationTable[nationalId];
                    AgeData age = ageData[x, y];
                    float xVal = (float)x / Columns;
                    float yVal = (float)y / Rows;
                    yield return new CellData(xVal, yVal, population, nation, age);
                }
            }
        }
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
