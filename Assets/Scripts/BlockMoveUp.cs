// using UnityEngine;

// public class BlockMoveUp : MonoBehaviour
// {
//     public float moveSpeed = 4f;
//     public float upDistance = 4f;   // how much the block moves up

//     private Vector3 startPos;
//     private Vector3 upPos;
//     private bool moveUp = false;

//     void Awake()
//     {
//         startPos = transform.position;
//         upPos = startPos + Vector3.up * upDistance;
//     }

//     void Update()
//     {
//         if (!moveUp) return;

//         transform.position = Vector3.MoveTowards(
//             transform.position,
//             upPos,
//             moveSpeed * Time.deltaTime
//         );
//     }

//     // 🔼 Called by buzzer
//     public void StartMoveUp()
//     {
//         moveUp = true;
//     }
// }


using UnityEngine;

public class BlockMoveUp : MonoBehaviour
{
    public float moveSpeed = 4f;
    public float upDistance = 4f;   // how much the block moves up

    private Vector3 startPos;
    private Vector3 upPos;

    private bool moveUp = false;

    void Awake()
    {
        startPos = transform.position;
        upPos = startPos + Vector3.up * upDistance;
    }

    void Update()
    {
        Vector3 targetPos = moveUp ? upPos : startPos;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );
    }

    // 🔼 Called when player is on buzzer
    public void StartMoveUp()
    {
        moveUp = true;
    }

    // 🔽 Called when player leaves buzzer
    public void StopMoveUp()
    {
        moveUp = false;
    }
}
