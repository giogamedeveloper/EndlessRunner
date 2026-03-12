using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Variables

    [Header("Game parameters")]
    public InputActionAsset actionAssets;


    [Header("Pause Menu")]
    [SerializeField]
    private CanvasGroup pauseMenu;

    public static GameManager Instance { get; private set; }

    [Header("References")]
    [SerializeField]
    public AudioMixer audioControl;

    public PlayerController player;

    [SerializeField]
    private bool isPaused = false;

    #endregion

    #region Unity Events

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;

        }
        Instance = this;

    }

    #endregion

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        if (HuDManager.Instance != null)
        {
            player.changeItems.AddListener(HuDManager.Instance.ChangeItems);
            //Initialize the interface.
            HuDManager.Instance.ShowMaxScores();
            HuDManager.Instance.ShowHUD(true);
            HuDManager.Instance.AddItemToInventory();
            HuDManager.Instance.AddItemsToHUD();
        }
        if (pauseMenu != null) pauseMenu.Active(false);
        pauseMenu.Active(false);
        ToggleInputs(true);
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        UpdateCollectableText();
        UpdateStepsText();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.started && !player.IsDead) PauseMenu(!isPaused);
    }

    #region Methods

    private void ToggleInputs(bool isPlayerInput)
    {
        if (isPlayerInput)
        {
            actionAssets.FindActionMap("Player").Enable();
            actionAssets.FindActionMap("UI").Disable();
        }
        else
        {
            actionAssets.FindActionMap("Player").Disable();
            actionAssets.FindActionMap("UI").Enable();
        }
    }

    private void UpdateCollectableText()
    {
        HuDManager.Instance.UpdateCollectableText(ScoreSystem.Instance.CollectableCount);
    }

    public void UpdateStepsText()
    {
        ScoreSystem.Instance.UpdateSteps(player.transform.position.x);
        if (ScoreSystem.Instance.StepsCount > 1000f)
        {
            player.acceleration = 12f;
            player.maxSpeed = 10f;
        }
        else if (ScoreSystem.Instance.StepsCount > 500f)
        {
            player.acceleration = 10f;
            player.maxSpeed = 8f;
        }

        HuDManager.Instance.UpdateStepsText(ScoreSystem.Instance.StepsCount);
    }

    public void PickUpCollectable(int value)
    {
        ScoreSystem.Instance.AddCollectable(value);
    }

    /// <summary>
    ///Execute endgame actions.
    /// </summary>
    public void EndGame()
    {
        ScoreSystem.Instance.SetEnemiesDefeated(player.EnemyCount);
        HuDManager.Instance.ShowEndGame(ScoreSystem.Instance.IsNewScoreRecord(), ScoreSystem.Instance.IsNewStepsRecord(),
            ScoreSystem.Instance.StepsCount, ScoreSystem.Instance.CollectableCount, ScoreSystem.Instance.EnemiesDefeated,
            ScoreSystem.Instance.Coins);

        ScoreSystem.Instance.SaveRecords();
        //We deactivate the HUD.
        HuDManager.Instance.ShowHUD(false);
        ToggleInputs(false);
        // MusicManager.Instance.PitchSlow();
    }

    /// <summary>
    ///Restart the game.
    /// </summary>
    public void Restart()
    {
        Time.timeScale = 1;
        //We reload the current scene.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        // MusicManager.Instance.PitchRegular();
    }

    /// <summary>
    ///Executes the pause action.
    /// </summary>
    /// <param name="isPause"></param>
    public void PauseMenu(bool isPause)
    {
        //We updated the pause status.
        isPaused = isPause;
        //We stop time by setting the time scale to 0.
        Time.timeScale = isPause ? 0 : 1;
        pauseMenu.Active(isPause);
        bool playerInput = !isPause && !player.IsDead;
        ToggleInputs(playerInput);
    }

    /// <summary>
    ///Changes the time scale according to the scale parameter.
    /// </summary>
    /// <param name="scale"></param>
    public void ChangeTimeScale(float scale)
    {
        Time.timeScale = scale;
    }

    #endregion
}
