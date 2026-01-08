// -----------

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

//     [Header("Game Over")]
//     public GameObject GameOverScreen;
//     public GameObject LevelCompleteScreen;

//     [Header("Collectibles UI")]
//     public TMP_Text coinText;
//     public TMP_Text diamondText;

//     // 🔧 ADDED: UI parents
//     public GameObject coinUI;
//     public GameObject diamondUI;
//     public GameObject keyUI;

//     [Header("Targets")]
//     public int totalCoins = 3;
//     public int totalDiamonds = 1;

//     [Header("Pause")]
//     public GameObject pausePanel;
//     public Image pauseButtonImage;
//     public Sprite pauseSprite;
//     public Sprite resumeSprite;

//     [Header("LEVEL OBJECTS")]
//     public GameObject level1;
//     public GameObject level2;

//     [Header("LEVEL 2 START POINT")]
//     public Transform level2StartPoint;

//     [Networked] private NetworkBool isPausedNetwork { get; set; }
//     [Networked] private NetworkBool isGameOverNetwork { get; set; }
//     [Networked] private NetworkBool isLevelCompleteNetwork { get; set; }

//     private int collectedCoins;
//     private int collectedDiamonds;

//     [HideInInspector] public bool isLevelSwitching = false;

//     // ============================
//     // LIFECYCLE
//     // ============================
//     void Awake()
//     {
//         Instance = this;
//         Time.timeScale = 1f;

//         // your existing logic
//         if (level1 != null) level1.SetActive(false);
//         if (level2 != null) level2.SetActive(true);

//         pausePanel?.SetActive(false);
//         GameOverScreen?.SetActive(false);
//         LevelCompleteScreen?.SetActive(false);

//         collectedCoins = 0;
//         collectedDiamonds = 0;
//         UpdateUI();

//         // 🔧 ADDED
//         UpdateCollectibleUI();

//         StartCoroutine(ForceStartAtLevel2());
//     }

//     IEnumerator ForceStartAtLevel2()
//     {
//         yield return null;

//         var player = Player.Instance;
//         if (player != null && level2StartPoint != null && player.Object.HasStateAuthority)
//         {
//             var netRb = player.GetComponent<NetworkRigidbody2D>();
//             if (netRb != null)
//             {
//                 netRb.Teleport(level2StartPoint.position);
//                 netRb.Rigidbody.linearVelocity = Vector2.zero;
//                 netRb.Rigidbody.angularVelocity = 0f;
//             }
//         }
//     }

//     // ============================
//     // 🔧 ADDED: UI VISIBILITY LOGIC
//     // ============================
//     void UpdateCollectibleUI()
//     {
//         bool isLevel2 = level2 != null && level2.activeSelf;

//         // Level 1 → Coin + Diamond
//         if (coinUI != null) coinUI.SetActive(!isLevel2);
//         if (diamondUI != null) diamondUI.SetActive(!isLevel2);

//         // Level 2 → Key only
//         if (keyUI != null) keyUI.SetActive(isLevel2);
//     }

//     public bool IsGameStopped()
//     {
//         return isGameOverNetwork || isLevelCompleteNetwork;
//     }

//     public override void Render()
//     {
//         pausePanel?.SetActive(isPausedNetwork);
//         GameOverScreen?.SetActive(isGameOverNetwork);
//         LevelCompleteScreen?.SetActive(isLevelCompleteNetwork);

//         if (!isPausedNetwork)
//             Time.timeScale = 1f;
//     }

//     // ============================
//     // COLLECTIBLES
//     // ============================
//     public void CollectCoin()
//     {
//         collectedCoins++;
//         UpdateUI();
//     }

//     public void CollectDiamond()
//     {
//         collectedDiamonds++;
//         UpdateUI();
//     }

//     void UpdateUI()
//     {
//         if (coinText != null)
//             coinText.text = $"{collectedCoins} / {totalCoins}";
//         if (diamondText != null)
//             diamondText.text = $"{collectedDiamonds} / {totalDiamonds}";
//     }

//     public bool AllCollected()
//     {
//         return collectedCoins >= totalCoins &&
//                collectedDiamonds >= totalDiamonds;
//     }

//     // ============================
//     // LEVEL COMPLETE
//     // ============================
//     public void CheckAllPlayersFinished()
//     {
//         if (FindObjectsOfType<Player>().All(p => p.HasReachedFinish))
//             LevelComplete();
//     }

//     public void LevelComplete()
//     {
//         if (AllCollected())
//             RPC_SetLevelComplete(true);
//     }

//     [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
//     private void RPC_SetLevelComplete(NetworkBool completed)
//     {
//         isLevelCompleteNetwork = completed;

//         if (completed)
//             StartCoroutine(SwitchToLevel2());
//     }

//     IEnumerator SwitchToLevel2()
//     {
//         isLevelSwitching = true;

//         yield return new WaitForSecondsRealtime(1.5f);

//         collectedCoins = 0;
//         collectedDiamonds = 0;
//         UpdateUI();

//         if (level1 != null) level1.SetActive(false);
//         if (level2 != null) level2.SetActive(true);

//         // 🔧 ADDED
//         UpdateCollectibleUI();

//         var player = Player.Instance;
//         if (player != null && level2StartPoint != null && player.Object.HasStateAuthority)
//         {
//             var netRb = player.GetComponent<NetworkRigidbody2D>();
//             if (netRb != null)
//             {
//                 netRb.Teleport(level2StartPoint.position);
//                 netRb.Rigidbody.linearVelocity = Vector2.zero;
//                 netRb.Rigidbody.angularVelocity = 0f;
//             }
//         }

//         isLevelCompleteNetwork = false;
//         LevelCompleteScreen?.SetActive(false);
//         isLevelSwitching = false;
//     }

//     // ============================
//     // GAME OVER
//     // ============================
//     public void GameOver()
//     {
//         if (!isLevelSwitching)
//             RPC_SetGameOver(true);
//     }

//     [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
//     private void RPC_SetGameOver(NetworkBool gameOver)
//     {
//         isGameOverNetwork = gameOver;
//     }

//     // ============================
//     // PAUSE
//     // ============================
//     public void OnPauseClicked()
//     {
//         RPC_SetPause(true);
//     }

//     public void OnResumeClicked()
//     {
//         RPC_SetPause(false);
//     }

//     [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
//     private void RPC_SetPause(NetworkBool paused)
//     {
//         isPausedNetwork = paused;
//     }

//     // ============================
//     // HOME
//     // ============================
//     public void OnHomeClicked()
//     {
//         StartCoroutine(GoHomeSafe());
//     }

//     IEnumerator GoHomeSafe()
//     {
//         Time.timeScale = 1f;
//         yield return null;

//         var runner = FindObjectOfType<NetworkRunner>();
//         if (runner != null)
//         {
//             runner.Shutdown();
//             Destroy(runner.gameObject);
//         }

//         UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyScene");
//     }
// }

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

//     [Header("Game Over")]
//     public GameObject GameOverScreen;
//     public GameObject LevelCompleteScreen;

//     [Header("Collectibles UI")]
//     public TMP_Text coinText;
//     public TMP_Text diamondText;

//     [Header("Key UI")]
//     public TMP_Text keyText;
//     public GameObject keyUI;

//     [Header("Targets")]
//     public int totalCoins = 3;
//     public int totalDiamonds = 1;
//     public int totalKeys = 6;

//     [Header("Pause")]
//     public GameObject pausePanel;

//     [Header("LEVEL OBJECTS")]
//     public GameObject level1;
//     public GameObject level2;

//     [Header("LEVEL 2 START POINT")]
//     public Transform level2StartPoint;

//     [Networked] private NetworkBool isPausedNetwork { get; set; }
//     [Networked] private NetworkBool isGameOverNetwork { get; set; }
//     [Networked] private NetworkBool isLevelCompleteNetwork { get; set; }

//     private int collectedCoins;
//     private int collectedDiamonds;
//     private int collectedKeys;

//     [HideInInspector] public bool isLevelSwitching = false;

//     // ============================
//     // LIFECYCLE
//     // ============================
//     void Awake()
//     {
//         Instance = this;
//         Time.timeScale = 1f;

//         // ✅ START WITH LEVEL 1
//         if (level1 != null) level1.SetActive(true);
//         if (level2 != null) level2.SetActive(false);

//         pausePanel?.SetActive(false);
//         GameOverScreen?.SetActive(false);
//         LevelCompleteScreen?.SetActive(false);

//         collectedCoins = 0;
//         collectedDiamonds = 0;
//         collectedKeys = 0;

//         UpdateCollectibleUI();
//         UpdateUI();
//     }

//     // ============================
//     // REQUIRED METHODS
//     // ============================
//     public bool IsGameStopped()
//     {
//         return isGameOverNetwork || isLevelCompleteNetwork;
//     }

//     public void CheckAllPlayersFinished()
//     {
//         if (FindObjectsOfType<Player>().All(p => p.HasReachedFinish))
//         {
//             LevelComplete();
//         }
//     }

//     // ============================
//     // 🔑 KEY COLLECTION
//     // ============================
//     public void OnKeyCollected()
//     {
//         collectedKeys = Mathf.Clamp(collectedKeys + 1, 0, totalKeys);
//         UpdateUI();
//     }

//     // ============================
//     // UPDATE
//     // ============================
//     public override void Render()
//     {
//         pausePanel?.SetActive(isPausedNetwork);
//         GameOverScreen?.SetActive(isGameOverNetwork);
//         LevelCompleteScreen?.SetActive(isLevelCompleteNetwork);
        

//         if (!isPausedNetwork)
//             Time.timeScale = 1f;
//     }

//     // ============================
//     // UI VISIBILITY
//     // ============================
//     void UpdateCollectibleUI()
//     {
//         bool isLevel2Active = level2 != null && level2.activeSelf;

//         // Level 1 UI
//         coinText?.transform.parent.gameObject.SetActive(!isLevel2Active);
//         diamondText?.transform.parent.gameObject.SetActive(!isLevel2Active);

//         // Level 2 UI
//         keyUI?.SetActive(isLevel2Active);
//     }

//     // ============================
//     // COLLECTIBLES
//     // ============================
//     public void CollectCoin()
//     {
//         collectedCoins++;
//         UpdateUI();
//     }

//     public void CollectDiamond()
//     {
//         collectedDiamonds++;
//         UpdateUI();
//     }

//     void UpdateUI()
//     {
//         if (coinText != null)
//             coinText.text = $"{collectedCoins} / {totalCoins}";

//         if (diamondText != null)
//             diamondText.text = $"{collectedDiamonds} / {totalDiamonds}";

//         if (keyText != null)
//             keyText.text = $"{collectedKeys} / {totalKeys}";
//     }

//     // ============================
//     // LEVEL COMPLETE FLOW
//     // ============================
//     void LevelComplete()
//     {
//         if (!isLevelSwitching)
//             RPC_SetLevelComplete(true);
//     }

//     [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
//     private void RPC_SetLevelComplete(NetworkBool completed)
//     {
//         isLevelCompleteNetwork = completed;

//         if (completed)
//             StartCoroutine(SwitchToLevel2());
//     }

//     IEnumerator SwitchToLevel2()
//     {
//         isLevelSwitching = true;

//         // Show Level Complete UI
//         LevelCompleteScreen?.SetActive(true);

//         yield return new WaitForSecondsRealtime(2f);

//         // Hide Level Complete UI
//         LevelCompleteScreen?.SetActive(false);

//         // Reset Level 1 collectibles
//         collectedCoins = 0;
//         collectedDiamonds = 0;

//         // Activate Level 2
//         if (level1 != null) level1.SetActive(false);
//         if (level2 != null) level2.SetActive(true);

//         UpdateCollectibleUI();
//         UpdateUI();

//         // Move player to Level 2 start
//         var player = Player.Instance;
//         if (player != null && level2StartPoint != null && player.Object.HasStateAuthority)
//         {
//             var netRb = player.GetComponent<NetworkRigidbody2D>();
//             if (netRb != null)
//             {
//                 netRb.Teleport(level2StartPoint.position);
//                 netRb.Rigidbody.linearVelocity = Vector2.zero;
//                 netRb.Rigidbody.angularVelocity = 0f;
//             }
//         }

//         isLevelCompleteNetwork = false;
//         isLevelSwitching = false;
//     }

//     // ============================
//     // GAME OVER / PAUSE
//     // ============================
//     public void GameOver()
//     {
//         if (!isLevelSwitching)
//             RPC_SetGameOver(true);
//     }

//     [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
//     private void RPC_SetGameOver(NetworkBool gameOver)
//     {
//         isGameOverNetwork = gameOver;
//     }

//     public void OnPauseClicked() => RPC_SetPause(true);
//     public void OnResumeClicked() => RPC_SetPause(false);

//     [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
//     private void RPC_SetPause(NetworkBool paused)
//     {
//         isPausedNetwork = paused;
//     }
// }


using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;
using Fusion.Addons.Physics;
using System.Collections;
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

    [Header("Key UI")]
    public TMP_Text keyText;
    public GameObject keyUI;

    [Header("Targets")]
    public int totalCoins = 3;
    public int totalDiamonds = 1;
    public int totalKeys = 6;

    [Header("Pause")]
    public GameObject pausePanel;

    // 🔧 ONLY ADDITION FOR FIX
    public Image pauseButtonImage;
    public Sprite pauseSprite;
    public Sprite resumeSprite;

    [Header("LEVEL OBJECTS")]
    public GameObject level1;
    public GameObject level2;

    [Header("LEVEL 2 START POINT")]
    public Transform level2StartPoint;

    [Networked] private NetworkBool isPausedNetwork { get; set; }
    [Networked] private NetworkBool isGameOverNetwork { get; set; }
    [Networked] private NetworkBool isLevelCompleteNetwork { get; set; }

    private int collectedCoins;
    private int collectedDiamonds;
    private int collectedKeys;

    [HideInInspector] public bool isLevelSwitching = false;

    // ============================
    // LIFECYCLE
    // ============================
    void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;

        // Start with Level 1
        if (level1 != null) level1.SetActive(true);
        if (level2 != null) level2.SetActive(false);

        pausePanel?.SetActive(false);
        GameOverScreen?.SetActive(false);
        LevelCompleteScreen?.SetActive(false);

        collectedCoins = 0;
        collectedDiamonds = 0;
        collectedKeys = 0;

        UpdateCollectibleUI();
        UpdateUI();
    }

    // ============================
    // REQUIRED METHODS
    // ============================
    public bool IsGameStopped()
    {
        return isGameOverNetwork || isLevelCompleteNetwork;
    }

    public void CheckAllPlayersFinished()
    {
        if (FindObjectsOfType<Player>().All(p => p.HasReachedFinish))
        {
            LevelComplete();
        }
    }

    // ============================
    // KEY COLLECTION
    // ============================
    public void OnKeyCollected()
    {
        collectedKeys = Mathf.Clamp(collectedKeys + 1, 0, totalKeys);
        UpdateUI();
    }

    // ============================
    // UPDATE
    // ============================
    public override void Render()
    {
        pausePanel?.SetActive(isPausedNetwork);
        GameOverScreen?.SetActive(isGameOverNetwork);
        LevelCompleteScreen?.SetActive(isLevelCompleteNetwork);

        // ✅ FIX: CHANGE PAUSE ICON (ONLY FIX)
        if (pauseButtonImage != null)
        {
            pauseButtonImage.sprite = isPausedNetwork ? resumeSprite : pauseSprite;
        }

        if (!isPausedNetwork)
            Time.timeScale = 1f;
    }

    // ============================
    // UI VISIBILITY
    // ============================
    void UpdateCollectibleUI()
    {
        bool isLevel2Active = level2 != null && level2.activeSelf;

        coinText?.transform.parent.gameObject.SetActive(!isLevel2Active);
        diamondText?.transform.parent.gameObject.SetActive(!isLevel2Active);
        keyUI?.SetActive(isLevel2Active);
    }

    // ============================
    // COLLECTIBLES
    // ============================
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
            coinText.text = $"{collectedCoins} / {totalCoins}";

        if (diamondText != null)
            diamondText.text = $"{collectedDiamonds} / {totalDiamonds}";

        if (keyText != null)
            keyText.text = $"{collectedKeys} / {totalKeys}";
    }

    // ============================
    // LEVEL COMPLETE FLOW
    // ============================
    void LevelComplete()
    {
        if (!isLevelSwitching)
            RPC_SetLevelComplete(true);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_SetLevelComplete(NetworkBool completed)
    {
        isLevelCompleteNetwork = completed;

        if (completed)
            StartCoroutine(SwitchToLevel2());
    }

    IEnumerator SwitchToLevel2()
    {
        isLevelSwitching = true;

        LevelCompleteScreen?.SetActive(true);
        yield return new WaitForSecondsRealtime(2f);
        LevelCompleteScreen?.SetActive(false);

        collectedCoins = 0;
        collectedDiamonds = 0;

        if (level1 != null) level1.SetActive(false);
        if (level2 != null) level2.SetActive(true);

        UpdateCollectibleUI();
        UpdateUI();

        var player = Player.Instance;
        if (player != null && level2StartPoint != null && player.Object.HasStateAuthority)
        {
            var netRb = player.GetComponent<NetworkRigidbody2D>();
            if (netRb != null)
            {
                netRb.Teleport(level2StartPoint.position);
                netRb.Rigidbody.linearVelocity = Vector2.zero;
                netRb.Rigidbody.angularVelocity = 0f;
            }
        }

        isLevelCompleteNetwork = false;
        isLevelSwitching = false;
    }

    // ============================
    // GAME OVER / PAUSE
    // ============================
    public void GameOver()
    {
        if (!isLevelSwitching)
            RPC_SetGameOver(true);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_SetGameOver(NetworkBool gameOver)
    {
        isGameOverNetwork = gameOver;
    }

    public void OnPauseClicked() => RPC_SetPause(true);
    public void OnResumeClicked() => RPC_SetPause(false);

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_SetPause(NetworkBool paused)
    {
        isPausedNetwork = paused;
    }
}
