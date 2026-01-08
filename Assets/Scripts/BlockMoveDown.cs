using UnityEngine;

public class BoxMoveDownPlatform : MonoBehaviour
{
    public float moveSpeed = 4f;
    public float downDistance = 3f;

    private Vector3 startPos;
    private Vector3 downPos;
    private bool moveDown;

    void Awake()
    {
        startPos = transform.position;
        downPos = startPos + Vector3.down * downDistance;
    }

    void Update()
    {
        Vector3 target = moveDown ? downPos : startPos;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            moveSpeed * Time.deltaTime
        );
    }

    public void StartMoveDown()
    {
        moveDown = true;
    }

    public void StopMoveDown()
    {
        moveDown = false;
    }
}
