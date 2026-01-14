// using Fusion;
// using System.Linq;
// using UnityEngine;

// public class TeamJumpRamp : NetworkBehaviour
// {
//     [Header("Ramp Settings")]
//     public float jumpTimeWindow = 0.4f;
//     public float moveDistance = 4f;
//     public float moveSpeed = 2f;

//     [Header("Failure Settings")]
//     public float fallSpeed = 6f;

//     [Networked] private int RequiredPlayers { get; set; }
//     [Networked] private int LandedPlayers { get; set; }
//     [Networked] private bool IsMoving { get; set; }
//     [Networked] private bool IsFalling { get; set; }

//     [Networked] private TickTimer JumpTimer { get; set; }

//     private Vector3 startPos;
//     private Vector3 targetPos;

//     public override void Spawned()
//     {
//         startPos = transform.position;
//         targetPos = startPos + Vector3.right * moveDistance;

//         if (Object.HasStateAuthority)
//         {
//             UpdateRequiredPlayers();
//         }
//     }

//     private void UpdateRequiredPlayers()
//     {
//         int totalPlayers = Runner.ActivePlayers.Count();

//         RequiredPlayers = Mathf.Clamp(totalPlayers, 1, 3);

//         Debug.Log($"[TeamJumpRamp] Players: {totalPlayers}, Required Jumps: {RequiredPlayers}");
//     }

//     /// <summary>
//     /// Called by Player when jump happens on ramp
//     /// </summary>
//     public void PlayerJumped()
//     {
//         if (!Object.HasStateAuthority)
//             return;

//         if (IsMovingForward || IsReturning)
//             return;

//         JumpedPlayers++;

//         LandedPlayers++;

//         // First player starts sync window
//         if (LandedPlayers == 1)
//         {
//             JumpTimer = TickTimer.CreateFromSeconds(Runner, jumpTimeWindow);
//         }
//     }

//     public override void FixedUpdateNetwork()
//     {
//         if (!Object.HasStateAuthority)
//             return;

//         // Check simultaneous jump window
//         if (JumpTimer.IsRunning && JumpTimer.Expired(Runner))
//         {
//             if (LandedPlayers >= RequiredPlayers)
//             {
//                 // ✅ SUCCESS
//                 IsMoving = true;
//             }
//             else
//             {
//                 // ❌ FAILURE
//                 IsFalling = true;
//                 UIManager.Instance.GameOver();
//             }

//             JumpedPlayers = 0;
//             JumpTimer = TickTimer.None;
//         }

//         // -------------------------
//         // MOVE FORWARD
//         // -------------------------
//         if (IsMoving)
//         {
//             transform.position = Vector3.MoveTowards(
//                 transform.position,
//                 targetPos,
//                 moveSpeed * Runner.DeltaTime
//             );

//             if (Vector3.Distance(transform.position, targetPos) < 0.01f)
//             {
//                 IsMoving = false;
//             }
//         }

//         // -------------------------
//         // FALL DOWN
//         // -------------------------
//         if (IsFalling)
//         {
//             IsReturning = true;
//             ReturnTimer = TickTimer.None;
//         }

//         // Move back
//         if (IsReturning)
//         {
//             transform.position = Vector3.MoveTowards(
//                 transform.position,
//                 startPos,
//                 moveSpeed * Runner.DeltaTime
//             );

//             if (Vector3.Distance(transform.position, startPos) < 0.01f)
//             {
//                 IsReturning = false;
//                 JumpedPlayers = 0;
//             }
//         }
//     }
// }


using Fusion;

using System.Linq;

using UnityEngine;
 
public class TeamJumpRamp : NetworkBehaviour

{

    [Header("Ramp Settings")]

    public float jumpTimeWindow = 0.3f;

    public float moveDistance = 6f;

    public float moveSpeed = 2f;

    public float returnDelay = 3f;
 
    [Networked] private int RequiredPlayers { get; set; }

    [Networked] private int JumpedPlayers { get; set; }

    [Networked] private bool IsMovingForward { get; set; }

    [Networked] private bool IsReturning { get; set; }
 
    [Networked] private TickTimer JumpTimer { get; set; }

    [Networked] private TickTimer ReturnTimer { get; set; }
 
    private Vector3 startPos;

    private Vector3 targetPos;
 
    public override void Spawned()

    {

        startPos = transform.position;

        targetPos = startPos + Vector3.right * moveDistance;
 
        if (Object.HasStateAuthority)

        {

            UpdateRequiredPlayers();

        }

    }
 
    private void UpdateRequiredPlayers()

    {

        int totalPlayers = Runner.ActivePlayers.Count();
 
        RequiredPlayers = Mathf.Clamp(totalPlayers, 1, 3);
 
        Debug.Log($"[TeamJumpRamp] Players: {totalPlayers}, Required Jumps: {RequiredPlayers}");

    }
 
    /// <summary>

    /// Called by Player when jump happens on ramp

    /// </summary>

    public void PlayerJumped()

    {

        if (!Object.HasStateAuthority)

            return;
 
        if (IsMovingForward || IsReturning)

            return;
 
        JumpedPlayers++;
 
        if (JumpedPlayers == 1)

        {

            JumpTimer = TickTimer.CreateFromSeconds(Runner, jumpTimeWindow);

        }

    }
 
    public override void FixedUpdateNetwork()

    {

        if (!Object.HasStateAuthority)

            return;
 
        // Check simultaneous jump window

        if (JumpTimer.IsRunning && JumpTimer.Expired(Runner))

        {

            if (JumpedPlayers >= RequiredPlayers)

            {

                IsMovingForward = true;

                ReturnTimer = TickTimer.CreateFromSeconds(Runner, returnDelay);

            }
 
            JumpedPlayers = 0;

            JumpTimer = TickTimer.None;

        }
 
        // Move forward

        if (IsMovingForward)

        {

            transform.position = Vector3.MoveTowards(

                transform.position,

                targetPos,

                moveSpeed * Runner.DeltaTime

            );
 
            if (Vector3.Distance(transform.position, targetPos) < 0.01f)

            {

                IsMovingForward = false;

            }

        }
 
        // Return after delay

        if (!IsMovingForward && ReturnTimer.IsRunning && ReturnTimer.Expired(Runner))

        {

            IsReturning = true;

            ReturnTimer = TickTimer.None;

        }
 
        // Move back

        if (IsReturning)

        {

            transform.position = Vector3.MoveTowards(

                transform.position,

                startPos,

                moveSpeed * Runner.DeltaTime

            );
 
            if (Vector3.Distance(transform.position, startPos) < 0.01f)

            {

                IsReturning = false;

                JumpedPlayers = 0;

            }

        }

    }

}
 