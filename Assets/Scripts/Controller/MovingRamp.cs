//using Fusion;
//using UnityEngine;

//public class MovingRamp : NetworkBehaviour
//{
//    public float moveDistance = 1.5f;
//    public float moveSpeed = 3.0f;

//    private Vector3 startPos;

//    public override void Spawned()
//    {
//        startPos = transform.position;
//    }

//    public override void FixedUpdateNetwork()
//    {
//        // ONLY host moves the ramp
//        if (!Object.HasStateAuthority) return;

//        float offset = Mathf.Sin(Runner.SimulationTime * moveSpeed);

//        Vector3 newPos = startPos
//            + Vector3.right * offset * (moveDistance / 2f);

//        transform.position = newPos;
//    }
//}


using Fusion;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class MovingRamp : NetworkBehaviour
{
    public float moveDistance = 2f;
    public float moveSpeed = 2.5f;
    private float playerMoveSpeed = 6f;

    Vector3 startPos;
    float lastPlatformX;

    // Player Rigidbody → relative X offset
    Dictionary<Rigidbody2D, float> relativeOffsets = new();

    // Rigidbody → owning NetworkObject
    Dictionary<Rigidbody2D, NetworkObject> rbOwners = new();

    public override void Spawned()
    {
        startPos = transform.position;
        lastPlatformX = startPos.x;
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;

        // Move ramp
        float offset = Mathf.Sin(Runner.SimulationTime * moveSpeed);
        Vector3 newPos = startPos + Vector3.right * offset * (moveDistance * 0.5f);

        float deltaX = newPos.x - lastPlatformX;
        lastPlatformX = newPos.x;

        transform.position = newPos;

        UpdatePlayers();
    }

    void UpdatePlayers()
    {
        foreach (var pair in relativeOffsets)
        {
            Rigidbody2D rb = pair.Key;
            if (!rb) continue;

            NetworkObject playerObj = rbOwners[rb];
            if (!playerObj) continue;

            // READ NETWORK INPUT (this fixes client movement)
            if (!Runner.TryGetInputForPlayer(playerObj.InputAuthority, out NetworkInputData input))
                continue;

            float moveStep =
                input.horizontalMovement * playerMoveSpeed * Runner.DeltaTime;

            relativeOffsets[rb] += moveStep;

            rb.position = new Vector2(
                transform.position.x + relativeOffsets[rb],
                rb.position.y
            );
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.collider.CompareTag("Player")) return;

        Rigidbody2D rb = col.collider.attachedRigidbody;
        if (!rb) return;

        NetworkObject netObj = rb.GetComponent<NetworkObject>();
        if (!netObj) return;

        relativeOffsets[rb] = rb.position.x - transform.position.x;
        rbOwners[rb] = netObj;
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (!col.collider.CompareTag("Player")) return;

        Rigidbody2D rb = col.collider.attachedRigidbody;
        if (!rb) return;

        relativeOffsets.Remove(rb);
        rbOwners.Remove(rb);
    }
}