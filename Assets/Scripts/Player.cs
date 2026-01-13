
// using Fusion;
// using Fusion.Addons.Physics;
// using Unity.Cinemachine;
// using UnityEngine;

// public class Player : NetworkBehaviour
// {
//     public static Player Instance;

//     private NetworkRigidbody2D netRb;
//     private Rigidbody2D rb;

//     [Networked] public NetworkBool HasReachedFinish { get; set; }
//     [Networked] private NetworkBool IsGrounded { get; set; }

//     public float moveSpeed = 6f;
//     public float jumpForce = 12f;

//     private CinemachineCamera cam;

//     private void Awake()
//     {
//         Instance = this;
//     }

//     public override void Spawned()
//     {
//         netRb = GetComponent<NetworkRigidbody2D>();
//         rb = netRb.Rigidbody;

//         if (HasInputAuthority)
//         {
//             cam = FindObjectOfType<CinemachineCamera>();
//             if (cam != null)
//                 cam.Follow = transform;
//         }

//         HasReachedFinish = false;
//     }

//     public override void FixedUpdateNetwork()
//     {
//         if (!Object.HasInputAuthority || rb == null)
//             return;

//         if (UIManager.Instance != null && UIManager.Instance.IsGameStopped())
//         {
//             rb.linearVelocity = Vector2.zero;
//             return;
//         }

//         if (GetInput(out NetworkInputData input))
//         {
//             rb.linearVelocity = new Vector2(input.horizontalMovement * moveSpeed, rb.linearVelocity.y);

//             if (input.jumpButton.IsSet(Buttons.Jump) && IsGrounded)
//             {
//                 rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
//                 IsGrounded = false;
//             }
//         }

//         // FALL GAME OVER
//         if (Object.HasStateAuthority &&
//             rb.position.y < -4f &&
//             UIManager.Instance != null &&
//             !UIManager.Instance.IsGameStopped())
//         {
//             UIManager.Instance.GameOver();
//         }
//     }

//     private void OnCollisionEnter2D(Collision2D col)
//     {
//         if (!Object.HasStateAuthority) return;

//         if (col.gameObject.CompareTag("Ground"))
//             IsGrounded = true;

//         if (col.gameObject.CompareTag("Spike"))
//             UIManager.Instance.GameOver();
//     }

//     private void OnCollisionStay2D(Collision2D col)
//     {
//         if (!Object.HasStateAuthority) return;

//         if (col.gameObject.CompareTag("Finish"))
//         {
//             HasReachedFinish = true;
//             UIManager.Instance.CheckAllPlayersFinished();
//         }
//     }

//     private void OnCollisionExit2D(Collision2D col)
//     {
//         if (!Object.HasStateAuthority) return;

//         if (col.gameObject.CompareTag("Ground"))
//             IsGrounded = false;

//         if (col.gameObject.CompareTag("Finish"))
//             HasReachedFinish = false;
//     }
// }


using Fusion;
using Fusion.Addons.Physics;
using Unity.Cinemachine;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public static Player Instance;

    [HideInInspector] public NetworkRigidbody2D _networkRb;
    private Rigidbody2D rb;

    // TEAM JUMP RAMP
    private TeamJumpRamp teamJumpRamp;
    private bool jumpReported;

    [Networked] public NetworkButtons JumpButtonsPrevious { get; set; }
    [Networked] public NetworkObject Carrier { get; set; }
    [Networked] public NetworkBool HasReachedFinish { get; set; }
    [Networked] private NetworkBool IsGrounded { get; set; }

    // COYOTE TIME (from Dev2)
    float coyoteTime = 0.1f;
    float coyoteCounter;

    [Header("Movement Settings")]
    public float moveSpeed = 6f;
    public float jumpForce = 12f;

    private CinemachineCamera cam;

    private const string GroundTag = "Ground";
    private const string PlayerName = "Player";

    // ============================
    // LIFECYCLE
    // ============================
    private void Awake()
    {
        Instance = this;
    }

    public override void Spawned()
    {
        base.Spawned();

        _networkRb = GetComponent<NetworkRigidbody2D>();
        rb = _networkRb.Rigidbody;

        if (HasInputAuthority)
        {
            cam = FindObjectOfType<CinemachineCamera>();
            if (cam != null)
                cam.Follow = transform;
        }

        HasReachedFinish = false;
        IsGrounded = false;
    }

    // ============================
    // MOVEMENT
    // ============================
    public override void FixedUpdateNetwork()
    {
        if (_networkRb == null || rb == null)
            return;

        if (!Object.HasInputAuthority && !Object.HasStateAuthority)
            return;

        // STOP PLAYER IF GAME IS STOPPED
        if (UIManager.Instance != null &&
            UIManager.Instance.Object != null &&
            UIManager.Instance.Object.IsValid &&
            UIManager.Instance.IsGameStopped())
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // COYOTE TIME UPDATE
        if (IsGrounded)
            coyoteCounter = coyoteTime;
        else
            coyoteCounter -= Runner.DeltaTime;

        if (GetInput(out NetworkInputData data))
        {
            rb.linearVelocity = new Vector2(
                data.horizontalMovement * moveSpeed,
                rb.linearVelocity.y
            );

            var jumpPressed = data.jumpButton.GetPressed(JumpButtonsPrevious);
            JumpButtonsPrevious = data.jumpButton;

            if (jumpPressed.IsSet(Buttons.Jump) && coyoteCounter > 0f)
            {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                IsGrounded = false;
                coyoteCounter = 0f;

                // TEAM JUMP RAMP
                if (teamJumpRamp != null && !jumpReported)
                {
                    teamJumpRamp.PlayerJumped();
                    jumpReported = true;
                }
            }
        }

        // SAFE GAME OVER CHECK
        if (Object.HasStateAuthority &&
            rb.position.y < -4f &&
            UIManager.Instance != null &&
            UIManager.Instance.Object != null &&
            UIManager.Instance.Object.IsValid &&
            !UIManager.Instance.IsGameStopped() &&
            !UIManager.Instance.isLevelSwitching)
        {
            UIManager.Instance.GameOver();
        }
    }

    // ============================
    // COLLISIONS
    // ============================
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (Object == null || !Object.IsValid || !Object.HasStateAuthority)
            return;

        if (UIManager.Instance != null &&
            UIManager.Instance.Object != null &&
            UIManager.Instance.Object.IsValid &&
            UIManager.Instance.IsGameStopped())
            return;

        if (other.gameObject.CompareTag(GroundTag) ||
            other.gameObject.name.Contains(PlayerName))
        {
            IsGrounded = true;
            jumpReported = false;
        }

        if (other.gameObject.TryGetComponent(out TeamJumpRamp r))
            teamJumpRamp = r;

        if (other.gameObject.name.Contains("Spike") ||
            other.gameObject.name.Contains("Pendulum"))
        {
            UIManager.Instance?.GameOver();
        }

        if (other.gameObject.name.Contains("Coin"))
        {
            UIManager.Instance?.CollectCoin();
            Destroy(other.gameObject);
        }

        if (other.gameObject.name.Contains("Diamond"))
        {
            UIManager.Instance?.CollectDiamond();
            Destroy(other.gameObject);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (Object == null || !Object.IsValid || !Object.HasStateAuthority)
            return;

        if (UIManager.Instance != null &&
            UIManager.Instance.Object != null &&
            UIManager.Instance.Object.IsValid &&
            UIManager.Instance.IsGameStopped())
            return;

        if (collision.gameObject.CompareTag(GroundTag) ||
            collision.gameObject.name.Contains(PlayerName))
        {
            IsGrounded = true;
            jumpReported = false;
        }

        if (collision.gameObject.CompareTag("Finish"))
        {
            HasReachedFinish = true;
            UIManager.Instance?.CheckAllPlayersFinished();
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (Object == null || !Object.IsValid || !Object.HasStateAuthority)
            return;

        if (UIManager.Instance != null &&
            UIManager.Instance.Object != null &&
            UIManager.Instance.Object.IsValid &&
            UIManager.Instance.IsGameStopped())
            return;

        if (other.gameObject.CompareTag(GroundTag) ||
            other.gameObject.name.Contains(PlayerName))
        {
            IsGrounded = false;
        }

        if (other.gameObject.TryGetComponent(out TeamJumpRamp r))
            teamJumpRamp = null;

        if (other.gameObject.CompareTag("Finish"))
            HasReachedFinish = false;
    }
}
