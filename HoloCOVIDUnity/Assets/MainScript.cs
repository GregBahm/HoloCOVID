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

    [SerializeField]
    private Transform flatMapTransform;
    [SerializeField]
    private Transform globeMapTransform;

    private MeshRenderer meshRenderer;
    private ComputeBuffer populationDataBuffer;

    private SourceData data;

    [Range(0, 249)]
    public int NationToHighlight = 0;

    void Start()
    {
        data = dataLoader.GetData();
        populationDataBuffer = GetPopulationDataBuffer();
    }

    private void OnDestroy()
    {
        populationDataBuffer.Dispose();
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
        mat.SetBuffer("_PopulationData", populationDataBuffer);
        mat.SetFloat("_MaxValue", data.PeakCellPopulation);
        mat.SetMatrix("_FlatMapTransform", flatMapTransform.localToWorldMatrix);
        mat.SetMatrix("_GlobeTransform", globeMapTransform.localToWorldMatrix);

        int idToHlighlight = NationToHighlight == 0 ? -1 : data.Nations[NationToHighlight - 1].Id;
        mat.SetInt("_NationToHighlight", idToHlighlight);
        Graphics.DrawMeshInstancedProcedural(mesh, 0, mat, boundsSource.bounds, data.CellsToRender.Count);
    }
}
