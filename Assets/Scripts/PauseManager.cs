// using UnityEngine;
// using UnityEngine.UI;

// public class PauseManager : MonoBehaviour
// {
//     [Header("Icons")]
//     public Sprite pauseIcon;   // ||
//     public Sprite playIcon;    // ▶

//     [Header("Pause Panel")]
//     public GameObject pausePanel; // Pause_Panel

//     public Image buttonImage;
//     private bool isPaused = false;

//     void Start()
//     {
//         buttonImage = GetComponent<Image>();

//         // Initial state
//         Time.timeScale = 1f;
//         isPaused = false;

//         buttonImage.sprite = pauseIcon;
//         pausePanel.SetActive(false);
//     }

//     // Called by Pause button
//     public void TogglePause()
//     {
//         if (!isPaused)
//         {
//             PauseGame();
//         }
//         else
//         {
//             ResumeGame();
//         }
//     }

//     // Called by Resume button
//     public void ResumeFromPanel()
//     {
//         ResumeGame();
//     }

//     void PauseGame()
//     {
//         buttonImage.sprite = playIcon;
//         Time.timeScale = 0f;
//         isPaused = true;

//         pausePanel.SetActive(true);
//     }

//     void ResumeGame()
//     {
//         buttonImage.sprite = pauseIcon;
//         Time.timeScale = 1f;
//         isPaused = false;

//         pausePanel.SetActive(false);
//     }
// }


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
