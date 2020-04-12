using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class DataLoader : MonoBehaviour
{
    [SerializeField]
    private int rows;
    [SerializeField]
    private int columns;
    [SerializeField]
    private TextAsset populationMap;
    [SerializeField]
    private TextAsset[] ageMaps;
    [SerializeField]
    private TextAsset nationIdsMap;
    [SerializeField]
    private TextAsset nationIdsTable;

    public SourceData GetData()
    {
        return new SourceData(rows, 
            columns, 
            populationMap.text, 
            ageMaps.Select(item => item.text).ToArray(), 
            nationIdsMap.text, 
            nationIdsTable.text);
    }
}
