//using Fusion;
//using UnityEngine;

//public class LevelManager : NetworkBehaviour
//{
//    public static LevelManager Instance;

//    [Header("Levels")]
//    public GameObject level1;
//    public GameObject level2;

//    [Networked] public int level { get; set; } = 1;

//    private void Awake()
//    {
//        if (Instance == null)
//            Instance = this;
//        else
//            Destroy(gameObject);
//    }

//    public override void Spawned()
//    {
//        if (Object.HasStateAuthority)
//        {
//            level = 1;
//            ApplyLevelState();
//        }
//    }

//    // Called when level is completed
//    public void IncreaseLevel()
//    {
//        if (!Object.HasStateAuthority)
//            return;

//        level++;

//        ApplyLevelState();
//    }

//    // Handles enabling/disabling level GameObjects
//    private void ApplyLevelState()
//    {
//        switch (level)
//        {
//            case 1:
//                level1.SetActive(true);
//                level2.SetActive(false);
//                break;

//            case 2:
//                level1.SetActive(false);
//                level2.SetActive(true);
//                break;
//        }

//        Debug.Log($"Level switched to: {level}");
//    }
//}



using Fusion;
using UnityEngine;

public class LevelManager : NetworkBehaviour
{
    public static LevelManager Instance;

    [Header("Levels")]
    public GameObject level1;
    public GameObject level2;

    [Header("Level 2 Hazards")]
    public GameObject[] level2Hazards;
    // spikes, pendulum, boulder, etc.

    [Networked] public int level { get; set; } = 1;

    private int lastAppliedLevel = -1;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        level1.SetActive(true);
        level2.SetActive(false);
    }

    public override void Render()
    {
        if (level != lastAppliedLevel)
        {
            ApplyLevelState();
            lastAppliedLevel = level;
        }
    }

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            level = 1;
        }
    }

    public void IncreaseLevel()
    {
        if (!Object.HasStateAuthority)
            return;

        level++;
    }

    private void ApplyLevelState()
    {
        // LEVEL OBJECTS
        level1.SetActive(level == 1);
        level2.SetActive(level == 2);

        // HAZARDS
        bool enableHazards = level == 2;

        foreach (var hazard in level2Hazards)
        {
            if (hazard != null)
                hazard.SetActive(enableHazards);
        }

        Debug.Log($"[ALL CLIENTS] Applied Level {level} state");
    }
}

