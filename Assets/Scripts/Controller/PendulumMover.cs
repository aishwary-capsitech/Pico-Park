using Fusion;
using UnityEngine;

public class PendulumMover : NetworkBehaviour
{
    public float maxAngle = 45f;   
    public float speed = 2f;       

    public override void FixedUpdateNetwork()
    {
        float angle = maxAngle * Mathf.Sin(Time.time * speed);
        transform.localEulerAngles = new Vector3(0, 0, angle);
    }
}
