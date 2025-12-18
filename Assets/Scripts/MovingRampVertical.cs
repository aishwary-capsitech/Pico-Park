using UnityEngine;

public class MovingRampVertical: MonoBehaviour
{
    public float moveHeight = 3.5f;
    public float moveSpeed = 2.5f;
    public float baseOffset = 0.1f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Sin gives continuous movement (NO pause)
        float yOffset = (Mathf.Sin(Time.time * moveSpeed) + 1f) / 2f;
        // converts -1..1 → 0..1

        float finalY = baseOffset + yOffset * moveHeight;

        transform.position = new Vector3(
            startPos.x,
            finalY,
            startPos.z
        );
    }
}
