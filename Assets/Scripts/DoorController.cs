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

    [Networked] private NetworkBool IsOpen { get; set; }

    private bool lastState;

    public override void Spawned()
    {
        ApplyState(IsOpen);
        lastState = IsOpen;
    }

    public override void FixedUpdateNetwork()
    {
        if (IsOpen != lastState)
        {
            lastState = IsOpen;
            ApplyState(IsOpen);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (Object.HasStateAuthority)
        {
            TryOpenDoor();
        }
        else
        {
            RPC_TryOpenDoor();
        }
    }

    // HOST ONLY
    private void TryOpenDoor()
    {
        if (!Object.HasStateAuthority) return;

        if (keyManager != null && keyManager.AllKeysCollected())
        {
            OpenDoor();
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_TryOpenDoor()
    {
        TryOpenDoor();
    }

    public void OpenDoor()
    {
        if (!Object.HasStateAuthority) return;
        IsOpen = true;
    }

    private void ApplyState(bool open)
    {
        doorBlock.SetActive(!open);
        doorOpen.SetActive(open);
        finishArea.SetActive(open);
    }
}
