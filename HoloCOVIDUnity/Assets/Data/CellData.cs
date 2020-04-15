using System;
using UnityEngine;

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

    public float GetCovidValue(float normalizeTime)
    {
        float unNormalized = normalizeTime * CovidTimeline.Length;
        int highIndex = Mathf.CeilToInt(unNormalized);
        if(highIndex == CovidTimeline.Length)
        {
            highIndex--;
        }
        int lowIndex = Mathf.FloorToInt(unNormalized);
        if (lowIndex == CovidTimeline.Length)
        {
            lowIndex--;
        }
        CovidDayData highDay = CovidTimeline[highIndex];
        CovidDayData lowDay = CovidTimeline[lowIndex];
        float subDay = unNormalized % 1;

        return Mathf.Lerp(lowDay.Deaths, highDay.Deaths, subDay);
    }

    public void AddCaseData(int day, float confirmed, float deaths, float recovered)
    {
        CovidTimeline[day].Confirmed += confirmed;
        CovidTimeline[day].Deaths += deaths;
        CovidTimeline[day].Recovered += recovered;
    }
}
