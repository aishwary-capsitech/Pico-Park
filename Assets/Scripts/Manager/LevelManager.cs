using Fusion;
using UnityEngine;

public class LevelManager : NetworkBehaviour
{
    public static LevelManager Instance;

    [Header("Level Roots (Scene Objects)")]
    public GameObject trainingLevel;
    public GameObject level1;
    public GameObject level2;

    [Networked] public int level { get; set; } = 1;

    private int lastAppliedLevel = -1;
    private bool isTrainingMode => PlayerPrefs.GetInt("TrainingMode", 0) == 1;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        level1.SetActive(true);
        level2.SetActive(true);
        trainingLevel.SetActive(true);
    }

    public override void Spawned()
    {
        // Host decides initial level
        if (Object.HasStateAuthority)
        {
            if (isTrainingMode)
            {
                level = 0;
                Debug.Log("Level : " + level);
            }
            else
            {
                level = 1;
                Debug.Log("Level : " + level);
            }
        }
    }

    public override void Render()
    {
        if (level != lastAppliedLevel)
        {
            ApplyLevelState();
            lastAppliedLevel = level;
        }
    }
    public void IncreaseLevel()
    {
        if (!Object.HasStateAuthority) return;
        level++;
    }
    private void ApplyLevelState()
    {
        trainingLevel.SetActive(level == 0);
        level1.SetActive(level == 1);
        level2.SetActive(level == 2);

        Debug.Log($"[LevelManager] Level switched to {level}");
    }
}
