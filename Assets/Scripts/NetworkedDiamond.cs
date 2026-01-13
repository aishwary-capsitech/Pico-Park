// using UnityEngine;
// using Fusion;

// public class NetworkedDiamond : NetworkBehaviour
// {
//     [Networked]
//     private NetworkBool isCollected { get; set; }

//     private void Start()
//     {
//         if (isCollected)
//         {
//             gameObject.SetActive(false);
//         }
//     }

//     private void OnTriggerEnter2D(Collider2D collision)
//     {
//         if (isCollected) return;

//         if (collision.CompareTag("Player"))
//         {
//             NetworkObject playerNetObj = collision.GetComponent<NetworkObject>();

//             if (playerNetObj == null)
//             {
//                 playerNetObj = collision.GetComponentInParent<NetworkObject>();
//             }

//             if (playerNetObj != null)
//             {
//                 // ANYONE can collect - let server decide
//                 RPC_TryCollect(playerNetObj);
//             }
//         }
//     }

//     [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
//     private void RPC_TryCollect(NetworkObject player, RpcInfo info = default)
//     {
//         if (!isCollected)
//         {
//             isCollected = true;
//             RPC_DiamondCollected();
//         }
//     }

//     [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
//     private void RPC_DiamondCollected()
//     {
//         if (UIManager.Instance != null)
//         {
//             UIManager.Instance.CollectDiamond();
//         }

//         gameObject.SetActive(false);
//     }

//     public override void Render()
//     {
//         base.Render();

//         if (isCollected && gameObject.activeSelf)
//         {
//             gameObject.SetActive(false);
//         }
//     }
// }


using UnityEngine;
using Fusion;

public class NetworkedDiamond : NetworkBehaviour
{
    [Networked] private NetworkBool isCollected { get; set; }

    public override void Spawned()
    {
        ApplyVisual();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isCollected) return;
        if (!Object.HasStateAuthority) return;

        if (collision.CompareTag("Player"))
        {
            RPC_Collect();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_Collect()
    {
        if (isCollected) return;

        isCollected = true;
        UIManager.Instance?.CollectDiamond();
        ApplyVisual();
    }

    // ✅ REQUIRED FOR RESTART
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ResetDiamond()
    {
        isCollected = false;
        ApplyVisual();
    }

    public override void Render()
    {
        ApplyVisual();
    }

    private void ApplyVisual()
    {
        gameObject.SetActive(!isCollected);
    }
}
