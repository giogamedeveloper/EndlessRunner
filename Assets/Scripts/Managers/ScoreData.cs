using System;

public class ScoreData
{
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

    public event Action OnScoreChanged;
    public event Action<float> OnStepsChanged;

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

    public bool IsNewStepsRecord(float maxStepsScore)
        => stepsCount > maxStepsScore;

    public bool IsNewScoreRecord(int maxScore)
        => TotalScore > maxScore;
}
