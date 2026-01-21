using Fusion;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class MovingRamp : NetworkBehaviour
{
    public float moveDistance = 2f;
    public float moveSpeed = 2.5f;

    private Vector3 startPos;
    private float lastPlatformX;

    private Collider2D rampCollider;

    // Store relative X position for players
    private Dictionary<Rigidbody2D, float> relativeOffsets = new();

    public override void Spawned()
    {
        startPos = transform.position;
        lastPlatformX = startPos.x;
        rampCollider = GetComponent<Collider2D>();
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;

        // Move platform
        float offset = Mathf.Sin(Runner.SimulationTime * moveSpeed);
        Vector3 newPos = startPos + Vector3.right * offset * (moveDistance / 2f);

        float platformDeltaX = newPos.x - lastPlatformX;
        lastPlatformX = newPos.x;

        transform.position = newPos;

        UpdatePlayers(platformDeltaX);
    }

    private void UpdatePlayers(float platformDeltaX)
    {
        foreach (var pair in relativeOffsets)
        {
            Rigidbody2D rb = pair.Key;
            if (rb == null) continue;

            float relativeX = pair.Value;

            // Player input (ONLY when player moves)
            float inputX = Input.GetAxisRaw("Horizontal");
            float inputMove = inputX * 6f * Runner.DeltaTime;

            // Update relative position ONLY by input
            relativeOffsets[rb] += inputMove;

            // Final position
            rb.position = new Vector2(
                transform.position.x + relativeOffsets[rb],
                rb.position.y
            );
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player")) return;

        Rigidbody2D rb = collision.collider.attachedRigidbody;
        if (rb == null) return;

        // Save relative X on landing
        float relativeX = rb.position.x - transform.position.x;
        relativeOffsets[rb] = relativeX;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player")) return;

        Rigidbody2D rb = collision.collider.attachedRigidbody;
        if (rb == null) return;

        // Stop sticking
        relativeOffsets.Remove(rb);
    }
}
