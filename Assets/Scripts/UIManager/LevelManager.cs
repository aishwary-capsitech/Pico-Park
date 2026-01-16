using Fusion;
using UnityEngine;

public class LevelManager : NetworkBehaviour
{
    public static LevelManager Instance;

    [Header("Level Roots (Scene Objects)")]
    public GameObject level1;
    public GameObject level2;

    [Networked] public int level { get; set; } = 1;

    private int lastAppliedLevel = -1;

    // -----------------------------------------------------

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        // IMPORTANT:
        // Do NOT disable levels here
        // Both must stay ACTIVE for Fusion
        level1.SetActive(true);
        level2.SetActive(true);
    }

    // -----------------------------------------------------

    public override void Spawned()
    {
        // Host decides initial level
        if (Object.HasStateAuthority)
        {
            level = 1;
        }
    }

    // -----------------------------------------------------

    public override void Render()
    {
        if (level != lastAppliedLevel)
        {
            ApplyLevelState();
            lastAppliedLevel = level;
        }
    }

    // -----------------------------------------------------

    public void IncreaseLevel()
    {
        if (!Object.HasStateAuthority) return;
        level++;
    }

    // -----------------------------------------------------
    // THIS DOES NOT TOUCH SPIKES OR HAZARDS
    // -----------------------------------------------------

    private void ApplyLevelState()
    {
        // ONLY toggle level ROOT visibility
        // (These roots MUST NOT contain NetworkObjects)

        level1.SetActive(level == 1);
        level2.SetActive(level == 2);

        Debug.Log($"[LevelManager] Level switched to {level}");
    }
}
