
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [Header("Icons")]
    public Sprite pauseSprite;   
    public Sprite resumeSprite;  

    [Header("Pause Panel")]
    public GameObject pausePanel;

    private Image pauseImage;
    private bool isPaused = false;

    void Start()
    {
        pauseImage = GetComponent<Image>();

        Time.timeScale = 1f;
        isPaused = false;

        pausePanel.SetActive(false);
        pauseImage.sprite = pauseSprite;   
    }

    // When pause button (top-right) is clicked
    public void OnPauseClicked()
    {
        if (!isPaused)
        {
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
            pauseImage.sprite = resumeSprite; 
            isPaused = true;
        }
    }

    public void OnResumeClicked()
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        pauseImage.sprite = pauseSprite; 
        isPaused = false;
    }
}
