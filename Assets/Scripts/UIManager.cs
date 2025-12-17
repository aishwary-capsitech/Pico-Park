


using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;   // ⭐ ADD THIS

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI")]
    public TMP_Text coinText;
    public TMP_Text diamondText;

    [Header("Targets")]
    public int totalCoins = 3;
    public int totalDiamonds = 1;

    private int collectedCoins = 0;
    private int collectedDiamonds = 0;

    // ⭐ ADD THIS
    [Header("Pause UI")]
    public GameObject pausePanel;

    void Awake()
    {
        Instance = this;
        UpdateUI();

        // ⭐ ADD THIS
        if (pausePanel != null)
            pausePanel.SetActive(false);

        Time.timeScale = 1f;
    }

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
        coinText.text = collectedCoins + " / " + totalCoins;
        diamondText.text = collectedDiamonds + " / " + totalDiamonds;
    }

    public bool AllCollected()
    {
        return collectedCoins >= totalCoins &&
               collectedDiamonds >= totalDiamonds;
    }

    // =========================
    // ⭐ PAUSE FUNCTIONS (ADD)
    // =========================

    public void PauseGame()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
