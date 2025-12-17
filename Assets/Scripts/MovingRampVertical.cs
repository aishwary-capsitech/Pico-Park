// using UnityEngine;

// public class MovingRampVertical: MonoBehaviour
// {
//     public float moveHeight = 3.2f;     // total travel range
//     public float moveSpeed = 2.5f;    // speed
//     public float baseOffset = 1f;     // minimum height above ground

//     private Vector3 startPos;

//     void Start()
//     {
//         startPos = transform.position;
//     }

//     void Update()
//     {
//         // Sin gives continuous movement (NO pause)
//         float yOffset = (Mathf.Sin(Time.time * moveSpeed) + 1f) / 2f;
//         // converts -1..1 → 0..1

//         float finalY = baseOffset + yOffset * moveHeight;

//         transform.position = new Vector3(
//             startPos.x,
//             finalY,
//             startPos.z
//         );
//     }
// }


using UnityEngine;

public class MovingRampVertical : MonoBehaviour
{
    public float moveHeight = 3.2f;
    public float moveSpeed = 3.0f;
    public float baseOffset = 0.5f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float yOffset =
            (Mathf.Sin(Time.time * moveSpeed) + 1f) / 2f;

        // 🔑 KEY FIX: include startPos.y
        float finalY =
            startPos.y + baseOffset + yOffset * moveHeight;

        transform.position = new Vector3(
            startPos.x,
            finalY,
            startPos.z
        );
    }
}
