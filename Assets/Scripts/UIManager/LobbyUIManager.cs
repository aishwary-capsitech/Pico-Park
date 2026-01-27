using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LobbyUIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject startPanel;     // Start
    public GameObject loadingPanel;   // loadingPanel
    public GameObject roomPanel;      // Canvas -> Image
    public GameObject gameLoader;
    public GameObject quitPanel;

    [Header("Start Button")]
    public Button startButton;        // Start_Button

    [Header("Loading")]
    public Slider loadingSlider;      // LoadingSlider
    public float loadingTime = 3f;

    void Start()
    {
        // Initial state
        startPanel.SetActive(true);
        loadingPanel.SetActive(false);
        roomPanel.SetActive(false);
        quitPanel.SetActive(false);

        loadingSlider.value = 0;

        startButton.onClick.AddListener(OnStartClicked);
    }

    void Update()
    {
        bool checkActive = roomPanel.activeSelf && !gameLoader.activeSelf;
        if ((startPanel.activeSelf || checkActive) && Input.GetKeyDown(KeyCode.Escape))
        {
            //QuitGame();
            quitPanel.SetActive(true);
        }
    }

    void OnStartClicked()
    {
        startPanel.SetActive(false);
        loadingPanel.SetActive(true);
        loadingSlider.value = 0;

        StartCoroutine(LoadingRoutine());
    }

    IEnumerator LoadingRoutine()
    {
        float t = 0f;

        while (t < loadingTime)
        {
            t += Time.deltaTime;
            loadingSlider.value = t / loadingTime;
            yield return null;
        }

        loadingSlider.value = 1f;

        yield return new WaitForSeconds(0.2f);

        loadingPanel.SetActive(false);
        roomPanel.SetActive(true);
    }

    public void QuitGame()

    {
#if UNITY_EDITOR

        UnityEditor.EditorApplication.isPlaying = false;

#else
	 	Application.Quit();
#endif

    }

    public void CloseQuitPanel()
    {
        quitPanel.SetActive(false);
    }
}
