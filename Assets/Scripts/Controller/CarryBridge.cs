using UnityEngine;
using System.Collections.Generic;

public class CarryBridge : MonoBehaviour
{
    [SerializeField] private Transform leftEnd;
    [SerializeField] private Transform rightEnd;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float syncTimeWindow = 0.3f;
    
    private HashSet<Rigidbody2D> playersOnBar = new HashSet<Rigidbody2D>();
    private Dictionary<Rigidbody2D, float> landingTimes = new Dictionary<Rigidbody2D, float>();
    private bool isMoving = false;
    private Vector3 targetPosition;
    private bool isAtRight = false;
    
    void Start()
    {
        if (leftEnd != null)
            transform.position = leftEnd.position;
        targetPosition = transform.position;
    }
    
    void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
                isMoving = false;
        }
    }
    
    void OnCollisionEnter2D(Collision2D col)
    {
        Rigidbody2D rb = col.rigidbody;
        if (rb == null) return;
        
        landingTimes[rb] = Time.time;
        playersOnBar.Add(rb);
        
        CheckSynchronizedLanding();
    }
    
    void OnCollisionExit2D(Collision2D col)
    {
        Rigidbody2D rb = col.rigidbody;
        if (rb != null)
            playersOnBar.Remove(rb);
    }
    
    void CheckSynchronizedLanding()
    {
        playersOnBar.RemoveWhere(rb => rb == null);
        
        int playerCount = playersOnBar.Count;
        if (playerCount == 0) return;
        
        // Get landing times
        List<float> times = new List<float>();
        foreach (var rb in playersOnBar)
        {
            if (landingTimes.ContainsKey(rb))
                times.Add(landingTimes[rb]);
        }
        
        if (times.Count == 0) return;
        
        // Check synchronization
        float minTime = float.MaxValue;
        float maxTime = float.MinValue;
        
        foreach (float time in times)
        {
            if (time < minTime) minTime = time;
            if (time > maxTime) maxTime = time;
        }
        
        float timeDiff = maxTime - minTime;
        
        if (timeDiff <= syncTimeWindow)
        {
            // 2+ players: move to right end
            if (playerCount >= 2 && !isAtRight)
            {
                targetPosition = rightEnd.position;
                isMoving = true;
                isAtRight = true;
                Debug.Log($"{playerCount} players jumped together - Moving to right!");
            }
            // 1 player: move to left end
            else if (playerCount == 1 && isAtRight)
            {
                targetPosition = leftEnd.position;
                isMoving = true;
                isAtRight = false;
                Debug.Log("1 player jumped - Moving to left!");
            }
        }
    }
}