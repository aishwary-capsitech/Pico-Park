// using UnityEngine;
// using Fusion;

// public class KeyManager : NetworkBehaviour
// {
//     public static KeyManager Instance;

//     [Header("Keys in order")]
//     public GameObject[] keys;

//     [Networked] private int collectedKeys { get; set; }

//     private void Awake()
//     {
//         Instance = this;
//     }

//     public override void Spawned()
//     {
//         // Ensure only first key is active
//         for (int i = 0; i < keys.Length; i++)
//         {
//             keys[i].SetActive(i == 0);
//         }
//     }

//     public void CollectKey(int keyIndex)
//     {
//         // Safety check
//         if (!Object.HasStateAuthority) return;

//         // Sequential check
//         if (keyIndex != collectedKeys) return;

//         collectedKeys++;

//         if (collectedKeys < keys.Length)
//         {
//             keys[collectedKeys].SetActive(true);
//         }
//         else
//         {
//            // DoorController.Instance.OpenDoor();
//         }
//     }
// }



using UnityEngine;
using Fusion;

public class KeyManager : NetworkBehaviour
{
    public static KeyManager Instance;

    [Header("Keys in order")]
    public KeyPickup[] keys;   // Assign in correct order

    [HideInInspector]
    [Networked] public int collectedKeys { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            collectedKeys = 0;
        }

        UpdateKeysVisibility();
    }

    public override void Render()
    {
        UpdateKeysVisibility();
    }

    // CALLED BY KEY
    public void CollectKey(int keyIndex)
    {
        if (!Object.HasStateAuthority) return;
        if (keyIndex != collectedKeys) return;

        collectedKeys++;

        //if (collectedKeys >= keys.Length)
        //{
        //    DoorController.Instance.OpenDoor();
        //}
    }

    public void CheckAllKeyCollected()
    {
        if (!Object.HasStateAuthority) return;

        if(collectedKeys >= keys.Length)
        {
            DoorController.Instance.OpenDoor();
        }
    }

    // VISIBILITY SYNC
    private void UpdateKeysVisibility()
    {
        for (int i = 0; i < keys.Length; i++)
        {
            bool visible = (i == collectedKeys);
            keys[i].SetVisible(visible);
        }
    }
}


