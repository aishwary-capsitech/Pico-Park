// using UnityEngine;

// public class MovingSpike : MonoBehaviour
// {
//     public float moveHeight = 1.5f;
//     public float moveSpeed = 2f;

//     private Vector3 startPos;
//     // Start is called once before the first execution of Update after the MonoBehaviour is created
//     void Start()
//     {
//         startPos = transform.position;
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         float offset = Mathf.Sin(Time.time * moveSpeed);

//         transform.position= startPos + Vector3.up * offset  * (moveHeight / 2f);
//     }
// }


using UnityEngine;

public class MovingSpike : MonoBehaviour
{
    public float moveHeight = 0.35f;
    public float moveSpeed = 0.4f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position; 
    }

    void Update()
    {
        float yOffset = Mathf.PingPong(Time.time * moveSpeed, moveHeight);

        transform.position = new Vector3(
            startPos.x,
            startPos.y + yOffset,
            startPos.z
        );
    }
}

