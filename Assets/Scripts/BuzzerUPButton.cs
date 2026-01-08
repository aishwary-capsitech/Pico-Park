// using UnityEngine;

// public class BuzzerUPButton : MonoBehaviour
// {
//     public BlockMoveUp targetBlock;
//     private bool pressed = false;

//     private void OnCollisionEnter2D(Collision2D collision)
//     {
//         if (pressed) return;

//         if (!collision.gameObject.CompareTag("Player"))
//             return;

//         pressed = true;
//         targetBlock.StartMoveUp();

//         Debug.Log("🔘 Upper buzzer pressed → block moving UP");
//     }

//     private void OnCollisionExit2D(Collision2D collision)
// {
//     if (!collision.gameObject.CompareTag("Player")) return;

//     targetBlock.StopMoveUp();
// }

// }

using UnityEngine;

public class BuzzerUPButton : MonoBehaviour
{
    public BlockMoveUp targetBlock;

    private int playersOnButton = 0; // supports multiple players safely

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        playersOnButton++;

        if (playersOnButton == 1) // first player presses
        {
            targetBlock.StartMoveUp();
            Debug.Log("🔘 Buzzer pressed → block moving UP");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        playersOnButton--;

        if (playersOnButton <= 0) // last player leaves
        {
            playersOnButton = 0;
            targetBlock.StopMoveUp();
            Debug.Log("🔘 Buzzer released → block moving DOWN");
        }
    }
}
