using Fusion;
using UnityEngine;

public class BackgroundParallax : NetworkBehaviour
{
    private float startPos, length;
    public Camera cam;
    public float parallaxSpeed;

    void Start()
    {
        startPos = cam.transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void FixedUpdate()
    {
        float distance = cam.transform.position.x * parallaxSpeed;
        float movement = cam.transform.position.x * (1 - parallaxSpeed);

        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);

        if (movement > startPos + length)
        {
            startPos += length;
        }
        else if (movement < startPos - length)
        {
            startPos -= length;
        }
    }
}
