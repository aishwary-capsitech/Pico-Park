//using DG.Tweening;
//using Fusion;
//using System.Collections.Generic;
//using UnityEngine;

//public class HeliManager : NetworkBehaviour
//{
//    //[Networked] private float screenTop { get; set; }
//    //[Networked] private float screenBottom { get; set; }
//    //[Networked] private float screenLeft { get; set; }
//    //[Networked] private float screenRight { get; set; }
//    [Networked] private float spawnTime { get; set; }

//    private GameObject heliPref;
//    private List<GameObject> helis = new List<GameObject>();

//    public GameObject heliToSpawn;
//    public GameObject EnemyBullet;
//    public int spawnX;
//    public float rotateHeliY = 0f, minY = 0f, maxY = 1.5f, spawnInterval = 5f, nextSpawnTime = 0f, nextFire = 0f, fireRate = 0.5f, enemyBulletTime = 0.9f, screenTop, screenBottom, screenLeft, screenRight;

//    private Vector3 direction;

//    private void Start()
//    {

//    }

//    public override void FixedUpdateNetwork()
//    {
//        screenTop = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)).y;
//        screenBottom = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;
//        screenLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
//        screenRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;

//        if (Time.time >= nextSpawnTime)
//        {
//            SpawnHeli(heliToSpawn);
//            nextSpawnTime = Time.time + spawnInterval;
//        }

//        if(Time.time >= nextFire)
//        {
//            SpawnEnemyBullet();
//            nextFire = Time.time + fireRate;
//        }

//        if (heliPref != null)
//        {
//            Destroy(heliPref, 5f);
//        }
//    }

//    private void SpawnHeli(GameObject heli)
//    {
//        if (heli == null)
//        {
//            return;
//        }

//        float[] screenArray = { screenLeft, screenRight };
//        spawnX = Random.Range(0, screenArray.Length);
//        float spawnY = Random.Range(screenTop - maxY, screenTop - minY);
//        Vector3 spawnPos = new Vector3(screenArray[spawnX], spawnY, 0f);
//        heliPref = Instantiate(heli, spawnPos, Quaternion.identity);
//        heliPref.SetActive(true);

//        if (spawnX == 0)
//        {
//            heliPref.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
//            rotateHeliY = 180f;
//        }
//        else
//        {
//            heliPref.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
//            rotateHeliY = 0f;
//        }

//        if (heliPref != null)
//        {
//            helis.Add(heliPref);
//        }
//    }

//    private void SpawnEnemyBullet()
//    {
//        GameObject enemyBullet = null;
//        GameObject player = null;

//        foreach(var p in FindObjectsOfType<Player>())
//        {
//            if (p.Object.HasStateAuthority)
//            {
//                player = p.gameObject;
//                break;
//            }
//        }
//        //GameObject player = Player.Instance.gameObject;

//        if (EnemyBullet == null || heliPref == null)
//        {
//            return;
//        }

//        enemyBullet = Instantiate(EnemyBullet, heliPref.transform.position, Quaternion.identity);
//        direction = player.transform.position - enemyBullet.transform.position;
//        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
//        enemyBullet.transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
//        enemyBullet.transform.DOMove(player.transform.position, enemyBulletTime);
//    }
//}


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