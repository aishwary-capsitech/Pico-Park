
using Fusion;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerSwipeCrouch : NetworkBehaviour
{
    [Header("Visual Scale (your original logic)")]
    public float crouchHeight = 0.1f;
    public float normalHeight = 0.2f;

    [Networked] private NetworkBool IsCrouching { get; set; }

    private Vector3 baseScale;
    private CapsuleCollider2D col;

    public override void Spawned()
    {
        baseScale = transform.localScale;
        col = GetComponent<CapsuleCollider2D>();

        if (Object.HasStateAuthority)
            IsCrouching = false;
    }
    public void StartCrouch()
    {
        if (!Object.HasInputAuthority) return;
        RPC_SetCrouch(true);
    }

    public void StopCrouch()
    {
        if (!Object.HasInputAuthority) return;

        if (!CanStand())
            return;

        RPC_SetCrouch(false);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_SetCrouch(NetworkBool crouch)
    {
        IsCrouching = crouch;
    }
   
    public override void FixedUpdateNetwork()
    {
        // FIX: prevent client-side flicker
        if (!Object.HasStateAuthority)
            return;

        if (IsCrouching && CanStand())
        {
            RPC_SetCrouch(false);
        }
    }

    private bool CanStand()
    {
        if (col == null) return true;

        Vector2 originalSize = col.size;
        Vector2 originalOffset = col.offset;

        float standScaleFactor = normalHeight / Mathf.Max(crouchHeight, 0.0001f);
        float standHeight = originalSize.y * standScaleFactor;

        col.size = new Vector2(originalSize.x, standHeight);
        col.offset = new Vector2(col.offset.x, standHeight / 2f);

        RaycastHit2D[] hits = new RaycastHit2D[1];
        int hitCount = col.Cast(Vector2.up, hits, 0f);

        col.size = originalSize;
        col.offset = originalOffset;

        return hitCount == 0;
    }
    public override void Render()
    {
        Vector3 scale = baseScale;
        scale.y = IsCrouching ? crouchHeight : normalHeight;
        transform.localScale = scale;
    }
}

