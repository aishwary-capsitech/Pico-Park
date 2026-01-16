
//working one

// using Fusion;
// using UnityEngine;

// public class PlayerSwipeCrouch : NetworkBehaviour
// {
//     [Header("Crouch Settings")]
//     public float crouchHeight = 0.1f;
//     public float normalHeight = 0.2f;

//     [Networked] private NetworkBool IsCrouching { get; set; }

//     private Vector3 baseScale;
    

//     public override void Spawned()
//     {
//         baseScale = transform.localScale;

//         // IMPORTANT: reset crouch on spawn / restart
//         if (Object.HasStateAuthority)
//             IsCrouching = false;
//     }

//     // Called from SwipeManager (LOCAL INPUT ONLY)
//     public void StartCrouch()
//     {
//         if (!Object.HasInputAuthority) return;

//         RPC_SetCrouch(true);
//     }

//     public void StopCrouch()
//     {
//         if (!Object.HasInputAuthority) return;

//         RPC_SetCrouch(false);
//     }

//     [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
//     private void RPC_SetCrouch(NetworkBool crouch)
//     {
//         IsCrouching = crouch;
//     }

//     public override void Render()
//     {
//         Vector3 scale = baseScale;

//         if (IsCrouching)
//             scale.y = crouchHeight;
//         else
//             scale.y = normalHeight;

//         transform.localScale = scale;
//     }
// }


// using Fusion;
// using UnityEngine;

// public class PlayerSwipeCrouch : NetworkBehaviour
// {
//     [Header("Crouch Settings")]
//     public float crouchHeight = 0.1f;
//     public float normalHeight = 0.2f;

//     [Header("Clearance Check")]
//     public LayerMask obstacleLayer;
//     public float checkWidth = 0.25f;

//     [Networked] private NetworkBool IsCrouching { get; set; }

//     private Vector3 baseScale;

//     public override void Spawned()
//     {
//         baseScale = transform.localScale;

//         if (Object.HasStateAuthority)
//             IsCrouching = false;
//     }

//     public void StartCrouch()
//     {
//         if (!Object.HasInputAuthority) return;
//         RPC_SetCrouch(true);
//     }

//     public void StopCrouch()
//     {
//         if (!Object.HasInputAuthority) return;

//         // 🔒 BLOCK if normal height would collide
//         if (!HasStandingClearance())
//             return;

//         RPC_SetCrouch(false);
//     }

//     [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
//     private void RPC_SetCrouch(NetworkBool crouch)
//     {
//         IsCrouching = crouch;
//     }

//     public override void Render()
//     {
//         Vector3 scale = baseScale;
//         scale.y = IsCrouching ? crouchHeight : normalHeight;
//         transform.localScale = scale;
//     }

//     // ✅ THIS IS THE FIX
//     private bool HasStandingClearance()
//     {
//         Vector2 origin = (Vector2)transform.position + Vector2.up * (normalHeight / 2f);

//         Vector2 size = new Vector2(checkWidth, normalHeight);

//         RaycastHit2D hit = Physics2D.BoxCast(
//             origin,
//             size,
//             0f,
//             Vector2.up,
//             0f,
//             obstacleLayer
//         );

//         return hit.collider == null;
//     }

//     private void OnDrawGizmosSelected()
//     {
//         Gizmos.color = Color.green;

//         Vector2 origin = (Vector2)transform.position + Vector2.up * (normalHeight / 2f);
//         Vector2 size = new Vector2(checkWidth, normalHeight);

//         Gizmos.DrawWireCube(origin, size);
//     }
// }



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

    // =========================
    // INPUT (UNCHANGED)
    // =========================

    public void StartCrouch()
    {
        if (!Object.HasInputAuthority) return;
        RPC_SetCrouch(true);
    }

    public void StopCrouch()
    {
        if (!Object.HasInputAuthority) return;

        // ❌ Block standing if not physically possible
        if (!CanStand())
            return;

        RPC_SetCrouch(false);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_SetCrouch(NetworkBool crouch)
    {
        IsCrouching = crouch;
    }

    // =========================
    // AUTO-RESOLUTION LOGIC (THE FIX)
    // =========================

    public override void FixedUpdateNetwork()
    {
        // 🔑 Permanent logic:
        // As soon as space is available → return to normal
        if (IsCrouching && CanStand())
        {
            RPC_SetCrouch(false);
        }
    }

    // =========================
    // PHYSICS CLEARANCE CHECK
    // =========================

    private bool CanStand()
    {
        if (col == null) return true;

        // Save current collider
        Vector2 originalSize = col.size;
        Vector2 originalOffset = col.offset;

        // Simulate standing collider
        float standScaleFactor = normalHeight / Mathf.Max(crouchHeight, 0.0001f);
        float standHeight = originalSize.y * standScaleFactor;

        col.size = new Vector2(originalSize.x, standHeight);
        col.offset = new Vector2(col.offset.x, standHeight / 2f);

        RaycastHit2D[] hits = new RaycastHit2D[1];
        int hitCount = col.Cast(Vector2.up, hits, 0f);

        // Restore collider
        col.size = originalSize;
        col.offset = originalOffset;

        return hitCount == 0;
    }

    // =========================
    // VISUAL (UNCHANGED)
    // =========================

    public override void Render()
    {
        Vector3 scale = baseScale;

        scale.y = IsCrouching ? crouchHeight : normalHeight;
        transform.localScale = scale;
    }
}
