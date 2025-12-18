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

    public bool canMove = true;   // 🔴 added

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position; 
    }

    void Update()
    {
        if (!canMove) return;   // 🔴 added

        float yOffset = Mathf.PingPong(Time.time * moveSpeed, moveHeight);

        transform.position = new Vector3(
            startPos.x,
            startPos.y + yOffset,
            startPos.z
        );
    }

    // 🔔 called by buzzer
    public void StopSpike()
    {
        canMove = false;
    }
}
