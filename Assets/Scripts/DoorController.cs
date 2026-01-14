using UnityEngine;
using Fusion;

public class DoorController : NetworkBehaviour
{
    public static DoorController Instance;

    public GameObject doorBlock;
    public GameObject doorOpen;
    public GameObject finishArea;

    private void Awake()
    {
        Instance = this;
        finishArea.SetActive(false);
        doorOpen.SetActive(false);
    }

    public void OpenDoor()
    {
        if (!Object.HasStateAuthority) return;

        doorBlock.SetActive(false);
        doorOpen.SetActive(true);
        finishArea.SetActive(true);
    }
}
