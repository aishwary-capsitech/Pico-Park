using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [Header("Icons")]
    public Sprite pauseSprite;   // pause_0
    public Sprite resumeSprite;  // resume icon (triangle)

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
        pauseImage.sprite = pauseSprite;   // show pause icon at start
    }

    // When pause button (top-right) is clicked
    public void OnPauseClicked()
    {
        if (!isPaused)
        {
            // PAUSE GAME
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
            pauseImage.sprite = resumeSprite; // change to play icon
            isPaused = true;
        }
    }

    // When RESUME button (text) is clicked
    public void OnResumeClicked()
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        pauseImage.sprite = pauseSprite;   // back to pause icon
        isPaused = false;
    }
}
