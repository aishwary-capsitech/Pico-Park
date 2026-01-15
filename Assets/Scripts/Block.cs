using UnityEngine;
using Fusion;
using System.Linq;

[RequireComponent(typeof(Rigidbody2D))]
public class Block : NetworkBehaviour
{
    // Networked transform data
    [Networked] private float currentPositionX { get; set; }
    [Networked] private float currentPositionY { get; set; }
    [Networked] private float currentRotationZ { get; set; }

    // Networked mass (derived from player count)
    [Networked] public float mass { get; set; }

    // Initial (scene-placed) transform
    private Vector2 initialPosition;
    private float initialRotationZ;

    private Rigidbody2D rb;

    public override void Spawned()
    {
        rb = GetComponent<Rigidbody2D>();

        // Save initial scene placement
        initialPosition = transform.position;
        initialRotationZ = transform.rotation.eulerAngles.z;

        // Initialize networked state
        currentPositionX = initialPosition.x;
        currentPositionY = initialPosition.y;
        currentRotationZ = initialRotationZ;

        Debug.Log($"Block {name} spawned at {initialPosition}");
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
            return;

        // Sync physics transform
        currentPositionX = transform.position.x;
        currentPositionY = transform.position.y;
        currentRotationZ = transform.rotation.eulerAngles.z;

        UpdateMass();
    }

    private void UpdateMass()
    {
        int playerCount = Runner.ActivePlayers.Count();
        mass = Mathf.Max(1f, playerCount); // safety
        rb.mass = mass;
    }

    public override void Render()
    {
        if (!HasStateAuthority)
        {
            transform.position = new Vector2(currentPositionX, currentPositionY);
            transform.rotation = Quaternion.Euler(0, 0, currentRotationZ);
        }
    }

    public void ResetBlock()
    {
        if (!HasStateAuthority)
            return;

        // Reset transform
        transform.position = initialPosition;
        transform.rotation = Quaternion.Euler(0, 0, initialRotationZ);

        // Reset physics
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        // Sync networked state
        currentPositionX = initialPosition.x;
        currentPositionY = initialPosition.y;
        currentRotationZ = initialRotationZ;

        Debug.Log($"Block {name} reset to initial position");
    }
}
