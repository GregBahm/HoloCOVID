using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FocusManager : MonoBehaviour
{
    public static FocusManager Instance;

    private readonly List<IFocusableItem> highPris = new List<IFocusableItem>();
    private readonly List<IFocusableItem> midPris = new List<IFocusableItem>();
    private readonly List<IFocusableItem> lowPris = new List<IFocusableItem>();
    
    
    public IFocusableItem FocusedItem { get; set; }

    public bool FocusForced
    {
        get { return FocusedItem != null && FocusedItem.ForceFocus; }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (!FocusForced)
        {
            FocusedItem = GetCurrentFocusedItem();
        }
    }

    public void RegisterFocusable(IFocusableItem focusable)
    {
        switch (focusable.Priority)
        {
            case FocusPriority.High:
                highPris.Add(focusable);
                break;
            case FocusPriority.Medium:
                midPris.Add(focusable);
                break;
            case FocusPriority.Low:
            default:
                lowPris.Add(focusable);
                break;
        }
    }

    private IFocusableItem GetCurrentFocusedItem()
    {
        IFocusableItem ret = TryGetFocusable(highPris);
        if (ret != null)
        {
            return ret;
        }

        ret = TryGetFocusable(midPris);
        if (ret != null)
        {
            return ret;
        }

        return TryGetFocusable(lowPris);
    }

    private IFocusableItem TryGetFocusable(IEnumerable<IFocusableItem> items)
    {
        Vector3 pokePoint = Hands.Instance.RightHandProxy.IndexTip.position;
        Vector3 grabPoint = PinchDetector.Instance.PinchPoint.position;

        IFocusableItem ret = null;
        float closestDistance = float.PositiveInfinity;
        foreach (IFocusableItem item in items.Where(item => item.IsFocusable))
        {
            Vector3 pointer = item.Source == FocusSource.Fingertip ? pokePoint : grabPoint;
            float dist = item.GetDistanceToPointer(pointer);
            if (dist <= item.ActivationDistance && dist < closestDistance)
            {
                ret = item;
                closestDistance = dist;
            }
        }
        return ret;
    }
}
