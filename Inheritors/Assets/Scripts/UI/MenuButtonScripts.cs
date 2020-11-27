using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MenuButtonScripts : MonoBehaviour
{
    SceneLoader sceneLoader;

    Button continueButton;
    Text continueText;

    void Awake()
    {
        sceneLoader = GameObject.Find("SceneLoader").GetComponent<SceneLoader>();

        continueButton = GameObject.Find("ContinueButton").GetComponent<Button>();
        continueText = GameObject.Find("ContinueText").GetComponent<Text>();
    }

    void Start()
    {
        Cursor.visible = true;
        GameObject camera = GameObject.FindWithTag("MainCamera");
        camera.transform.DOMoveY(camera.transform.position.y, 8f).From(0.8f).SetEase(Ease.OutQuad);

        // Resets player prefs in a build only.
        if (!Application.isEditor && !PlayerPrefs.HasKey("playedBefore"))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetInt("playedBefore", 1);
            PlayerPrefs.Save();
        }

        PlayerPrefs.DeleteKey("continuing");
        SetUpContinueButton();
    }

    public void BeginGame()
    {
        sceneLoader.LoadSceneByName("Day0");
    }

    public void ContinueGame()
    {
        PlayerPrefs.SetInt("continuing", 1);
        PlayerPrefs.Save();
        int savedDayNumber = PlayerPrefs.GetInt("savedDayNumber");
        print("savedDayNumber: " + savedDayNumber);
        string sceneName = "Day" + savedDayNumber.ToString();
        sceneLoader.LoadSceneByName(sceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    void SetUpContinueButton()
    {
        if (PlayerPrefs.HasKey("saveExists"))
        {
            continueButton.interactable = true;
            continueText.color = new Color(1f, 1f, 1f, 0.5f);
        }
        else
        {
            continueButton.interactable = false;
            continueText.color = new Color(80f / 255f, 80f / 255f, 80f / 255f, 0.5f);
        }
    }
}
