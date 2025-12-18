using UnityEngine;
using System.Collections.Generic;
using Fusion;

public class BridgeRotation : NetworkBehaviour
{
    [SerializeField] private float requiredMass = 5f;
    [SerializeField] private float rotationSpeed = 45f;

    [Networked] private NetworkBool isRotating { get; set; }
    [Networked] private NetworkBool isHorizontal { get; set; }
    [Networked] private float currentRotationZ { get; set; }

    private Quaternion targetRotation;
    private Quaternion initialRotation;
    private float targetRotationZ;
    private HashSet<Rigidbody2D> players = new HashSet<Rigidbody2D>();

    public override void Spawned()
    {
        initialRotation = transform.rotation;
        targetRotation = transform.rotation * Quaternion.Euler(0, 0, -90f);
        targetRotationZ = targetRotation.eulerAngles.z;

        // Initialize networked rotation
        currentRotationZ = transform.rotation.eulerAngles.z;

        Debug.Log($"Bridge spawned - Initial: {initialRotation.eulerAngles.z}, Target: {targetRotationZ}");
    }

    public override void FixedUpdateNetwork()
    {
        // Only server calculates rotation
        if (HasStateAuthority)
        {
            if (isRotating && !isHorizontal)
            {
                Quaternion newRotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Runner.DeltaTime
                );

                transform.rotation = newRotation;
                currentRotationZ = newRotation.eulerAngles.z;

                if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
                {
                    isHorizontal = true;
                    isRotating = false;
                    Debug.Log("Bridge rotation complete!");
                }
            }
        }
    }

    public override void Render()
    {
        base.Render();

        // All clients update their visual rotation from networked value
        if (!HasStateAuthority)
        {
            transform.rotation = Quaternion.Euler(0, 0, currentRotationZ);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!HasStateAuthority) return;

        Rigidbody2D rb = col.rigidbody;
        if (rb != null)
        {
            players.Add(rb);
            Debug.Log($"Player entered bridge. Total players: {players.Count}");
            CheckMass();
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (!HasStateAuthority) return;

        Rigidbody2D rb = col.rigidbody;
        if (rb != null)
        {
            players.Remove(rb);
            Debug.Log($"Player exited bridge. Total players: {players.Count}");
        }
    }

    void CheckMass()
    {
        if (isHorizontal) return;

        float totalMass = 0f;
        players.RemoveWhere(rb => rb == null);

        foreach (var rb in players)
            totalMass += rb.mass;

        Debug.Log($"Total mass on bridge: {totalMass} / {requiredMass}");

        if (totalMass >= requiredMass && !isRotating)
        {
            isRotating = true;
            Debug.Log("Bridge rotation started!");
        }
    }
}