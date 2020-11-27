﻿/* INHERITORS by Nick Perrin (c) 2020 */
using System;
using UnityEngine;

// Abstracts inputs and sends events on input. Runs before Default Time in the script execution order.
// Currently supporting PC keyboard & X360 controller only.
// TODO: PC bindings must be fixed up via project settings input manager.
public class InputManager : MonoBehaviour
{
    StateManager stateManager;

    public event EventHandler<ButtonArgs> OnButtonDown;
    public class ButtonArgs : EventArgs
    {
        public int buttonCode;
    }

    // Button codes used for button down/up events.
    // Note that not all buttons are mapped to codes as we may not need them.
    public const int DPAD_LEFT = 0;
    public const int DPAD_RIGHT = 1;
    public const int DPAD_DOWN = 2;
    public const int DPAD_UP = 3;
    public const int BACK = 4;
    public const int START = 5;
    public const int A = 6;
    public const int B = 7;
    public const int X = 8;
    public const int Y = 9;

    // Joystick axes storage.
    [HideInInspector] public float leftStickHorizontal = 0;
    [HideInInspector] public float leftStickVertical = 0;
    [HideInInspector] public float rightStickHorizontal = 0;
    [HideInInspector] public float rightStickVertical = 0;

    // Condition for blocking all input regardless of State.
    bool allowInput = true;

    void Awake()
    {
        stateManager = FindObjectOfType<StateManager>();
        stateManager.OnState += HandleState;
    }

    void OnDestroy()
    {
        stateManager.OnState -= HandleState;
    }

    // Poll for input dependent on input allowed & current State.
    void Update()
    {
        if (allowInput)
        {
            switch (stateManager.GetState())
            {
                case State.Normal:
                    GetJoystickAxes();
                    GetMiddleButtons();
                    GetRightSidebuttons();
                    break;
                case State.PickingUp:
                    GetJoystickAxes();
                    GetMiddleButtons();
                    break;
                case State.Holding:
                    GetJoystickAxes();
                    GetMiddleButtons();
                    GetRightSidebuttons(); // We'll see if we need to block B later.
                    break;
                case State.DroppingOff:
                    GetJoystickAxes();
                    GetMiddleButtons();
                    break;
                case State.Dialog:
                    GetButtonDown("A", A);
                    break;
                case State.Inert:
                    break;
                default:
                    Debug.Log("Input manager trying to get input in unknown State.");
                    break;
            }
            /* Currently unused inputs. */
            // GetDPad();
            // GetTriggers();
            // GetJoystickButtons();
            // GetBumpers();
        }
    }

    // State handler ensures controls start & stop as necessary.
    private void HandleState(object sender, StateManager.StateArgs args)
    {
        switch (args.state)
        {
            case State.Normal:
                AllowInput();
                break;
            case State.Dialog:
                AllowInput();
                ZeroAxes();
                break;
            case State.PickingUp:
                AllowInput();
                break;
            case State.Holding:
                AllowInput();
                break;
            case State.DroppingOff:
                BlockInput();
                ZeroAxes();
                break;
            case State.Inert:
                BlockInput();
                ZeroAxes();
                break;
            default:
                Debug.Log("Input manager tried to handle unknown State event.");
                break;
        }
    }

    public void AllowInput()
    {
        allowInput = true;
    }

    public void BlockInput()
    {
        allowInput = false;
    }

    public bool IsLeftJoystickMoving()
    {
        return leftStickHorizontal != 0 || leftStickVertical != 0;
    }

    void GetJoystickAxes()
    {
        leftStickHorizontal = Input.GetAxis("LeftStickHorizontal");
        leftStickVertical = Input.GetAxis("LeftStickVertical");

        rightStickHorizontal = Input.GetAxis("RightStickHorizontal");
        rightStickVertical = Input.GetAxis("RightStickVertical");
    }

    public void ZeroAxes()
    {
        leftStickHorizontal = 0f;
        leftStickVertical = 0f;
        rightStickHorizontal = 0f;
        rightStickVertical = 0f;
    }

    void GetMiddleButtons()
    {
        GetButtonDown("Start", START);
        GetButtonDown("Back", BACK);
    }

    void GetRightSidebuttons()
    {
        GetButtonDown("A", A);
        GetButtonDown("B", B);
        GetButtonDown("X", X);
        GetButtonDown("Y", Y);
    }

    void GetButtonDown(string button, int code)
    {
        if (Input.GetButtonDown(button))
            OnButtonDown?.Invoke(this, new ButtonArgs { buttonCode = code });
    }

    void GetDPad()
    {
        // Horizontal is -left, +right.
        float h = Input.GetAxis("DPadHorizontal");
        // Vertical is -down, +up.
        float v = Input.GetAxis("DPadVertical");

        // We don't send the axis values, just digital outputs.
        if (h < 0)
            OnButtonDown?.Invoke(this, new ButtonArgs { buttonCode = DPAD_LEFT });
        if (h > 0)
            OnButtonDown?.Invoke(this, new ButtonArgs { buttonCode = DPAD_RIGHT });
        if (v < 0)
            OnButtonDown?.Invoke(this, new ButtonArgs { buttonCode = DPAD_DOWN });
        if (v > 0)
            OnButtonDown?.Invoke(this, new ButtonArgs { buttonCode = DPAD_UP });
    }

    // Not implemented.
    void GetJoystickButtons()
    {
        if (Input.GetButtonDown("LeftStickClick"))
            return;
        if (Input.GetButtonDown("RightStickClick"))
            return;
    }

    // Not implemented.
    void GetBumpers()
    {
        if (Input.GetButtonDown("LeftBumper"))
            return;
        if (Input.GetButtonDown("RightBumper"))
            return;
    }

    // Not implemented.
    void GetTriggers()
    {
        // Each goes from 0 -> 1.
        if (Input.GetAxis("LeftTrigger") > 0)
            return;
        if (Input.GetAxis("RightTrigger") > 0)
            return;
    }
}
