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

    public TMP_Text levelText;
    public TMP_Text pingText;

    [Header("Game Over")]
    public GameObject GameOverScreen;
    public GameObject LevelCompleteScreen;

    [Header("Collectibles UI")]
    public TMP_Text coinText;
    public TMP_Text diamondText;
    public TMP_Text keyText;

    [Header("Collectible Targets")]
    public int totalCoins = 3;
    public int totalDiamonds = 1;
    public int totalKeys = 6;

    [Header("Pause System")]
    public GameObject pausePanel;
    public Image pauseButtonImage;
    public Sprite pauseSprite;
    public Sprite resumeSprite;

    [Header("Levels")]
    public GameObject level1;
    public GameObject level2;

    [Networked] private NetworkBool isPausedNetwork { get; set; }
    [Networked] private NetworkBool isGameOverNetwork { get; set; }
    [Networked] private NetworkBool isLevelCompleteNetwork { get; set; }
    [Networked] public int NetworkedCoins { get; set; }
    [Networked] public int NetworkedDiamonds { get; set; }
    [Networked] private int collectedKeys { get; set; }

    //[SerializeField] private int simulationTickRate = 240;
    [SerializeField] private float updateInterval = 0.5f;
    private float timer;
    private NetworkRunner runner;

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

        runner = FindObjectOfType<NetworkRunner>();
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
            collectedKeys = 0;
        }

        UpdateUI();
        UpdateCollectibleUI();
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
            collectedKeys = KeyManager.Instance.collectedKeys;
            UpdateUI();
        }

        if (runner == null || !runner.IsRunning)
            return;

        timer += Time.deltaTime;
        if (timer < updateInterval)
            return;

        timer = 0f;
        UpdatePingUI();
    }

    private void UpdatePingUI()
    {
        if (runner == null || !runner.IsRunning || pingText == null)
        {
            pingText.text = "Ping: --";
            return;
        }

        // RTT returned in SECONDS (Fusion API in your version)
        double rttSeconds = runner.GetPlayerRtt(runner.LocalPlayer);

        // Convert seconds → milliseconds
        int rttMs = Mathf.RoundToInt((float)(rttSeconds * 1000.0));

        // Color coding
        if (rttMs < 60)
            pingText.color = Color.green;
        else if (rttMs < 120)
            pingText.color = Color.yellow;
        else
            pingText.color = Color.red;

        pingText.text = $"Ping: {rttMs} ms";
        Debug.Log($"Updated Ping: {rttMs} ms" );
        Debug.Log($"RTT Seconds from Fusion API: {rttSeconds}" );
    }


    //public override void Render()
    //{
    //    base.Render();

    //    if (!Object || !Object.IsValid) return;

    //    // Sync panel states from network
    //    if (pausePanel != null)
    //        pausePanel.SetActive(isPausedNetwork);

    //    if (GameOverScreen != null)
    //        GameOverScreen.SetActive(isGameOverNetwork);

    //    if (LevelCompleteScreen != null)
    //        LevelCompleteScreen.SetActive(isLevelCompleteNetwork);

    //    // Update pause button icon
    //    if (pauseButtonImage != null)
    //        pauseButtonImage.sprite = isPausedNetwork ? resumeSprite : pauseSprite;

    //    // Sync time scale
    //    Time.timeScale = isPausedNetwork ? 0f : 1f;

    //    UpdateCollectibleUI();
    //}

    public override void Render()
    {
        base.Render();

        if (!Object || !Object.IsValid) return;

        pausePanel?.SetActive(isPausedNetwork);
        GameOverScreen?.SetActive(isGameOverNetwork);
        LevelCompleteScreen?.SetActive(isLevelCompleteNetwork);

        pauseButtonImage.sprite = isPausedNetwork ? resumeSprite : pauseSprite;
        Time.timeScale = isPausedNetwork ? 0f : 1f;

        UpdateCollectibleUI();
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

    public void CollectKey()
    {
        if (Object && Object.HasStateAuthority)
        {
            collectedKeys++;
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

        if (keyText != null)
            keyText.text = $"{collectedKeys}/{totalKeys}";

        if (levelText != null)
            levelText.text = "Level " + LevelManager.Instance.level;
    }

    //private void UpdateCollectibleUI()
    //{
    //    bool isLevel1 = LevelManager.Instance.level1;
    //    coinText.transform.parent.gameObject.SetActive(isLevel1);
    //    diamondText.transform.parent.gameObject.SetActive(isLevel1);
    //    keyText.transform.parent.gameObject.SetActive(!isLevel1);
    //}

    private void UpdateCollectibleUI()
    {
        int currentLevel = LevelManager.Instance.level;

        bool isLevel1 = currentLevel == 1;
        bool isLevel2 = currentLevel == 2;

        if (coinText != null)
            coinText.transform.parent.gameObject.SetActive(isLevel1);

        if (diamondText != null)
            diamondText.transform.parent.gameObject.SetActive(isLevel1);

        if (keyText != null)
            keyText.transform.parent.gameObject.SetActive(isLevel2);

        Debug.Log($"UpdateCollectibleUI → Level: {currentLevel}");
    }

    public bool AllCollected()
    {
        if (!Object || !Object.IsValid) return false;

        int currentLevel = LevelManager.Instance.level;
        if (currentLevel == 2)
        {
            return collectedKeys >= totalKeys;
        }
        else
        {
            return NetworkedCoins >= totalCoins && NetworkedDiamonds >= totalDiamonds;
        }
    }

    private void ResetCollectibles()
    {
        if (!Object || !Object.HasStateAuthority) return;

        NetworkedCoins = 0;
        NetworkedDiamonds = 0;

        int currentLevel = LevelManager.Instance.level;
        bool isLevel2 = currentLevel == 2;
        if (isLevel2)
        {
            collectedKeys = 0;
        }

        var coins = FindObjectsOfType<NetworkedCoin>(true);
        Debug.Log($"Resetting {coins.Length} coins");

        foreach (var coin in coins)
        {
            coin.RPC_ResetCoin();
        }

        var diamonds = FindObjectsOfType<NetworkedDiamond>(true);
        Debug.Log($"Resetting {diamonds.Length} diamonds");

        foreach (var diamond in diamonds)
        {
            diamond.RPC_ResetDiamond();
        }

        if (KeyManager.Instance != null)
        {
            KeyManager.Instance.ResetKeys();
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
        {
            LevelComplete();
        }
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
        RPC_SetGameOver(false);
        if (NetworkManager.Instance != null && NetworkManager.Instance.runner.IsServer)
        {
            ResetCollectibles();
            UpdateCollectibleUI();
            NetworkManager.Instance.RestartGamePlayer();
        }

        BridgeRotation bridge = FindObjectOfType<BridgeRotation>();
        if (bridge != null && bridge.Object != null)
        {
            bridge.ResetBridge();
        }

        Block block = FindObjectOfType<Block>();
        if (block != null && block.Object != null)
        {
            block.ResetBlock();
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

    public void OnNextLevelClicked()
    {
        if (!Object || !Object.IsValid)
            return;

        // Only StateAuthority should change level
        RPC_NextLevel();
    }

    //[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    //private void RPC_NextLevel()
    //{
    //    RPC_SetLevelComplete(false);

    //    LevelManager.Instance.IncreaseLevel();

    //    if (NetworkManager.Instance != null && NetworkManager.Instance.runner.IsServer)
    //    {
    //        ResetCollectibles();
    //        UpdateCollectibleUI();
    //        NetworkManager.Instance.RestartGamePlayer();
    //    }
    //}

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_NextLevel()
    {
        // Hide level complete UI
        isLevelCompleteNetwork = false;

        // Reset values
        NetworkedCoins = 0;
        NetworkedDiamonds = 0;
        collectedKeys = 0;

        // Switch levels
        LevelManager.Instance.IncreaseLevel();

        if (NetworkManager.Instance != null && NetworkManager.Instance.runner.IsServer)
        {
            ResetCollectibles();
            UpdateCollectibleUI();
            NetworkManager.Instance.RestartGamePlayer();
        }

        UpdateUI();

        Debug.Log("Switched to Level 2 via Next Button");
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