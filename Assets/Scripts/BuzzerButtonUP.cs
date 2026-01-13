using UnityEngine;

public class BuzzerButtonUP : MonoBehaviour
{
    public BoxLiftPlatform targetBox;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        ContactPoint2D contact = collision.GetContact(0);

        // Player landed ON TOP
        if (contact.normal.y < -0.5f)
        {
            targetBox.StartLift();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        ContactPoint2D contact = collision.GetContact(0);

        // Keep lifting ONLY while standing on top
        if (contact.normal.y < -0.5f)
        {
            targetBox.StartLift();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        // Player left -> STOP immediately
        targetBox.StopLift();
    }
}
