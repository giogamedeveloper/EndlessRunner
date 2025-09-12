using TMPro;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI coinsText;
    [SerializeField]
    private int coins;
    private static Game _instance;
    public static Game Instance => _instance;
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if (_instance == null) _instance = this;
        else Destroy(this);

    }
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        coinsText.text = DataManager.Instance.coins.ToString();
        coins = DataManager.Instance.coins;

    }

    /// <summary>
    ///UseCoins -> Manage the currencies you are using by updating as you purchase.
    /// </summary>
    /// <param name="amount"></param>
    public void UseCoins(int amount)
    {
        coins -= amount;
        coinsText.text = coins.ToString();
        DataManager.Instance.SaveCoins(coins);
    }
    /// <summary>
    ///HasEnoughtCoins ->It is a boolean method that returns true/false depending on the expense you want to make;
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    public bool HasEnoughtCoins(int amount)
    {
        return (coins >= amount);
    }
}
