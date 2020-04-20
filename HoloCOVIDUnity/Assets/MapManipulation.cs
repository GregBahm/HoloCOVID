using UnityEngine;

public class MapManipulation : MonoBehaviour
{
    [SerializeField]
    private Hands hands;

    [SerializeField]
    private BoxFocusable mainMapFocus;

    [SerializeField]
    private Transform mainTransform;

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
    }

    private void UpdateTransform()
    {
        mainTransform.position = Vector3.Lerp(mainTransform.position, helper.position, Time.deltaTime * 10);
        mainTransform.rotation = Quaternion.Lerp(mainTransform.rotation, helper.rotation, Time.deltaTime * 10);
    }

    private void UpdateTransformTargets()
    {
        if (ShouldStartPinch())
        {
            helper.position = mainTransform.position;
            helper.rotation = mainTransform.rotation;
            helper.SetParent(hands.RightHandProxy.Palm, true);
        }
        if(!PinchDetector.Instance.Pinching)
        {
            helper.SetParent(null, true);
        }
    }

    private bool ShouldStartPinch()
    {
        return FocusManager.Instance.FocusedItem == mainMapFocus
            && PinchDetector.Instance.PinchBeginning;
    }
}