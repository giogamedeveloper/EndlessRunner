using System;
using TMPro;
using UnityEngine;

public class HuDManager : MonoBehaviour
{
    public static HuDManager Instance { get; private set; }

    [Header("HUD")]
    [SerializeField] private CanvasGroup hud;

    [Header("Textos en partida")]
    [SerializeField] private TextMeshProUGUI collectableText;

    [SerializeField] private TextMeshProUGUI stepsText;

    [Header("Menú fin de partida")]
    [SerializeField] private CanvasGroup endGameMenu;

    [SerializeField] private TextMeshProUGUI finalStepsScoreText;
    [SerializeField] private TextMeshProUGUI totalPickUpText;
    [SerializeField] private TextMeshProUGUI enemiesKilledText;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI newStepsRecordText;
    [SerializeField] private TextMeshProUGUI newFinalRecordText;
    [SerializeField] private TextMeshProUGUI maxStepsScoreText;
    [SerializeField] private TextMeshProUGUI maxScoreText;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        endGameMenu.Active(false);
    }

    public void UpdateCollectableText(int value)
    {
        collectableText.text = value.ToString("00");
    }

    public void UpdateStepsText(float value)
    {
        stepsText.text = value.ToString("00");
    }

    public void ShowHUD(bool show)
    {
        hud.Active(show);
    }

    public void ShowEndGame(bool isNewScoreRecord,
        bool isNewStepsRecord,
        float steps,
        int collectables,
        int enemies,
        float coins)
    {
        newFinalRecordText.enabled = isNewScoreRecord;
        newStepsRecordText.enabled = isNewStepsRecord;
        finalStepsScoreText.text = MathF.Round(steps).ToString();
        totalPickUpText.text = collectables.ToString("00");
        enemiesKilledText.text = enemies.ToString("00");
        coinsText.text = coins.ToString("00");
        endGameMenu.Active(true);
    }

    public void ShowMaxScores()
    {
        if (maxScoreText != null)
            maxScoreText.text = DataManager.Instance.maxScore.ToString();
        if (maxStepsScoreText != null)
            maxStepsScoreText.text = DataManager.Instance.maxStepsScore.ToString();
    }
}
