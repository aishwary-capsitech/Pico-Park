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



//using Fusion;

//using UnityEngine;

//[RequireComponent(typeof(CapsuleCollider2D))]

//public class PlayerSwipeCrouch : NetworkBehaviour

//{

//    public float standingHeight = 1.8f;

//    public float crouchHeight = 1.0f;

//    private CapsuleCollider2D col;

//    [Networked] private NetworkBool IsCrouching { get; set; }

//    public override void Spawned()

//    {

//        col = GetComponent<CapsuleCollider2D>();

//        if (Object.HasStateAuthority)

//            IsCrouching = false;

//        ApplyCollider();

//    }

//    public void StartCrouch()

//    {

//        if (!Object.HasInputAuthority) return;

//        RPC_SetCrouch(true);

//    }

//    public void StopCrouch()

//    {

//        if (!Object.HasInputAuthority) return;

//        if (!CanStand()) return;

//        RPC_SetCrouch(false);

//    }

//    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]

//    private void RPC_SetCrouch(NetworkBool crouch)

//    {

//        IsCrouching = crouch;

//        ApplyCollider();

//    }

//    private void ApplyCollider()

//    {

//        if (col == null) return;

//        col.size = new Vector2(col.size.x, IsCrouching ? crouchHeight : standingHeight);

//        col.offset = new Vector2(col.offset.x, col.size.y / 2f);

//    }

//    private bool CanStand()

//    {

//        if (col == null) return false;

//        Vector2 originalSize = col.size;

//        Vector2 originalOffset = col.offset;

//        col.size = new Vector2(col.size.x, standingHeight);

//        col.offset = new Vector2(col.offset.x, standingHeight / 2f);

//        RaycastHit2D[] hits = new RaycastHit2D[1];

//        int hitCount = col.Cast(Vector2.up, hits, 0f);

//        col.size = originalSize;

//        col.offset = originalOffset;

//        return hitCount == 0;

//    }

//}

