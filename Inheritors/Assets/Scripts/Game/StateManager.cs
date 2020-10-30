using System;
using UnityEngine;

// State machine for the whole game, based primarily on player state.
public class StateManager : MonoBehaviour
{
    // SceneLoader sceneLoader;
    InputManager inputManager;
    InteractManager interactManager;
    DialogManager dialogManager;
    TaskManager taskManager;

    public StateManager.State state;
    public enum State
    {
        Normal,         /* Allow all controls, movement, interaction, etc. */
        Dialog,         /* Stop current movement, disallow further, allow dialog inputs only. */
        PickingUp,      /* In the process of interacting, disallow certain inputs and events. */
        Holding,        /* Has an item in hand. */
        Inert           /* Nothing allowed, static. */
    }
    public event EventHandler<StateArgs> OnState;
    public class StateArgs : EventArgs
    {
        public StateManager.State state;
    }

    void Start()
    {
        SetState(State.Normal);
        // sceneLoader = GameObject.Find("SceneLoader").GetComponent<SceneLoader>();
        inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
        interactManager = GameObject.Find("InteractManager").GetComponent<InteractManager>();
        dialogManager = GameObject.Find("DialogManager").GetComponent<DialogManager>();
        taskManager = GameObject.Find("TaskManager").GetComponent<TaskManager>();
    }

    // This method is for the other managers to tell us what state we're in.
    public void SetState(StateManager.State newState)
    {
        this.state = newState;
        Debug.Log("State was just set to: " + this.state);
        OnState?.Invoke(this, new StateArgs { state = newState });
    }

    public StateManager.State GetState()
    {
        return this.state;
    }

}
