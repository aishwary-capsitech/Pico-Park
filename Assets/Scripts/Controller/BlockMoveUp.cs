using Fusion;
using UnityEngine;

public class BlockMoveUp : NetworkBehaviour
{
    public float moveSpeed = 4f;
    public float upDistance = 4f;

    private Vector3 startPos;
    private Vector3 upPos;

    [Networked] private NetworkBool moveUp { get; set; }

    public override void Spawned()
    {
        startPos = transform.position;
        upPos = startPos + Vector3.up * upDistance;
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
            return;

        Vector3 target = moveUp ? upPos : startPos;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            moveSpeed * Runner.DeltaTime
        );
    }

    public void StartMoveUp()
    {
        if (HasStateAuthority)
            moveUp = true;
    }

    public void StopMoveUp()
    {
        if (HasStateAuthority)
            moveUp = false;
    }
}
