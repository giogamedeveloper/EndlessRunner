using UnityEngine;

public class TargetingItemShop : MonoBehaviour
{
    [SerializeField] public PlayerController player;
    public SectionManager section;

    void Start()
    {
        if (player == null)
            player = FindFirstObjectByType<PlayerController>();
        section = FindFirstObjectByType<SectionManager>();
    }

    public void UseItem(int index)
    {
        if (HuDManager.Instance.itemInventory[index].Quantity <= 0) return;

        HuDManager.Instance.itemInventory[index].Quantity--;
        DataManager.Instance.UpdateItem(index);
        HuDManager.Instance.quantityText[index].text = 
            HuDManager.Instance.itemInventory[index].Quantity.ToString();

        if (HuDManager.Instance.itemInventory[index].Quantity == 0)
            HuDManager.Instance.UpdateItems(index);

        switch (index)
        {
            case 0: player.ActivePowerUpShield(); break;
            case 1: section.CheckEnemies(); break;
            case 2: player.lifePlayer.HealLife(1); break;
            case 3: player.lifePlayer.UpdateMaxLife(1); break;
        }
    }
}
