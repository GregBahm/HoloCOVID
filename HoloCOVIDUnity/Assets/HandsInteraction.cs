using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandsInteraction : MonoBehaviour
{
    [SerializeField]
    private Hands hands;

    [SerializeField]
    private Transform mainTransform;

    [SerializeField]
    private MainScript mainScript;
    
    private Transform helper;
    
    private void Start()
    {
        helper = new GameObject("Helper").transform;
        helper.position = mainTransform.position;
        helper.rotation = mainTransform.rotation;
    }
    
    private void Update()
    {
        UpdateTransformTargets();
        UpdateTransform();
        UpdateGlobification();
    }

    private Vector3 rightPinchStart;
    private bool wasRightHandPinching;
    float globificationStart;
    float globificationTarget;

    private void UpdateGlobification()
    {
        Vector3 index = hands.RightHandProxy.IndexTip.position;
        Vector3 thumb = hands.RightHandProxy.ThumbTip.position;
        Vector3 pinchPoint = (index + thumb) / 2;
        float pinchDist = (index - thumb).magnitude;
        bool pinching = pinchDist < .03f;
        if (pinching)
        {
            if(!wasRightHandPinching)
            {
                rightPinchStart = pinchPoint;
                globificationStart = mainScript.Globification;
            }
            Vector3 startCameraSpace = Camera.main.WorldToScreenPoint(rightPinchStart);
            Vector3 currentCameraSpace = Camera.main.WorldToScreenPoint(pinchPoint);
            float xMotion = startCameraSpace.x - currentCameraSpace.x;
            float change = xMotion / 200;
            globificationTarget = Mathf.Clamp01(globificationStart + change);
        }
        mainScript.Globification = Mathf.Lerp(mainScript.Globification, globificationTarget, Time.deltaTime * 5);
        wasRightHandPinching = pinching;
    }

    private void UpdateTransform()
    {
        mainTransform.position = Vector3.Lerp(mainTransform.position, helper.position, Time.deltaTime * 10);
        mainTransform.rotation = Quaternion.Lerp(mainTransform.rotation, helper.rotation, Time.deltaTime * 10);
    }
    bool wasLeftPinching;

    private void UpdateTransformTargets()
    {
        Vector3 index = hands.LeftHandProxy.IndexTip.position;
        Vector3 thumb = hands.LeftHandProxy.ThumbTip.position;
        float pinchDist = (index - thumb).magnitude;
        bool pinching = pinchDist < .03f;
        if (pinching)
        {
            if(!wasLeftPinching)
            {
                helper.position = mainTransform.position;
                helper.rotation = mainTransform.rotation;
                helper.SetParent(hands.LeftHandProxy.Palm, true);
            }
        }
        else
        {
            helper.SetParent(null, true);
        }
        wasLeftPinching = pinching;
    }
}
