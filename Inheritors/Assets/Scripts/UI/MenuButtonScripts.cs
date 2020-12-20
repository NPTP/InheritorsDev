using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class MenuButtonScripts : MonoBehaviour
{
    SceneLoader sceneLoader;
    Button continueButton;
    Text continueText;

    bool clickedButton = false;
    float cameraRiseTime = 10f;

    public GameObject defaultSelectedButton;
    public GameObject postGameObjects;
    public GameObject normalObjects;

    void Awake()
    {
        sceneLoader = GameObject.Find("SceneLoader").GetComponent<SceneLoader>();
        continueButton = GameObject.Find("ContinueButton").GetComponent<Button>();
        continueText = GameObject.Find("ContinueText").GetComponent<Text>();
    }

    void Start()
    {
        Cursor.visible = false;

        // Set up the scene differently if we've completed the game.
        if (PlayerPrefs.GetInt("CompletedGame", 0) == 1)
        {
            normalObjects.SetActive(false);
            postGameObjects.SetActive(true);
        }
        else
        {
            normalObjects.SetActive(true);
            postGameObjects.SetActive(false);
        }

        GameObject camera = GameObject.FindWithTag("MainCamera");
        camera.transform.DOMoveY(camera.transform.position.y, cameraRiseTime).From(0.8f).SetEase(Ease.OutQuad);

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

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(defaultSelectedButton);
        }
    }

    public void BeginGame()
    {
        if (!clickedButton)
        {
            clickedButton = true;
            sceneLoader.LoadSceneByName("Day0");
        }
    }

    public void ContinueGame()
    {
        if (!clickedButton)
        {
            clickedButton = true;

            PlayerPrefs.SetInt("continuing", 1);
            PlayerPrefs.Save();
            int savedDayNumber = PlayerPrefs.GetInt("savedDayNumber");
            print("savedDayNumber: " + savedDayNumber);
            string sceneName = "Day" + savedDayNumber.ToString();
            sceneLoader.LoadSceneByName(sceneName);
        }
    }

    public void ExitGame()
    {
        if (!clickedButton)
        {
            clickedButton = true;

            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
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
