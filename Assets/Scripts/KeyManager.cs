using UnityEngine;
using Fusion;

public class KeyManager : NetworkBehaviour
{
    public static KeyManager Instance;
    [SerializeField] private DoorController doorController;

    [Header("Keys in order")]
    public KeyPickup[] keys;

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
    }

    public void TryOpenDoor()
    {
        if (!Object.HasStateAuthority) return;

        if (collectedKeys >= keys.Length)
        {
            doorController.OpenDoor();
        }
    }

    public bool AllKeysCollected()
    {
        return collectedKeys >= keys.Length;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_TryOpenDoor()
    {
        TryOpenDoor();
    }

    private void UpdateKeysVisibility()
    {
        for (int i = 0; i < keys.Length; i++)
        {
            bool visible = (i == collectedKeys);
            keys[i].SetVisible(visible);
        }
    }

    public void ResetKeys()
    {
        if (!Object.HasStateAuthority) return;

        collectedKeys = 0;

        for (int i = 0; i < keys.Length; i++)
        {
            keys[i].SetVisible(i == 0);
        }
    }
}


