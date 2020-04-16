using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonDetector : MonoBehaviour
{
    public UiPositionCore PositionCore;

    public bool IsSummoned { get; private set; }

    [Range(0, 1)]
    public float PalmSummonThreshold;
    [Range(0, 1)]
    public float PalmDismissThreshold;

    private void Update()
    {
        IsSummoned = GetPrimaryVisibility();
    }

    private bool GetPrimaryVisibility()
    {
        Vector3 toCamera = (PositionCore.CoreTransform.position - Camera.main.transform.position).normalized;
        float palmDot = Vector3.Dot(toCamera, PositionCore.CoreTransform.up);

        if (IsSummoned)
        {
            return palmDot > PalmDismissThreshold;
        }
        return palmDot > PalmSummonThreshold;
    }
}
