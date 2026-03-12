using System;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    public static ScoreSystem Instance { get; private set; }

    private int collectableCount = 0;
    private float stepsCount = 0f;
    private int enemiesDefeated = 0;

    public int CollectableCount => collectableCount;
    public float StepsCount => stepsCount;
    public int EnemiesDefeated => enemiesDefeated;
    public int TotalScore => (int)MathF.Round(stepsCount) 
                             + collectableCount 
                             + enemiesDefeated;
    public float Coins => TotalScore / 3f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddCollectable(int value)
    {
        collectableCount += value;
        OnScoreChanged?.Invoke();
    }

    public void UpdateSteps(float playerPositionX)
    {
        stepsCount = MathF.Round(playerPositionX + 3);
        OnStepsChanged?.Invoke(stepsCount);
    }

    public void SetEnemiesDefeated(int count)
    {
        enemiesDefeated = count;
    }

    public bool IsNewStepsRecord()
        => stepsCount > DataManager.Instance.maxStepsScore;

    public bool IsNewScoreRecord()
        => TotalScore > DataManager.Instance.maxScore;

    public void SaveRecords()
    {
        if (IsNewScoreRecord())
        {
            DataManager.Instance.maxScore = TotalScore;
            DataManager.Instance.Save();
        }
        if (IsNewStepsRecord())
        {
            DataManager.Instance.maxStepsScore = stepsCount;
            DataManager.Instance.Save();
        }
        DataManager.Instance.SaveCoins((int)Coins);
    }

    // Eventos para notificar a la UI sin conocerla
    public event System.Action OnScoreChanged;
    public event System.Action<float> OnStepsChanged;
}
