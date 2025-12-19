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

    public void StopSpike()
    {
        canMove = false;
        transform.position = startPos; // keep spike hidden
    }

    public void ResumeSpike()
    {
        timeOffset = Time.time;
        canMove = true;
    }
}