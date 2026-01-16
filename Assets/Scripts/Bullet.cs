using Fusion;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [Networked] private Vector3 MoveDir { get; set; }
    [Networked] private float Speed { get; set; }

    private float screenTop, screenBottom, screenLeft, screenRight;

    // Called from HeliManager after Runner.Spawn
    public void Init(Vector3 direction, float speed)
    {
        MoveDir = direction.normalized;
        Speed = speed;
    }

    public override void Spawned()
    {
        CacheScreenBounds();
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
            return;

        // Move bullet
        transform.position += MoveDir * Speed * Runner.DeltaTime;

        // Despawn if outside screen
        Vector3 pos = transform.position;
        if (pos.y < screenBottom || pos.y > screenTop ||
            pos.x < screenLeft || pos.x > screenRight)
        {
            Runner.Despawn(Object);
        }
    }

    void CacheScreenBounds()
    {
        Camera cam = Camera.main;
        if (!cam) return;

        screenLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
        screenRight = cam.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
        screenTop = cam.ViewportToWorldPoint(new Vector3(0, 1, 0)).y;
        screenBottom = cam.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!HasStateAuthority)
            return;

        // Hit ground / level
        if (collision.CompareTag("Ground") ||
            collision.name.Contains("StartPoint"))
        {
            Runner.Despawn(Object);
            return;
        }

        // Hit player
        if (collision.CompareTag("Player"))
        {
            // TODO: Apply damage here (Networked Health)
            Runner.Despawn(Object);
        }
    }
}

