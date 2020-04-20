using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandUiVisualController : MonoBehaviour
{
    [SerializeField]
    private HandsInteraction interaction;

    [SerializeField]
    private ToolButton MoveButton;

    [SerializeField]
    private ToolButton RotateButton;

    [SerializeField]
    private ToolButton ScaleButton;

    [SerializeField]
    private VisualizationButton PopulationButton;

    [SerializeField]
    private VisualizationButton RiskButton;

    [SerializeField]
    private VisualizationButton COVIDButton;

    [SerializeField]
    private Transform ProjectionHandle;

    [SerializeField]
    private Transform TimeHandle;
    
    private void Update()
    {
        MoveButton.DoUpdate(interaction.CurrentToolState == HandsInteraction.ToolState.Move);
        RotateButton.DoUpdate(interaction.CurrentToolState == HandsInteraction.ToolState.Rotate);
        ScaleButton.DoUpdate(interaction.CurrentToolState == HandsInteraction.ToolState.Scale);
        
        PopulationButton.DoUpdate(interaction.CurrentVisualState == HandsInteraction.VisualState.Population, false);
        RiskButton.DoUpdate(interaction.CurrentVisualState == HandsInteraction.VisualState.Risk, interaction.CurrentVisualState == HandsInteraction.VisualState.Covid);
        COVIDButton.DoUpdate(interaction.CurrentVisualState == HandsInteraction.VisualState.Covid, false);
    }
}
