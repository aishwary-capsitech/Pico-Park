using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class LobbyUIManager : MonoBehaviour
{
    public GameObject soloTrainingRoom;

    [Header("Panels")]
    public GameObject startPanel;    
    public GameObject loadingPanel;  
    public GameObject roomPanel;      
    public GameObject gameLoader;
    public GameObject quitPanel;

    [Header("Start Button")]
    public Button startButton;       

    [Header("Loading")]
    public Slider loadingSlider;      
    public float loadingTime = 3f;

    void Start()
    {
        int isGameOver = PlayerPrefs.GetInt("GameOver");

        if(isGameOver == 0)
        {
            startPanel.SetActive(true);
            loadingPanel.SetActive(false);
            roomPanel.SetActive(false);
            PlayerPrefs.SetInt("GameOver", 1);
        }
        else
        {
            startPanel.SetActive(false);
            loadingPanel.SetActive(false);
            roomPanel.SetActive(true);
            PlayerPrefs.SetInt("GameOver", 0);
        }

        // Initial state
        //startPanel.SetActive(true);
        //loadingPanel.SetActive(false);
        //roomPanel.SetActive(false);
        quitPanel.SetActive(false);

        loadingSlider.value = 0;

        startButton.onClick.AddListener(OnStartClicked);
    }

    void Update()
    {
        bool checkActive = roomPanel.activeSelf && !gameLoader.activeSelf;
        if ((startPanel.activeSelf || checkActive) && Input.GetKeyDown(KeyCode.Escape))
        {
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

    public void JoinTraining()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
