using Fusion;
using UnityEngine;

public class KeyManager : NetworkBehaviour
{
    public static KeyManager Instance;

    [Header("Keys in order")]
    public KeyPickup[] keys;
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

    // CALLED BY KEY PICKUP
       public void CollectKey(int keyIndex)
    {
        if (!Object.HasStateAuthority)
            return;

        if (keyIndex != collectedKeys)
            return;

        collectedKeys++;
    }

    // QUERY USED BY DOOR
    public bool AllKeysCollected()
    {
        return collectedKeys >= keys.Length;
    }

    // KEY VISIBILITY
    private void UpdateKeysVisibility()
    {
        if (keys == null)
            return;

        for (int i = 0; i < keys.Length; i++)
        {
            bool visible = (i == collectedKeys);
            keys[i].SetVisible(visible);
        }
    }
   
    public void ResetKeys()
    {
        if (!Object.HasStateAuthority)
            return;

        collectedKeys = 0;
    }
}
