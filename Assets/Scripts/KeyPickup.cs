//using UnityEngine;
//using Fusion;

//public class KeyPickup : NetworkBehaviour
//{
//    public int keyIndex;

//    private bool collected;

//    private SpriteRenderer sr;
//    private Collider2D col;

//    private void Awake()
//    {
//        sr = GetComponent<SpriteRenderer>();
//        col = GetComponent<Collider2D>();
//    }

//    public void SetVisible(bool visible)
//    {
//        if (sr) sr.enabled = visible;
//        if (col) col.enabled = visible;
//    }

//    private void OnTriggerEnter2D(Collider2D other)
//    {
//        if (collected) return;
//        if (!other.CompareTag("Player")) return;
//        if (!Object.HasStateAuthority) return;

//        collected = true;

//        // Update UI (already networked in your UIManager)
//        UIManager.Instance.CollectKey();

//        // Notify manager
//        KeyManager.Instance.CollectKey(keyIndex);

//        // Hide locally (visual only)
//        SetVisible(false);
//    }
//}



using Fusion;
using UnityEngine;

public class KeyPickup : NetworkBehaviour
{
    public int keyIndex;

    private SpriteRenderer sr;
    private Collider2D col;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    public void SetVisible(bool visible)
    {
        if (sr) sr.enabled = visible;
        if (col) col.enabled = visible;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        KeyManager keyManager = FindObjectOfType<KeyManager>();
        keyManager.CollectKey(keyIndex);

        UIManager.Instance.CollectKey();

        // local visual hide
        //SetVisible(false);
    }
}
