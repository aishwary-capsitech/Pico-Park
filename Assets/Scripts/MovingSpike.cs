// using UnityEngine;

// public class MovingSpike : MonoBehaviour
// {
//     public float moveHeight = 0.35f;
//     public float moveSpeed = 0.4f;

//     private Vector3 startPos;

//     void Start()
//     {
//         startPos = transform.position; 
//     }

//     void Update()
//     {
//         float yOffset = Mathf.PingPong(Time.time * moveSpeed, moveHeight);

//         transform.position = new Vector3(
//             startPos.x,
//             startPos.y + yOffset,
//             startPos.z
//         );
//     }
// }


using UnityEngine;

public class MovingSpike : MonoBehaviour
{
    public float moveHeight = 0.35f;
    public float moveSpeed = 0.4f;

    public bool canMove = true;

    private Vector3 startPos;
    private float timeOffset;

    void Start()
    {
        startPos = transform.position;
        timeOffset = Time.time;
    }

    void Update()
    {
        if (!canMove) return;

        float yOffset = Mathf.PingPong((Time.time - timeOffset) * moveSpeed, moveHeight);

        transform.position = new Vector3(
            startPos.x,
            startPos.y + yOffset,
            startPos.z
        );
    }

    // 🔔 Called when buzzer pressed
    public void StopSpike()
    {
        canMove = false;
        transform.position = startPos;   // keep spike inside ground
    }

    // 🔔 Called when buzzer released
    public void ResumeSpike()
    {
        timeOffset = Time.time;
        canMove = true;
    }
}
