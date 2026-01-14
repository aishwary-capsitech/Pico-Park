using UnityEngine;
using Fusion;

public class NetworkedKey : NetworkBehaviour
{
    [Networked] private NetworkBool isCollected { get; set; }

    public override void Spawned()
    {
        // Ensure correct visual state on spawn / rejoin
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

        // Update UI through UIManager
        if (UIManager.Instance != null)
        {
            UIManager.Instance.CollectKey();
        }

        ApplyVisualState();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ResetKey()
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