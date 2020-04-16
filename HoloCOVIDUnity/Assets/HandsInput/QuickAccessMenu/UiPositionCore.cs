using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiPositionCore : MonoBehaviour
{
    public Transform CoreTransform { get; private set; }

    private void Awake()
    {
        CoreTransform = new GameObject("UiPositionCore").transform;
    }

    private void Update()
    {
        UpdateCorePosition();
    }

    private void UpdateCorePosition()
    {
        CoreTransform.position = Hands.Instance.LeftHandProxy.Palm.position;
        Vector3 psuedoPalmForward = GetForwardVector();
        Vector3 psuedoPalmUp = GetUpVector(psuedoPalmForward);
        CoreTransform.rotation = Quaternion.LookRotation(psuedoPalmUp, -psuedoPalmForward);
    }
    private Vector3 GetUpVector(Vector3 forward)
    {
        Vector3 fingerTipAverage = (Hands.Instance.LeftHandProxy.IndexTip.position +
            Hands.Instance.LeftHandProxy.MiddleTip.position +
            Hands.Instance.LeftHandProxy.LittleTip.position +
            Hands.Instance.LeftHandProxy.RingTip.position) / 4;
        Vector3 toPalm = Hands.Instance.LeftHandProxy.Palm.position - fingerTipAverage;
        return Vector3.Cross(toPalm, forward);
    }


    private Vector3 GetForwardVector()
    {
        Vector3 pointA = (Hands.Instance.LeftHandProxy.IndexTip.position + Hands.Instance.LeftHandProxy.MiddleTip.position) / 2;
        Vector3 pointB = (Hands.Instance.LeftHandProxy.RingTip.position + Hands.Instance.LeftHandProxy.LittleTip.position) / 2;

        Vector3 toPalmA = pointA - Hands.Instance.LeftHandProxy.Palm.position;
        Vector3 toPalmB = pointB - Hands.Instance.LeftHandProxy.Palm.position;
        return Vector3.Cross(toPalmA, toPalmB);
    }

}
