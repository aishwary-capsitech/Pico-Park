using Fusion;
using Fusion.Addons.Physics;
using Unity.Cinemachine;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public static Player Instance;

    [HideInInspector] public NetworkRigidbody2D _networkRb;
    private Rigidbody2D rb;

    private TeamJumpRamp teamJumpRamp;
    private bool jumpReported;

    [Networked] public NetworkButtons JumpButtonsPrevious { get; set; }
    [Networked] public NetworkObject Carrier { get; set; }
    [Networked] public NetworkBool HasReachedFinish { get; set; }
    [Networked] private NetworkBool IsGrounded { get; set; }

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
        _networkRb = GetComponent<NetworkRigidbody2D>();
        rb = _networkRb.Rigidbody;

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
        if (_networkRb == null || rb == null)
            return;

        if (!Object.HasInputAuthority && !Object.HasStateAuthority)
            return;

        if (GetInput(out NetworkInputData data))
        {
            Vector2 velocity = new Vector2(
            data.horizontalMovement * moveSpeed,
            rb.linearVelocity.y
            );

            rb.linearVelocity = velocity;

            // Jump button handling
            var jumpPressed = data.jumpButton.GetPressed(JumpButtonsPrevious);
            JumpButtonsPrevious = data.jumpButton;

            if (jumpPressed.IsSet(Buttons.Jump) &&
                Mathf.Abs(rb.linearVelocity.y) < 0.01f &&
                IsGrounded)
            {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                IsGrounded = false;

                //if (teamJumpRamp != null && !jumpReported)
                //{
                //    jumpReported = true;
                //    teamJumpRamp.PlayerJumped();
                //}
            }
        }

        // Only StateAuthority handles game rules
        if (Object.HasStateAuthority && rb.position.y < -5f)
        {
            UIManager.Instance.GameOver();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!Object.HasStateAuthority)
            return;

        if (other.gameObject.CompareTag(GroundTag) ||
            other.gameObject.name.Contains(PlayerName))
        {
            IsGrounded = true;
        }

        // Team Jump Ramp
        //TeamJumpRamp ramp = other.gameObject.GetComponent<TeamJumpRamp>();
        //if (ramp != null)
        //{
        //    teamJumpRamp = ramp;
        //    jumpReported = false;
        //}

        if (other.gameObject.TryGetComponent(out TeamJumpRamp r))
        {
            teamJumpRamp = r;
            teamJumpRamp.PlayerEntered(Object);
        }

        // Hazards
        if (other.gameObject.name.Contains("Spike") ||
            other.gameObject.name.Contains("Pendulum"))
        {
            UIManager.Instance.GameOver();
        }

        // Collectibles
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

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!Object.HasStateAuthority)
            return;

        if (collision.gameObject.CompareTag(GroundTag) ||
            collision.gameObject.name.Contains(PlayerName))
        {
            IsGrounded = true;
        }

        // Finish
        if (collision.gameObject.CompareTag("Finish"))
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

        //if (other.gameObject.GetComponent<TeamJumpRamp>() != null)
        //{
        //    teamJumpRamp = null;
        //    jumpReported = false;
        //}

        if (other.gameObject.TryGetComponent(out TeamJumpRamp r))
        {
            r.PlayerExited(Object);
            teamJumpRamp = null;
        }

        if (other.gameObject.CompareTag("Finish"))
        {
            HasReachedFinish = false;
        }
    }
}

