

using UnityEngine;
using Fusion;

public class DoorController : NetworkBehaviour
{
    public static DoorController Instance;

    public GameObject doorBlock;
    public GameObject finishArea;

    private void Awake()
    {
        Instance = this;
    }

    public void OpenDoor()
    {
        if (!Object.HasStateAuthority) return;

        doorBlock.SetActive(false);
        finishArea.SetActive(true);
    }
}


// using Fusion;
// using UnityEngine;
// using System.Collections.Generic;

// public class DoorController : NetworkBehaviour
// {
//     public Collider2D doorBlock;     // DoorBlock collider
//     public Animator doorAnimator;    // Door open animation

//     private HashSet<PlayerRef> playersAtDoor = new HashSet<PlayerRef>();

//     private bool doorOpened = false;

//     // When player enters door area
//     private void OnTriggerEnter2D(Collider2D other)
//     {
//         if (!Object.HasStateAuthority || doorOpened) return;

//         if (other.CompareTag("Player"))
//         {
//             NetworkObject netObj = other.GetComponent<NetworkObject>();
//             playersAtDoor.Add(netObj.InputAuthority);

//             CheckDoorConditions();
//         }
//     }

//     // When player leaves door area
//     private void OnTriggerExit2D(Collider2D other)
//     {
//         if (!Object.HasStateAuthority || doorOpened) return;

//         if (other.CompareTag("Player"))
//         {
//             NetworkObject netObj = other.GetComponent<NetworkObject>();
//             playersAtDoor.Remove(netObj.InputAuthority);
//         }
//     }

//     // Core door logic
//     void CheckDoorConditions()
//     {
//         bool allPlayersAtDoor =
//             playersAtDoor.Count == Runner.ActivePlayers.Count;

//         bool allKeysCollected =
//             KeyManager.Instance.CollectedKeys == KeyManager.Instance.TotalKeys;

//         if (allPlayersAtDoor && allKeysCollected)
//         {
//             OpenDoor();
//         }
//     }

//     void OpenDoor()
//     {
//         doorOpened = true;
//         RPC_OpenDoor();
//     }

//     [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
//     void RPC_OpenDoor()
//     {
//         doorAnimator.SetTrigger("Open");
//         doorBlock.enabled = false;
//     }
// }
