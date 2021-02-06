using UnityEngine;

public class ReturningFromGame : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(this);
    }
}
