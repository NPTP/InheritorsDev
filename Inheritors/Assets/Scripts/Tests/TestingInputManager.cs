using UnityEngine;

// Tests buttons going down
public class TestingInputManager : MonoBehaviour
{
    InputManager inputManager;
    public bool lowFramerate = true;
    void Start()
    {
        //If activated, set framerate super low to test multiple inputs coming on the same frame.
        if (lowFramerate)
            Application.targetFrameRate = 5;

        inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
        inputManager.OnButtonDown += Handle_OnButtonDown; // Subscribe
    }

    private void Handle_OnButtonDown(object sender, InputManager.ButtonArgs args)
    {
        Debug.Log("Button pressed: " + args.buttonCode);
        Debug.Log(Time.frameCount);
    }

    void OnDestroy()
    {
        inputManager.OnButtonDown -= Handle_OnButtonDown; // Unsubscribe
    }
}




