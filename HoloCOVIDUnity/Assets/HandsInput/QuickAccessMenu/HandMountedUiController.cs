using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandMountedUiController : MonoBehaviour
{
    public float Smoothing;
    public float SummonTime;
    private float currentSummonTime;
    public float Summonedness { get; private set; }
    public SummonDetector Summoning;
    public UiPositionCore PositionCore;

    public Transform HandContent;
    public Transform HandRotationPivot;
    
    private void UpdatePrimaryVisibility()
    {
        HandContent.gameObject.SetActive(Summonedness > .1f);
    }

    private void UpdatePosition()
    {
        Vector3 positionTarget = Vector3.Lerp(HandContent.position, PositionCore.CoreTransform.position, Time.deltaTime * Smoothing);
        HandContent.position = positionTarget;
        HandContent.LookAt(Camera.main.transform.position, Vector3.up);
        HandRotationPivot.LookAt(Camera.main.transform.position, Vector3.up);
    }

    private void Update()
    {
        UpdateSummonedness();
        UpdatePrimaryVisibility();
        UpdatePosition();
    }

    private void UpdateSummonedness()
    {
        float summonednessTarget = Summoning.IsSummoned ? 1 : 0;
        Summonedness = Mathf.Lerp(Summonedness, summonednessTarget, Time.deltaTime * 10);
    }
}
