using Fusion;
using UnityEngine;

public class MovingSpike : NetworkBehaviour
{
    public float moveHeight = 0.35f;
    public float moveSpeed = 0.4f;

    [Networked] private bool CanMove { get; set; }

    private Vector3 startPos;
    private float startTime;

    public override void Spawned()
    {
        startPos = transform.position;

        // Only host initializes
        if (Object.HasStateAuthority)
        {
            CanMove = true;
            startTime = (float)Runner.SimulationTime;
        }
    }

    public override void FixedUpdateNetwork()
    {
        // Only host moves spike
        if (!Object.HasStateAuthority || !CanMove)
            return;

        float t = ((float)Runner.SimulationTime - startTime) * moveSpeed;
        float yOffset = Mathf.PingPong(t, moveHeight);

        transform.position = new Vector3(
            startPos.x,
            startPos.y + yOffset,
            startPos.z
        );
    }

    // CALL THIS FROM HOST ONLY
    public void StopSpike()
    {
        if (!Object.HasStateAuthority) return;

        CanMove = false;
        transform.position = startPos;
    }

    // CALL THIS FROM HOST ONLY
    public void ResumeSpike()
    {
        if (!Object.HasStateAuthority) return;

        startTime = (float)Runner.SimulationTime;
        CanMove = true;
    }
}
