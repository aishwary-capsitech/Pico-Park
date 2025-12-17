using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
   
    public NetworkPrefabRef playerPrefab;

    private Dictionary<PlayerRef, NetworkObject> spawnedPlayers =
        new Dictionary<PlayerRef, NetworkObject>();

    // private void Awake()
    // {
    //     // Network Runner  callbacks will run (we need to call the network runner callbacks to run)
    //     var runner = FindObjectOfType<NetworkRunner>();
    //     if (runner != null)
    //     {
    //         runner.AddCallbacks(this);
    //     }
    //     else
    //     {
    //         Debug.LogError("NetworkRunner not found in hierarchy!");
    //     }
    // }

 
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if(runner .IsServer)
        {
           Debug.Log($"OnPlayerJoined: Player {player.PlayerId}");

        Vector3 spawnPos = new Vector3(Random.Range(-10,10) , 1 , 0); 

        NetworkObject obj = runner.Spawn(playerPrefab, spawnPos, Quaternion.identity, player);

        if (obj == null)
        {
            Debug.LogError(" FAILED TO SPAWN PLAYER! Prefab missing or not in NetworkProjectConfig.");
            return;
        }

        spawnedPlayers.Add(player, obj);
        Debug.Log(" PLAYER SPAWNED SUCCESSFULLY"); 
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

   
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
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
