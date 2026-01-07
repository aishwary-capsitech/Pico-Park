using Fusion;
using UnityEngine;

public class PendulumMover : NetworkBehaviour
{
    public float maxAngle = 45f;   // how far left-right it swings
    public float speed = 2f;       // swing speed

    public override void FixedUpdateNetwork()
    {
        float angle = maxAngle * Mathf.Sin(Time.time * speed);
        transform.localEulerAngles = new Vector3(0, 0, angle);
    }
}
