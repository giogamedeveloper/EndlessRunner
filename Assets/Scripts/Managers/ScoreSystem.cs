using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    public static ScoreSystem Instance { get; private set; }

    private ScoreData _data;

    public int CollectableCount => _data.CollectableCount;
    public float StepsCount => _data.StepsCount;
    public int EnemiesDefeated => _data.EnemiesDefeated;
    public int TotalScore => _data.TotalScore;
    public float Coins => _data.Coins;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _data = new ScoreData();
    }

    public void AddCollectable(int value) => _data.AddCollectable(value);
    public void UpdateSteps(float playerPositionX) => _data.UpdateSteps(playerPositionX);
    public void SetEnemiesDefeated(int count) => _data.SetEnemiesDefeated(count);
    public bool IsNewStepsRecord() => _data.IsNewStepsRecord(DataManager.Instance.maxStepsScore);
    public bool IsNewScoreRecord() => _data.IsNewScoreRecord(DataManager.Instance.maxScore);

    public void SaveRecords()
    {
        if (IsNewScoreRecord())
        {
            DataManager.Instance.maxScore = TotalScore;
            DataManager.Instance.Save();
        }
        if (IsNewStepsRecord())
        {
            DataManager.Instance.maxStepsScore = StepsCount;
            DataManager.Instance.Save();
        }
        DataManager.Instance.SaveCoins((int)Coins);
    }
}
