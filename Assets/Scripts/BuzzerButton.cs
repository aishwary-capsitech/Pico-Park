using UnityEngine;

public class BuzzerButton : MonoBehaviour
{
    public MovingSpike spike;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            spike.StopSpike();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            spike.ResumeSpike();
        }
    }
}
