using UnityEngine;

public class BoxFocusable : FocusableItemBehavior
{
    [SerializeField]
    private BoxCollider box;
    public BoxCollider Box { get { return this.box; } }

    private void Start()
    {
        FocusManager.Instance.RegisterFocusable(this);
    }

    public override float GetDistanceToPointer(Vector3 pointerPos)
    {
        if(box.bounds.Contains(pointerPos))
        {
            return 0;
        }
        Vector3 closest = box.ClosestPoint(pointerPos);
        return (closest - pointerPos).magnitude;
    }
}
