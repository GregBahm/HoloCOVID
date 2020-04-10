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
    private TextAsset populationDataSource;

    [SerializeField]
    private Material mat;

    [SerializeField]
    private Mesh mesh;

    private MeshRenderer meshRenderer;
    private ComputeBuffer populationDataBuffer;

    private float maxPopValue;

    void Start()
    {
        PopulationDataPoint[] populationData = GetPopulationData().ToArray();
        maxPopValue = populationData.Max(item => item.Population);
        populationDataBuffer = GetPopulationDataBuffer(populationData);
    }

    private void OnDestroy()
    {
        populationDataBuffer.Dispose();
    }

    private ComputeBuffer GetPopulationDataBuffer(PopulationDataPoint[] populationData)
    {
        int stride = sizeof(float) + sizeof(float) + sizeof(float);
        ComputeBuffer ret = new ComputeBuffer(populationData.Length, stride);
        ret.SetData(populationData);
        return ret;
    }

    private void Update()
    {
        mat.SetBuffer("_PopulationData", populationDataBuffer);
        mat.SetFloat("_MaxValue", maxPopValue);
        mat.SetMatrix("_MasterTransform", transform.localToWorldMatrix);
        Graphics.DrawMeshInstancedProcedural(mesh, 0, mat, boundsSource.bounds, 360 * 180);
    }

    private IEnumerable<PopulationDataPoint> GetPopulationData()
    {
        string[] lines = populationDataSource.text.Split('\n');
        for (int y = 0; y < DataY; y++)
        {
            string[] cells = lines[y].Split(' ');
            for (int x = 0; x < DataX; x++)
            {
                float populationVal = Convert.ToSingle(cells[x]);
                
                if(populationVal > 0)
                {
                    float retX = (float)x / DataX;
                    float retY = (float)y / DataY;
                    yield return new PopulationDataPoint(retX, retY, populationVal);
                }
            }
        }
    }

    private struct PopulationDataPoint
    {
        public float X { get; }
        public float Y { get; }
        public float Population { get; }

        public PopulationDataPoint(float x, float y, float population)
        {
            X = x;
            Y = y;
            Population = population;
        }
    }
}
