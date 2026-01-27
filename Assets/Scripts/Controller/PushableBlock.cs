using UnityEngine;

public class PushableBlock : MonoBehaviour
{
    public float pushForce = 2.5f;
    public float maxSpeed = 2f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.collider.CompareTag("Player"))
            return;

        Vector2 dir = transform.position - col.transform.position;
        dir.y = 0;

        rb.linearVelocity = Vector2.ClampMagnitude(dir.normalized * pushForce, maxSpeed);
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.collider.CompareTag("Player"))
            rb.linearVelocity = Vector2.zero;
    }
}
