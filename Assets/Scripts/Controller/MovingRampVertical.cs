//using UnityEngine;
//using Fusion;

//public class MovingRampVertical : NetworkBehaviour
//{
//    public float moveHeight = 3.5f;
//    public float moveSpeed = 2.5f;
//    public float baseOffset = 0.1f;

//    [Networked] private float networkTime { get; set; }
//    [Networked] private NetworkBool isInitialized { get; set; }

//    private Vector3 startPos;

//    public override void Spawned()
//    {
//        startPos = transform.position;

//        if (HasStateAuthority)
//        {
//            networkTime = 0f;
//            isInitialized = true;
//            Debug.Log($"Moving Ramp spawned at: {startPos}");
//        }
//    }

//    public override void FixedUpdateNetwork()
//    {
//        if (HasStateAuthority)
//        {
//            // Server updates the network time
//            networkTime += Runner.DeltaTime;
//        }
//    }

//    public override void Render()
//    {
//        base.Render();

//        if (!isInitialized) return;

//        // All clients calculate position using synchronized networkTime
//        float yOffset = (Mathf.Sin(networkTime * moveSpeed) + 1f) / 2f;
//        float finalY = baseOffset + yOffset * moveHeight;

//        transform.position = new Vector3(
//            startPos.x,
//            finalY,
//            startPos.z
//        );
//    }
//}


using UnityEngine;
using Fusion;
using Fusion.Addons.Physics;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(NetworkRigidbody2D))]
public class MovingRampVertical : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float moveHeight = 3.5f;
    public float moveSpeed = 2.5f;
    public float baseOffset = 0.1f;

    [Networked] private float networkTime { get; set; }
    [Networked] private NetworkBool isInitialized { get; set; }

    private Vector2 startPos;

    private Rigidbody2D rb;
    private NetworkRigidbody2D netRb;

    public override void Spawned()
    {
        rb = GetComponent<Rigidbody2D>();
        netRb = GetComponent<NetworkRigidbody2D>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        startPos = rb.position;

        if (HasStateAuthority)
        {
            networkTime = 0f;
            isInitialized = true;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!isInitialized)
            return;

        if (UIManager.Instance != null &&
            UIManager.Instance.IsGameStopped())
        {
            if (HasStateAuthority)
                rb.linearVelocity = Vector2.zero;

            return;
        }

        if (!HasStateAuthority)
            return;

        networkTime += Runner.DeltaTime;

        float yOffset = (Mathf.Sin(networkTime * moveSpeed) + 1f) * 0.5f;
        float targetY = baseOffset + yOffset * moveHeight;

        float currentY = rb.position.y;
        float velocityY = (targetY - currentY) / Runner.DeltaTime;

        rb.linearVelocity = new Vector2(0f, velocityY);
    }

    public override void Render()
    {
        if (!isInitialized)
            return;

        if (UIManager.Instance != null &&
            UIManager.Instance.IsGameStopped())
            return;

        float yOffset = (Mathf.Sin(networkTime * moveSpeed) + 1f) * 0.5f;
        float finalY = baseOffset + yOffset * moveHeight;

        transform.position = new Vector3(
            startPos.x,
            finalY,
            transform.position.z
        );
    }
}