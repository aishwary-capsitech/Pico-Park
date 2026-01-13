// using UnityEngine;
// using Fusion;

// public class NetworkedCoin : NetworkBehaviour
// {
//     [Networked] private NetworkBool isCollected { get; set; }

//     public override void Spawned()
//     {
//         ApplyVisualState();
//     }

//     private void OnTriggerEnter2D(Collider2D collision)
//     {
//         if (isCollected) return;
//         if (!collision.CompareTag("Player")) return;

//         // Only request collection, do NOT change state here
//         if (Object.HasInputAuthority || Object.HasStateAuthority)
//         {
//             RPC_RequestCollect();
//         }
//     }

//     // Client → Server
//     [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
//     private void RPC_RequestCollect()
//     {
//         if (isCollected) return;

//         isCollected = true;                 // ✅ ONLY StateAuthority writes
//         UIManager.Instance?.CollectCoin();  // ✅ Server-side increment
//     }

//     // Server → Everyone (visual sync only)
//     public override void Render()
//     {
//         ApplyVisualState();
//     }

//     public void RPC_ResetCoin()
//     {
//         if (!Object.HasStateAuthority) return;

//         isCollected = false;
//         ApplyVisualState();
//     }

//     private void ApplyVisualState()
//     {
//         gameObject.SetActive(!isCollected);
//     }
// }


using UnityEngine;
using Fusion;

public class NetworkedCoin : NetworkBehaviour
{
    [Networked] private NetworkBool isCollected { get; set; }

    private Vector3 startPosition;
    private Quaternion startRotation;

    public override void Spawned()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        ApplyState();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!Object.HasStateAuthority) return;
        if (isCollected) return;

        if (other.CompareTag("Player"))
        {
            RPC_Collect();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_Collect()
    {
        isCollected = true;
        UIManager.Instance?.CollectCoin();
        ApplyState();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ResetCoin()
    {
        isCollected = false;
        transform.SetPositionAndRotation(startPosition, startRotation);
        ApplyState();
    }

    void ApplyState()
    {
        gameObject.SetActive(!isCollected);
    }
}
