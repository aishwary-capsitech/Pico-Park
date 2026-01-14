
// using Fusion;
// using UnityEngine;

// public class LevelManager : NetworkBehaviour
// {
//     public static LevelManager Instance;

//     [Header("Levels")]
//     public GameObject level1;
//     public GameObject level2;

//     [Header("Level 2 Hazards")]
//     public GameObject[] level2Hazards;
//     // spikes, pendulum, boulder, etc.

//     [Networked] public int level { get; set; } = 1;

//     private int lastAppliedLevel = -1;

//     private void Awake()
//     {
//         if (Instance == null)
//             Instance = this;
//         else
//             Destroy(gameObject);

//         level1.SetActive(true);
//         level2.SetActive(false);
//     }

//     public override void Render()
//     {
//         if (level != lastAppliedLevel)
//         {
//             ApplyLevelState();
//             lastAppliedLevel = level;
//         }
//     }

//     public override void Spawned()
//     {
//         if (Object.HasStateAuthority)
//         {
//             level = 1;
//         }
//     }

//     public void IncreaseLevel()
//     {
//         if (!Object.HasStateAuthority)
//             return;

//         level++;
//     }

//     private void ApplyLevelState()
//     {
//         // LEVEL OBJECTS
//         level1.SetActive(level == 1);
//         level2.SetActive(level == 2);

//         // HAZARDS
//         bool enableHazards = level == 2;

//         foreach (var hazard in level2Hazards)
//         {
//             if (hazard != null)
//                 hazard.SetActive(enableHazards);
//         }

//         Debug.Log($"[ALL CLIENTS] Applied Level {level} state");
//     }
// }


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
