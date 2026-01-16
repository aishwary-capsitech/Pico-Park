using UnityEngine;
using Fusion;

public class NetworkedDiamond : NetworkBehaviour
{
    [Networked] private NetworkBool isCollected { get; set; }

    public override void Spawned()
    {
        // Sync visual state when spawned
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
        UIManager.Instance?.CollectDiamond();
        ApplyVisualState();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ResetDiamond()
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