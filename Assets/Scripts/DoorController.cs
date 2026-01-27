using Fusion;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DoorController : NetworkBehaviour
{
    [Header("Door Parts")]
    [SerializeField] private GameObject doorBlock;
    [SerializeField] private GameObject doorOpen;
    [SerializeField] private GameObject finishArea;

    [Header("Logic")]
    [SerializeField] private KeyManager keyManager;

    // Networked door state
    [Networked] private NetworkBool IsOpen { get; set; }

    // SPAWN
    public override void Spawned()
    {
        ApplyState(IsOpen);
    }

    // VISUAL SYNC (HOST + CLIENT)
    public override void Render()
    {
        ApplyState(IsOpen);
    }
    // PLAYER REACHES DOOR
   
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Only State Authority decides
        if (!Object.HasStateAuthority)
            return;

        TryOpenDoor();
    }
    private void TryOpenDoor()
    {
        if (keyManager != null && keyManager.AllKeysCollected())
        {
            IsOpen = true;
        }
    }
    // APPLY VISUAL STATE
    private void ApplyState(bool open)
    {
        doorBlock.SetActive(!open);
        doorOpen.SetActive(open);
        finishArea.SetActive(open);
    }
}

