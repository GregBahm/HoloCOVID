using UnityEngine;

public class PointFocusable : FocusableItemBehavior
{
    private void Start()
    {
        FocusManager.Instance.RegisterFocusable(this);
    }

    public override float GetDistanceToPointer(Vector3 pointerPos)
    {
        return (transform.position - pointerPos).magnitude;
    }
}
