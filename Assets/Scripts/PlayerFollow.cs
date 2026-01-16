using Fusion;
using UnityEngine;

public class PlayerFollow : NetworkBehaviour
{
    private Transform target;

    private Vector3 currentPos;
    private Vector3 newPos;

    [SerializeField] private float followSpeed = 1f;
    private float screenTop;

    public override void Spawned()
    {
        if (!HasStateAuthority)
            return;

        FindTargetPlayer();
        CacheScreenTop();
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority || target == null)
            return;

        CacheScreenTop();
        FollowPlayer();
    }

    void FindTargetPlayer()
    {
        foreach (var player in FindObjectsOfType<Player>())
        {
            if (player.Object.HasStateAuthority)
            {
                target = player.transform;
                break;
            }
        }
    }

    void CacheScreenTop()
    {
        Camera cam = Camera.main;
        if (!cam) return;

        screenTop = cam.ViewportToWorldPoint(new Vector3(0, 1, 0)).y;
    }

    void FollowPlayer()
    {
        currentPos = transform.position;
        newPos = currentPos;

        // Smooth follow on X axis
        newPos.x = Mathf.Lerp(
            currentPos.x,
            target.position.x,
            followSpeed * Runner.DeltaTime
        );

        // Vertical clamp logic (same as your script)
        if (newPos.y > screenTop - 0.5f)
        {
            newPos.y = Mathf.Lerp(
                currentPos.y,
                screenTop - 0.5f,
                followSpeed * 0.5f * Runner.DeltaTime
            );
        }
        else if (newPos.y < screenTop - 2f)
        {
            newPos.y = Mathf.Lerp(
                currentPos.y,
                screenTop - 1.5f,
                followSpeed * 0.5f * Runner.DeltaTime
            );
        }

        transform.position = newPos;
    }
}
