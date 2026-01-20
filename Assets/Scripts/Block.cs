// using UnityEngine;
// using Fusion;
// using System.Linq;

// [RequireComponent(typeof(Rigidbody2D))]
// public class Block : NetworkBehaviour
// {
//     // Networked transform data
//     [Networked] private float currentPositionX { get; set; }
//     [Networked] private float currentPositionY { get; set; }
//     [Networked] private float currentRotationZ { get; set; }

//     // Networked mass (derived from player count)
//     [Networked] public float mass { get; set; }

//     // Initial (scene-placed) transform
//     private Vector2 initialPosition;
//     private float initialRotationZ;

//     private Rigidbody2D rb;

//     public override void Spawned()
//     {
//         rb = GetComponent<Rigidbody2D>();

//         // Save initial scene placement
//         initialPosition = transform.position;
//         initialRotationZ = transform.rotation.eulerAngles.z;

//         // Initialize networked state
//         currentPositionX = initialPosition.x;
//         currentPositionY = initialPosition.y;
//         currentRotationZ = initialRotationZ;

//         Debug.Log($"Block {name} spawned at {initialPosition}");
//     }

//     public override void FixedUpdateNetwork()
//     {
//         if (!HasStateAuthority)
//             return;

//         // Sync physics transform
//         currentPositionX = transform.position.x;
//         currentPositionY = transform.position.y;
//         currentRotationZ = transform.rotation.eulerAngles.z;

//         UpdateMass();
//     }

//     private void UpdateMass()
//     {
//         int playerCount = Runner.ActivePlayers.Count();
//         mass = Mathf.Max(1f, playerCount); // safety
//         rb.mass = mass;
//     }

//     public override void Render()
//     {
//         if (!HasStateAuthority)
//         {
//             transform.position = new Vector2(currentPositionX, currentPositionY);
//             transform.rotation = Quaternion.Euler(0, 0, currentRotationZ);
//         }
//     }

//     public void ResetBlock()
//     {
//         if (!HasStateAuthority)
//             return;

//         // Reset transform
//         transform.position = initialPosition;
//         transform.rotation = Quaternion.Euler(0, 0, initialRotationZ);

//         // Reset physics
//         rb.linearVelocity = Vector2.zero;
//         rb.angularVelocity = 0f;

//         // Sync networked state
//         currentPositionX = initialPosition.x;
//         currentPositionY = initialPosition.y;
//         currentRotationZ = initialRotationZ;

//         Debug.Log($"Block {name} reset to initial position");
//     }
// }


// using Fusion;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;

// [RequireComponent(typeof(Rigidbody2D))]
// public class Block : NetworkBehaviour
// {
//     // =========================
//     // EXISTING NETWORKED DATA (UNCHANGED)
//     // =========================
//     [Networked] private float currentPositionX { get; set; }
//     [Networked] private float currentPositionY { get; set; }
//     [Networked] private float currentRotationZ { get; set; }

//     [Networked] public float mass { get; set; }

//     private Vector2 initialPosition;
//     private float initialRotationZ;

//     private Rigidbody2D rb;

//     // =========================
//     // ADDED (BRIDGE-LIKE LOGIC)
//     // =========================
//     private HashSet<NetworkObject> playersOnBlock = new HashSet<NetworkObject>();
//     private bool allPlayersOnBlock = false;

//     // =========================
//     // SPAWN
//     // =========================
//     public override void Spawned()
//     {
//         rb = GetComponent<Rigidbody2D>();

//         initialPosition = transform.position;
//         initialRotationZ = transform.rotation.eulerAngles.z;

//         currentPositionX = initialPosition.x;
//         currentPositionY = initialPosition.y;
//         currentRotationZ = initialRotationZ;
//     }

//     // =========================
//     // NETWORK UPDATE
//     // =========================
//     public override void FixedUpdateNetwork()
//     {
//         if (!HasStateAuthority)
//             return;

//         // EXISTING SYNC LOGIC (UNCHANGED)
//         currentPositionX = transform.position.x;
//         currentPositionY = transform.position.y;
//         currentRotationZ = transform.rotation.eulerAngles.z;

//         UpdateMass();

//         // 🔒 NEW: BRIDGE-STYLE CONDITION
//         CheckAllPlayersOnBlock();
//     }

//     // =========================
//     // EXISTING MASS LOGIC (UNCHANGED)
//     // =========================
//     private void UpdateMass()
//     {
//         int playerCount = Runner.ActivePlayers.Count();
//         mass = Mathf.Max(1f, playerCount);
//         rb.mass = mass;
//     }

//     // =========================
//     // ADDED: SAME CONDITION AS BRIDGE
//     // =========================
//     private void CheckAllPlayersOnBlock()
//     {
//         int requiredPlayers = Runner.ActivePlayers.Count();

//         // Until all players are on block → freeze X
//         if (!allPlayersOnBlock)
//         {
//             if (playersOnBlock.Count >= requiredPlayers)
//             {
//                 allPlayersOnBlock = true;
//             }
//             else
//             {
//                 rb.constraints =
//                     RigidbodyConstraints2D.FreezePositionX |
//                     RigidbodyConstraints2D.FreezeRotation;
//                 return;
//             }
//         }

//         // Once condition satisfied → restore original behavior
//         rb.constraints = RigidbodyConstraints2D.FreezeRotation;
//     }

//     // =========================
//     // PLAYER DETECTION (SAME AS BRIDGE)
//     // =========================
//     private void OnCollisionEnter2D(Collision2D col)
//     {
//         if (!HasStateAuthority)
//             return;

//         Player player = col.gameObject.GetComponent<Player>();
//         if (player != null)
//         {
//             playersOnBlock.Add(player.Object);
//         }
//     }

//     private void OnCollisionExit2D(Collision2D col)
//     {
//         if (!HasStateAuthority)
//             return;

//         Player player = col.gameObject.GetComponent<Player>();
//         if (player != null)
//         {
//             playersOnBlock.Remove(player.Object);
//         }
//     }

//     // =========================
//     // CLIENT RENDER (UNCHANGED)
//     // =========================
//     public override void Render()
//     {
//         if (!HasStateAuthority)
//         {
//             transform.position = new Vector2(currentPositionX, currentPositionY);
//             transform.rotation = Quaternion.Euler(0, 0, currentRotationZ);
//         }
//     }

//     // =========================
//     // RESET (OPTIONAL)
//     // =========================
//     public void ResetBlock()
//     {
//         if (!HasStateAuthority)
//             return;

//         transform.position = initialPosition;
//         transform.rotation = Quaternion.Euler(0, 0, initialRotationZ);

//         rb.linearVelocity = Vector2.zero;
//         rb.angularVelocity = 0f;

//         currentPositionX = initialPosition.x;
//         currentPositionY = initialPosition.y;
//         currentRotationZ = initialRotationZ;

//         playersOnBlock.Clear();
//         allPlayersOnBlock = false;
//     }
// }


using Fusion;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Block : NetworkBehaviour
{
    // =========================
    // NETWORKED SYNC (UNCHANGED)
    // =========================
    [Networked] private float currentPositionX { get; set; }
    [Networked] private float currentPositionY { get; set; }
    [Networked] private float currentRotationZ { get; set; }

    [Networked] public float mass { get; set; }

    private Vector2 initialPosition;
    private float initialRotationZ;
    private Rigidbody2D rb;

    // =========================
    // TOUCH TRACKING (CORE LOGIC)
    // =========================
    private HashSet<NetworkObject> playersTouchingBlock = new HashSet<NetworkObject>();

    public override void Spawned()
    {
        rb = GetComponent<Rigidbody2D>();

        initialPosition = transform.position;
        initialRotationZ = transform.rotation.eulerAngles.z;

        currentPositionX = initialPosition.x;
        currentPositionY = initialPosition.y;
        currentRotationZ = initialRotationZ;
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
            return;

        // Existing sync
        currentPositionX = transform.position.x;
        currentPositionY = transform.position.y;
        currentRotationZ = transform.rotation.eulerAngles.z;

        UpdateMass();
        CheckAllPlayersTouching();
    }

    // =========================
    // EXISTING MASS LOGIC
    // =========================
    private void UpdateMass()
    {
        int playerCount = Runner.ActivePlayers.Count();
        mass = Mathf.Max(1f, playerCount);
        rb.mass = mass;
    }

    // =========================
    // BRIDGE-STYLE TOUCH CHECK
    // =========================
    private void CheckAllPlayersTouching()
    {
        int requiredPlayers = Runner.ActivePlayers.Count();

        if (playersTouchingBlock.Count >= requiredPlayers)
        {
            // Unlock movement
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        else
        {
            // Lock movement
            rb.constraints =
                RigidbodyConstraints2D.FreezePositionX |
                RigidbodyConstraints2D.FreezeRotation;
        }
    }

    // =========================
    // TOUCH DETECTION (ANY SIDE)
    // =========================
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!HasStateAuthority)
            return;

        Player player = col.gameObject.GetComponent<Player>();
        if (player != null)
        {
            playersTouchingBlock.Add(player.Object);
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (!HasStateAuthority)
            return;

        Player player = col.gameObject.GetComponent<Player>();
        if (player != null)
        {
            playersTouchingBlock.Remove(player.Object);
        }
    }

    // =========================
    // CLIENT RENDER
    // =========================
    public override void Render()
    {
        if (!HasStateAuthority)
        {
            transform.position = new Vector2(currentPositionX, currentPositionY);
            transform.rotation = Quaternion.Euler(0, 0, currentRotationZ);
        }
    }

    // =========================
    // RESET
    // =========================
    public void ResetBlock()
    {
        if (!HasStateAuthority)
            return;

        transform.position = initialPosition;
        transform.rotation = Quaternion.Euler(0, 0, initialRotationZ);

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        playersTouchingBlock.Clear();
    }
}


