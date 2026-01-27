using Fusion;
using Fusion.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public enum Buttons
{
    Jump = 0,
    Left = 1,
    Right = 2
}

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{

    public static NetworkManager Instance;
    public NetworkPrefabRef[] playerPrefab;
    public GameObject mobileControlsUI;

    [HideInInspector] public bool moveLeft, moveRight, jump = false;

    private Dictionary<PlayerRef, NetworkObject> spawnedPlayers =
        new Dictionary<PlayerRef, NetworkObject>();

    private List<PlayerRef> pendingSpawns = new List<PlayerRef>();
    private bool sceneReady = false;

    [HideInInspector] public NetworkRunner runner;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Show mobile controls only on Android
        //         if (mobileControlsUI != null)
        //         {
        // #if UNITY_ANDROID && !UNITY_EDITOR
        //             mobileControlsUI.SetActive(true);
        // #else
        //             mobileControlsUI.SetActive(false);
        // #endif
        //         }



        runner = FindObjectOfType<NetworkRunner>();

        if (runner != null)
        {
            runner.AddCallbacks(this);
        }
        else
        {
            Debug.LogError("NetworkRunner not found in hierarchy!");
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "GameScene") return;

        // Force EventSystem refresh
        EventSystem.current.SetSelectedGameObject(null);

        // Rebind EventTriggers
        RebindMobileButtons();
    }

    void RebindMobileButtons()
    {
        var triggers = FindObjectsOfType<EventTrigger>(true);

        foreach (var trigger in triggers)
        {
            trigger.triggers.Clear();

            AddTrigger(trigger, EventTriggerType.PointerDown, (data) =>
            {
                if (trigger.gameObject.name.Contains("Left"))
                    OnLeftButtonDown();
                else if (trigger.gameObject.name.Contains("Right"))
                    OnRightButtonDown();
                else if (trigger.gameObject.name.Contains("Up"))
                    OnJumpButtonDown();
            });

            AddTrigger(trigger, EventTriggerType.PointerUp, (data) =>
            {
                if (trigger.gameObject.name.Contains("Left"))
                    OnLeftButtonUp();
                else if (trigger.gameObject.name.Contains("Right"))
                    OnRightButtonUp();
                else if (trigger.gameObject.name.Contains("Up"))
                    OnJumpButtonUp();
            });
        }

        Debug.Log("Mobile buttons rebound successfully");
    }

    void AddTrigger(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = type
        };
        entry.callback.AddListener(action);
        trigger.triggers.Add(entry);
    }

    public void OnLeftButtonDown()
    {
        moveLeft = true;
    }

    public void OnLeftButtonUp()
    {
        moveLeft = false;
    }

    public void OnRightButtonDown()
    {
        moveRight = true;
    }

    public void OnRightButtonUp()
    {
        moveRight = false;
    }

    public void OnJumpButtonDown()
    {
        jump = true;
    }

    public void OnJumpButtonUp()
    {
        jump = false;
    }

    public void RestartGamePlayer()
    {
        if (!runner.IsServer) return;

        // Despawn all existing players
        foreach (var kvp in spawnedPlayers)
        {
            if(kvp.Value != null && kvp.Value.IsValid)
            {
                runner.Despawn(kvp.Value);
            }
        }

        spawnedPlayers.Clear();

        // Respawn all active players at start positions
        foreach (var player in runner.ActivePlayers)
        {
            SpawnPlayer(runner, player);
        }

        Debug.Log("Game Restarted → Players respawned at start");
    }

    private void SpawnPlayer(NetworkRunner runner, PlayerRef player)
    {
        if(spawnedPlayers.ContainsKey(player))
        {
            Debug.LogWarning($"Player {player.PlayerId} already exist!");
            return;
        }

        Vector3 spawnPos = new Vector3(Random.Range(-3, 0), 2, 0);
        int i = player.PlayerId % playerPrefab.Length;
        NetworkPrefabRef playerPref = playerPrefab[i];

        NetworkObject obj = runner.Spawn(playerPref, spawnPos, Quaternion.identity, player);

        if(obj == null)
        {
            Debug.LogError("FAILED TO SPAWN PLAYER! Prefab missing or not in NetworkProjectConfig.");
            return;
        }

        spawnedPlayers.Add(player, obj);
    }

    public void SetRunner(NetworkRunner newRunner)
    {
        // Unbind old runner
        if (runner != null)
        {
            runner.RemoveCallbacks(this);
        }

        runner = newRunner;

        if (runner != null)
        {
            runner.AddCallbacks(this);
            Debug.Log("NetworkManager bound to new NetworkRunner");
        }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (!runner.IsServer) return;

        if (!sceneReady)
        {
            pendingSpawns.Add(player);
            return;
        }

        SpawnPlayer(runner, player);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (spawnedPlayers.TryGetValue(player, out NetworkObject obj))
        {
            if (obj != null)
            {
                runner.Despawn(obj);
            }
            spawnedPlayers.Remove(player);
            Debug.Log("Player removed");
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if(!runner.IsRunning) return;

        var data = new NetworkInputData();
        float move = 0;

        // Keyboard input
        bool leftKey = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        bool rightKey = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        bool upKey = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.Space);

        // Combine keyboard and button input
        bool leftInput = leftKey || moveLeft;
        bool rightInput = rightKey || moveRight;
        bool jumpInput = upKey || jump;

        if (leftInput)
        {
            move -= 1f;
        }

        if (rightInput)
        {
            move += 1f;
        }

        data.horizontalMovement = move;
        data.jumpButton.Set(Buttons.Jump, jumpInput);

        input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.LogWarning("NETWORK SHUTDOWN → " + shutdownReason);

        moveLeft = false;
        moveRight = false;
        jump = false;
        sceneReady = false;

        pendingSpawns.Clear();
        spawnedPlayers.Clear();

        // Clear runner reference ONLY
        if (this.runner == runner)
        {
            this.runner = null;
        }

        if (SceneManager.GetActiveScene().name != "LobbyScene")
        {
            SceneManager.LoadScene("LobbyScene");
        }
    }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        Debug.LogWarning("DEVICE DISCONNECTED → " + reason);
    }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken token) { }
    public void OnSceneLoadDone(NetworkRunner runner)
    {
        if (!runner.IsServer) return;

        sceneReady = true;

        foreach (var player in pendingSpawns)
        {
            SpawnPlayer(runner, player);
        }

        pendingSpawns.Clear();

        Debug.Log("Scene ready → pending players spawned");
}
public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, System.ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
}