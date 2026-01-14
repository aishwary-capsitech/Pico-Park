// //working one 
// ﻿using UnityEngine;
// using UnityEngine.EventSystems;
// using System.Collections;
// using Fusion;

// public class SwipeManager : MonoBehaviour
// {
//     private PlayerSwipeCrouch playerCrouch;
//     private NetworkObject playerNetObj;

//     [Header("Swipe Settings")]
//     public float minSwipeDistance = 120f;

//     private Vector2 startPos;
//     private bool swipeActive;

//     void Start()
//     {
//         StartCoroutine(FindLocalPlayer());
//     }

//     IEnumerator FindLocalPlayer()
//     {
//         while (playerCrouch == null)
//         {
//             PlayerSwipeCrouch[] players = FindObjectsOfType<PlayerSwipeCrouch>();

//             foreach (var p in players)
//             {
//                 NetworkObject netObj = p.GetComponent<NetworkObject>();

//                 // ONLY local input authority player
//                 if (netObj != null && netObj.HasInputAuthority)
//                 {
//                     playerCrouch = p;
//                     playerNetObj = netObj;
//                     Debug.Log("✅ Local player linked for swipe");
//                     yield break;
//                 }
//             }

//             yield return null;
//         }
//     }

//     void Update()
//     {
//         // no local player → no swipe
//         if (playerCrouch == null || playerNetObj == null)
//             return;

//         // Touch start
//         if (Input.GetMouseButtonDown(0))
//         {
//             if (EventSystem.current != null &&
//                 EventSystem.current.IsPointerOverGameObject())
//                 return;

//             startPos = Input.mousePosition;
//             swipeActive = true;
//         }

//         // Swipe down
//         if (Input.GetMouseButton(0) && swipeActive)
//         {
//             float deltaY = startPos.y - Input.mousePosition.y;

//             if (deltaY > minSwipeDistance)
//             {
//                 playerCrouch.StartCrouch();
//             }
//         }

//         // Release
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
    private Coroutine findRoutine;

    void OnEnable()
    {
        StartFindingPlayer();
    }

    void OnDisable()
    {
        if (findRoutine != null)
            StopCoroutine(findRoutine);
    }

    void StartFindingPlayer()
    {
        playerCrouch = null;
        playerNetObj = null;

        if (findRoutine != null)
            StopCoroutine(findRoutine);

        findRoutine = StartCoroutine(FindLocalPlayer());
    }

    IEnumerator FindLocalPlayer()
    {
        while (true)
        {
            PlayerSwipeCrouch[] players = FindObjectsOfType<PlayerSwipeCrouch>();

            foreach (var p in players)
            {
                NetworkObject netObj = p.GetComponent<NetworkObject>();

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
        // 🔴 Player lost (restart / despawn) → rebind
        if (playerNetObj == null || !playerNetObj.IsValid)
        {
            StartFindingPlayer();
            return;
        }

        // Touch start
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject())
                return;

            startPos = Input.mousePosition;
            swipeActive = true;
        }

        // Swipe down
        if (Input.GetMouseButton(0) && swipeActive)
        {
            float deltaY = startPos.y - Input.mousePosition.y;

            if (deltaY > minSwipeDistance)
            {
                playerCrouch.StartCrouch();
            }
        }

        // Release
        if (Input.GetMouseButtonUp(0))
        {
            swipeActive = false;
            playerCrouch.StopCrouch();
        }
    }
}
