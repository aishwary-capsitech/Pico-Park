// using Fusion;
// using UnityEngine;

// [RequireComponent(typeof(Collider2D))]
// public class DoorController : NetworkBehaviour
// {
//     [Header("Door Parts")]
//     [SerializeField] private GameObject doorBlock;
//     [SerializeField] private GameObject doorOpen;
//     [SerializeField] private GameObject finishArea;

//     [Header("Logic")]
//     [SerializeField] private KeyManager keyManager;

//     [Networked] private NetworkBool IsOpen { get; set; }

//     private bool lastState;

//     public override void Spawned()
//     {
//         ApplyState(IsOpen);
//         lastState = IsOpen;
//     }

//     public override void FixedUpdateNetwork()
//     {
//         if (IsOpen != lastState)
//         {
//             lastState = IsOpen;
//             ApplyState(IsOpen);
//         }
//     }

//     private void OnTriggerEnter2D(Collider2D other)
//     {
//         if (!other.CompareTag("Player")) return;

//         if (Object.HasStateAuthority)
//         {
//             TryOpenDoor();
//         }
//         else
//         {
//             RPC_TryOpenDoor();
//         }
//     }

//     // HOST ONLY
//     private void TryOpenDoor()
//     {
//         if (!Object.HasStateAuthority) return;

//         if (keyManager != null && keyManager.AllKeysCollected())
//         {
//             OpenDoor();
//         }
//     }

//     [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
//     private void RPC_TryOpenDoor()
//     {
//         TryOpenDoor();
//     }

//     public void OpenDoor()
//     {
//         if (!Object.HasStateAuthority) return;
//         IsOpen = true;
//     }

//     private void ApplyState(bool open)
//     {
//         doorBlock.SetActive(!open);
//         doorOpen.SetActive(open);
//         finishArea.SetActive(open);
//     }
// }


// using Fusion;
// using UnityEngine;

// [RequireComponent(typeof(Collider2D))]
// public class DoorController : NetworkBehaviour
// {
//     [Header("Door Parts")]
//     [SerializeField] private GameObject doorBlock;
//     [SerializeField] private GameObject doorOpen;
//     [SerializeField] private GameObject finishArea;

//     [Header("Logic")]
//     [SerializeField] private KeyManager keyManager;

//     // Networked state (single source of truth)
//     [Networked] private NetworkBool IsOpen { get; set; }

//     // =========================
//     // SPAWN
//     // =========================
//     public override void Spawned()
//     {
//         ApplyState(IsOpen);
//     }

//     // =========================
//     // VISUAL SYNC (IMPORTANT)
//     // =========================
//     public override void Render()
//     {
//         ApplyState(IsOpen);
//     }

//     // =========================
//     // TRIGGER
//     // =========================
//     private void OnTriggerEnter2D(Collider2D other)
//     {
//         if (!other.CompareTag("Player"))
//             return;

//         // ONLY state authority decides
//         if (!Object.HasStateAuthority)
//             return;

//         TryOpenDoor();
//     }

//     // =========================
//     // DOOR LOGIC (AUTHORITY)
//     // =========================
//     private void TryOpenDoor()
//     {
//         if (keyManager != null && keyManager.AllKeysCollected())
//         {
//             IsOpen = true;
//         }
//     }

//     // =========================
//     // CALLED BY KEY MANAGER
//     // =========================
//     public void OpenDoorFromKeys()
//     {
//         if (!Object.HasStateAuthority)
//             return;

//         IsOpen = true;
//     }

//     // =========================
//     // APPLY VISUAL STATE
//     // =========================
//     private void ApplyState(bool open)
//     {
//         if (doorBlock != null)
//             doorBlock.SetActive(!open);

//         if (doorOpen != null)
//             doorOpen.SetActive(open);

//         if (finishArea != null)
//             finishArea.SetActive(open);
//     }
// }


using Fusion;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DoorController : NetworkBehaviour
{
    [Header("Door Parts")]
    [SerializeField] private GameObject doorBlock;
    [SerializeField] private GameObject doorOpen;
    [SerializeField] private GameObject finishArea;

    [Header("Logic")]
    [SerializeField] private KeyManager keyManager;

    // Networked door state
    [Networked] private NetworkBool IsOpen { get; set; }

    // =========================
    // SPAWN
    // =========================
    public override void Spawned()
    {
        ApplyState(IsOpen);
    }

    // =========================
    // VISUAL SYNC (HOST + CLIENT)
    // =========================
    public override void Render()
    {
        ApplyState(IsOpen);
    }

    // =========================
    // PLAYER REACHES DOOR
    // =========================
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Only State Authority decides
        if (!Object.HasStateAuthority)
            return;

        TryOpenDoor();
    }

    // =========================
    // DOOR OPEN LOGIC
    // =========================
    private void TryOpenDoor()
    {
        // 🔑 Door opens ONLY if all keys are collected
        if (keyManager != null && keyManager.AllKeysCollected())
        {
            IsOpen = true;
        }
    }

    // =========================
    // APPLY VISUAL STATE
    // =========================
    private void ApplyState(bool open)
    {
        doorBlock.SetActive(!open);
        doorOpen.SetActive(open);
        finishArea.SetActive(open);
    }
}

