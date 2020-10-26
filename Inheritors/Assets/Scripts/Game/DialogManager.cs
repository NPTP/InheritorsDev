using System;
using UnityEngine;

// The dialog manager will send dialog events to the UI fully packaged and ready to be unpacked
// and read and delivered.
public class DialogManager : MonoBehaviour
{
    public event EventHandler<DialogArgs> OnDialog;
    public class DialogArgs : EventArgs
    {
        public string[] dialogLines;
    }

    void Start()
    {

    }

    void SendDialog(string[] lines)
    {
        OnDialog?.Invoke(this, new DialogArgs { dialogLines = lines });
    }

}
