// using UnityEngine;

// public class BuzzerButton : MonoBehaviour
// {
//     public MovingSpike spike;

//     private void OnTriggerEnter2D(Collider2D other)
//     {
//         if (other.CompareTag("Player"))
//         {
//             spike.StopSpike();
//         }
//     }

//     private void OnTriggerExit2D(Collider2D other)
//     {
//         if (other.CompareTag("Player"))
//         {
//             spike.ResumeSpike();
//         }
//     }
// }


using UnityEngine;

public class BuzzerButton : MonoBehaviour
{
    public MovingSpike spike;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Something touched buzzer: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player stepped on buzzer");
            spike.StopSpike();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player left buzzer");
            spike.ResumeSpike();
        }
    }
}
