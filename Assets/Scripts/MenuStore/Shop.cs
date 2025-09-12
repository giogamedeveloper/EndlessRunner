using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Shop : MonoBehaviour, IPointerEnterHandler
{
    [System.Serializable]
    public class ShopItem
    {
        public Sprite image;
        public int price;
        public bool isPurchased = false;
        public TextMeshProUGUI description;
    }
    [SerializeField]
    public List<ShopItem> shopItemsList;
    public List<TextMeshProUGUI> itemsDescription;
    public GameObject description;

    [SerializeField]
    private Animator _animator;
    [SerializeField]
    GameObject itemTemplate;
    GameObject g;
    [SerializeField]
    public Transform shopScrollView;
    Button buyButton;

    public List<ShopItem> Inventory => shopItemsList;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        itemTemplate = shopScrollView.GetChild(0).gameObject;
        for (int i = 0; i < shopItemsList.Count; i++)
        {
            g = Instantiate(itemTemplate, shopScrollView);
            g.transform.GetChild(0).GetComponent<Image>().sprite = shopItemsList[i].image;
            g.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = shopItemsList[i].price.ToString();
            g.transform.GetChild(2).GetComponent<Button>().interactable = !shopItemsList[i].isPurchased;
            // g.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = shopItemsList[i].description.ToString();
            buyButton = g.transform.GetChild(2).GetComponent<Button>();
            DataManager.Instance.LoadItems();

            buyButton.interactable = !shopItemsList[i].isPurchased;
            buyButton.AddEventListener(i, OnShopItemBtnClicked);
            if (i == shopItemsList.Count - 1 && DataManager.Instance.itemInventory[4].Quantity == 1)
                buyButton.interactable = false;
        }
        Destroy(itemTemplate);

        // CheckDescription();
        // itemsDescription = new GameObject[description.transform.childCount];
    }
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {

    }
    private void CheckDescription()
    {
        description = GameObject.FindGameObjectWithTag("Description");
        itemsDescription.Add(description.GetComponent<TextMeshProUGUI>());
    }

    /// <summary>
    ///OnShopItemBtnClicked ->Method used once the purchase is made and the corresponding check is made to see if it can be carried out correctly; if not, it shows you a text.
    /// </summary>
    /// <param name="itemIndex"></param>
    public void OnShopItemBtnClicked(int itemIndex)
    {
        if (Game.Instance.HasEnoughtCoins(shopItemsList[itemIndex].price))
        {
            Game.Instance.UseCoins(shopItemsList[itemIndex].price);
            //The button was pressed.
            shopItemsList[itemIndex].isPurchased = true;
            //Disable button.
            buyButton = shopScrollView.GetChild(itemIndex).GetChild(2).GetComponent<Button>();
            if (itemIndex == shopItemsList.Count - 1)
            {
                buyButton.interactable = false;
            }
            DataManager.Instance.SaveItem(itemIndex);
            //When you buy it, it will say that it is out of stock, this will be implemented for a specific item.
            //buyButton.transform.GetChild (0).GetComponent<TextMeshProUGUI>().text = "PURCHASED" ;
        }
        else
        {
            _animator.SetTrigger("NoCoins");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }
}
