// using UnityEngine;

// public class PlayerSwipeCrouch : MonoBehaviour
// {
//      [Header("Crouch Size")]
//     public Vector3 crouchScale = new Vector3(1f, 0.5f, 1f);

//     private Vector3 normalScale;
//     private bool isCrouching = false;

//     void Awake()
//     {
//         normalScale = transform.localScale;
//     }

//     public void StartCrouch()
//     {
//         if (isCrouching) return;

//         isCrouching = true;
//         transform.localScale = crouchScale;
//     }

//     public void StopCrouch()
//     {
//         if (!isCrouching) return;

//         isCrouching = false;
//         transform.localScale = normalScale;
//     }
// }

using UnityEngine;

public class PlayerSwipeCrouch : MonoBehaviour
{
    [Header("Crouch Size")]
    public Vector3 crouchScale = new Vector3(1f, 0.5f, 1f);

    private Vector3 normalScale;
    private bool isCrouching = false;

    void Awake()
    {
        normalScale = transform.localScale;
    }

    public void StartCrouch()
    {
        if (isCrouching) return;

        isCrouching = true;

        // ✅ PRESERVE CURRENT FACE DIRECTION (X)
        transform.localScale = new Vector3(
            transform.localScale.x,   // keep left/right
            crouchScale.y,            // crouch height
            transform.localScale.z
        );
    }

    public void StopCrouch()
    {
        if (!isCrouching) return;

        isCrouching = false;

        // ✅ RESTORE HEIGHT, KEEP DIRECTION
        transform.localScale = new Vector3(
            transform.localScale.x,   // keep direction
            normalScale.y,
            transform.localScale.z
        );
    }
}
