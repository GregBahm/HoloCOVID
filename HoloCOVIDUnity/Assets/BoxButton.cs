using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxButton : MonoBehaviour
{
    [SerializeField]
    private BoxFocusable focus;

    public bool Toggled { get; set; }

    public event EventHandler Pressed;

    private State lastState;
    private State currentState;

    private enum State
    {
        OutOfFocus,
        HoveringOver,
        Pressing
    }

    private void Update()
    {
        UpdateState();
    }

    private void UpdateState()
    {
        if (FocusManager.Instance.FocusedItem == focus)
        {
            Vector3 fingerTip = Hands.Instance.RightHandProxy.IndexTip.position;
            Vector3 localFingerTip = transform.InverseTransformPoint(fingerTip);
            if (localFingerTip.z < 0)
            {
                currentState = State.HoveringOver;
            }
            else
            {
                if(currentState == State.HoveringOver || currentState == State.Pressing)
                {
                    currentState = State.Pressing;
                }
            }
        }
        else
        {
            currentState = State.OutOfFocus;
        }
        if (currentState == State.Pressing && lastState != State.Pressing)
        {
            OnPressed();
        }
        lastState = currentState;
    }

    private void OnPressed()
    {
        Pressed(this, EventArgs.Empty);
    }
}
