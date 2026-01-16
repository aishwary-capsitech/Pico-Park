using Fusion;
using UnityEngine;

public class BlockMoveDown : NetworkBehaviour
{
    public float moveSpeed = 4f;
    public float downDistance = 3f;

    private Vector3 startPos;
    private Vector3 downPos;

    [Networked] private NetworkBool moveDown { get; set; }

    public override void Spawned()
    {
        startPos = transform.position;
        downPos = startPos + Vector3.down * downDistance;
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
            return;

        Vector3 target = moveDown ? downPos : startPos;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            moveSpeed * Runner.DeltaTime
        );
    }

    public void StartMoveDown()
    {
        if (HasStateAuthority)
            moveDown = true;
    }

    public void StopMoveDown()
    {
        if (HasStateAuthority)
            moveDown = false;
    }
}

