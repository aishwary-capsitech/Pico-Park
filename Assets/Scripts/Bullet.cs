using Fusion;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    float screenTop, screenBottom, screenLeft, screenRight;

    public override void FixedUpdateNetwork()
    {
        screenLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
        screenRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
        screenTop = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)).y;
        screenBottom = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;
        var pos = transform.position;

        if (pos.y < screenBottom || pos.y > screenTop || pos.x < screenLeft || pos.x > screenRight)
        {
            Destroy(gameObject);
        }
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.name.Contains("Ground") || collision.gameObject.name.Contains("StartPoint"))
    //    {
    //        Destroy(gameObject);
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.name.Contains("StartPoint"))
        {
            Destroy(gameObject);
        }

        if (collision.gameObject.name.Contains("helicopter") && !gameObject.name.Contains("Enemy"))
        {
            //SpawnManager.Instance.heliPrefHealth -= 2f;
            //GameUIManager.Instance.score += 2;
            Destroy(gameObject);
        }

        if(collision.gameObject.CompareTag("Player"))
        {
            //PlayerHealth.Instance.playerHealth -= 10f;
            Destroy(gameObject);
        }
    }
}
