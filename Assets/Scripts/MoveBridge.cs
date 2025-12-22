using UnityEngine;
using System.Collections.Generic;
using Fusion;

public class MoveBridge : NetworkBehaviour
{
    [SerializeField] private float requiredMass = 5f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Transform startPosition;
    [SerializeField] private Transform endPosition;

    [Networked] private NetworkBool isMoving { get; set; }
    [Networked] private NetworkBool hasReachedTarget { get; set; }
    [Networked] private float currentPositionX { get; set; }
    [Networked] private float currentPositionY { get; set; }

    private Vector2 initialPosition;
    private Vector2 targetPosition;
    private HashSet<Rigidbody2D> players = new HashSet<Rigidbody2D>();

    public override void Spawned()
    {
        if (startPosition != null)
        {
            initialPosition = startPosition.position;
            transform.position = initialPosition;
        }
        else
        {
            initialPosition = transform.position;
        }

        if (endPosition != null)
        {
            targetPosition = endPosition.position;
        }
        else
        {
            Debug.LogError("End Position is not assigned!");
            targetPosition = initialPosition;
        }

        // Initialize networked position
        currentPositionX = transform.position.x;
        currentPositionY = transform.position.y;

        Debug.Log($"Bridge spawned - Initial: {initialPosition}, Target: {targetPosition}");
    }

    public override void FixedUpdateNetwork()
    {
        // Only server calculates movement
        if (HasStateAuthority)
        {
            if (isMoving && !hasReachedTarget)
            {
                Vector2 newPosition = Vector2.MoveTowards(
                    transform.position,
                    targetPosition,
                    moveSpeed * Runner.DeltaTime
                );

                transform.position = newPosition;
                currentPositionX = newPosition.x;
                currentPositionY = newPosition.y;

                if (Vector2.Distance(transform.position, targetPosition) < 0.01f)
                {
                    hasReachedTarget = true;
                    isMoving = false;
                    transform.position = targetPosition;
                    Debug.Log("Bridge movement complete!");
                }
            }
        }
    }

    public override void Render()
    {
        base.Render();

        // All clients update their visual position from networked value
        if (!HasStateAuthority)
        {
            transform.position = new Vector2(currentPositionX, currentPositionY);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!HasStateAuthority) return;

        Rigidbody2D rb = col.rigidbody;
        if (rb != null)
        {
            players.Add(rb);
            Debug.Log($"Player entered bridge. Total players: {players.Count}");
            CheckMass();
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (!HasStateAuthority) return;

        Rigidbody2D rb = col.rigidbody;
        if (rb != null)
        {
            players.Remove(rb);
            Debug.Log($"Player exited bridge. Total players: {players.Count}");
        }
    }

    void CheckMass()
    {
        if (hasReachedTarget) return;

        float totalMass = 0f;
        players.RemoveWhere(rb => rb == null);

        foreach (var rb in players)
            totalMass += rb.mass;

        Debug.Log($"Total mass on bridge: {totalMass} / {requiredMass}");

        if (totalMass >= requiredMass && !isMoving)
        {
            isMoving = true;
            Debug.Log("Bridge movement started!");
        }
    }
}