using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Abstract all inputs with this input manager class.
// This should be the only guy listening for input!
public class InputManager : MonoBehaviour
{
    void Update()
    {
        // Middle buttons
        if (Input.GetButtonDown("Start"))   Debug.Log("Start");
        if (Input.GetButtonDown("Back"))    Debug.Log("Back");

        // Right side buttons
        if (Input.GetButtonDown("A"))       Debug.Log("A");
        if (Input.GetButtonDown("B"))       Debug.Log("B");
        if (Input.GetButtonDown("X"))       Debug.Log("X");
        if (Input.GetButtonDown("Y"))       Debug.Log("Y");

        // Left stick
        if (Input.GetAxis("LeftStickHorizontal") != 0)  Debug.Log("Left Stick Horizontal axis");
        if (Input.GetAxis("LeftStickVertical") != 0)    Debug.Log("Left Stick Vertical axis");

        // Right stick
        if (Input.GetAxis("RightStickHorizontal") != 0) Debug.Log("Right Stick Horizontal axis");
        if (Input.GetAxis("RightStickVertical") != 0)   Debug.Log("Right Stick Vertical axis");

        // Stick clicks
        if (Input.GetButtonDown("LeftStickClick"))  Debug.Log("LeftStickClick");
        if (Input.GetButtonDown("RightStickClick")) Debug.Log("RightStickClick");

        // Triggers. Each goes from 0 -> 1.
        if (Input.GetAxis("LeftTrigger") > 0)   Debug.Log("Left Trigger: " + Input.GetAxis("LeftTrigger"));
        if (Input.GetAxis("RightTrigger") > 0)  Debug.Log("Right Trigger: " + Input.GetAxis("RightTrigger"));

        // DPad. +up/+right, -down/-left.
        if (Input.GetAxis("DPadHorizontal") != 0)    Debug.Log("DPadHorizontal: " + Input.GetAxis("DPadHorizontal"));
        if (Input.GetAxis("DPadVertical") != 0)      Debug.Log("DPadVertical: " + Input.GetAxis("DPadVertical"));

        // Bumpers
        if (Input.GetButtonDown("LeftBumper"))  Debug.Log("LeftBumper");
        if (Input.GetButtonDown("RightBumper")) Debug.Log("RightBumper");
    }
}
