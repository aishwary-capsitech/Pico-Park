using UnityEngine;

public class BuzzerButtonDOWN : MonoBehaviour
{
    public BlockMoveDown targetBox;

    private int playerCount = 0;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        ContactPoint2D contact = collision.GetContact(0);

        // Player landed ON TOP of buzzer
        if (contact.normal.y < -0.5f)
        {
            playerCount++;
            targetBox.StartMoveDown();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        ContactPoint2D contact = collision.GetContact(0);

        // Keep moving down ONLY while standing on top
        if (contact.normal.y < -0.5f)
        {
            targetBox.StartMoveDown();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        playerCount--;

        if (playerCount <= 0)
        {
            playerCount = 0;
            targetBox.StopMoveDown();
        }
    }
}
