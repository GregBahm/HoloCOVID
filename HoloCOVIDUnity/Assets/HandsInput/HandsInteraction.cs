using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandsInteraction : MonoBehaviour
{
    [SerializeField]
    private Transform mainTransform;

    [SerializeField]
    private MainScript mainScript;

    [SerializeField]
    private BoxButton MoveButton;
    [SerializeField]
    private BoxButton RotateButton;
    [SerializeField]
    private BoxButton ScaleButton;

    //[SerializeField]
    //private BoxButton FlatMapButton;
    //[SerializeField]
    //private BoxButton GlobeButton;

    [SerializeField]
    private BoxButton PopulationButton;
    [SerializeField]
    private BoxButton RiskButton;
    [SerializeField]
    private BoxButton CovidButton;

    public MapState CurrentMapState;

    public ToolState CurrentToolState;

    public VisualState CurrentVisualState;

    public enum VisualState
    {
        Population,
        Risk,
        Covid
    }

    public enum MapState
    {
        Flat,
        Sphere
    }

    public enum ToolState
    {
        Move,
        Rotate,
        Scale
    }

    private void Start()
    {
        MoveButton.Pressed += MoveButton_Pressed;
        RotateButton.Pressed += RotateButton_Pressed;
        ScaleButton.Pressed += ScaleButton_Pressed;

        //FlatMapButton.Pressed += FlatMapButton_Pressed;
        //GlobeButton.Pressed += GlobeButton_Pressed;

        PopulationButton.Pressed += PopulationButton_Pressed;
        RiskButton.Pressed += RiskButton_Pressed;
        CovidButton.Pressed += CovidButton_Pressed;
    }

    private void CovidButton_Pressed(object sender, EventArgs e)
    {
        CurrentVisualState = VisualState.Covid;
    }

    private void RiskButton_Pressed(object sender, EventArgs e)
    {
        CurrentVisualState = VisualState.Risk;
    }

    private void PopulationButton_Pressed(object sender, EventArgs e)
    {
        CurrentVisualState = VisualState.Population;
    }

    private void GlobeButton_Pressed(object sender, EventArgs e)
    {
        CurrentMapState = MapState.Sphere;
    }

    private void FlatMapButton_Pressed(object sender, EventArgs e)
    {
        CurrentMapState = MapState.Flat;
    }

    private void Update()
    {
        UpdateMainScript();
    }

    private void UpdateMainScript()
    {
        float globificationTarget = CurrentMapState == MapState.Flat ? 0 : 1;
        mainScript.Globification = Mathf.Lerp(mainScript.Globification, globificationTarget, Time.deltaTime * 4);

        float riskWeightTarget = CurrentVisualState == VisualState.Risk ? 1 : 0;
        mainScript.RiskWeight = Mathf.Lerp(mainScript.RiskWeight, riskWeightTarget, Time.deltaTime * 4);

        float covidWeightTarget = CurrentVisualState == VisualState.Covid ? 1 : 0;
        mainScript.CovidWeight = Mathf.Lerp(mainScript.CovidWeight, covidWeightTarget, Time.deltaTime * 4);
    }

    private void ScaleButton_Pressed(object sender, EventArgs e)
    {
        CurrentToolState = ToolState.Scale;
    }

    private void RotateButton_Pressed(object sender, EventArgs e)
    {
        CurrentToolState = ToolState.Rotate;
    }

    private void MoveButton_Pressed(object sender, EventArgs e)
    {
        CurrentToolState = ToolState.Move;
    }
}
