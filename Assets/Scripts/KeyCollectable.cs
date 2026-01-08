using UnityEngine;
using Fusion;

public class KeyCollectable : NetworkBehaviour
{
    public int keyIndex;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Player has authority, NOT the key
        if (!KeyManager.Instance.Object.HasStateAuthority) return;

        KeyManager.Instance.CollectKey(keyIndex);

        Runner.Despawn(Object);
    }
}
