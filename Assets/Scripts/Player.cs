// using Fusion;
// using Fusion.Addons.Physics;
// using Unity.Cinemachine;
// using UnityEngine;

// public class Player : NetworkBehaviour
// {
//     public static Player Instance;

//     [HideInInspector] public NetworkRigidbody2D networkRb;
//     private Rigidbody2D rb;

//     // TEAM JUMP RAMP
//     private TeamJumpRamp teamJumpRamp;
//     private bool jumpReported;

//     [Networked] public NetworkButtons JumpButtonsPrevious { get; set; }
//     [Networked] public NetworkObject Carrier { get; set; }
//     [Networked] public NetworkBool HasReachedFinish { get; set; }

//     // NETWORKED VISUAL STATE
//     [Networked] private NetworkBool NetIsMoving { get; set; }
//     [Networked] private NetworkBool NetIsJumping { get; set; }
//     [Networked] private NetworkBool NetIsGrounded { get; set; }
//     [Networked] private NetworkBool NetFacingRight { get; set; }

//     [Header("Coyote Time")]
//     public float coyoteTime = 0.05f;
//     private float coyoteCounter;

//     [Header("Movement Settings")]
//     public float moveSpeed = 6f;
//     public float jumpForce = 12f;

//     private CinemachineCamera cam;

//     // VISUALS
//     private Animator animator;
//     private Transform capsuleTransform;

//     private const string GroundTag = "Ground";
//     private const string PlayerName = "Player";

//     private void Awake()
//     {
//         Instance = this;
//     }

//     public override void Spawned()
//     {
//         networkRb = GetComponent<NetworkRigidbody2D>();
//         rb = networkRb.Rigidbody;

//         animator = GetComponent<Animator>();

//         var t = transform.Find("Capsule");
//         if (t != null)
//             capsuleTransform = t;

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
//         if (rb == null)
//             return;

//         // ONLY STATE AUTHORITY MOVES PLAYER
//         if (!Object.HasStateAuthority)
//             return;

//         if (UIManager.Instance != null &&
//             UIManager.Instance.Object != null &&
//             UIManager.Instance.Object.IsValid &&
//             UIManager.Instance.IsGameStopped())
//         {
//             rb.linearVelocity = Vector2.zero;
//             return;
//         }

//         // COYOTE TIME
//         if (NetIsGrounded)
//             coyoteCounter = coyoteTime;
//         else
//             coyoteCounter -= Runner.DeltaTime;

//         if (GetInput(out NetworkInputData data))
//         {
//             // MOVE
//             rb.linearVelocity = new Vector2(
//                 data.horizontalMovement * moveSpeed,
//                 rb.linearVelocity.y
//             );

//             // JUMP
//             var jumpPressed = data.jumpButton.GetPressed(JumpButtonsPrevious);
//             JumpButtonsPrevious = data.jumpButton;

//             if (jumpPressed.IsSet(Buttons.Jump) && coyoteCounter > 0)
//             {
//                 rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
//                 NetIsGrounded = false;
//                 coyoteCounter = 0f;

//                 if (teamJumpRamp != null && !jumpReported)
//                 {
//                     teamJumpRamp.PlayerJumped();
//                     jumpReported = true;
//                 }
//             }
//         }

//         // AUTHORITATIVE VISUAL STATE
//         NetIsMoving = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
//         NetIsJumping = rb.linearVelocity.y > 0.1f;

//         // AUTHORITATIVE FACE DIRECTION (FIX)
//         if (rb.linearVelocity.x > 0.05f)
//             NetFacingRight = true;
//         else if (rb.linearVelocity.x < -0.05f)
//             NetFacingRight = false;

//         // FALL DEATH
//         if (rb.position.y < -4f &&
//             UIManager.Instance != null &&
//             !UIManager.Instance.IsGameStopped())
//         {
//             UIManager.Instance.GameOver();
//         }
//     }

//     // VISUALS (RUNS ON ALL PEERS)
//     public override void Render()
//     {
//         if (animator == null)
//             return;

//         // ANIMATION (PING SAFE)
//         if (NetIsJumping)
//             animator.Play("Jump");
//         else if (NetIsMoving)
//             animator.Play("Move");
//         else
//             animator.Play("Idle");

//         // FACE DIRECTION (FIXED � NO VELOCITY DEPENDENCE)
//         if (capsuleTransform != null)
//         {
//             capsuleTransform.localEulerAngles =
//                 new Vector3(
//                     capsuleTransform.localEulerAngles.x,
//                     NetFacingRight ? 180f : 0f,
//                     capsuleTransform.localEulerAngles.z
//                 );
//         }
//     }

//     private void OnCollisionEnter2D(Collision2D other)
//     {
//         if (!Object.HasStateAuthority)
//             return;

//         if (UIManager.Instance != null && UIManager.Instance.IsGameStopped())
//             return;

//         if (other.gameObject.CompareTag(GroundTag) ||
//             other.gameObject.name.Contains(PlayerName))
//         {
//             NetIsGrounded = true;
//             jumpReported = false;
//         }

//         if (other.gameObject.TryGetComponent(out TeamJumpRamp ramp))
//             teamJumpRamp = ramp;

//         if (other.gameObject.name.Contains("Spike") ||
//             other.gameObject.name.Contains("Pendulum"))
//             UIManager.Instance.GameOver();

//         if (other.gameObject.name.Contains("Coin"))
//         {
//             UIManager.Instance.CollectCoin();
//             Destroy(other.gameObject);
//         }

//         if (other.gameObject.name.Contains("Diamond"))
//         {
//             UIManager.Instance.CollectDiamond();
//             Destroy(other.gameObject);
//         }
//     }

//     private void OnCollisionStay2D(Collision2D other)
//     {
//         if (!Object.HasStateAuthority)
//             return;

//         if (other.gameObject.CompareTag(GroundTag) ||
//             other.gameObject.name.Contains(PlayerName))
//         {
//             NetIsGrounded = true;
//             jumpReported = false;
//         }

//         if (other.gameObject.CompareTag("Finish"))
//         {
//             HasReachedFinish = true;
//             UIManager.Instance.CheckAllPlayersFinished();
//         }
//     }

//     private void OnCollisionExit2D(Collision2D other)
//     {
//         if (!Object.HasStateAuthority)
//             return;

//         if (other.gameObject.CompareTag(GroundTag) ||
//             other.gameObject.name.Contains(PlayerName))
//         {
//             NetIsGrounded = false;
//         }

//         if (other.gameObject.TryGetComponent(out TeamJumpRamp ramp))
//             teamJumpRamp = null;

//         if (other.gameObject.CompareTag("Finish"))
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
 
    [HideInInspector] public NetworkRigidbody2D networkRb;
    private Rigidbody2D rb;
 
    // TEAM JUMP RAMP
    private TeamJumpRamp teamJumpRamp;
    private bool jumpReported;
 
    [Networked] public NetworkButtons JumpButtonsPrevious { get; set; }
    [Networked] public NetworkObject Carrier { get; set; }
    [Networked] public NetworkBool HasReachedFinish { get; set; }
 
    // NETWORKED VISUAL STATE
    [Networked] private NetworkBool NetIsMoving { get; set; }
    [Networked] private NetworkBool NetIsJumping { get; set; }
    [Networked] private NetworkBool NetIsGrounded { get; set; }
    [Networked] private NetworkBool NetFacingRight { get; set; }
 
    [Header("Coyote Time")]
    public float coyoteTime = 0.05f;
    private float coyoteCounter;
 
    [Header("Movement Settings")]
    public float moveSpeed = 6f;
    public float jumpForce = 12f;
    public float maxY = 18f;
 
    private CinemachineCamera cam;
 
    // VISUALS
    private Animator animator;
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
 
        animator = GetComponent<Animator>();
 
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
 
        // ONLY STATE AUTHORITY MOVES PLAYER
        if (!Object.HasStateAuthority)
            return;
 
        if (UIManager.Instance != null &&
            UIManager.Instance.Object != null &&
            UIManager.Instance.Object.IsValid &&
            UIManager.Instance.IsGameStopped())
        {
            rb.linearVelocity = Vector2.zero;
            NetIsJumping = false;
            return;
        }
 
        // COYOTE TIME
        if (NetIsGrounded)
            coyoteCounter = coyoteTime;
        else
            coyoteCounter -= Runner.DeltaTime;
 
        if (GetInput(out NetworkInputData data))
        {
            // MOVE
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
                NetIsGrounded = false;
                coyoteCounter = 0f;
 
                NetIsJumping = true;
 
                if (teamJumpRamp != null && !jumpReported)
                {
                    teamJumpRamp.PlayerJumped();
                    jumpReported = true;
                }
            }
        }
 
        // AUTHORITATIVE VISUAL STATE
        NetIsMoving = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
 
        if(NetIsJumping && rb.linearVelocity.y <= 0f)
        {
            NetIsJumping = false;
        }
 
        // AUTHORITATIVE FACE DIRECTION (FIX)
        if (rb.linearVelocity.x > 0.05f)
            NetFacingRight = true;
        else if (rb.linearVelocity.x < -0.05f)
            NetFacingRight = false;
 
        if (rb.position.y > maxY && rb.position.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        }
 
        // FALL DEATH
        if (rb.position.y < -4f &&
            UIManager.Instance != null &&
            !UIManager.Instance.IsGameStopped())
        {
            UIManager.Instance.GameOver();
        }
    }
 
    // VISUALS (RUNS ON ALL PEERS)
    public override void Render()
    {
        if (animator == null)
            return;
 
        // ANIMATION (PING SAFE)
        if (NetIsJumping)
            animator.Play("Jump");
        else if (NetIsMoving)
            animator.Play("Move");
        else
            animator.Play("Idle");
 
        // FACE DIRECTION (FIXED – NO VELOCITY DEPENDENCE)
        if (capsuleTransform != null)
        {
            capsuleTransform.localEulerAngles =
                new Vector3(
                    capsuleTransform.localEulerAngles.x,
                    NetFacingRight ? 180f : 0f,
                    capsuleTransform.localEulerAngles.z
                );
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
            NetIsGrounded = true;
            NetIsJumping = false;
            jumpReported = false;
        }
 
        if (other.gameObject.TryGetComponent(out TeamJumpRamp ramp))
            teamJumpRamp = ramp;
 
        if (other.gameObject.name.Contains("Spike") ||
            other.gameObject.name.Contains("Pendulum"))
            UIManager.Instance.GameOver();
 
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
            NetIsGrounded = true;
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
            NetIsGrounded = false;
        }
 
        if (other.gameObject.TryGetComponent(out TeamJumpRamp ramp))
            teamJumpRamp = null;
 
        if (other.gameObject.CompareTag("Finish"))
            HasReachedFinish = false;
    }
}