using UnityEngine;

public class MovingRamp : MonoBehaviour
{
    public float moveDistance = 1.5f;   // total movement range
    public float moveSpeed = 3.0f;      // speed

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position; // center position
    }

    void Update()
    {
        // Value goes from -1 to +1 smoothly
        float offset = Mathf.Sin(Time.time * moveSpeed);

        // Move equally left and right
        transform.position = startPos + Vector3.right * offset * (moveDistance / 2f);
    }
}
