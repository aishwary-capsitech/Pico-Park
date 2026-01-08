using DG.Tweening;
using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class HeliManager : NetworkBehaviour
{
    //[Networked] private float screenTop { get; set; }
    //[Networked] private float screenBottom { get; set; }
    //[Networked] private float screenLeft { get; set; }
    //[Networked] private float screenRight { get; set; }
    [Networked] private float spawnTime { get; set; }

    private GameObject heliPref;
    private List<GameObject> helis = new List<GameObject>();

    public GameObject heliToSpawn;
    public GameObject EnemyBullet;
    public int spawnX;
    public float rotateHeliY = 0f, minY = 0f, maxY = 1.5f, spawnInterval = 5f, nextSpawnTime = 0f, nextFire = 0f, fireRate = 0.5f, enemyBulletTime = 0.9f, screenTop, screenBottom, screenLeft, screenRight;

    private Vector3 direction;

    private void Start()
    {

    }

    public override void FixedUpdateNetwork()
    {
        screenTop = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)).y;
        screenBottom = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;
        screenLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
        screenRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;

        if (Time.time >= nextSpawnTime)
        {
            SpawnHeli(heliToSpawn);
            nextSpawnTime = Time.time + spawnInterval;
        }

        if(Time.time >= nextFire)
        {
            SpawnEnemyBullet();
            nextFire = Time.time + fireRate;
        }

        if (heliPref != null)
        {
            Destroy(heliPref, 5f);
        }
    }

    private void SpawnHeli(GameObject heli)
    {
        if (heli == null)
        {
            return;
        }

        float[] screenArray = { screenLeft, screenRight };
        spawnX = Random.Range(0, screenArray.Length);
        float spawnY = Random.Range(screenTop - maxY, screenTop - minY);
        Vector3 spawnPos = new Vector3(screenArray[spawnX], spawnY, 0f);
        heliPref = Instantiate(heli, spawnPos, Quaternion.identity);
        heliPref.SetActive(true);

        if (spawnX == 0)
        {
            heliPref.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            rotateHeliY = 180f;
        }
        else
        {
            heliPref.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            rotateHeliY = 0f;
        }

        if (heliPref != null)
        {
            helis.Add(heliPref);
        }
    }

    private void SpawnEnemyBullet()
    {
        GameObject enemyBullet = null;
        GameObject player = null;

        foreach(var p in FindObjectsOfType<Player>())
        {
            if (p.Object.HasStateAuthority)
            {
                player = p.gameObject;
                break;
            }
        }
        //GameObject player = Player.Instance.gameObject;

        if (EnemyBullet == null || heliPref == null)
        {
            return;
        }

        enemyBullet = Instantiate(EnemyBullet, heliPref.transform.position, Quaternion.identity);
        direction = player.transform.position - enemyBullet.transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        enemyBullet.transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
        enemyBullet.transform.DOMove(player.transform.position, enemyBulletTime);
    }
}
