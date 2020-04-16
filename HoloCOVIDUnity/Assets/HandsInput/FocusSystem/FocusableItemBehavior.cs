using UnityEngine;

public abstract class FocusableItemBehavior : MonoBehaviour, IFocusableItem
{
    [SerializeField]
    private FocusSource focusType = FocusSource.Fingertip;
    public FocusSource Source { get { return this.focusType; } }

    [SerializeField]
    private bool isFocusable = true;
    public bool IsFocusable
    {
        get
        {
            return this.isFocusable &&
                isActiveAndEnabled;
        }
        set { this.isFocusable = value; }
    }

    [SerializeField]
    private FocusPriority priority = FocusPriority.Medium;
    public FocusPriority Priority { get { return this.priority; } }

    [SerializeField]
    private float activationDistance = 0.03f;
    public float ActivationDistance { get => activationDistance; set => activationDistance = value; }

    private bool forceFocus;
    public bool ForceFocus
    {
        get
        {
            return forceFocus;
        }
        set
        {
            if(this.forceFocus != value)
            {
                this.forceFocus = value;
                if (FocusManager.Instance.FocusedItem != this)
                {
                    if(value)
                    {
                        if (FocusManager.Instance.FocusedItem != null &&
                            FocusManager.Instance.FocusedItem.ForceFocus)
                        {
                            FocusManager.Instance.FocusedItem.ForceFocus = false;
                        }
                        FocusManager.Instance.FocusedItem = this;
                    }
                }
            }
        }
    }

    public abstract float GetDistanceToPointer(Vector3 pointerPos);
}
