using Fusion;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BridgeRotation : NetworkBehaviour
{
    [SerializeField] private float rotationSpeed = 45f;

    [Networked] private int requiredMass { get; set; }
    [Networked] private NetworkBool isRotating { get; set; }
    [Networked] private NetworkBool isHorizontal { get; set; }
    [Networked] private float currentRotationZ { get; set; }

    private Quaternion targetRotation;
    private HashSet<NetworkObject> playersOnBridge = new HashSet<NetworkObject>();

    public override void Spawned()
    {
        targetRotation = transform.rotation * Quaternion.Euler(0, 0, -90f);
        currentRotationZ = transform.rotation.eulerAngles.z;
    }

    private void UpdateRequiredMass()
    {
        requiredMass = Runner.ActivePlayers.Count();
        Debug.Log($"Required Mass set to {requiredMass}");
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
            return;

        UpdateRequiredMass();

        if (isRotating && !isHorizontal)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Runner.DeltaTime
            );

            currentRotationZ = transform.rotation.eulerAngles.z;

            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
            {
                isHorizontal = true;
                isRotating = false;
            }
        }
    }

    public override void Render()
    {
        if (!HasStateAuthority)
            transform.rotation = Quaternion.Euler(0, 0, currentRotationZ);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!HasStateAuthority) return;

        Player player = col.gameObject.GetComponent<Player>();
        if (player != null)
        {
            playersOnBridge.Add(player.Object);
            CheckMass();
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (!HasStateAuthority) return;

        Player player = col.gameObject.GetComponent<Player>();
        if (player != null)
        {
            playersOnBridge.Remove(player.Object);
        }
    }

    private void CheckMass()
    {
        if (isHorizontal || isRotating)
            return;

        int currentMass = playersOnBridge.Count;

        Debug.Log($"Players on bridge: {currentMass}/{requiredMass}");

        if (currentMass >= requiredMass)
        {
            isRotating = true;
            Debug.Log("Bridge rotation started");
        }
    }

    public void ResetBridge()
    {
        if (!HasStateAuthority) return;
        isHorizontal = false;
        isRotating = false;
        targetRotation = transform.rotation * Quaternion.Euler(0, 0, 0);
        playersOnBridge.Clear();
        Debug.Log("Bridge reset to initial position");
    }
}
