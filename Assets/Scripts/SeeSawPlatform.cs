// using UnityEngine;
// using Fusion;

// public class SeeSawPlatform : NetworkBehaviour
// {
//     [Header("Player Torque")]
//     public float torqueForce = 80f;
//     public float deadZone = 0.15f;
//     public float maxAngularVelocity = 120f;

//     [Header("Return To Center")]
//     public float returnStrength = 25f;   // how strong it comes back
//     public float returnDamping = 6f;     // smoothness

//     private Rigidbody2D rb;
//     private float torqueInput;
//     private bool playerOnPlatform;
//     private float restAngle; // original angle (flat)

//     public override void Spawned()
//     {
//         rb = GetComponent<Rigidbody2D>();

//         rb.linearVelocity = Vector2.zero;
//         rb.angularVelocity = 0f;
//         rb.centerOfMass = Vector2.zero;

//         restAngle = rb.rotation; // save initial flat angle
//     }

//     public override void FixedUpdateNetwork()
//     {
//         if (!Object.HasStateAuthority)
//             return;

//         // Clamp angular speed
//         rb.angularVelocity = Mathf.Clamp(
//             rb.angularVelocity,
//             -maxAngularVelocity,
//             maxAngularVelocity
//         );

//         // 🔹 Player torque
//         if (Mathf.Abs(torqueInput) > 0f)
//         {
//             rb.AddTorque(torqueInput * Time.fixedDeltaTime);
//         }
//         // 🔹 Auto return when empty
//         else if (!playerOnPlatform)
//         {
//             float angleError = Mathf.DeltaAngle(rb.rotation, restAngle);

//             float returnTorque =
//                 (-angleError * returnStrength) -
//                 (rb.angularVelocity * returnDamping);

//             rb.AddTorque(returnTorque * Time.fixedDeltaTime);
//         }

//         // Reset for next tick
//         torqueInput = 0f;
//         playerOnPlatform = false;
//     }

//     private void OnCollisionStay2D(Collision2D collision)
//     {
//         if (!Object.HasStateAuthority)
//             return;

//         if (!collision.collider.CompareTag("Player"))
//             return;

//         playerOnPlatform = true;

//         float delta = collision.transform.position.x - transform.position.x;

//         if (Mathf.Abs(delta) < deadZone)
//             return;

//         torqueInput = -Mathf.Sign(delta) * torqueForce;
//     }
// }

// using UnityEngine;
// using Fusion;

// public class SeeSawPlatform : NetworkBehaviour
// {
//     [Header("Tilt Limits")]
//     public float maxTiltAngle = 20f;

//     [Header("Jump Impact")]
//     public float torqueForce = 180f;   // 🔥 FAST RESPONSE
//     public float minFallVelocity = -2f; // must be falling to activate

//     [Header("Return To Center")]
//     public float returnStrength = 35f;
//     public float returnDamping = 8f;

//     private Rigidbody2D rb;
//     private bool activatedByJump;
//     private float restAngle;

//     public override void Spawned()
//     {
//         rb = GetComponent<Rigidbody2D>();

//         rb.linearVelocity = Vector2.zero;
//         rb.angularVelocity = 0f;
//         rb.centerOfMass = Vector2.zero;

//         restAngle = rb.rotation;
//     }

//     public override void FixedUpdateNetwork()
//     {
//         if (!Object.HasStateAuthority)
//             return;

//         // 🔒 HARD ROTATION LIMIT (NO 360)
//         float clampedAngle = Mathf.Clamp(rb.rotation, -maxTiltAngle, maxTiltAngle);
//         rb.MoveRotation(clampedAngle);

//         // 🔁 Return only when not activated
//         if (!activatedByJump)
//         {
//             float angleError = Mathf.DeltaAngle(rb.rotation, restAngle);

//             float correctiveTorque =
//                 (-angleError * returnStrength) -
//                 (rb.angularVelocity * returnDamping);

//             rb.AddTorque(correctiveTorque * Time.fixedDeltaTime);
//         }

//         // reset each tick
//         activatedByJump = false;
//     }

//     private void OnCollisionEnter2D(Collision2D collision)
//     {
//         if (!Object.HasStateAuthority)
//             return;

//         if (!collision.collider.CompareTag("Player"))
//             return;

//         Rigidbody2D playerRb = collision.collider.GetComponent<Rigidbody2D>();
//         if (playerRb == null)
//             return;

//         // ❗ ONLY ACTIVATE IF PLAYER IS FALLING (JUMP LAND)
//         if (playerRb.linearVelocity.y > minFallVelocity)
//             return;

//         activatedByJump = true;

//         float delta =
//             collision.transform.position.x -
//             transform.position.x;

//         // 🔥 STRONG INSTANT TORQUE
//         rb.AddTorque(
//             -Mathf.Sign(delta) * torqueForce,
//             ForceMode2D.Impulse
//         );
//     }
// }


// using UnityEngine;
// using Fusion;

// public class SeeSawPlatform : NetworkBehaviour
// {
//     [Header("Seesaw Settings")]
//     public float tiltForce = 1200f;        // FAST & HEAVY tilt
//     public float maxTiltAngle = 20f;       // degrees
//     public float returnForce = 800f;       // snap back strength
//     public float damping = 10f;            // stability

//     private Rigidbody2D rb;
//     private bool playerStanding;
//     private float targetTorque;

//     public override void Spawned()
//     {
//         rb = GetComponent<Rigidbody2D>();
//         rb.angularVelocity = 0f;
//         rb.rotation = 0f;
//     }

//     public override void FixedUpdateNetwork()
//     {
//         if (!Object.HasStateAuthority)
//             return;

//         float angle = rb.rotation;

//         // 🔒 HARD ANGLE LIMIT (NO FLYING / NO SPIN)
//         if (angle > maxTiltAngle)
//         {
//             rb.rotation = maxTiltAngle;
//             rb.angularVelocity = 0f;
//         }
//         else if (angle < -maxTiltAngle)
//         {
//             rb.rotation = -maxTiltAngle;
//             rb.angularVelocity = 0f;
//         }

//         if (playerStanding)
//         {
//             // 🔥 FAST PRESSURE TILT
//             rb.AddTorque(targetTorque * Time.fixedDeltaTime);
//         }
//         else
//         {
//             // 🔁 RETURN TO FLAT WHEN EMPTY
//             float error = -angle;
//             float returnTorque =
//                 (error * returnForce) -
//                 (rb.angularVelocity * damping);

//             rb.AddTorque(returnTorque * Time.fixedDeltaTime);
//         }

//         // reset every tick
//         playerStanding = false;
//         targetTorque = 0f;
//     }

//     private void OnCollisionStay2D(Collision2D collision)
//     {
//         if (!Object.HasStateAuthority)
//             return;

//         if (!collision.collider.CompareTag("Player"))
//             return;

//         // ✅ Only react when player is ON TOP
//         foreach (ContactPoint2D contact in collision.contacts)
//         {
//             if (contact.normal.y > 0.6f)
//             {
//                 playerStanding = true;

//                 float deltaX =
//                     collision.transform.position.x -
//                     transform.position.x;

//                 // 🔥 pressure-based torque
//                 targetTorque = -deltaX * tiltForce;
//                 break;
//             }
//         }
//     }
// }


using UnityEngine;

public class SeeSawController : MonoBehaviour
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
