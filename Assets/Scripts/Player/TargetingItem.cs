using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TargetingItemShop : MonoBehaviour, IPointerClickHandler
{
    private new Renderer renderer;
    [SerializeField]
    public PlayerController player;
    public SectionManager section;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (player == null)
            player = FindFirstObjectByType<PlayerController>();
        section = FindFirstObjectByType<SectionManager>();
        renderer = GetComponent<Renderer>();
    }
/// <summary>
/// This event is for the use of each item, where it enters a switch that determines which item you want to apply.
/// </summary>
/// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        switch (gameObject.name)
        {
            case "IcoFlame(Clone)":
                if (HuDManager.Instance.itemInventory[0].Quantity > 0)
                {
                    HuDManager.Instance.itemInventory[0].Quantity--;
                    player.ActivePowerUpShield();
                }
                else HuDManager.Instance.UpdateItems(0);
                DataManager.Instance.UpdateItem(1);
                HuDManager.Instance.quantityText[0].text = HuDManager.Instance.itemInventory[0].Quantity.ToString();
                break;
            //activa escudo
            case "IcoWater(Clone)":
                if (HuDManager.Instance.itemInventory[1].Quantity > 0)
                {
                    HuDManager.Instance.itemInventory[1].Quantity--;
                    section.CheckEnemies();
                }
                else HuDManager.Instance.UpdateItems(1);
                DataManager.Instance.UpdateItem(1);
                HuDManager.Instance.quantityText[1].text = HuDManager.Instance.itemInventory[1].Quantity.ToString();
                break;
            //elimina todos los enemigos en la pantalla
            case "IcoBull(Clone)":
                if (HuDManager.Instance.itemInventory[2].Quantity > 0)
                {
                    HuDManager.Instance.itemInventory[2].Quantity--;
                    player.lifePlayer.HealLife(1);

                }
                else HuDManager.Instance.UpdateItems(2);
                DataManager.Instance.UpdateItem(2);
                HuDManager.Instance.quantityText[2].text = HuDManager.Instance.itemInventory[2].Quantity.ToString();
                break;
            case "IcoBible(Clone)":
                if (HuDManager.Instance.itemInventory[3].Quantity > 0)
                {
                    HuDManager.Instance.itemInventory[3].Quantity--;
                    player.lifePlayer.UpdateMaxLife(1);
                }
                else HuDManager.Instance.UpdateItems(3);
                DataManager.Instance.UpdateItem(3);
                HuDManager.Instance.quantityText[3].text = HuDManager.Instance.itemInventory[3].Quantity.ToString();
                break;
        }
    }
}
