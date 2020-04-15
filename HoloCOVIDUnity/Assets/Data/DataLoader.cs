using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class DataLoader : MonoBehaviour
{
    [SerializeField]
    private int width;
    [SerializeField]
    private int height;
    [SerializeField]
    private TextAsset populationMap;
    [SerializeField]
    private TextAsset[] ageMaps;
    [SerializeField]
    private TextAsset nationIdsMap;
    [SerializeField]
    private TextAsset nationIdsTable;
    [SerializeField]
    private TextAsset covidReport;
    public SourceData GetData()
    {
        return new SourceData(width, 
            height, 
            populationMap.text, 
            ageMaps.Select(item => item.text).ToArray(), 
            nationIdsMap.text, 
            nationIdsTable.text,
            covidReport.text);
    }
}
