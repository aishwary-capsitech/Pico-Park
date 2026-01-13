// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
// using Fusion;
// using Fusion.Addons.Physics;
// using System.Collections;
// using System.Linq;

// public class UIManager : NetworkBehaviour
// {
//     public static UIManager Instance;

//     // ============================
//     // UI SCREENS
//     // ============================
//     [Header("Screens")]
//     public GameObject pausePanel;
//     public GameObject gameOverPanel;
//     public GameObject levelCompletePanel;

//     // ============================
//     // UI TEXT
//     // ============================
//     [Header("Collectibles UI")]
//     public TMP_Text coinText;
//     public TMP_Text diamondText;
//     public TMP_Text keyText;
//     public GameObject keyUI;

//     // ============================
//     // TARGET COUNTS
//     // ============================
//     public int totalCoins = 3;
//     public int totalDiamonds = 1;
//     public int totalKeys = 6;


//     // ============================
//     // LEVEL OBJECTS
//     // ============================
//     [Header("Levels")]
//     public GameObject level1;
//     public GameObject level2;

//     [Header("Level 2 Start Point")]
//     public Transform level2SpawnPoint;

//     // ============================
//     // NETWORKED STATE
//     // ============================
//     [Networked] private NetworkBool isPaused { get; set; }
//     [Networked] private NetworkBool isGameOver { get; set; }
//     [Networked] private NetworkBool isLevelComplete { get; set; }

//     [Networked] public int NetworkedCoins { get; set; }
//     [Networked] public int NetworkedDiamonds { get; set; }

//     // ============================
//     // LOCAL STATE
//     // ============================
//     private int collectedKeys;
//     [HideInInspector] public bool isLevelSwitching = false;

//     // 🔥 ORIGINAL START POSITION (NO SPAWN POINT NEEDED)
//     private Vector2 initialPlayerSpawnPosition;
//     private bool initialSpawnCaptured = false;

//     // ============================
//     // LIFECYCLE
//     // ============================
//     private void Awake()
//     {
//         Instance = this;

//         pausePanel?.SetActive(false);
//         gameOverPanel?.SetActive(false);
//         levelCompletePanel?.SetActive(false);
//     }

//     public override void Spawned()
//     {
//         if (Object.HasStateAuthority)
//         {
//             CaptureInitialSpawnPosition();
//             ResetToFreshGame();
//         }

//         UpdateUI();
//         UpdateCollectibleUI();
//     }

//     // ============================
//     // GAME STATE
//     // ============================
//     public bool IsGameStopped()
//     {
//         if (Object == null || !Object.IsValid) return false;
//         return isPaused || isGameOver || isLevelComplete;
//     }

//     public override void Render()
//     {
//         pausePanel?.SetActive(isPaused);
//         gameOverPanel?.SetActive(isGameOver);
//         levelCompletePanel?.SetActive(isLevelComplete);

//         Time.timeScale = isPaused ? 0f : 1f;
//     }

//     // ============================
//     // COLLECTIBLES
//     // ============================
//     public void CollectCoin()
//     {
//         if (!Object.HasStateAuthority) return;
//         NetworkedCoins++;
//         UpdateUI();
//     }

//     public void CollectDiamond()
//     {
//         if (!Object.HasStateAuthority) return;
//         NetworkedDiamonds++;
//         UpdateUI();
//     }

//     public void OnKeyCollected()
//     {
//         collectedKeys++;
//         UpdateUI();
//     }

//     private void UpdateUI()
//     {
//         if (coinText != null)
//             coinText.text = $"{NetworkedCoins}/{totalCoins}";

//         if (diamondText != null)
//             diamondText.text = $"{NetworkedDiamonds}/{totalDiamonds}";

//         if (keyText != null)
//             keyText.text = $"{collectedKeys}/{totalKeys}";
//     }

//     private void UpdateCollectibleUI()
//     {
//         bool level2Active = level2 != null && level2.activeSelf;

//         coinText.transform.parent.gameObject.SetActive(!level2Active);
//         diamondText.transform.parent.gameObject.SetActive(!level2Active);
//         keyUI.SetActive(level2Active);
//     }

//     // ============================
//     // FINISH CHECK
//     // ============================
//     public void CheckAllPlayersFinished()
//     {
//         if (!Object.HasStateAuthority) return;

//         if (FindObjectsOfType<Player>().All(p => p.HasReachedFinish))
//             LevelComplete();
//     }

//     // ============================
//     // LEVEL COMPLETE → LEVEL 2
//     // ============================
//     public void LevelComplete()
//     {
//         if (!Object.HasStateAuthority || isLevelSwitching) return;
//         RPC_StartLevel2();
//     }

//     [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
//     private void RPC_StartLevel2()
//     {
//         StartCoroutine(SwitchToLevel2());
//     }

//     private IEnumerator SwitchToLevel2()
//     {
//         isLevelSwitching = true;
//         isLevelComplete = true;

//         levelCompletePanel.SetActive(true);
//         yield return new WaitForSecondsRealtime(2f);
//         levelCompletePanel.SetActive(false);

//         NetworkedCoins = 0;
//         NetworkedDiamonds = 0;
//         collectedKeys = 0;

//         level1.SetActive(false);
//         level2.SetActive(true);

//         TeleportPlayers(level2SpawnPoint.position);
//         ResetAllCollectibles();

//         UpdateCollectibleUI();
//         UpdateUI();

//         isLevelComplete = false;
//         isLevelSwitching = false;
//     }

//     // ============================
//     // 🔁 RESTART GAME (BRAND NEW)
//     // ============================
//     public void RestartGame()
//     {
//         if (!Object.HasStateAuthority) return;
//         RPC_RestartGame();
//     }

//     [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
//     private void RPC_RestartGame()
//     {
//         ResetToFreshGame();
//     }

//     private void ResetToFreshGame()
//     {
//         isPaused = false;
//         isGameOver = false;
//         isLevelComplete = false;
//         isLevelSwitching = false;

//         NetworkedCoins = 0;
//         NetworkedDiamonds = 0;
//         collectedKeys = 0;

//         level1.SetActive(true);
//         level2.SetActive(false);

//         TeleportPlayers(initialPlayerSpawnPosition);
//         ResetAllCollectibles();

//         UpdateCollectibleUI();
//         UpdateUI();
//     }

//     // ============================
//     // PLAYER RESET
//     // ============================
//     private void TeleportPlayers(Vector2 position)
//     {
//         foreach (Player player in FindObjectsOfType<Player>())
//         {
//             if (!player.Object.HasStateAuthority) continue;

//             var rb = player.GetComponent<NetworkRigidbody2D>();
//             rb.Teleport(position);
//             rb.Rigidbody.linearVelocity = Vector2.zero;
//             rb.Rigidbody.angularVelocity = 0f;

//             player.HasReachedFinish = false;
//         }
//     }

//     // ============================
//     // COLLECTIBLE RESET
//     // ============================
//     private void ResetAllCollectibles()
//     {
//         var coins = FindObjectsOfType<NetworkedCoin>(true);
//         foreach (var coin in coins)
//             coin.RPC_ResetCoin();
//     }

//     // ============================
//     // INITIAL SPAWN CAPTURE
//     // ============================
//     private void CaptureInitialSpawnPosition()
//     {
//         if (initialSpawnCaptured) return;

//         Player player = FindObjectOfType<Player>();
//         if (player != null)
//         {
//             initialPlayerSpawnPosition = player.transform.position;
//             initialSpawnCaptured = true;
//         }
//     }

//     // ============================
//     // PAUSE
//     // ============================
//     public void OnPauseClicked()
//     {
//         if (!Object.HasStateAuthority) return;
//         RPC_SetPause(true);
//     }

//     public void OnResumeClicked()
//     {
//         if (!Object.HasStateAuthority) return;
//         RPC_SetPause(false);
//     }

//     [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
//     private void RPC_SetPause(NetworkBool value)
//     {
//         isPaused = value;
//     }

//     // ============================
//     // GAME OVER
//     // ============================
//     public void GameOver()
//     {
//         if (!Object.HasStateAuthority) return;
//         isGameOver = true;
//     }
// }


using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Fusion;
using Fusion.Addons.Physics;
using System.Linq;

public class UIManager : NetworkBehaviour
{
    public static UIManager Instance;

    // ============================
    // SCREENS
    // ============================
    [Header("Screens")]
    public GameObject pausePanel;
    public GameObject GameOverScreen;
    public GameObject LevelCompleteScreen;

    // ============================
    // COLLECTIBLES UI
    // ============================
    [Header("Collectibles UI")]
    public TMP_Text coinText;
    public TMP_Text diamondText;

    [Header("Key UI (Level 2)")]
    public TMP_Text keyText;
    public GameObject keyUI;

    // ============================
    // TARGET COUNTS
    // ============================
    public int totalCoins = 3;
    public int totalDiamonds = 1;
    public int totalKeys = 6;

    // ============================
    // PAUSE BUTTON
    // ============================
    public Image pauseButtonImage;
    public Sprite pauseSprite;
    public Sprite resumeSprite;

    // ============================
    // LEVEL OBJECTS
    // ============================
    public GameObject level1;
    public GameObject level2;

    [Header("Level 2 Start Point")]
    public Transform level2StartPoint;

    // ============================
    // NETWORKED STATE
    // ============================
    [Networked] private NetworkBool isPausedNetwork { get; set; }
    [Networked] private NetworkBool isGameOverNetwork { get; set; }
    [Networked] private NetworkBool isLevelCompleteNetwork { get; set; }

    [Networked] public int NetworkedCoins { get; set; }
    [Networked] public int NetworkedDiamonds { get; set; }

    // ============================
    // LOCAL STATE
    // ============================
    private int collectedKeys = 0;
    [HideInInspector] public bool isLevelSwitching = false;

    // 🔥 IMPORTANT: REAL GAME START POSITION
    private Vector2 initialPlayerSpawnPos;
    private bool spawnCaptured = false;

    // ============================
    // LIFECYCLE
    // ============================
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        pausePanel?.SetActive(false);
        GameOverScreen?.SetActive(false);
        LevelCompleteScreen?.SetActive(false);

        if (pauseButtonImage != null)
            pauseButtonImage.sprite = pauseSprite;
    }

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            CaptureInitialSpawn();
            ResetToFreshGame();
        }

        UpdateCollectibleUI();
        UpdateUI();
    }

    // ============================
    // GAME STATE
    // ============================
    public bool IsGameStopped()
    {
        if (!Object || !Object.IsValid) return false;
        return isPausedNetwork || isGameOverNetwork || isLevelCompleteNetwork;
    }

    public override void Render()
    {
        pausePanel?.SetActive(isPausedNetwork);
        GameOverScreen?.SetActive(isGameOverNetwork);
        LevelCompleteScreen?.SetActive(isLevelCompleteNetwork);

        if (pauseButtonImage != null)
            pauseButtonImage.sprite = isPausedNetwork ? resumeSprite : pauseSprite;

        Time.timeScale = isPausedNetwork ? 0f : 1f;
    }

    // ============================
    // UI VISIBILITY
    // ============================
    private void UpdateCollectibleUI()
    {
        bool level2Active = level2.activeSelf;

        coinText.transform.parent.gameObject.SetActive(!level2Active);
        diamondText.transform.parent.gameObject.SetActive(!level2Active);
        keyUI.SetActive(level2Active);
    }

    // ============================
    // COLLECTIBLES
    // ============================
    public void CollectCoin()
    {
        if (!Object.HasStateAuthority) return;
        NetworkedCoins++;
        UpdateUI();
    }

    public void CollectDiamond()
    {
        if (!Object.HasStateAuthority) return;
        NetworkedDiamonds++;
        UpdateUI();
    }

    public void CollectKey()
    {
        collectedKeys = Mathf.Clamp(collectedKeys + 1, 0, totalKeys);
        UpdateUI();
    }

    public void OnKeyCollected() => CollectKey();

    private void UpdateUI()
    {
        coinText.text = $"{NetworkedCoins}/{totalCoins}";
        diamondText.text = $"{NetworkedDiamonds}/{totalDiamonds}";
        keyText.text = $"{collectedKeys}/{totalKeys}";
    }

    public bool AllCollected()
    {
        return NetworkedCoins >= totalCoins &&
               NetworkedDiamonds >= totalDiamonds;
    }

    // ============================
    // RESET COLLECTIBLES
    // ============================
    private void ResetCollectibles()
    {
        NetworkedCoins = 0;
        NetworkedDiamonds = 0;
        collectedKeys = 0;

        var coins = FindObjectsByType<NetworkedCoin>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (var coin in coins)
            coin.RPC_ResetCoin();
    }

    // ============================
    // LEVEL COMPLETE → LEVEL 2
    // ============================
    public void LevelComplete()
    {
        if (!Object.HasStateAuthority || isLevelSwitching) return;
        if (AllCollected())
            RPC_StartLevel2();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_StartLevel2()
    {
        StartCoroutine(Level2Flow());
    }

    private System.Collections.IEnumerator Level2Flow()
    {
        isLevelSwitching = true;
        isLevelCompleteNetwork = true;

        LevelCompleteScreen.SetActive(true);
        yield return new WaitForSecondsRealtime(2f);
        LevelCompleteScreen.SetActive(false);

        ResetCollectibles();

        level1.SetActive(false);
        level2.SetActive(true);

        TeleportPlayer(level2StartPoint.position);

        UpdateCollectibleUI();
        UpdateUI();

        isLevelCompleteNetwork = false;
        isLevelSwitching = false;
    }

    // ============================
    // RESTART GAME (REAL FRESH START)
    // ============================
    public void RestartGame()
    {
        if (!Object.HasStateAuthority) return;
        RPC_RestartGame();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_RestartGame()
    {
        ResetToFreshGame();
    }

    private void ResetToFreshGame()
    {
        isPausedNetwork = false;
        isGameOverNetwork = false;
        isLevelCompleteNetwork = false;
        isLevelSwitching = false;

        ResetCollectibles();

        level1.SetActive(true);
        level2.SetActive(false);

        TeleportPlayer(initialPlayerSpawnPos);

        UpdateCollectibleUI();
        UpdateUI();
    }

    // ============================
// CHECK ALL PLAYERS FINISHED
// ============================
public void CheckAllPlayersFinished()
{
    // Safety checks
    if (!Object || !Object.IsValid)
        return;

    // Only server/state authority decides
    if (!Object.HasStateAuthority)
        return;

    Player[] players = FindObjectsOfType<Player>();
    if (players.Length == 0)
        return;

    bool allFinished = players.All(p => p.HasReachedFinish);

    if (allFinished)
    {
        LevelComplete();
    }
}


    // ============================
    // PLAYER HANDLING
    // ============================
    private void CaptureInitialSpawn()
    {
        if (spawnCaptured) return;

        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            initialPlayerSpawnPos = player.transform.position;
            spawnCaptured = true;
        }
    }

    private void TeleportPlayer(Vector2 pos)
    {
        Player player = FindObjectOfType<Player>();
        if (player == null || !player.Object.HasStateAuthority) return;

        var rb = player.GetComponent<NetworkRigidbody2D>();
        rb.Teleport(pos);
        rb.Rigidbody.linearVelocity = Vector2.zero;
        rb.Rigidbody.angularVelocity = 0f;

        player.HasReachedFinish = false;
    }

    // ============================
    // PAUSE
    // ============================
    public void OnPauseClicked() => RPC_SetPause(true);
    public void OnResumeClicked() => RPC_SetPause(false);

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_SetPause(NetworkBool paused)
    {
        isPausedNetwork = paused;
    }

    // ============================
    // GAME OVER
    // ============================
    public void GameOver()
    {
        if (!Object.HasStateAuthority) return;
        isGameOverNetwork = true;
    }

    // ============================
    // QUIT
    // ============================
    public void QuitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("LobbyScene");
    }
}
