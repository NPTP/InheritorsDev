/* INHERITORS by Nick Perrin (c) 2020 */
using System;
using UnityEngine;

public enum State
{
    Normal,         /* Allow all controls, movement, interaction, etc. */
    Dialog,         /* Stop current movement, disallow further, allow dialog inputs only. */
    PickingUp,      /* In the process of interacting, disallow certain inputs and events. */
    DroppingOff,    /* In the process of interacting, disallow certain inputs and events. */
    Holding,        /* Has an item in hand. */
    Inert,          /* Nothing allowed, cutscenes, pause, etc. */
    Debug
}

// State machine for the whole game, based primarily on player state.
public class StateManager : MonoBehaviour
{
    public State state;

    public event EventHandler<StateArgs> OnState;
    public class StateArgs : EventArgs
    {
        public State state;
    }

    void Awake()
    {
        InitializeReferences();
    }

    void Start()
    {
        SetState(State.Normal);
    }

    // This method is for the other managers to tell us what state we're in.
    public void SetState(State newState)
    {
        this.state = newState;
        // Debug.Log(">>>>>>>>>> STATE SET TO: " + this.state);
        OnState?.Invoke(this, new StateArgs { state = newState });
    }

    public State GetState()
    {
        return this.state;
    }

    void InitializeReferences()
    {
        // None needed presently.
    }

}
