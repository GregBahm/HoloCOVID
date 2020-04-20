using UnityEngine;

public class ToolButton : MonoBehaviour
{
    [SerializeField]
    private SkinnedMeshRenderer renderer;

    [SerializeField]
    private Transform iconContent;
    
    private float pressedness;
    
    public void DoUpdate(bool isPressed)
    {
        float blendTarget = isPressed ? 1 : 0;
        pressedness = Mathf.Lerp(pressedness, blendTarget, Time.deltaTime * 10);
        renderer.SetBlendShapeWeight(0, pressedness * 100);
    }
}