using UnityEngine;

public interface IFocusableItem
{
    FocusSource Source { get; }
    bool IsFocusable { get; }
    FocusPriority Priority { get; }
    float ActivationDistance { get; }
    float GetDistanceToPointer(Vector3 pointerPos);
    bool ForceFocus { get; set; }
}