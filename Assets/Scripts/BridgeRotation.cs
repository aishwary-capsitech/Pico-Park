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
    [Networked] private float rotationZ { get; set; }

    private float initialRotationZ;
    private HashSet<NetworkObject> playersOnBridge = new HashSet<NetworkObject>();

    public override void Spawned()
    {
        initialRotationZ = transform.rotation.eulerAngles.z;

        if (HasStateAuthority)
        {
            rotationZ = initialRotationZ;
            isRotating = false;
            isHorizontal = false;
        }

        ApplyRotation();
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
            return;

        UpdateRequiredMass();

        if (isRotating && !isHorizontal)
        {
            float targetZ = initialRotationZ - 90f;

            rotationZ = Mathf.MoveTowardsAngle(
                rotationZ,
                targetZ,
                rotationSpeed * Runner.DeltaTime
            );

            ApplyRotation();

            if (Mathf.Abs(Mathf.DeltaAngle(rotationZ, targetZ)) < 0.1f)
            {
                rotationZ = targetZ;
                ApplyRotation();

                isHorizontal = true;
                isRotating = false;
            }
        }
    }


    public override void Render()
    {
        if (!HasStateAuthority)
            ApplyRotation();
    }

    private void ApplyRotation()
    {
        transform.rotation = Quaternion.Euler(0, 0, rotationZ);
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

        if (playersOnBridge.Count >= requiredMass)
        {
            isRotating = true;
        }
    }

    private void UpdateRequiredMass()
    {
        requiredMass = Runner.ActivePlayers.Count();
    }

    public void ResetBridge()
    {
        if (!HasStateAuthority) return;

        isHorizontal = false;
        isRotating = false;
        rotationZ = initialRotationZ;

        ApplyRotation();
        playersOnBridge.Clear();
    }
}