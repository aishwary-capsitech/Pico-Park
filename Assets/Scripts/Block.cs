using UnityEngine;
using Fusion;
using System.Linq;

public class Block : NetworkBehaviour
{
    [Networked] private float currentPositionX { get; set; }
    [Networked] private float currentPositionY { get; set; }
    [Networked] private float currentRotationZ { get; set; }
    [Networked] private float mass { get; set; }

    public override void Spawned()
    {
        // Initialize networked values
        currentPositionX = transform.position.x;
        currentPositionY = transform.position.y;
        currentRotationZ = transform.rotation.eulerAngles.z;

        Debug.Log($"Block {gameObject.name} spawned at position {transform.position}");
    }

    public override void FixedUpdateNetwork()
    {
        // Only server reads the physics position and syncs it
        if (HasStateAuthority)
        {
            currentPositionX = transform.position.x;
            currentPositionY = transform.position.y;
            currentRotationZ = transform.rotation.eulerAngles.z;
            UpdateRequiredMass();
        }
    }

    private void UpdateRequiredMass()
    {
        int playerCount = Runner.ActivePlayers.Count();
        mass = playerCount * 3f;
        gameObject.GetComponent<Rigidbody2D>().mass = mass;
    }

    public override void Render()
    {
        base.Render();

        // All clients update their visual transform from networked values
        if (!HasStateAuthority)
        {
            transform.position = new Vector2(currentPositionX, currentPositionY);
            transform.rotation = Quaternion.Euler(0, 0, currentRotationZ);
        }
    }
}
