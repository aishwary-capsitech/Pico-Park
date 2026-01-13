using Fusion;
using UnityEngine;

public class LevelManager : NetworkBehaviour
{
    public static LevelManager Instance;

    [Header("Levels")]
    public GameObject level1;
    public GameObject level2;

    [Networked] public int level { get; set; } = 1;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            level = 1;
            ApplyLevelState();
        }
    }

    // Called when level is completed
    public void IncreaseLevel()
    {
        if (!Object.HasStateAuthority)
            return;

        level++;

        ApplyLevelState();
    }

    // Handles enabling/disabling level GameObjects
    private void ApplyLevelState()
    {
        switch (level)
        {
            case 1:
                level1.SetActive(true);
                level2.SetActive(false);
                break;

            case 2:
                level1.SetActive(false);
                level2.SetActive(true);
                break;
        }

        Debug.Log($"Level switched to: {level}");
    }
}
