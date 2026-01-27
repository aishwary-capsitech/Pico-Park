using Fusion;
using Fusion.Addons.Physics;
using System.Collections.Generic;
using UnityEngine;
 
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
//[RequireComponent(typeof(NetworkRigidbody2D))]
public class MovingRamp : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float moveDistance = 2f;
    public float moveSpeed = 2.5f;
 
    private float playerMoveSpeed = 6f;
 
    [Networked] private float networkTime { get; set; }
    [Networked] private NetworkBool isInitialized { get; set; }
 
    private Rigidbody2D rb;
    //private NetworkRigidbody2D netRb;
 
    private Vector2 startPos;
    private float lastPlatformX;
 
    // Player Rigidbody → relative X offset
    Dictionary<Rigidbody2D, float> relativeOffsets = new();
 
    // Rigidbody → owning NetworkObject
    Dictionary<Rigidbody2D, NetworkObject> rbOwners = new();
 
    public override void Spawned()
    {
        rb = GetComponent<Rigidbody2D>();
        //netRb = GetComponent<NetworkRigidbody2D>();
 
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
 
        startPos = rb.position;
        lastPlatformX = startPos.x;
 
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
 
        float xOffset = Mathf.Sin(networkTime * moveSpeed);
        float targetX = startPos.x + xOffset * (moveDistance * 0.5f);
 
        float currentX = rb.position.x;
        float velocityX = (targetX - currentX) / Runner.DeltaTime;
 
        rb.linearVelocity = new Vector2(velocityX, 0f);
 
        UpdatePlayers();
    }
 
    public override void Render()
    {
        if (!isInitialized)
            return;
 
        if (UIManager.Instance != null &&
            UIManager.Instance.IsGameStopped())
            return;
 
        float xOffset = Mathf.Sin(networkTime * moveSpeed);
        float finalX = startPos.x + xOffset * (moveDistance * 0.5f);
 
        transform.position = new Vector3(
            finalX,
            startPos.y,
            transform.position.z
        );
    }
 
    // PLAYER HANDLING
 
    void UpdatePlayers()
    {
        foreach (var pair in relativeOffsets)
        {
            Rigidbody2D playerRb = pair.Key;
            if (!playerRb) continue;
 
            NetworkObject playerObj = rbOwners[playerRb];
            if (!playerObj) continue;
 
            // Read player input (fixes client movement)
            if (!Runner.TryGetInputForPlayer(playerObj.InputAuthority, out NetworkInputData input))
                continue;
 
            float moveStep =
                input.horizontalMovement * playerMoveSpeed * Runner.DeltaTime;
 
            relativeOffsets[playerRb] += moveStep;
 
            playerRb.position = new Vector2(
                transform.position.x + relativeOffsets[playerRb],
                playerRb.position.y
            );
        }
    }
 
    void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.collider.CompareTag("Player")) return;
 
        Rigidbody2D playerRb = col.collider.attachedRigidbody;
        if (!playerRb) return;
 
        NetworkObject netObj = playerRb.GetComponent<NetworkObject>();
        if (!netObj) return;
 
        relativeOffsets[playerRb] =
            playerRb.position.x - transform.position.x;
 
        rbOwners[playerRb] = netObj;
    }
 
    void OnCollisionExit2D(Collision2D col)
    {
        if (!col.collider.CompareTag("Player")) return;
 
        Rigidbody2D playerRb = col.collider.attachedRigidbody;
        if (!playerRb) return;
 
        relativeOffsets.Remove(playerRb);
        rbOwners.Remove(playerRb);
    }
}