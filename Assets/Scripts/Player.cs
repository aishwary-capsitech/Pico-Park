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

    // Animator for visual states
    private Animator animator;
    private float lastMoveInput;
    private float lastRemoteMove;
    private float lastRemoteVertical;
    private Transform capsuleTransform;

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

        // Cache animator if present
        animator = GetComponent<Animator>();
        lastMoveInput = 0f;
        lastRemoteMove = 0f;
        lastRemoteVertical = 0f;

        // Cache child named "Capsule" if present
        var t = transform.Find("Capsule");
        if (t != null)
            capsuleTransform = t;

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

            // Use previous move input so checks compare against prior state
            float prevMoveInput = lastMoveInput;

            // Trigger Move animation on input start
  
                float mv = Mathf.Abs(data.horizontalMovement);
                if (mv > 0.1f && Mathf.Abs(prevMoveInput) <= 0.1f)
                    animator.SetTrigger("Move");
                else if (mv <= 0.1f && Mathf.Abs(prevMoveInput) > 0.1f)
                    animator.SetTrigger("Idle");

            

            // Flip capsule child based on movement direction (use previous input for edge detection)
            if (capsuleTransform != null)
            {
                if (data.horizontalMovement < -0.1f && Mathf.Abs(prevMoveInput) <= 0.1f)
                {
                    // Left -> y = 0
                    capsuleTransform.localEulerAngles = new Vector3(
                        capsuleTransform.localEulerAngles.x,
                        0f,
                        capsuleTransform.localEulerAngles.z);
                }
                else if (data.horizontalMovement > 0.1f && Mathf.Abs(prevMoveInput) <= 0.1f)
                {
                    // Right -> y = 180
                    capsuleTransform.localEulerAngles = new Vector3(
                        capsuleTransform.localEulerAngles.x,
                        180f,
                        capsuleTransform.localEulerAngles.z);
                }
            }

            // store current input for next frame
            lastMoveInput = data.horizontalMovement;

            // JUMP
            var jumpPressed = data.jumpButton.GetPressed(JumpButtonsPrevious);
            JumpButtonsPrevious = data.jumpButton;

            if (jumpPressed.IsSet(Buttons.Jump) && coyoteCounter > 0)
            {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                IsGrounded = false;
                coyoteCounter = 0f;

                // Trigger Jump animation
                if (animator != null)
                {
                    animator.SetTrigger("Jump");
                }

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

    public override void Render()
    {
        base.Render();

        if (animator == null || rb == null)
            return;

        // For remote/non-authoritative instances, trigger move/jump based on velocity changes
        if (!Object.HasStateAuthority)
        {
            float mv = Mathf.Abs(rb.linearVelocity.x);
            float prevRemoteMove = lastRemoteMove;

/*            if (mv > 0.1f && prevRemoteMove <= 0.1f)
                animator.SetTrigger("Move");
            else
                animator.SetTrigger("Idle");*/

            lastRemoteMove = mv;

            float vert = rb.linearVelocity.y;
            float prevRemoteVertical = lastRemoteVertical;

        /*    if (vert > 0.1f && prevRemoteVertical <= 0.1f)
                animator.SetTrigger("Jump");*/
           /* else
                animator.SetTrigger("Idle");*/

            lastRemoteVertical = vert;

            // Flip capsule child for remote instances based on velocity direction (use previous move state)
            if (capsuleTransform != null)
            {
                float vx = rb.linearVelocity.x;
                if (vx < -0.1f && prevRemoteMove <= 0.1f)
                {
                    capsuleTransform.localEulerAngles = new Vector3(
                        capsuleTransform.localEulerAngles.x,
                        0f,
                        capsuleTransform.localEulerAngles.z);
                }
                else if (vx > 0.1f && prevRemoteMove <= 0.1f)
                {
                    capsuleTransform.localEulerAngles = new Vector3(
                        capsuleTransform.localEulerAngles.x,
                        180f,
                        capsuleTransform.localEulerAngles.z);
                }
            }

            // If Jump animation finished, go to Idle (or Move if moving)
            try
            {
                var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                int jumpHash = Animator.StringToHash("Jump");
                if ((stateInfo.IsName("Jump") || stateInfo.shortNameHash == jumpHash) && stateInfo.normalizedTime >= 1f)
                {
                    // Choose Idle or Move based on movement after jump
                    if (Mathf.Abs(lastRemoteMove) <= 0.1f)
                        animator.SetTrigger("Idle");
                    else
                        animator.SetTrigger("Move");
                }
            }
            catch { }
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
            // If we landed while still moving, retrigger Move animation
            if (animator != null)
            {
                float vx = rb != null ? rb.linearVelocity.x : 0f;
                if (Mathf.Abs(vx) > 0.1f)
                {
                    animator.SetTrigger("Move");
                    lastMoveInput = vx;
                }
            }
            // If capsule exists, ensure it's facing correct direction after landing
            if (capsuleTransform != null && rb != null)
            {
                if (rb.linearVelocity.x < -0.1f)
                    capsuleTransform.localEulerAngles = new Vector3(capsuleTransform.localEulerAngles.x, 0f, capsuleTransform.localEulerAngles.z);
                else if (rb.linearVelocity.x > 0.1f)
                    capsuleTransform.localEulerAngles = new Vector3(capsuleTransform.localEulerAngles.x, 180f, capsuleTransform.localEulerAngles.z);
            }
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

