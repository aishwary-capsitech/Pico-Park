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

    [Header("Quick Chat")]
    public GameObject quickChatMessagePanel;
    public GameObject quickChatAllMessage;
    public TMP_Text quickChatMessageText;
    public GameObject options;
    public GameObject quickChatButton;
    private bool isQuickChatOpen = false;
    private bool IsChattingEnabled => SettingManager.Instance.isChattingEnabled;
    private Coroutine quickChatRoutine;

    [Networked] private NetworkBool isPausedNetwork { get; set; }
    [Networked] private NetworkBool isGameOverNetwork { get; set; }
    [Networked] private NetworkBool isLevelCompleteNetwork { get; set; }
    [Networked] public int NetworkedCoins { get; set; }
    [Networked] public int NetworkedDiamonds { get; set; }
    [Networked] private int collectedKeys { get; set; }

    [SerializeField] private float updateInterval = 0.5f;
    private float timer;
    private NetworkRunner runner;
    public bool isGameOver = false;

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
        
        level2.SetActive(false);
        SelectQuickChatOption();
        ToggleChat();
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

        if (quickChatMessagePanel != null)
            quickChatMessagePanel.SetActive(false);

        if (quickChatAllMessage != null) 
            quickChatAllMessage.SetActive(false);

        if (options != null)
            options.SetActive(isQuickChatOpen);
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
            if (KeyManager.Instance != null)
            {
                collectedKeys = KeyManager.Instance.collectedKeys;
            }
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

        //Debug.Log($"UpdateCollectibleUI → Level: {currentLevel}");
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

        if (isLevel2 && KeyManager.Instance != null)
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
            //if (isQuickChatOpen)
            //{
            //    isQuickChatOpen = false;
            //    options.SetActive(isQuickChatOpen);
            //}
        }

        BridgeRotation bridge = FindObjectOfType<BridgeRotation>();
        if (bridge != null && bridge.Object != null)
        {
            bridge.ResetBridge();
        }

        Block[] blocks = FindObjectsOfType<Block>();

        foreach (var block in blocks)
        {
            if (block != null)
            {
                block.ResetBlock();
            }
        }

        //var allMessagetext = quickChatAllMessage.GetComponentInChildren<TMP_Text>();
        //if (allMessagetext != null)
        //{
        //    allMessagetext.text = "";
        //}

        //quickChatMessagePanel.SetActive(false);
        //quickChatAllMessage.SetActive(false);
        RPC_ClearQuickChatUI();
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

        int currentLevel = LevelManager.Instance.level;
        if (currentLevel != 2)
            RPC_NextLevel();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_NextLevel()
    {
    
        isLevelCompleteNetwork = false;

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
        isGameOver = gameOver;
        PlayerPrefs.SetInt("GameOver", gameOver ? 1 : 0);
        Debug.Log("Game over screen shown to all players");
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;

        NetworkRunner runner = FindObjectOfType<NetworkRunner>();

        if (runner != null)
            Destroy(runner.gameObject);

        PlayerPrefs.SetInt("GameOver", 1);
        SceneManager.LoadScene("LobbyScene");
    }

    public void ExitTraining()
    {
        QuitGame();
        PlayerPrefs.SetInt("GameOver", 1);
        PlayerPrefs.DeleteKey("TrainingMode");
    }

    //public void ToggleSetting()
    //{
    //    if(SettingManager.Instance != null)
    //        SettingManager.Instance.ToggleSettingPanel();
    //}

    public void ToggleChat()
    {
        quickChatButton.SetActive(IsChattingEnabled);
        Debug.Log("Chatting Enabled: " + SettingManager.Instance.isChattingEnabled);
        Debug.Log("Game Scene Chat : " + IsChattingEnabled);
    }

    public void ToggleQuickChat()
    {
        if (!isQuickChatOpen)
        {
            isQuickChatOpen = true;
            options.SetActive(isQuickChatOpen);
        }
        else
        {
            isQuickChatOpen = false;
            options.SetActive(isQuickChatOpen);
        }
    }

    public void SelectQuickChatOption()
    {
        foreach (Button btn in options.GetComponentsInChildren<Button>())
        {
            btn.onClick.AddListener(() =>
            {
                QuickChatType type = (QuickChatType)btn.transform.GetSiblingIndex();
                string message = btn.GetComponentInChildren<TMP_Text>().text;
                Player localPlayer = FindObjectsOfType<Player>()
                    .FirstOrDefault(p => p.Object.HasInputAuthority);
                if(localPlayer != null)
                    localPlayer.SendQuickChat(type);
                if (isQuickChatOpen)
                {
                    isQuickChatOpen = false;
                    options.SetActive(isQuickChatOpen);
                }
            });
        }
    }

    public void ShowQuickChat(QuickChatType type)
    {
        if (quickChatRoutine != null)
            StopCoroutine(quickChatRoutine);

        quickChatMessageText.text = GetQuickChatText(type);
        quickChatMessagePanel.SetActive(true); 
        quickChatAllMessage.SetActive(true);

        quickChatRoutine = StartCoroutine(HideQuickChatAfterDelay());
    }

    public void ShowQuickChatMessage(string message)
    {
        quickChatMessagePanel.SetActive(true);
        quickChatAllMessage.SetActive(true);

        quickChatMessageText.text = message;

        var allMessageText = quickChatAllMessage.GetComponentInChildren<TMP_Text>();
        if (allMessageText != null)
        {
            allMessageText.text += $" {message}";
        }

        var scrollRect = quickChatAllMessage.GetComponent<ScrollRect>();
        if (scrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }

        if (quickChatRoutine != null)
            StopCoroutine(quickChatRoutine);

        quickChatRoutine = StartCoroutine(HideQuickChatAfterDelay());
    }

    IEnumerator HideQuickChatAfterDelay()
    {
        yield return new WaitForSeconds(2.5f);
        quickChatMessagePanel.SetActive(false);
        quickChatAllMessage.SetActive(false);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_ClearQuickChatUI()
    {
        if (quickChatAllMessage != null)
        {
            var allMessageText = quickChatAllMessage.GetComponentInChildren<TMP_Text>();
            if (allMessageText != null)
            {
                allMessageText.text = "";
            }
        }

        if (quickChatMessagePanel != null)
            quickChatMessagePanel.SetActive(false);

        if (quickChatAllMessage != null)
            quickChatAllMessage.SetActive(false);

        isQuickChatOpen = false;
        options.SetActive(false);
    }

    public string GetQuickChatText(QuickChatType type)
    {
        switch (type)
        {
            case QuickChatType.Hii:
                return "Hi!";
            case QuickChatType.Wait:
                return "Wait!";
            case QuickChatType.Help:
                return "Help!";
            case QuickChatType.LetsGo:
                return "Let's Go!";
            default:
                return "";
        }
    }
}