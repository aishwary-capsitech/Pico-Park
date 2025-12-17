using Fusion;
using Fusion.Addons.Physics;
using Unity.Cinemachine;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public static Player Instance;

    [HideInInspector] public NetworkRigidbody2D _networkRb;

    private Rigidbody2D rb;

    [Networked] public NetworkButtons JumpButtonsPrevious { get; set; }
    [Networked] public NetworkButtons LeftButtonsPrevious { get; set; }
    [Networked] public NetworkButtons RightButtonsPrevious { get; set; }
    [Networked] public NetworkObject Carrier { get; set; }
    [SerializeField] private Vector3 headOffset = new Vector3(0, 1.1f, 0);

    [Header("Movement Settings")]
    public float moveSpeed = 6f;
    public float jumpForce = 12f;
    private CinemachineCamera cam;

    [Networked]
    private NetworkBool IsGrounded { get; set; }

    [Networked]
    private Vector2 PreviousDirection { get; set; }

    private const string GroundTag = "Ground";
    private const string PlayerName = "Player";

    private void Awake()
    {
        Instance = this;
    }

    public override void Spawned()
    {
        _networkRb = GetComponent<NetworkRigidbody2D>();
        if (_networkRb == null)
        {
            Debug.LogError($"Player {Object.Id} is missing NetworkRigidbody2D!");
        }

        if(HasInputAuthority)
        {
            cam = FindObjectOfType<CinemachineCamera>();
            if (cam != null)
            {
                cam.Follow = this.transform;
            }
            else
            {
                Debug.LogError("Cinemachine Virtual Camera not found in the scene!");
            }
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (_networkRb == null) return;

        if (GetInput(out NetworkInputData data))
        {
            rb = _networkRb.Rigidbody;
            Vector2 targetVelocity = new Vector2(data.horizontalMovement * moveSpeed, rb.linearVelocity.y);
            rb.linearVelocity = targetVelocity;

            var jumpPressed = data.jumpButton.GetPressed(JumpButtonsPrevious);
            //var leftPressed = data.jumpButton.GetPressed(LeftButtonsPrevious);
            //var rightPressed = data.jumpButton.GetPressed(RightButtonsPrevious);

            JumpButtonsPrevious = data.jumpButton;
            //LeftButtonsPrevious = data.leftButtons;
            //RightButtonsPrevious = data.rightButtons;

            if(data.horizontalMovement != 0)
            {
                Debug.Log("Horizontal Movement: " + data.horizontalMovement);
            }

            if (jumpPressed.IsSet(Buttons.Jump) && Mathf.Abs(rb.linearVelocity.y) < 0.01f && IsGrounded)
            {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                IsGrounded = false;
                Debug.Log("IsGrounded : " + IsGrounded);
            }

            //if (leftPressed.IsSet(Buttons.Left))
            //{
            //    transform.position += Vector3.left * moveSpeed * Runner.DeltaTime;
            //}

            //if (rightPressed.IsSet(Buttons.Right))
            //{
            //    transform.position += Vector3.right * moveSpeed * Runner.DeltaTime;
            //}
        }

        //if(Carrier != null)
        //{
        //    transform.position = Carrier.transform.position + headOffset;

        //    if (Carrier.TryGetComponent<Rigidbody2D>(out var carrierRb))
        //    {
        //        rb.linearVelocity = carrierRb.linearVelocity;
        //    }

        //    return;
        //}
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        //if (!Object.HasStateAuthority) return;

        if (other.gameObject.name.Contains(GroundTag))
        {
            //if (IsCollisionFromBelow(other))
            //{
            IsGrounded = true;
            //}
        }
        if (other.gameObject.name.Contains(PlayerName))
        {
            Vector2 contactNormal = other.GetContact(0).normal;

            if (contactNormal.y > 0.5f)
            {
                IsGrounded = true;
                //Carrier = other.gameObject.GetComponent<NetworkObject>();
            }
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        //if (!Object.HasStateAuthority) return;

        if (other.gameObject.name.Contains(GroundTag))
        {
            IsGrounded = false;
        }

        if(other.gameObject.name.Contains(PlayerName))
        {
            IsGrounded = false;
        }

        //if (Carrier != null && other.gameObject.name == Carrier.gameObject.name)
        //{
        //    IsGrounded = false;
        //    Carrier = null;
        //}
    }

    //private bool IsCollisionFromBelow(Collision2D collision)
    //{
    //    foreach (ContactPoint2D contact in collision.contacts)
    //    {
    //        if (contact.normal.y > 0.7f)
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}
}