//using UnityEngine;
//using System.Collections.Generic;
//using Fusion;

//public class BridgeRotation : NetworkBehaviour
//{
//    public static BridgeRotation Instance;

//    private Player joinedPlayer;
//    [SerializeField] private float requiredMass = 5f;
//    [SerializeField] private float rotationSpeed = 45f;

//    [Networked] private NetworkBool isRotating { get; set; }
//    [Networked] private NetworkBool isHorizontal { get; set; }
//    [Networked] private float currentRotationZ { get; set; }

//    private Quaternion targetRotation;
//    private Quaternion initialRotation;
//    private float targetRotationZ;
//    private HashSet<Rigidbody2D> players = new HashSet<Rigidbody2D>();

//    private void Awake()
//    {
//        Instance = this;
//    }

//    public override void Spawned()
//    {
//        initialRotation = transform.rotation;
//        targetRotation = transform.rotation * Quaternion.Euler(0, 0, -90f);
//        targetRotationZ = targetRotation.eulerAngles.z;

//        // Initialize networked rotation
//        currentRotationZ = transform.rotation.eulerAngles.z;

//        Debug.Log($"Bridge spawned - Initial: {initialRotation.eulerAngles.z}, Target: {targetRotationZ}");
//    }

//    public override void FixedUpdateNetwork()
//    {
//        // Only server calculates rotation
//        if (HasStateAuthority)
//        {
//            if (isRotating && !isHorizontal)
//            {
//                Quaternion newRotation = Quaternion.RotateTowards(
//                    transform.rotation,
//                    targetRotation,
//                    rotationSpeed * Runner.DeltaTime
//                );

//                transform.rotation = newRotation;
//                currentRotationZ = newRotation.eulerAngles.z;

//                if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
//                {
//                    isHorizontal = true;
//                    isRotating = false;
//                    Debug.Log("Bridge rotation complete!");
//                }
//            }
//        }

//        if (joinedPlayer.playerCount == 1)
//        {
//            requiredMass = 1f;
//        }
//        else if (joinedPlayer.playerCount == 2)
//        {
//            requiredMass = 2f;
//        }
//        else if (joinedPlayer.playerCount == 3)
//        {
//            requiredMass = 3f;
//        }
//        else if (joinedPlayer.playerCount == 4)
//        {
//            requiredMass = 4f;
//        }
//        else
//        {
//            requiredMass = 5f;
//        }
//    }

//    public override void Render()
//    {
//        base.Render();

//        // All clients update their visual rotation from networked value
//        if (!HasStateAuthority)
//        {
//            transform.rotation = Quaternion.Euler(0, 0, currentRotationZ);
//        }
//    }

//    void OnCollisionEnter2D(Collision2D col)
//    {
//        if (!HasStateAuthority) return;

//        Rigidbody2D rb = col.rigidbody;
//        if (rb != null)
//        {
//            players.Add(rb);
//            Debug.Log($"Player entered bridge. Total players: {players.Count}");
//            CheckMass();
//        }
//    }

//    void OnCollisionExit2D(Collision2D col)
//    {
//        if (!HasStateAuthority) return;

//        Rigidbody2D rb = col.rigidbody;
//        if (rb != null)
//        {
//            players.Remove(rb);
//            Debug.Log($"Player exited bridge. Total players: {players.Count}");
//        }
//    }

//    void CheckMass()
//    {
//        if (isHorizontal) return;

//        float totalMass = 0f;
//        players.RemoveWhere(rb => rb == null);

//        foreach (var rb in players)
//            totalMass += rb.mass;

//        Debug.Log($"Total mass on bridge: {totalMass} / {requiredMass}");

//        if (totalMass >= requiredMass && !isRotating)
//        {
//            isRotating = true;
//            Debug.Log("Bridge rotation started!");
//        }
//    }
//}

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
}
