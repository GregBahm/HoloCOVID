using System;
using UnityEngine;

public class VisualizationButton : MonoBehaviour
{
    [SerializeField]
    private SkinnedMeshRenderer renderer;

    [SerializeField]
    private Transform titleText;

    private float pressedness;

    private Vector3 basePosition;
    [SerializeField]
    private Vector3 expandedPosition;
    [SerializeField]
    private Vector3 pushedPosition;

    [SerializeField]
    private Vector3 expandedTitlePosition;

    private Vector3 targetPosition;
    private Vector3 baseTitlePosition;

    private void Start()
    {
        basePosition = transform.localPosition;
        baseTitlePosition = titleText.localPosition;
    }

    public void DoUpdate(bool isPressed, bool isPushed)
    {
        float blendTarget = isPressed ? 1 : 0;
        pressedness = Mathf.Lerp(pressedness, blendTarget, Time.deltaTime * 4f);
        renderer.SetBlendShapeWeight(0, pressedness * 100);
        renderer.SetBlendShapeWeight(1, pressedness * 100);

        targetPosition = GetTargetPosition(isPressed, isPushed);
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * 4f);

        Vector3 targetTitlePosition = isPressed ? expandedTitlePosition : baseTitlePosition;
        titleText.localPosition = Vector3.Lerp(titleText.localPosition, targetTitlePosition, Time.deltaTime * 4f);
    }

    private Vector3 GetTargetPosition(bool isPressed, bool isPushed)
    {
        if(isPressed)
        {
            return expandedPosition;
        }
        if(isPushed)
        {
            return pushedPosition;
        }
        return basePosition;
    }
}
