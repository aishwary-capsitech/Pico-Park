using Fusion;
using UnityEngine;

public class SeeSawController : NetworkBehaviour
{
    private Rigidbody2D rb;

    [Header("Forces")]
    public float pushTorque = 40f;     // tilt speed when player stands
    public float returnTorque = 6f;    // smooth return
    public float maxAngularSpeed = 120f;

    private bool playerOn;
    private float direction;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (playerOn)
        {
            rb.AddTorque(-direction * pushTorque, ForceMode2D.Force);
        }
        else
        {
            // Smoothly return to 0 rotation
            float correction = -rb.rotation * returnTorque;
            rb.AddTorque(correction, ForceMode2D.Force);
        }

        // Clamp angular speed (prevents vibration / flying)
        rb.angularVelocity = Mathf.Clamp(
            rb.angularVelocity,
            -maxAngularSpeed,
            maxAngularSpeed
        );

        // Reset for next physics frame
        playerOn = false;
        direction = 0f;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player"))
            return;

        playerOn = true;

        float deltaX =
            collision.transform.position.x - transform.position.x;

        direction = Mathf.Sign(deltaX);
    }
}       
