using Fusion;
using Fusion.Addons.Physics;
using Unity.Cinemachine;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public static Player Instance;

    [HideInInspector] public NetworkRigidbody2D networkRb;
    private Rigidbody2D rb;

    // TEAM JUMP RAMP
    private TeamJumpRamp teamJumpRamp;
    private bool jumpReported;

    [Networked] public NetworkButtons JumpButtonsPrevious { get; set; }
    [Networked] public NetworkObject Carrier { get; set; }
    [Networked] public NetworkBool HasReachedFinish { get; set; }
    [Networked] private NetworkBool IsGrounded { get; set; }

    [Header("Coyote Time")]
    public float coyoteTime = 0.05f;
    private float coyoteCounter;

    [Header("Movement Settings")]
    public float moveSpeed = 6f;
    public float jumpForce = 12f;

    private CinemachineCamera cam;

    private const string GroundTag = "Ground";
    private const string PlayerName = "Player";

    private void Awake()
    {
        Instance = this;
    }

    public override void Spawned()
    {
        networkRb = GetComponent<NetworkRigidbody2D>();
        rb = networkRb.Rigidbody;

        if (HasInputAuthority)
        {
            cam = FindObjectOfType<CinemachineCamera>();
            if (cam != null)
                cam.Follow = transform;
        }

        HasReachedFinish = false;
    }

    public override void FixedUpdateNetwork()
    {
        if (rb == null)
            return;

        // IMPORTANT: ONLY STATE AUTHORITY MOVES PLAYER
        if (!Object.HasStateAuthority)
            return;

        // STOP PLAYER WHEN GAME PAUSED / OVER
        if (UIManager.Instance != null &&
            UIManager.Instance.Object != null &&
            UIManager.Instance.Object.IsValid &&
            UIManager.Instance.IsGameStopped())
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // COYOTE TIME
        if (IsGrounded)
            coyoteCounter = coyoteTime;
        else
            coyoteCounter -= Runner.DeltaTime;

        if (GetInput(out NetworkInputData data))
        {
            // HORIZONTAL MOVEMENT
            rb.linearVelocity = new Vector2(
                data.horizontalMovement * moveSpeed,
                rb.linearVelocity.y
            );

            // JUMP
            var jumpPressed = data.jumpButton.GetPressed(JumpButtonsPrevious);
            JumpButtonsPrevious = data.jumpButton;

            if (jumpPressed.IsSet(Buttons.Jump) && coyoteCounter > 0)
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

        // FALL DEATH CHECK
        if (rb.position.y < -4f &&
            UIManager.Instance != null &&
            !UIManager.Instance.IsGameStopped())
        {
            UIManager.Instance.GameOver();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!Object.HasStateAuthority)
            return;

        if (UIManager.Instance != null && UIManager.Instance.IsGameStopped())
            return;

        if (other.gameObject.CompareTag(GroundTag) ||
            other.gameObject.name.Contains(PlayerName))
        {
            IsGrounded = true;
            jumpReported = false;
        }

        if (other.gameObject.TryGetComponent(out TeamJumpRamp ramp))
        {
            teamJumpRamp = ramp;
        }

        if (other.gameObject.name.Contains("Spike") ||
            other.gameObject.name.Contains("Pendulum"))
        {
            UIManager.Instance.GameOver();
        }

        if (other.gameObject.name.Contains("Coin"))
        {
            UIManager.Instance.CollectCoin();
            Destroy(other.gameObject);
        }

        if (other.gameObject.name.Contains("Diamond"))
        {
            UIManager.Instance.CollectDiamond();
            Destroy(other.gameObject);
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (!Object.HasStateAuthority)
            return;

        if (other.gameObject.CompareTag(GroundTag) ||
            other.gameObject.name.Contains(PlayerName))
        {
            IsGrounded = true;
            jumpReported = false;
        }

        if (other.gameObject.CompareTag("Finish"))
        {
            HasReachedFinish = true;
            UIManager.Instance.CheckAllPlayersFinished();
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (!Object.HasStateAuthority)
            return;

        if (other.gameObject.CompareTag(GroundTag) ||
            other.gameObject.name.Contains(PlayerName))
        {
            IsGrounded = false;
        }

        if (other.gameObject.TryGetComponent(out TeamJumpRamp ramp))
        {
            teamJumpRamp = null;
        }

        if (other.gameObject.CompareTag("Finish"))
        {
            HasReachedFinish = false;
        }
    }
}

