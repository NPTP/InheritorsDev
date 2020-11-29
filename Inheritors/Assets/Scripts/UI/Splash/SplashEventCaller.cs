using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashEventCaller : MonoBehaviour
{
    SplashScreen splashScreen;

    void Awake()
    {
        splashScreen = FindObjectOfType<SplashScreen>();
    }

    public void EndOfAnimation()
    {
        splashScreen.HandleEndOfAnimation();
    }
    
}
