//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;
//using UnityEngine.SceneManagement;
//using System.Collections;
//using Fusion;
//using System.Linq;

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
//    public int totalCoins = 3;
//    public int totalDiamonds = 1;

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
//            Instance = this;

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

//    public bool IsGameStopped()
//    {
//        return isGameOverNetwork || isLevelCompleteNetwork || isPausedNetwork;
//    }

//    private void Update()
//    {
//        UpdateUI();
//    }

//    public override void Render()
//    {
//        if (pausePanel != null)
//            pausePanel.SetActive(isPausedNetwork);

//        if (GameOverScreen != null)
//            GameOverScreen.SetActive(isGameOverNetwork);

//        if (LevelCompleteScreen != null)
//            LevelCompleteScreen.SetActive(isLevelCompleteNetwork);

//        if (pauseButtonImage != null)
//            pauseButtonImage.sprite = isPausedNetwork ? resumeSprite : pauseSprite;

//        Time.timeScale = isPausedNetwork ? 0f : 1f;
//    }

//    // COLLECTIBLES
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

//    // CHECK ALL PLAYERS FINISHED
//    public void CheckAllPlayersFinished()
//    {
//        Player[] allPlayers = FindObjectsOfType<Player>();

//        if (allPlayers.Length == 0)
//            return;

//        bool allFinished = allPlayers.All(player => player.HasReachedFinish);

//        if (allFinished)
//            LevelComplete();
//    }

//    // PAUSE SYSTEM
//    public void OnPauseClicked()
//    {
//        if (!Object || !Object.IsValid) {
//            Debug.LogWarning("UIManager not spawned yet!");
//            return;
//        }

//        if (!isPausedNetwork)
//            RPC_SetPause(true);
//    }

//    public void OnResumeClicked()
//    {
//        if (!Object || !Object.IsValid) return;

//        RPC_SetPause(false);
//    }

//    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
//    private void RPC_SetPause(NetworkBool paused)
//    {
//        isPausedNetwork = paused;
//    }

//    public void PauseGame()
//    {
//        if(Object && Object.IsValid)
//            RPC_SetPause(true);
//    }

//    public void ResumeGame()
//    {
//        if(Object && Object.IsValid)
//            RPC_SetPause(false);
//    }

//    // GAME MANAGEMENT
//    public void RestartGame()
//    {
//        if(!Object || !Object.IsValid) return;

//        RPC_RestartGame();
//    }

//    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
//    private void RPC_RestartGame()
//    {
//        Time.timeScale = 1f;
//        NetworkRunner runner = FindObjectOfType<NetworkRunner>();

//        if (runner != null && runner.IsServer)
//        {
//            NetworkManager.Instance.ClearSpawnedPlayers();
//            Player[] players = FindObjectsOfType<Player>();
//            foreach (var player in players)
//            {
//                if(player.Object != null)
//                {
//                    runner.Despawn(player.Object);
//                }
//            }
//        }

//        RPC_LoadScene(SceneManager.GetActiveScene().buildIndex);
//    }

//    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
//    private void RPC_LoadScene(int sceneIndex)
//    {
//        Time.timeScale = 1f;
//        SceneManager.LoadScene(sceneIndex);
//    }

//    public void LevelComplete()
//    {
//        if(!Object || !Object.IsValid) return;

//        if (AllCollected())
//            RPC_SetLevelComplete(true);
//    }

//    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
//    private void RPC_SetLevelComplete(NetworkBool completed)
//    {
//        isLevelCompleteNetwork = completed;
//    }

//    public void GameOver()
//    {
//        if(!Object || !Object.IsValid) return;

//        RPC_SetGameOver(true);
//    }

//    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
//    private void RPC_SetGameOver(NetworkBool gameOver)
//    {
//        isGameOverNetwork = gameOver;
//    }

//    public void QuitGame()
//    {
//        Time.timeScale = 1f;

//        NetworkRunner runner = FindObjectOfType<NetworkRunner>();

//        if (runner != null)
//            Destroy(runner.gameObject);

//        SceneManager.LoadScene("LobbyScene");
//    }
//}




using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using Fusion;
using System.Linq;

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
    public int totalCoins = 3;
    public int totalDiamonds = 1;

    [Header("Pause System")]
    public GameObject pausePanel;
    public Image pauseButtonImage;
    public Sprite pauseSprite;
    public Sprite resumeSprite;

    [Networked] private NetworkBool isPausedNetwork { get; set; }
    [Networked] private NetworkBool isGameOverNetwork { get; set; }
    [Networked] private NetworkBool isLevelCompleteNetwork { get; set; }
    [Networked] public int NetworkedCoins { get; set; }
    [Networked] public int NetworkedDiamonds { get; set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("UIManager instance set");
        }
        else if (Instance != this)
        {
            Debug.LogWarning("Duplicate UIManager found, destroying");
            Destroy(gameObject);
            return;
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
    }

    public override void Spawned()
    {
        base.Spawned();

        // Reset networked state when spawned
        if (Object.HasStateAuthority)
        {
            isPausedNetwork = false;
            isGameOverNetwork = false;
            isLevelCompleteNetwork = false;
            NetworkedCoins = 0;
            NetworkedDiamonds = 0;
        }

        UpdateUI();
        Debug.Log("UIManager spawned and initialized");
    }

    public bool IsGameStopped()
    {
        if (!Object || !Object.IsValid) return false;
        return isGameOverNetwork || isLevelCompleteNetwork || isPausedNetwork;
    }

    private void Update()
    {
        if (Object && Object.IsValid)
        {
            UpdateUI();
        }
    }

    public override void Render()
    {
        base.Render();

        if (!Object || !Object.IsValid) return;

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

    // COLLECTIBLES
    public void CollectCoin()
    {
        if (Object && Object.HasStateAuthority)
        {
            NetworkedCoins++;
            UpdateUI();
        }
    }

    public void CollectDiamond()
    {
        if (Object && Object.HasStateAuthority)
        {
            NetworkedDiamonds++;
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        if (!Object || !Object.IsValid) return;

        if (coinText != null)
            coinText.text = NetworkedCoins + " / " + totalCoins;

        if (diamondText != null)
            diamondText.text = NetworkedDiamonds + " / " + totalDiamonds;
    }

    public bool AllCollected()
    {
        if (!Object || !Object.IsValid) return false;
        return NetworkedCoins >= totalCoins && NetworkedDiamonds >= totalDiamonds;
    }

    //private void ResetCollectibles()
    //{
    //    if (!Object || !Object.HasStateAuthority) return;

    //    NetworkedCoins = 0;
    //    NetworkedDiamonds = 0;

    //    Debug.Log("Collectibles reset to 0");
    //}

    private void ResetCollectibles()
    {
        if (!Object || !Object.HasStateAuthority) return;

        NetworkedCoins = 0;
        NetworkedDiamonds = 0;

        var coins = FindObjectsOfType<NetworkedCoin>(true);
        Debug.Log($"Resetting {coins.Length} coins");

        foreach (var coin in coins)
        {
            coin.RPC_ResetCoin();
        }

        Debug.Log("All coins reset and re-enabled");
    }

    // CHECK ALL PLAYERS FINISHED
    public void CheckAllPlayersFinished()
    {
        Player[] allPlayers = FindObjectsOfType<Player>();

        if (allPlayers.Length == 0)
            return;

        bool allFinished = allPlayers.All(player => player.HasReachedFinish);

        if (allFinished)
            LevelComplete();
    }

    // PAUSE SYSTEM
    public void OnPauseClicked()
    {
        if (!Object || !Object.IsValid)
        {
            Debug.LogWarning("UIManager not spawned yet!");
            return;
        }

        if (!isPausedNetwork)
            RPC_SetPause(true);
    }

    public void OnResumeClicked()
    {
        if (!Object || !Object.IsValid) return;
        RPC_SetPause(false);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_SetPause(NetworkBool paused)
    {
        isPausedNetwork = paused;
        Debug.Log($"Pause state changed to: {paused}");
    }

    public void PauseGame()
    {
        if (Object && Object.IsValid)
            RPC_SetPause(true);
    }

    public void ResumeGame()
    {
        if (Object && Object.IsValid)
            RPC_SetPause(false);
    }

    // GAME MANAGEMENT
    public void RestartGame()
    {
        if (!Object || !Object.IsValid) return;

        RPC_RestartGame();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_RestartGame()
    {
        Debug.Log("Restart game RPC received on server");

        RPC_SetPause(false);
        if (NetworkManager.Instance != null && NetworkManager.Instance.runner.IsServer)
        {
            ResetCollectibles();
            NetworkManager.Instance.RestartGamePlayer();
        }
        //RPC_LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_LoadScene(int sceneIndex)
    {
        Debug.Log($"Loading scene {sceneIndex}");
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneIndex);
    }

    public void LevelComplete()
    {
        if (!Object || !Object.IsValid) return;

        if (AllCollected())
            RPC_SetLevelComplete(true);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_SetLevelComplete(NetworkBool completed)
    {
        isLevelCompleteNetwork = completed;
        Debug.Log("Level complete screen shown to all players");
    }

    public void GameOver()
    {
        if (!Object || !Object.IsValid) return;
        RPC_SetGameOver(true);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_SetGameOver(NetworkBool gameOver)
    {
        isGameOverNetwork = gameOver;
        Debug.Log("Game over screen shown to all players");
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;

        NetworkRunner runner = FindObjectOfType<NetworkRunner>();

        if (runner != null)
            Destroy(runner.gameObject);

        SceneManager.LoadScene("LobbyScene");
    }
}