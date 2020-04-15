using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainScript : MonoBehaviour
{
    private const int DataX = 360;
    private const int DataY = 180;

    [SerializeField]
    private MeshRenderer boundsSource;

    [SerializeField]
    private DataLoader dataLoader;

    [SerializeField]
    private Material mat;

    [SerializeField]
    private Mesh mesh;

    [Range(0, 1)]
    public float Globification;

    private float lastTimline;
    [Range(0, 1)]
    public float Timeline;

    [SerializeField]
    private Transform flatMapTransform;
    [SerializeField]
    private Transform globeMapTransform;

    private MeshRenderer meshRenderer;
    private ComputeBuffer populationDataBuffer;
    private ComputeBuffer covidDataBuffer;

    private SourceData data;

    [Range(0, 249)]
    public int NationToHighlight = 0;

    [Range(0, 1)]
    public float RiskWeight;
    [Range(0, 1)]
    public float CovidWeight;
    
    void Start()
    {
        data = dataLoader.GetData();
        populationDataBuffer = GetPopulationDataBuffer();
        covidDataBuffer = new ComputeBuffer(data.CellsToRender.Count, sizeof(float));
        SetCovidData();
    }
    
    private void OnDestroy()
    {
        populationDataBuffer.Dispose();
        covidDataBuffer.Dispose();
    }

    public void UpdateCovidData()
    {
        if(Mathf.Abs(Timeline - lastTimline) > float.Epsilon)
        {
            SetCovidData();
        }
        lastTimline = Timeline;
    }

    private void SetCovidData()
    {
        float[] covidData = data.CellsToRender.Select(item => item.GetCovidValue(Timeline)).ToArray();
        covidDataBuffer.SetData(covidData);
    }

    private ComputeBuffer GetPopulationDataBuffer()
    {
        BufferDataPoint[] populationData = data.CellsToRender.Select(item => item.GetBufferDatum()).ToArray();
        ComputeBuffer ret = new ComputeBuffer(populationData.Length, BufferDataPoint.Stride);
        ret.SetData(populationData);
        return ret;
    }

    private void Update()
    {
        UpdateCovidData();
        UpdateShaderProperties();
        Graphics.DrawMeshInstancedProcedural(mesh, 0, mat, boundsSource.bounds, data.CellsToRender.Count);
    }

    private void UpdateShaderProperties()
    {
        mat.SetBuffer("_PopulationData", populationDataBuffer);
        mat.SetBuffer("_CovidData", covidDataBuffer);
        mat.SetFloat("_MaxPop", data.MaxCellPopulation);
        mat.SetFloat("_MaxMortality", data.MaxCellMortality);
        mat.SetMatrix("_FlatMapTransform", flatMapTransform.localToWorldMatrix);
        mat.SetMatrix("_GlobeTransform", globeMapTransform.localToWorldMatrix);
        mat.SetFloat("_Globification", Globification);
        mat.SetFloat("_RiskWeight", RiskWeight);
        mat.SetFloat("_CovidWeight", CovidWeight);

        int idToHlighlight = NationToHighlight == 0 ? -1 : data.Nations[NationToHighlight - 1].Id;
        mat.SetInt("_NationToHighlight", idToHlighlight);
    }
}
