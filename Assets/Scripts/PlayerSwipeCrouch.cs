using Fusion;
using UnityEngine;

public class PlayerSwipeCrouch : NetworkBehaviour
{
    [Header("Crouch Settings")]
    public float crouchHeight = 0.1f;
    public float normalHeight = 0.2f;

    [Networked] private NetworkBool IsCrouching { get; set; }

    private Vector3 baseScale;

    public override void Spawned()
    {
        baseScale = transform.localScale;

        // IMPORTANT: reset crouch on spawn / restart
        if (Object.HasStateAuthority)
            IsCrouching = false;
    }

    // Called from SwipeManager (LOCAL INPUT ONLY)
    public void StartCrouch()
    {
        if (!Object.HasInputAuthority) return;

        RPC_SetCrouch(true);
    }

    public void StopCrouch()
    {
        if (!Object.HasInputAuthority) return;

        RPC_SetCrouch(false);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_SetCrouch(NetworkBool crouch)
    {
        IsCrouching = crouch;
    }

    public override void Render()
    {
        Vector3 scale = baseScale;

        if (IsCrouching)
            scale.y = crouchHeight;
        else
            scale.y = normalHeight;

        transform.localScale = scale;
    }
}
