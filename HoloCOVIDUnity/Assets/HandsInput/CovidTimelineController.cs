using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CovidTimelineController : MonoBehaviour
{
    [SerializeField]
    private HandsInteraction handsInteraction;

    [SerializeField]
    private MainScript main;

    [SerializeField]
    private PointFocusable focus;

    [SerializeField]
    private Transform range;

    [SerializeField]
    private Transform handle;

    [SerializeField]
    private GameObject timelineUiRoot;

    private void Update()
    {
        timelineUiRoot.SetActive(handsInteraction.CurrentVisualState == HandsInteraction.VisualState.Covid);

        main.Timeline = GetCovidWeight();
    }

    private float GetCovidWeight()
    {
        float width = range.localScale.x;
        float pos = handle.localPosition.x;
        return (pos + width / 2) / width;
    }
}
