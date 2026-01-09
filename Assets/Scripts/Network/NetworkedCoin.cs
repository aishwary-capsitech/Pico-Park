//using UnityEngine;
//using Fusion;
//using System.Linq;

//public class NetworkedCoin : NetworkBehaviour
//{
//    [Networked]
//    private NetworkBool isCollected { get; set; }

//    private void Start()
//    {
//        if (isCollected)
//        {
//            gameObject.SetActive(false);
//        }
//    }

//    private void OnTriggerEnter2D(Collider2D collision)
//    {
//        if (isCollected) return;

//        if (collision.CompareTag("Player"))
//        {
//            NetworkObject playerNetObj = collision.GetComponent<NetworkObject>();

//            if (playerNetObj == null)
//            {
//                playerNetObj = collision.GetComponentInParent<NetworkObject>();
//            }

//            if (playerNetObj != null)
//            {
//                RPC_TryCollect(playerNetObj);
//            }
//        }
//    }

//    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
//    private void RPC_TryCollect(NetworkObject player, RpcInfo info = default)
//    {
//        if (!isCollected)
//        {
//            isCollected = true;
//            RPC_CoinCollected();
//        }
//    }

//    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
//    private void RPC_CoinCollected()
//    {
//        if (UIManager.Instance != null)
//        {
//            UIManager.Instance.CollectCoin();
//        }

//        gameObject.SetActive(false);
//    }

//    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
//    public void RPC_ResetCoin()
//    {
//        isCollected = false;
//        gameObject.SetActive(true);
//    }

//    public override void Render()
//    {
//        base.Render();

//        if (isCollected && gameObject.activeSelf)
//        {
//            gameObject.SetActive(false);
//        }
//    }
//}


using UnityEngine;
using Fusion;

public class NetworkedCoin : NetworkBehaviour
{
    [Networked] private NetworkBool isCollected { get; set; }

    public override void Spawned()
    {
        // Visual sync when object spawns
        ApplyVisualState();
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
        isCollected = true;
        UIManager.Instance?.CollectCoin();
        ApplyVisualState();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ResetCoin()
    {
        isCollected = false;
        ApplyVisualState();
    }

    public override void Render()
    {
        ApplyVisualState();
    }

    private void ApplyVisualState()
    {
        gameObject.SetActive(!isCollected);
    }
}
