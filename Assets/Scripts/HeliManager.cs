using Fusion;
using UnityEngine;

public class HeliManager : NetworkBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private NetworkPrefabRef heliPrefab;
    [SerializeField] private NetworkPrefabRef bulletPrefab;

    [Header("Spawn Timings")]
    [SerializeField] private float heliSpawnInterval = 5f;
    [SerializeField] private float heliLifeTime = 5f;
    [SerializeField] private float fireRate = 0.6f;

    [Header("Bullet")]
    [SerializeField] private float bulletSpeed = 6f;

    [Networked] private TickTimer heliSpawnTimer { get; set; }
    [Networked] private TickTimer heliLifeTimer { get; set; }
    [Networked] private TickTimer fireTimer { get; set; }

    private NetworkObject heli;
    //private Transform targetPlayer;
    [Networked] private NetworkObject targetPlayer { get; set; }

    public override void Spawned()
    {
        if (!HasStateAuthority)
            return;

        heliSpawnTimer = TickTimer.CreateFromSeconds(Runner, heliSpawnInterval);
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
            return;

        FindTargetPlayer();

        // Spawn helicopter
        if (heli == null && heliSpawnTimer.Expired(Runner) && UIManager.Instance != null && !UIManager.Instance.IsGameStopped())
        {
            SpawnHelicopter();
            return;
        }

        if (heli == null || targetPlayer == null)
            return;

        // Fire bullets
        if (fireTimer.Expired(Runner))
        {
            FireBullet();
            fireTimer = TickTimer.CreateFromSeconds(Runner, fireRate);
        }

        // Despawn helicopter
        if (heliLifeTimer.Expired(Runner))
        {
            Runner.Despawn(heli);
            heli = null;
            heliSpawnTimer = TickTimer.CreateFromSeconds(Runner, heliSpawnInterval);
        }
    }

    void SpawnHelicopter()
    {
        var spawnPos = GetSpawnData();

        heli = Runner.Spawn(
            heliPrefab,
            spawnPos.pos,
            spawnPos.rot,
            Object.InputAuthority
        );

        heliLifeTimer = TickTimer.CreateFromSeconds(Runner, heliLifeTime);
        fireTimer = TickTimer.CreateFromSeconds(Runner, fireRate);
    }

    void FireBullet()
    {
        if (heli == null || targetPlayer == null)
            return;

        Vector3 dir = (targetPlayer.transform.position - heli.transform.position);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle - 90f);

        Runner.Spawn(
            bulletPrefab,
            heli.transform.position,
            rotation,
            Object.InputAuthority,
            (runner, obj) =>
            {
                obj.GetComponent<Bullet>().Init(dir, bulletSpeed);
            }
        );
    }

    void FindTargetPlayer()
    {
        if (targetPlayer != null)
            return;

        foreach (var p in FindObjectsOfType<Player>())
        {
            if (p.Object.InputAuthority != PlayerRef.None && p.Object.HasStateAuthority)
            {
                targetPlayer = p.Object;
                break;
            }
        }
    }

    (Vector3 pos, Quaternion rot) GetSpawnData()
    {
        Camera cam = Camera.main;
        if (!cam) return (Vector3.zero, Quaternion.identity);

        float top = cam.ViewportToWorldPoint(new Vector3(0, 1, 0)).y;
        float left = cam.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
        float right = cam.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;

        bool spawnFromRight = Random.value > 0.5f;

        float x = spawnFromRight ? right : left;
        float y = Random.Range(top - 1.5f, top - 0.5f);

        Quaternion rot = spawnFromRight
            ? Quaternion.identity
            : Quaternion.Euler(0f, 180f, 0f);

        return (new Vector3(x, y, 0f), rot);
    }
}