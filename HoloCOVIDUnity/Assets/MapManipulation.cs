using System;
using UnityEngine;

public class MapManipulation : MonoBehaviour
{
    [SerializeField]
    private HandsInteraction interaction;

    [SerializeField]
    private Hands hands;

    [SerializeField]
    private BoxFocusable mainMapFocus;

    [SerializeField]
    private Transform mainTransform;

    private Transform rotationHelper;
    private Transform transitionHelper;
    
    private Vector3 startGrabPoint;
    private float startScale;

    private void Start()
    {
        rotationHelper = new GameObject("Rotation Helper").transform;
        transitionHelper = new GameObject("Transformation Helper").transform;

        transitionHelper.position = mainTransform.position;
    }

    private void Update()
    {
        if (ShouldStartPinch())
        {
            StartPinch();
        }
        rotationHelper.LookAt(PinchDetector.Instance.PinchPoint);

        if (PinchDetector.Instance.Pinching)
        {
            if (interaction.CurrentToolState == HandsInteraction.ToolState.Scale)
            {
                UpdateScale();
            }
        }

        if (!PinchDetector.Instance.Pinching)
        {
            transitionHelper.SetParent(null, true);
            mainTransform.parent = null;
        }
        mainTransform.position = Vector3.Lerp(mainTransform.position, transitionHelper.position, Time.deltaTime * 10);
    }

    private void UpdateScale()
    {
        float baseDist = (startGrabPoint - mainTransform.position).magnitude;
        float currentDist = (PinchDetector.Instance.PinchPoint.position - mainTransform.position).magnitude;
        float mod =  currentDist / baseDist;
        float newScale = startScale * mod;
        mainTransform.localScale = new Vector3(newScale, newScale, newScale);
    }

    private void StartPinch()
    {
        if(interaction.CurrentToolState == HandsInteraction.ToolState.Move)
        {
            transitionHelper.position = mainTransform.position;
            transitionHelper.SetParent(hands.RightHandProxy.Palm, true);
        }
        if(interaction.CurrentToolState == HandsInteraction.ToolState.Rotate)
        {
            rotationHelper.position = mainTransform.position;
            rotationHelper.LookAt(PinchDetector.Instance.PinchPoint);
            mainTransform.parent = rotationHelper;
        }
        startGrabPoint = PinchDetector.Instance.PinchPoint.position;
        startScale = mainTransform.localScale.x;
    }

    private bool ShouldStartPinch()
    {
        return FocusManager.Instance.FocusedItem == mainMapFocus
            && PinchDetector.Instance.PinchBeginning;
    }
}