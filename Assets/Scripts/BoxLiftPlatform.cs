using UnityEngine;
using Fusion;
using System.Collections;

public class BoxLiftPlatform : NetworkBehaviour
{
    [Header("Positions")]
    public Transform upPosition;

    [Header("Movement Speeds")]
    public float upSpeed = 3f;
    public float downSpeed = 6f;

    [Header("Timing")]
    public float holdAtTopTime = 0.3f;   // 🟢 pause at top

    private bool isPressed = false;
    private bool isHolding = false;

    private Vector3 downPosition;

    public override void Spawned()
    {
        downPosition = transform.position;
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
            return;

        // ⛔ stop movement while holding
        if (isHolding)
            return;

        Vector3 target;
        float speed;

        if (isPressed)
        {
            target = upPosition.position;
            speed = upSpeed;
        }
        else
        {
            target = downPosition;
            speed = downSpeed;
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            speed * Runner.DeltaTime
        );

        // ✅ clean STOP at top
        if (isPressed && Vector3.Distance(transform.position, upPosition.position) < 0.01f)
        {
            StartCoroutine(HoldAtTop());
        }
    }

    IEnumerator HoldAtTop()
    {
        isHolding = true;
        yield return new WaitForSecondsRealtime(holdAtTopTime);
        isHolding = false;
    }

    // 🔔 called by buzzer
    public void StartLift()
    {
        if (!Object.HasStateAuthority)
            return;

        isPressed = true;
    }

    // 🔕 called when player leaves
    public void StopLift()
    {
        if (!Object.HasStateAuthority)
            return;

        isPressed = false;
    }
}

