using UnityEngine;
using Fusion;

public class MovingRampVertical : NetworkBehaviour
{
    public float moveHeight = 3.5f;
    public float moveSpeed = 2.5f;
    public float baseOffset = 0.1f;

    [Networked] private float networkTime { get; set; }
    [Networked] private NetworkBool isInitialized { get; set; }

    private Vector3 startPos;

    public override void Spawned()
    {
        startPos = transform.position;

        if (HasStateAuthority)
        {
            networkTime = 0f;
            isInitialized = true;
            Debug.Log($"Moving Ramp spawned at: {startPos}");
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority)
        {
            // Server updates the network time
            networkTime += Runner.DeltaTime;
        }
    }

    public override void Render()
    {
        base.Render();

        if (!isInitialized) return;

        // All clients calculate position using synchronized networkTime
        float yOffset = (Mathf.Sin(networkTime * moveSpeed) + 1f) / 2f;
        float finalY = baseOffset + yOffset * moveHeight;

        transform.position = new Vector3(
            startPos.x,
            finalY,
            startPos.z
        );
    }
}