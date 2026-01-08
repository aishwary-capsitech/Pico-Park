// using UnityEngine;
// using UnityEngine.EventSystems;

// public class SwipeManager : MonoBehaviour
// {
//     public PlayerSwipeCrouch playerCrouch;

//     [Header("Swipe Settings")]
//     public float minSwipeDistance = 120f; // pixels

//     private Vector2 startPos;
//     private bool swipeActive = false;

//     void Update()
//     {
//         // 1️⃣ Touch start
//         if (Input.GetMouseButtonDown(0))
//         {
//             // ❌ Ignore if touching UI (arrow buttons)
//             if (EventSystem.current.IsPointerOverGameObject())
//                 return;

//             startPos = Input.mousePosition;
//             swipeActive = true;
//         }

//         // 2️⃣ Finger moving
//         if (Input.GetMouseButton(0) && swipeActive)
//         {
//             Vector2 currentPos = Input.mousePosition;
//             float deltaY = startPos.y - currentPos.y;

//             // 👇 Real downward swipe
//             if (deltaY > minSwipeDistance)
//             {
//                 playerCrouch.StartCrouch();
//             }
//         }

//         // 3️⃣ Finger released
//         if (Input.GetMouseButtonUp(0))
//         {
//             swipeActive = false;
//             playerCrouch.StopCrouch();
//         }
//     }
// }

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using Fusion;

public class SwipeManager : MonoBehaviour
{
    private PlayerSwipeCrouch playerCrouch;
    private NetworkObject playerNetObj;

    [Header("Swipe Settings")]
    public float minSwipeDistance = 120f;

    private Vector2 startPos;
    private bool swipeActive;

    void Start()
    {
        StartCoroutine(FindLocalPlayer());
    }

    IEnumerator FindLocalPlayer()
    {
        while (playerCrouch == null)
        {
            PlayerSwipeCrouch[] players = FindObjectsOfType<PlayerSwipeCrouch>();

            foreach (var p in players)
            {
                NetworkObject netObj = p.GetComponent<NetworkObject>();

                // 🔥 ONLY local input authority player
                if (netObj != null && netObj.HasInputAuthority)
                {
                    playerCrouch = p;
                    playerNetObj = netObj;
                    Debug.Log("✅ Local player linked for swipe");
                    yield break;
                }
            }

            yield return null;
        }
    }

    void Update()
    {
        // ❌ no local player → no swipe
        if (playerCrouch == null || playerNetObj == null)
            return;

        // 1️⃣ Touch start
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject())
                return;

            startPos = Input.mousePosition;
            swipeActive = true;
        }

        // 2️⃣ Swipe down
        if (Input.GetMouseButton(0) && swipeActive)
        {
            float deltaY = startPos.y - Input.mousePosition.y;

            if (deltaY > minSwipeDistance)
            {
                playerCrouch.StartCrouch();
            }
        }

        // 3️⃣ Release
        if (Input.GetMouseButtonUp(0))
        {
            swipeActive = false;
            playerCrouch.StopCrouch();
        }
    }
}
