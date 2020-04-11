using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandsInteraction : MonoBehaviour
{
    [SerializeField]
    private Hands hands;

    [SerializeField]
    private Transform main;
    
    private Vector3 targetPos;
    private Quaternion targetRot;

    private void Start()
    {
        targetPos = main.position;
        targetRot = main.rotation;
    }
    
    private void Update()
    {
        UpdateTargets();
        UpdateTransform();
    }

    private void UpdateTransform()
    {
        main.position = Vector3.Lerp(main.position, targetPos, Time.deltaTime * 10);
        main.rotation = Quaternion.Lerp(main.rotation, targetRot, Time.deltaTime * 10);
    }

    private void UpdateTargets()
    {
        Vector3 index = hands.LeftHandProxy.IndexTip.position;
        Vector3 thumb = hands.LeftHandProxy.ThumbTip.position;
        float pinchDist = (index - thumb).magnitude;
        if (pinchDist < .03f)
        {
            Transform palm = hands.LeftHandProxy.Palm;
            targetPos = palm.position + palm.forward * .1f;
            Vector3 heading = new Vector3(palm.forward.x, 0, palm.forward.z).normalized;
            targetRot = Quaternion.LookRotation(palm.forward);
        }
    }
}
