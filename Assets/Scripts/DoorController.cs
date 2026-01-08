// using Fusion;
// using UnityEngine;

// public class DoorController : NetworkBehaviour
// {
//     public static DoorController Instance;

//     public GameObject doorBlock;
//     public GameObject finishArea;

//     [Networked] private NetworkBool isUnlocked { get; set; }

//     public override void Spawned()
//     {
//         Instance = this;

//         finishArea.SetActive(false);
//     }

//     public void UnlockDoor()
//     {
//         if (!Object.HasStateAuthority) return;
//         RPC_OpenDoor();
//     }

//     [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
//     void RPC_OpenDoor()
//     {
//         doorBlock.SetActive(false);
//         finishArea.SetActive(true);
//         isUnlocked = true;
//     }
// }

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
