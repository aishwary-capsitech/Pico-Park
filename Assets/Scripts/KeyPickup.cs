using UnityEngine;
using Fusion;

public class KeyPickup : NetworkBehaviour
{
    private bool collected = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected)
            return;

        if (!other.CompareTag("Player"))
            return;

        collected = true;

        // ✅ THIS IS THE FIX
        //UIManager.Instance.OnKeyCollected();

        // Hide / despawn key
        gameObject.SetActive(false);
    }
}
