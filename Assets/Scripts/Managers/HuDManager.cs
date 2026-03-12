using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("Inventario")]
    private int currentIndex;

    private int quantityItems;
    private List<Image> itemsIco;
    private List<Image> itemsImage;

    [HideInInspector]
    public List<TextMeshProUGUI> quantityText;

    public ItemInventory[] itemInventory = new ItemInventory[5];
    public Transform inventoryContent;
    public List<GameObject> ItemsObject;

    public struct ItemInventory
    {
        public Sprite Image;
        public int Quantity;
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // Inicializar listas
        itemsIco = new List<Image>();
        itemsImage = new List<Image>();
        quantityText = new List<TextMeshProUGUI>();
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

    public void AddItemsToHUD()
    {
        if (quantityItems > 0) CreateSlotsItems(quantityItems);
        else ChangeItems(quantityItems);
    }

    private void CreateSlotsItems(int quantityItems)
    {
        for (int i = 0; i < itemInventory.Length; i++)
        {
            if (inventoryContent == null) return;
            if (ItemsObject[i] == null) continue;

            GameObject itemObject = Instantiate(ItemsObject[i], inventoryContent);

            Image marcoImage = itemObject.transform.GetChild(0).GetComponent<Image>();
            Image iconoImage = itemObject.transform.GetChild(1).GetComponent<Image>();
            TextMeshProUGUI textComponent = itemObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            itemsIco.Add(marcoImage);
            itemsImage.Add(iconoImage);
            quantityText.Add(textComponent);

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

    public void ChangeItems(int quantityItems)
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

    public void AddItemToInventory()
    {
        DataManager.Instance.LoadItems();
        for (int i = 0; i < itemInventory.Length; i++)
        {
            itemInventory[i].Quantity = DataManager.Instance.itemInventory[i].Quantity;
            if (itemInventory[i].Quantity > 0) quantityItems++;
        }
    }
}
