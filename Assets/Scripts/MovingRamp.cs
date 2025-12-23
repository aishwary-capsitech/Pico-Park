using Fusion;
using UnityEngine;

public class MovingRamp : NetworkBehaviour
{
    public float moveDistance = 1.5f;
    public float moveSpeed = 3.0f;

    private Vector3 startPos;

    public override void Spawned()
    {
        startPos = transform.position;
    }

    public override void FixedUpdateNetwork()
    {
        // ONLY host moves the ramp
        if (!Object.HasStateAuthority) return;

        float offset = Mathf.Sin(Runner.SimulationTime * moveSpeed);

        Vector3 newPos = startPos
            + Vector3.right * offset * (moveDistance / 2f);

        transform.position = newPos;
    }
}
