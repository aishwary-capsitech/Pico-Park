using UnityEngine;
using Fusion;
using System.Linq;

public class NetworkedCoin : NetworkBehaviour
{
    [Networked]
    private NetworkBool isCollected { get; set; }

    private void Start()
    {
        if (isCollected)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isCollected) return;

        if (collision.CompareTag("Player"))
        {
            NetworkObject playerNetObj = collision.GetComponent<NetworkObject>();

            if (playerNetObj == null)
            {
                playerNetObj = collision.GetComponentInParent<NetworkObject>();
            }

            if (playerNetObj != null)
            {
                RPC_TryCollect(playerNetObj);
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_TryCollect(NetworkObject player, RpcInfo info = default)
    {
        if (!isCollected)
        {
            isCollected = true;
            RPC_CoinCollected();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_CoinCollected()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.CollectCoin();
        }

        gameObject.SetActive(false);
    }

    public override void Render()
    {
        base.Render();

        if (isCollected && gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }
}