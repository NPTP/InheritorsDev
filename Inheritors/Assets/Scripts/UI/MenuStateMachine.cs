using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MenuStateMachine : MonoBehaviour
{
    protected MenuState MenuState;

    public void SetState(MenuState menuState)
    {
        MenuState = menuState;
        StartCoroutine(menuState.Start());
    }
}