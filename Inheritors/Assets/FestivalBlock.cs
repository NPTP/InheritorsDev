using UnityEngine;

public class FestivalBlock : MonoBehaviour
{
    StateManager stateManager;
    DialogManager dialogManager;
    DialogContent dialogContent;

    void Start()
    {
        stateManager = FindObjectOfType<StateManager>();
        dialogManager = FindObjectOfType<DialogManager>();
        dialogContent = FindObjectOfType<Day4DialogContent>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && stateManager.GetState() == State.Normal)
        {
            dialogManager.NewDialog(dialogContent.Get("Grandmother_FestivalBlock"));
            Destroy(this);
        }
    }
}
