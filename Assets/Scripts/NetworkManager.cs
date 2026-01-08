using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            Instance = this;
        }

        // Show mobile controls only on Android
//         if (mobileControlsUI != null)
//         {
// #if UNITY_ANDROID && !UNITY_EDITOR
//             mobileControlsUI.SetActive(true);
// #else
//             mobileControlsUI.SetActive(false);
// #endif
//         }

        // Network Runner callbacks will run
        var runner = FindObjectOfType<NetworkRunner>();
        if (runner != null)
        {
            runner.AddCallbacks(this);
        }
        else
        {
            Debug.LogError("NetworkRunner not found in hierarchy!");
        }
    }

    // Button event methods - called from UI buttons
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

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            Debug.Log($"OnPlayerJoined: Player {player.PlayerId}");

            Vector3 spawnPos = new Vector3(Random.Range(-3, 0), 2, 0);
            int i = player.PlayerId % playerPrefab.Length;
            NetworkPrefabRef playerPref = playerPrefab[i];

            NetworkObject obj = runner.Spawn(playerPref, spawnPos, Quaternion.identity, player);

            if (obj == null)
            {
                Debug.LogError("FAILED TO SPAWN PLAYER! Prefab missing or not in NetworkProjectConfig.");
                return;
            }

            spawnedPlayers.Add(player, obj);
            Debug.Log("PLAYER SPAWNED SUCCESSFULLY");
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (spawnedPlayers.TryGetValue(player, out NetworkObject obj))
        {
            runner.Despawn(obj);
            spawnedPlayers.Remove(player);
            Debug.Log("Player removed");
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
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
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        Debug.Log("DEVICE DISCONNECTED → " + reason);
    }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken token) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, System.ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
}