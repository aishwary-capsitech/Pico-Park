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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Something touched buzzer: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Player"))
        {
            ContactPoint2D contact = collision.GetContact(0);

            if(contact.normal.y < -0.5f)
            {
                Debug.Log("Player landed on top of buzzer");
                spike.StopSpike();
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player left buzzer");
            spike.ResumeSpike();
        }
    }
}
