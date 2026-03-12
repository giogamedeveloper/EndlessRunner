using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Variables

    public struct ItemInventory
    {
        public Sprite Image;
        public int Quantity;
    }

    [Header("Tutorial")]
    public TutorialController tutorialController;

    public bool skipTutorial = false;

    [Header("Game parameters")]
    public InputActionAsset actionAssets;

    [Header("HUD")]
    public int currentIndex;

    public int quantityItems;
    public List<Image> itemsIco;
    public List<Image> itemsImage;

    public List<TextMeshProUGUI> quantityText;
    public ItemInventory[] itemInventory = new ItemInventory[5];
    public Transform inventoryContent;
    public List<GameObject> ItemsObject;

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
        player.changeItems.AddListener(changeItems);

    }

    #endregion

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        HuDManager.Instance.ShowMaxScores();
        //Initialize the interface.
        HuDManager.Instance.ShowHUD(true);
        pauseMenu.Active(false);
        ToggleInputs(true);
        AddItemToInventory();
        AddItemsToHUD();
        if (player != null && player.IsPlayingTuto && !skipTutorial)
        {
            InitializeTutorial();
        }
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        UpdateCollectableText();
        UpdateStepsText();
    }

    private void InitializeTutorial()
    {
        if (tutorialController == null)
            tutorialController = FindObjectOfType<TutorialController>();

        if (tutorialController != null)
        {
            tutorialController.OnTutorialComplete.AddListener(OnTutorialComplete);
            // Pausar el juego durante tutorial
            ChangeTimeScale(0.5f); // Slow motion para tutorial
        }
    }

    private void OnTutorialComplete()
    {
        // Reanudar juego normal
        ChangeTimeScale(1f);

        if (player != null)
        {
            player.SetTutorialMode(false);
            player.isAutoMove = true;
        }

        Debug.Log("Tutorial completado - Juego normal iniciado");
    }

// Agrega método para skipear tutorial desde UI
    public void SkipTutorial()
    {
        if (tutorialController != null)
            tutorialController.SkipTutorial();

        OnTutorialComplete();
    }

    public void EnablePlayerControls(bool enable)
    {
        if (player != null)
            player.EnableControls(enable);
    }

    public void SetPlayerTutorialMode(bool tutorialMode)
    {
        if (player != null)
            player.SetTutorialMode(tutorialMode);
    }

    public void ResumeAfterTutorialMessage()
    {
        // Método que puedes llamar desde OnTypingComplete
        ChangeTimeScale(1f);
    }

    private void AddItemsToHUD()
    {
        if (quantityItems > 0) createSlotsItems(quantityItems);
        else changeItems(quantityItems);
    }

    private void createSlotsItems(int quantityItems)
    {
        for (int i = 0; i < quantityItems; i++)
        {
            if (inventoryContent == null) return;
            if (ItemsObject[i] == null) continue;

            GameObject itemObject = Instantiate(ItemsObject[i], inventoryContent);

            // El objeto instanciado es el MARCO

            // Los hijos son Text e Image
            Image marcoImage = itemObject.transform.GetChild(0).GetComponent<Image>();
            Image iconoImage = itemObject.transform.GetChild(1).GetComponent<Image>();
            TextMeshProUGUI textComponent = itemObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            Debug.Log(textComponent);
            // Añadir a las listas
            itemsIco.Add(marcoImage); // MARCO
            itemsImage.Add(iconoImage); // ICONO
            quantityText.Add(textComponent); // TEXTO

            // Configurar
            if (itemInventory[i].Quantity == 0)
            {
                marcoImage.enabled = false;
                textComponent.enabled = false;
                iconoImage.enabled = false;
            }
            else
            {
                textComponent.text = itemInventory[i].Quantity.ToString();
            }
        }
        currentIndex = quantityItems;
    }

    private void changeItems(int quantityItems)
    {
        if (quantityItems < currentIndex) QuitItem(quantityItems);
        else AddItem(quantityItems);
    }

    public void QuitItem(int quantityItems)
    {
        for (int i = 0; i < quantityItems; i--)
        {
            currentIndex = i;
            itemsIco[currentIndex].sprite = itemsImage[currentIndex].sprite;
            quantityText[currentIndex].text = itemInventory[currentIndex].Quantity.ToString();
        }
    }

    public void UpdateItems(int index)
    {
        itemsIco[index].enabled = false;
        quantityText[index].enabled = false;
    }

    private void AddItem(int quantity)
    {
        if (itemsIco.Count < 1) return;
        for (int i = 0; i <= quantity; i++)
        {
            currentIndex = i;
            itemsIco[currentIndex].sprite = itemsImage[currentIndex].sprite;
            quantityText[currentIndex].text = itemInventory[currentIndex].Quantity.ToString();
        }
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

    public void AddItemToInventory()
    {
        DataManager.Instance.LoadItems();
        for (int i = 0; i < itemInventory.Length; i++)
        {
            itemInventory[i].Quantity = DataManager.Instance.itemInventory[i].Quantity;
            if (itemInventory[i].Quantity > 0) quantityItems++;
        }
    }

    #endregion
}
