using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using Fusion;

public class UIManager : NetworkBehaviour
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

    [Networked] private NetworkBool isPausedNetwork { get; set; }
    [Networked] private NetworkBool isGameOverNetwork { get; set; }
    [Networked] private NetworkBool isLevelCompleteNetwork { get; set; }

    private int collectedCoins = 0;
    private int collectedDiamonds = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        Time.timeScale = 1f;

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

    public override void Render()
    {
        base.Render();

        // Sync panel states from network
        if (pausePanel != null)
            pausePanel.SetActive(isPausedNetwork);

        if (GameOverScreen != null)
            GameOverScreen.SetActive(isGameOverNetwork);

        if (LevelCompleteScreen != null)
            LevelCompleteScreen.SetActive(isLevelCompleteNetwork);

        // Update pause button icon
        if (pauseButtonImage != null)
            pauseButtonImage.sprite = isPausedNetwork ? resumeSprite : pauseSprite;

        // Sync time scale
        Time.timeScale = isPausedNetwork ? 0f : 1f;
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
    // PAUSE SYSTEM (SYNCED)
    // ==================

    public void OnPauseClicked()
    {
        if (!isPausedNetwork)
        {
            RPC_SetPause(true);
        }
    }

    public void OnResumeClicked()
    {
        RPC_SetPause(false);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_SetPause(NetworkBool paused)
    {
        isPausedNetwork = paused;
        Debug.Log($"Pause state changed to: {paused}");
    }

    // Kept for backward compatibility
    public void PauseGame()
    {
        RPC_SetPause(true);
    }

    public void ResumeGame()
    {
        RPC_SetPause(false);
    }

    // ==================
    // GAME MANAGEMENT (SYNCED)
    // ==================

    public void RestartGame()
    {
        RPC_RestartGame();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_RestartGame()
    {
        RPC_LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_LoadScene(int sceneIndex)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneIndex);
    }

    public void LevelComplete()
    {
        if (AllCollected())
        {
            RPC_SetLevelComplete(true);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_SetLevelComplete(NetworkBool completed)
    {
        isLevelCompleteNetwork = completed;
        Debug.Log("Level complete screen shown to all players");
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;

        // Just destroy all network objects and load scene
        NetworkRunner runner = FindObjectOfType<NetworkRunner>();
        if (runner != null)
        {
            Destroy(runner.gameObject);
        }

        SceneManager.LoadScene("LobbyScene");
    }

    public void GameOver()
    {
        RPC_SetGameOver(true);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_SetGameOver(NetworkBool gameOver)
    {
        isGameOverNetwork = gameOver;
        Debug.Log("Game over screen shown to all players");
    }

    private void ClosePanel(GameObject panel)
    {
        if (panel != null && panel.activeSelf == true)
        {
            panel.SetActive(false);
        }
    }
}

//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;
//using UnityEngine.SceneManagement;
//using System.Collections;
//using Fusion;

//public class UIManager : NetworkBehaviour
//{
//    public static UIManager Instance;

//    [Header("Game Over")]
//    public GameObject GameOverScreen;
//    public GameObject LevelCompleteScreen;

//    [Header("Collectibles UI")]
//    public TMP_Text coinText;
//    public TMP_Text diamondText;

//    [Header("Collectible Targets")]
//    public int totalCoins = 1;
//    public int totalDiamonds = 0;

//    [Header("Pause System")]
//    public GameObject pausePanel;
//    public Image pauseButtonImage;
//    public Sprite pauseSprite;
//    public Sprite resumeSprite;

//    [Networked] private NetworkBool isPausedNetwork { get; set; }
//    [Networked] private NetworkBool isGameOverNetwork { get; set; }
//    [Networked] private NetworkBool isLevelCompleteNetwork { get; set; }

//    private int collectedCoins = 0;
//    private int collectedDiamonds = 0;

//    void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//        }

//        Time.timeScale = 1f;

//        if (pausePanel != null)
//            pausePanel.SetActive(false);

//        if (GameOverScreen != null)
//            GameOverScreen.SetActive(false);

//        if (LevelCompleteScreen != null)
//            LevelCompleteScreen.SetActive(false);

//        if (pauseButtonImage != null)
//            pauseButtonImage.sprite = pauseSprite;

//        UpdateUI();
//    }

//    private void Update()
//    {
//        UpdateUI();
//    }

//    public override void Render()
//    {
//        base.Render();

//        // Sync panel states from network
//        if (pausePanel != null)
//            pausePanel.SetActive(isPausedNetwork);

//        if (GameOverScreen != null)
//            GameOverScreen.SetActive(isGameOverNetwork);

//        if (LevelCompleteScreen != null)
//            LevelCompleteScreen.SetActive(isLevelCompleteNetwork);

//        // Update pause button icon
//        if (pauseButtonImage != null)
//            pauseButtonImage.sprite = isPausedNetwork ? resumeSprite : pauseSprite;

//        // Sync time scale
//        Time.timeScale = isPausedNetwork ? 0f : 1f;
//    }

//    // ==================
//    // COLLECTIBLES
//    // ==================

//    public void CollectCoin()
//    {
//        collectedCoins++;
//        UpdateUI();
//    }

//    public void CollectDiamond()
//    {
//        collectedDiamonds++;
//        UpdateUI();
//    }

//    void UpdateUI()
//    {
//        if (coinText != null)
//            coinText.text = collectedCoins + " / " + totalCoins;

//        if (diamondText != null)
//            diamondText.text = collectedDiamonds + " / " + totalDiamonds;
//    }

//    public bool AllCollected()
//    {
//        return collectedCoins >= totalCoins &&
//               collectedDiamonds >= totalDiamonds;
//    }

//    // ==================
//    // PAUSE SYSTEM (SYNCED)
//    // ==================

//    public void OnPauseClicked()
//    {
//        if (!isPausedNetwork)
//        {
//            RPC_SetPause(true);
//        }
//    }

//    public void OnResumeClicked()
//    {
//        RPC_SetPause(false);
//    }

//    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
//    private void RPC_SetPause(NetworkBool paused)
//    {
//        isPausedNetwork = paused;
//        Debug.Log($"Pause state changed to: {paused}");
//    }

//    // Kept for backward compatibility
//    public void PauseGame()
//    {
//        RPC_SetPause(true);
//    }

//    public void ResumeGame()
//    {
//        RPC_SetPause(false);
//    }

//    // ==================
//    // GAME MANAGEMENT
//    // ==================

//    // SYNCED - Restarts game for ALL players
//    public void RestartGame()
//    {
//        RPC_RestartGame();
//    }

//    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
//    private void RPC_RestartGame()
//    {
//        RPC_LoadScene(SceneManager.GetActiveScene().buildIndex);
//    }

//    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
//    private void RPC_LoadScene(int sceneIndex)
//    {
//        Time.timeScale = 1f;
//        Debug.Log($"All players reloading scene: {sceneIndex}");
//        SceneManager.LoadScene(sceneIndex);
//    }

//    // SYNCED - Shows level complete for ALL players
//    public void LevelComplete()
//    {
//        if (AllCollected())
//        {
//            RPC_SetLevelComplete(true);
//        }
//    }

//    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
//    private void RPC_SetLevelComplete(NetworkBool completed)
//    {
//        isLevelCompleteNetwork = completed;
//        Debug.Log("Level complete screen shown to all players");
//    }

//    // LOCAL ONLY - Only THIS player quits, others continue playing
//    public void QuitGame()
//    {
//        StartCoroutine(QuitGameLocal());
//    }

//    private IEnumerator QuitGameLocal()
//    {
//        Time.timeScale = 1f;

//        Debug.Log("Local player quitting...");

//        // Find the NetworkRunner
//        NetworkRunner runner = FindObjectOfType<NetworkRunner>();

//        if (runner != null)
//        {
//            if (runner.IsRunning)
//            {
//                Debug.Log("Disconnecting from session...");

//                // Shutdown THIS player's connection only
//                // This properly removes this player from the session
//                runner.Shutdown(true); // true = destroy runner after shutdown

//                // Wait for shutdown to complete
//                yield return new WaitForSeconds(0.5f);
//            }

//            // Ensure runner GameObject is destroyed
//            if (runner != null && runner.gameObject != null)
//            {
//                Destroy(runner.gameObject);
//            }
//        }

//        // Wait one frame for cleanup
//        yield return null;

//        Debug.Log("Loading LobbyScene...");

//        // Load lobby scene for THIS player only
//        SceneManager.LoadScene("LobbyScene", LoadSceneMode.Single);
//    }

//    // SYNCED - Shows game over for ALL players
//    public void GameOver()
//    {
//        RPC_SetGameOver(true);
//    }

//    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
//    private void RPC_SetGameOver(NetworkBool gameOver)
//    {
//        isGameOverNetwork = gameOver;
//        Debug.Log("Game over screen shown to all players");
//    }

//    private void ClosePanel(GameObject panel)
//    {
//        if (panel != null && panel.activeSelf == true)
//        {
//            panel.SetActive(false);
//        }
//    }
//}