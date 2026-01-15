//using Fusion;
//using Fusion.Addons.Physics;
//using Unity.Cinemachine;
//using UnityEngine;

//public class Player : NetworkBehaviour
//{
//    public static Player Instance;

//    [HideInInspector] public NetworkRigidbody2D _networkRb;
//    private Rigidbody2D rb;

//    // TEAM JUMP RAMP
//    private TeamJumpRamp teamJumpRamp;
//    private bool jumpReported;

//    [Networked] public NetworkButtons JumpButtonsPrevious { get; set; }
//    [Networked] public NetworkObject Carrier { get; set; }
//    [Networked] public NetworkBool HasReachedFinish { get; set; }
//    [Networked] private NetworkBool IsGrounded { get; set; }

//    [Header("Movement Settings")]
//    public float moveSpeed = 6f;
//    public float jumpForce = 12f;

//    private CinemachineCamera cam;

//    private const string GroundTag = "Ground";
//    private const string PlayerName = "Player";

//    private void Awake()
//    {
//        Instance = this;
//    }

//    public override void Spawned()
//    {
//        _networkRb = GetComponent<NetworkRigidbody2D>();
//        rb = _networkRb.Rigidbody;

//        if (HasInputAuthority)
//        {
//            cam = FindObjectOfType<CinemachineCamera>();
//            if (cam != null)
//                cam.Follow = transform;
//        }

//        HasReachedFinish = false;
//    }

//    public override void FixedUpdateNetwork()
//    {
//        if (_networkRb == null || rb == null)
//            return;

//        if (!Object.HasInputAuthority && !Object.HasStateAuthority)
//            return;

//        // STOP PLAYER AFTER GAME END OR PAUSE
//        if (UIManager.Instance != null && UIManager.Instance.IsGameStopped())
//        {
//            rb.linearVelocity = Vector2.zero;
//            return;
//        }

//        if (GetInput(out NetworkInputData data))
//        {
//            Vector2 velocity = new Vector2(
//                data.horizontalMovement * moveSpeed,
//                rb.linearVelocity.y
//            );

//            rb.linearVelocity = velocity;

//            var jumpPressed = data.jumpButton.GetPressed(JumpButtonsPrevious);
//            JumpButtonsPrevious = data.jumpButton;

//            if (jumpPressed.IsSet(Buttons.Jump) &&
//                Mathf.Abs(rb.linearVelocity.y) < 0.01f &&
//                IsGrounded)
//            {
//                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
//                IsGrounded = false;

//                // TEAM JUMP RAMP (report jump once)
//                if (teamJumpRamp != null && !jumpReported)
//                {
//                    teamJumpRamp.PlayerJumped();
//                    jumpReported = true;
//                }
//            }
//        }

//        // SAFE GAME OVER CHECK
//        if (Object.HasStateAuthority &&
//            rb.position.y < -4f &&
//            UIManager.Instance != null &&
//            !UIManager.Instance.IsGameStopped())
//        {
//            UIManager.Instance.GameOver();
//        }
//    }

//    private void OnCollisionEnter2D(Collision2D other)
//    {
//        if (!Object.HasStateAuthority)
//            return;

//        if (UIManager.Instance != null && UIManager.Instance.IsGameStopped())
//            return;

//        if (other.gameObject.CompareTag(GroundTag) ||
//            other.gameObject.name.Contains(PlayerName))
//        {
//            IsGrounded = true;

//            // TEAM JUMP RAMP
//            jumpReported = false;
//        }

//        // TEAM JUMP RAMP
//        if (other.gameObject.TryGetComponent(out TeamJumpRamp r))
//        {
//            teamJumpRamp = r;
//        }

//        if (other.gameObject.name.Contains("Spike") ||
//            other.gameObject.name.Contains("Pendulum"))
//        {
//            UIManager.Instance.GameOver();
//        }

//        if (other.gameObject.name.Contains("Coin"))
//        {
//            UIManager.Instance.CollectCoin();
//            Destroy(other.gameObject);
//        }

//        if (other.gameObject.name.Contains("Diamond"))
//        {
//            UIManager.Instance.CollectDiamond();
//            Destroy(other.gameObject);
//        }
//    }

//    private void OnCollisionStay2D(Collision2D collision)
//    {
//        if (!Object.HasStateAuthority)
//            return;

//        if (UIManager.Instance != null && UIManager.Instance.IsGameStopped())
//            return;

//        if (collision.gameObject.CompareTag(GroundTag) ||
//            collision.gameObject.name.Contains(PlayerName))
//        {
//            IsGrounded = true;

//            // TEAM JUMP RAMP
//            jumpReported = false;
//        }

//        if (collision.gameObject.CompareTag("Finish"))
//        {
//            HasReachedFinish = true;
//            UIManager.Instance.CheckAllPlayersFinished();
//        }
//    }

//    private void OnCollisionExit2D(Collision2D other)
//    {
//        if (!Object.HasStateAuthority)
//            return;

//        if (UIManager.Instance != null && UIManager.Instance.IsGameStopped())
//            return;

//        if (other.gameObject.CompareTag(GroundTag) ||
//            other.gameObject.name.Contains(PlayerName))
//        {
//            IsGrounded = false;
//        }

//        // TEAM JUMP RAMP
//        if (other.gameObject.TryGetComponent(out TeamJumpRamp r))
//        {
//            teamJumpRamp = null;
//        }

//        if (other.gameObject.CompareTag("Finish"))
//        {
//            HasReachedFinish = false;
//        }
//    }
//}


//using UnityEngine;

//float coyoteTime = 0.15f;

//float coyoteCounter;

//void Update()

//{

//    if (isGrounded)

//        coyoteCounter = coyoteTime;

//    else

//        coyoteCounter -= Time.deltaTime;

//    if (Input.GetKeyDown(KeyCode.Space) && coyoteCounter > 0)

//    {

//        Jump();

//        coyoteCounter = 0;

//    }

//}


//using Fusion;
//using Fusion.Addons.Physics;
//using Unity.Cinemachine;
//using UnityEngine;

//public class Player : NetworkBehaviour
//{
//    public static Player Instance;

//    [HideInInspector] public NetworkRigidbody2D _networkRb;
//    private Rigidbody2D rb;

//    // TEAM JUMP RAMP
//    private TeamJumpRamp teamJumpRamp;
//    private bool jumpReported;

//    [Networked] public NetworkButtons JumpButtonsPrevious { get; set; }
//    [Networked] public NetworkObject Carrier { get; set; }
//    [Networked] public NetworkBool HasReachedFinish { get; set; }
//    [Networked] private NetworkBool IsGrounded { get; set; }

//    float coyoteTime = 0.1f;
//    float coyoteCounter;

//    [Header("Movement Settings")]
//    public float moveSpeed = 6f;
//    public float jumpForce = 12f;

//    private CinemachineCamera cam;

//    private const string GroundTag = "Ground";
//    private const string PlayerName = "Player";

//    private void Awake()
//    {
//        Instance = this;
//    }

//    public override void Spawned()
//    {
//        base.Spawned();

//        _networkRb = GetComponent<NetworkRigidbody2D>();
//        rb = _networkRb.Rigidbody;

//        if (HasInputAuthority)
//        {
//            Debug.Log($"Player spawned with input authority - setting up camera");
//            cam = FindObjectOfType<CinemachineCamera>();
//            if (cam != null)
//            {
//                cam.Follow = transform;
//                Debug.Log("Camera follow set to player");
//            }
//            else
//            {
//                Debug.LogWarning("CinemachineCamera not found in scene");
//            }
//        }

//        HasReachedFinish = false;
//        Debug.Log($"Player spawned successfully: InputAuth={HasInputAuthority}, StateAuth={HasStateAuthority}, PlayerId={Object.InputAuthority.PlayerId}");
//    }

//    public override void FixedUpdateNetwork()
//    {
//        if (_networkRb == null || rb == null)
//        {
//            Debug.LogWarning("NetworkRigidbody2D or Rigidbody2D is null");
//            return;
//        }

//        if (!Object.HasInputAuthority && !Object.HasStateAuthority)
//            return;

//        // STOP PLAYER AFTER GAME END OR PAUSE
//        if (UIManager.Instance != null && UIManager.Instance.Object != null &&
//            UIManager.Instance.Object.IsValid && UIManager.Instance.IsGameStopped())
//        {
//            rb.linearVelocity = Vector2.zero;
//            return;
//        }

//        if (IsGrounded)
//        {
//            coyoteCounter = coyoteTime;
//        }
//        else
//        {
//            coyoteCounter -= Runner.DeltaTime;
//        }

//        if (GetInput(out NetworkInputData data))
//        {
//            Vector2 velocity = new Vector2(
//                data.horizontalMovement * moveSpeed,
//                rb.linearVelocity.y
//            );

//            rb.linearVelocity = velocity;

//            var jumpPressed = data.jumpButton.GetPressed(JumpButtonsPrevious);
//            JumpButtonsPrevious = data.jumpButton;

//            //if (jumpPressed.IsSet(Buttons.Jump) &&
//            //    Mathf.Abs(rb.linearVelocity.y) < 0.01f &&
//            //    IsGrounded)
//            if (jumpPressed.IsSet(Buttons.Jump) &&
//                coyoteCounter > 0)
//            {
//                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
//                IsGrounded = false;
//                coyoteCounter = 0f;

//                // TEAM JUMP RAMP (report jump once)
//                if (teamJumpRamp != null && !jumpReported)
//                {
//                    teamJumpRamp.PlayerJumped();
//                    jumpReported = true;
//                }
//            }
//        }

//        // SAFE GAME OVER CHECK
//        if (Object.HasStateAuthority &&
//            rb.position.y < -4f &&
//            UIManager.Instance != null &&
//            UIManager.Instance.Object != null &&
//            UIManager.Instance.Object.IsValid &&
//            !UIManager.Instance.IsGameStopped())
//        {
//            UIManager.Instance.GameOver();
//        }
//    }

//    private void OnCollisionEnter2D(Collision2D other)
//    {
//        // Add null check for Object
//        if (Object == null || !Object.IsValid || !Object.HasStateAuthority)
//            return;

//        if (UIManager.Instance != null && UIManager.Instance.Object != null &&
//            UIManager.Instance.Object.IsValid && UIManager.Instance.IsGameStopped())
//            return;

//        if (other.gameObject.CompareTag(GroundTag) ||
//            other.gameObject.name.Contains(PlayerName))
//        {
//            IsGrounded = true;
//            Debug.Log("Grounded : "+IsGrounded);

//            // TEAM JUMP RAMP
//            jumpReported = false;
//        }

//        // TEAM JUMP RAMP
//        if (other.gameObject.TryGetComponent(out TeamJumpRamp r))
//        {
//            teamJumpRamp = r;
//        }

//        if (other.gameObject.name.Contains("Spike") ||
//            other.gameObject.name.Contains("Pendulum"))
//        {
//            if (UIManager.Instance != null && UIManager.Instance.Object != null &&
//                UIManager.Instance.Object.IsValid)
//            {
//                UIManager.Instance.GameOver();
//            }
//        }

//        if (other.gameObject.name.Contains("Coin"))
//        {
//            if (UIManager.Instance != null && UIManager.Instance.Object != null &&
//                UIManager.Instance.Object.IsValid)
//            {
//                UIManager.Instance.CollectCoin();
//                Destroy(other.gameObject);
//            }
//        }

//        if (other.gameObject.name.Contains("Diamond"))
//        {
//            if (UIManager.Instance != null && UIManager.Instance.Object != null &&
//                UIManager.Instance.Object.IsValid)
//            {
//                UIManager.Instance.CollectDiamond();
//                Destroy(other.gameObject);
//            }
//        }
//    }

//    private void OnCollisionStay2D(Collision2D collision)
//    {
//        // Add null check for Object
//        if (Object == null || !Object.IsValid || !Object.HasStateAuthority)
//            return;

//        if (UIManager.Instance != null && UIManager.Instance.Object != null &&
//            UIManager.Instance.Object.IsValid && UIManager.Instance.IsGameStopped())
//            return;

//        if (collision.gameObject.CompareTag(GroundTag) ||
//            collision.gameObject.name.Contains(PlayerName))
//        {
//            IsGrounded = true;
//            Debug.Log("Grounded : " + IsGrounded);

//            // TEAM JUMP RAMP
//            jumpReported = false;
//        }

//        if (collision.gameObject.CompareTag("Finish"))
//        {
//            HasReachedFinish = true;
//            if (UIManager.Instance != null && UIManager.Instance.Object != null &&
//                UIManager.Instance.Object.IsValid)
//            {
//                UIManager.Instance.CheckAllPlayersFinished();
//            }
//        }
//    }

//    private void OnCollisionExit2D(Collision2D other)
//    {
//        // Add null check for Object - CRITICAL FIX
//        if (Object == null || !Object.IsValid || !Object.HasStateAuthority)
//            return;

//        // Add null check for UIManager
//        if (UIManager.Instance != null && UIManager.Instance.Object != null &&
//            UIManager.Instance.Object.IsValid && UIManager.Instance.IsGameStopped())
//            return;

//        if (other.gameObject.CompareTag(GroundTag) ||
//            other.gameObject.name.Contains(PlayerName))
//        {
//            IsGrounded = false;
//            Debug.Log("Grounded : " + IsGrounded);
//        }

//        // TEAM JUMP RAMP
//        if (other.gameObject.TryGetComponent(out TeamJumpRamp r))
//        {
//            teamJumpRamp = null;
//        }

//        if (other.gameObject.CompareTag("Finish"))
//        {
//            HasReachedFinish = false;
//        }
//    }
//}

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
            if (animator != null)
            {
                float mv = Mathf.Abs(data.horizontalMovement);
                if (mv > 0.1f && Mathf.Abs(prevMoveInput) <= 0.1f)
                    animator.SetTrigger("Move");
            }

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
                    animator.SetTrigger("Jump");

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
            if (mv > 0.1f && prevRemoteMove <= 0.1f)
                animator.SetTrigger("Move");
            lastRemoteMove = mv;

            float vert = rb.linearVelocity.y;
            float prevRemoteVertical = lastRemoteVertical;
            if (vert > 0.1f && prevRemoteVertical <= 0.1f)
                animator.SetTrigger("Jump");
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

        if(other.gameObject.name.Contains("Door"))
        {
            KeyManager.Instance.CheckAllKeyCollected();
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

