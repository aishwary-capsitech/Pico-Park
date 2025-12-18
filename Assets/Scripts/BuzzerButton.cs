using UnityEngine;

public class BuzzerButton : MonoBehaviour
{
    public MovingSpike spike;
    private bool pressed = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (pressed) return;

        if (other.CompareTag("Player"))
        {
            pressed = true;
            spike.StopSpike();
        }
    }
}
