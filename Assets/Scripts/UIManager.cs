using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using Fusion;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Game Over")]
    public GameObject GameOverScreen;
    public GameObject LevelCompleteScreen;

    [Header("Collectibles UI")]
    public TMP_Text coinText;
    public TMP_Text diamondText;

    [Header("Collectible Targets")]
    public int totalCoins = 1;
    public int totalDiamonds = 0;

    [Header("Pause System")]
    public GameObject pausePanel;
    public Image pauseButtonImage;
    public Sprite pauseSprite;
    public Sprite resumeSprite;

    private int collectedCoins = 0;
    private int collectedDiamonds = 0;
    private bool isPaused = false;

    void Awake()
    {
        Instance = this;

        Time.timeScale = 1f;
        isPaused = false;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (GameOverScreen != null)
            GameOverScreen.SetActive(false);

        if (LevelCompleteScreen != null)
            LevelCompleteScreen.SetActive(false);

        if (pauseButtonImage != null)
            pauseButtonImage.sprite = pauseSprite;

        UpdateUI();
    }

    private void Update()
    {
        UpdateUI();
    }

    // ==================
    // COLLECTIBLES
    // ==================

    public void CollectCoin()
    {
        collectedCoins++;
        UpdateUI();
    }

    public void CollectDiamond()
    {
        collectedDiamonds++;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (coinText != null)
            coinText.text = collectedCoins + " / " + totalCoins;

        if (diamondText != null)
            diamondText.text = collectedDiamonds + " / " + totalDiamonds;
    }

    public bool AllCollected()
    {
        return collectedCoins >= totalCoins &&
               collectedDiamonds >= totalDiamonds;
    }

    // ==================
    // PAUSE SYSTEM
    // ==================

    public void OnPauseClicked()
    {
        if (!isPaused)
        {
            PauseGame();
        }
    }

    public void OnResumeClicked()
    {
        ResumeGame();
    }

    public void PauseGame()
    {
        if (pausePanel != null)
            pausePanel.SetActive(true);

        if (pauseButtonImage != null)
            pauseButtonImage.sprite = resumeSprite;

        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (pauseButtonImage != null)
            pauseButtonImage.sprite = pauseSprite;

        Time.timeScale = 1f;
        isPaused = false;
    }

    // ==================
    // GAME MANAGEMENT
    // ==================

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        ClosePanel(GameOverScreen);
        ClosePanel(LevelCompleteScreen);
        ClosePanel(pausePanel);
    }

    public void LevelComplete()
    {
        if (AllCollected())
        {
            LevelCompleteScreen.SetActive(true);
        }
    }

    public void QuitGame()
    {
        StartCoroutine(DisconnectAndQuit());
    }

    private IEnumerator DisconnectAndQuit()
    {
        Time.timeScale = 1f;

        // Close all panels
        ClosePanel(GameOverScreen);
        ClosePanel(LevelCompleteScreen);
        ClosePanel(pausePanel);

        // Find and shutdown the NetworkRunner for this player
        NetworkRunner runner = FindObjectOfType<NetworkRunner>();

        if (runner != null && runner.IsRunning)
        {
            Debug.Log("Disconnecting player from session...");

            // Shutdown this player's connection
            // This will properly remove this player from the session
            runner.Shutdown();

            // Wait a bit to ensure shutdown completes
            yield return new WaitForSeconds(0.5f);
        }

        // Now load the lobby scene
        SceneManager.LoadScene("LobbyScene");
    }

    public void GameOver()
    {
        if (GameOverScreen != null)
        {
            GameOverScreen.SetActive(true);
        }
    }

    private void ClosePanel(GameObject panel)
    {
        if (panel != null && panel.activeSelf == true)
        {
            panel.SetActive(false);
        }
    }

    private IEnumerator OpenCloseScreen()
    {
        if (GameOverScreen != null)
        {
            GameOverScreen.SetActive(true);
            yield return new WaitForSeconds(1.5f);
            GameOverScreen.SetActive(false);
        }
    }
}