using System;
using UnityEngine;

// Abstracts inputs and sends events on input. Runs before Default Time in the script execution order.
// Currently supporting PC keyboard & X360 controller only.
// TODO: PC bindings must be fixed up via project settings input manager.
public class InputManager : MonoBehaviour
{
    public event EventHandler<ButtonArgs> OnButtonDown;
    public class ButtonArgs : EventArgs
    {
        public int buttonCode;
    }

    // Button codes used for button down/up events.
    // Note that not all buttons are mapped to codes as we may not need them.
    public static int DPAD_LEFT = 0;
    public static int DPAD_RIGHT = 1;
    public static int DPAD_DOWN = 2;
    public static int DPAD_UP = 3;
    public static int BACK = 4;
    public static int START = 5;
    public static int A = 6;
    public static int B = 7;
    public static int X = 8;
    public static int Y = 9;

    // Joystick axes storage.
    public float leftStickHorizontal = 0;
    public float leftStickVertical = 0;
    public float rightStickHorizontal = 0;
    public float rightStickVertical = 0;

    // Condition for blocking input.
    bool allowInput = true;

    void Update()
    {
        if (allowInput)
        {
            /* Axis-based inputs */
            GetJoystickAxes();
            GetDPad();

            /* Button-based inputs */
            GetMiddleButtons();
            GetRightSidebuttons();

            /* Currently unused inputs. */
            // GetTriggers();
            // GetJoystickButtons();
            // GetBumpers();
        }
    }

    public void BlockInput()
    {
        allowInput = false;
    }

    public void AllowInput()
    {
        allowInput = true;
    }

    void GetJoystickAxes()
    {
        leftStickHorizontal = Input.GetAxis("LeftStickHorizontal");
        leftStickVertical = Input.GetAxis("LeftStickVertical");

        rightStickHorizontal = Input.GetAxis("RightStickHorizontal");
        rightStickVertical = Input.GetAxis("RightStickVertical");
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

    void GetMiddleButtons()
    {
        if (Input.GetButtonDown("Start"))
            OnButtonDown?.Invoke(this, new ButtonArgs { buttonCode = START });
        if (Input.GetButtonDown("Back"))
            OnButtonDown?.Invoke(this, new ButtonArgs { buttonCode = BACK });
    }

    void GetRightSidebuttons()
    {
        if (Input.GetButtonDown("A"))
            OnButtonDown?.Invoke(this, new ButtonArgs { buttonCode = A });
        if (Input.GetButtonDown("B"))
            OnButtonDown?.Invoke(this, new ButtonArgs { buttonCode = B });
        if (Input.GetButtonDown("X"))
            OnButtonDown?.Invoke(this, new ButtonArgs { buttonCode = X });
        if (Input.GetButtonDown("Y"))
            OnButtonDown?.Invoke(this, new ButtonArgs { buttonCode = Y });
    }


    void GetJoystickButtons()
    {
        if (Input.GetButtonDown("LeftStickClick"))
            return;
        if (Input.GetButtonDown("RightStickClick"))
            return;
    }

    void GetBumpers()
    {
        if (Input.GetButtonDown("LeftBumper"))
            return;
        if (Input.GetButtonDown("RightBumper"))
            return;
    }

    void GetTriggers()
    {
        // Each goes from 0 -> 1.
        if (Input.GetAxis("LeftTrigger") > 0)
            return;
        if (Input.GetAxis("RightTrigger") > 0)
            return;
    }
}
