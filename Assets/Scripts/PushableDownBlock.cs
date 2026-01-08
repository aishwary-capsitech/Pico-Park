// using UnityEngine;

// public class PushableDownBlock : MonoBehaviour
// {
//     private Rigidbody2D rb;

//     void Awake()
//     {
//         rb = GetComponent<Rigidbody2D>();
//     }

//     private void OnCollisionStay2D(Collision2D collision)
//     {
//         if (!collision.collider.CompareTag("Player"))
//             return;

//         // Slight damping so block doesn’t slide forever
//         rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.98f, rb.linearVelocity.y);
//     }
// }


using UnityEngine;

public class PushableDownBlock : MonoBehaviour
{
    [Header("Mass Settings")]
    public float heavyMass = 6f;   // hard to move
    public float lightMass = 2f;   // easy to move

    [Header("Damping")]
    public float horizontalDamping = 0.98f;

    private Rigidbody2D rb;
    private bool topBlockOnMe;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.mass = heavyMass; // start heavy
    }

    void FixedUpdate()
    {
        // Apply slight damping so it doesn't slide forever
        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x * horizontalDamping,
            rb.linearVelocity.y
        );
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 🔻 Top block landed → make lighter
        if (collision.collider.CompareTag("TopBlock"))
        {
            topBlockOnMe = true;
            rb.mass = lightMass;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // 🔺 Top block removed → heavy again
        if (collision.collider.CompareTag("TopBlock"))
        {
            topBlockOnMe = false;
            rb.mass = heavyMass;
        }
    }
}
