using UnityEngine;

using Fusion;

public class KeyCollectable : NetworkBehaviour

{
    public int keyIndex;
    private bool collected = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;

        if (!other.CompareTag("Player")) return;

        if (!KeyManager.Instance.Object.HasStateAuthority) return;

        collected = true;

        KeyManager.Instance.CollectKey(keyIndex);

        //DO NOT despawn
        gameObject.SetActive(false);
    }
}