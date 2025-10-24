using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{

    public struct ItemInventory
    {
        public Sprite Image;
        public int Quantity;
    }
    public ItemInventory[] itemInventory = new ItemInventory[5];
    //Maximum score recorded.
    public int maxScore = 0;
    public int enemies = 0;
    public int maxHp;
    public int coins;
    public float volume;
    public float maxStepsScore = 0f;

    private static DataManager _intance;
    public static DataManager Instance => _intance;
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if (_intance == null)
        {
            _intance = this;
            LoadScore();
            LoadSTeps();
            LoadEnemies();
            LoadCoins();
            LoadItems();
            LoadMusic();
            LoadHp();

        }
        else
        {
            Destroy(this);
        }
    }
    /// <summary>
    /// It will persistently store information in the PlayerPrefs.
    /// </summary>
    public void Save()
    {
        //Stores the information about the maximum number of coins obtained and the steps achieved in the playerprefs, in a space with the key associated with its name respectively.
        PlayerPrefs.SetInt("maxScore", maxScore);
        PlayerPrefs.SetFloat("maxStepsScore", maxStepsScore);
    }

    public void SaveHp(int maxHp)
    {
        PlayerPrefs.SetInt("life", maxHp);
    }
    public void SaveItem(int _index)
    {
        switch (_index)
        {
            case 0:
                itemInventory[_index].Quantity++;
                PlayerPrefs.SetInt("item 1", itemInventory[_index].Quantity);
                break;
            case 1:
                itemInventory[_index].Quantity++;
                PlayerPrefs.SetInt("item 2", itemInventory[_index].Quantity);
                break;
            case 2:
                itemInventory[_index].Quantity++;
                PlayerPrefs.SetInt("item 3", itemInventory[_index].Quantity);
                break;
            case 3:
                itemInventory[_index].Quantity++;
                PlayerPrefs.SetInt("item 4", itemInventory[_index].Quantity);
                break;
            case 4:
                PlayerPrefs.SetInt("item 5", 1);
                break;

        }
    }public void UpdateItem(int _index)
    {
        switch (_index)
        {
            case 0:
                itemInventory[_index].Quantity--;
                PlayerPrefs.SetInt("item 1", itemInventory[_index].Quantity);
                break;
            case 1:
                itemInventory[_index].Quantity--;
                PlayerPrefs.SetInt("item 2", itemInventory[_index].Quantity);
                break;
            case 2:
                itemInventory[_index].Quantity--;
                PlayerPrefs.SetInt("item 3", itemInventory[_index].Quantity);
                break;
            case 3:
                itemInventory[_index].Quantity--;
                PlayerPrefs.SetInt("item 4", itemInventory[_index].Quantity);
                break;
        }
    }
    public void SaveCoins(int _coins)
    {
        coins += _coins;
        PlayerPrefs.SetInt("coins", _coins);
    }
    /// <summary>
    /// Save the music for all scenes.
    /// </summary>
    /// <param name="volume"></param>
    public void SaveMusic(float volume)
    {
        PlayerPrefs.SetFloat("music", volume);
    }
    public void SaveEnemies(int enemy)
    {
        PlayerPrefs.SetInt("enemy", enemy);
    }
    public void LoadItems()
    {
        // for (int i = 1; i <= itemInventory.Length-1; i++)
        // {
        //     itemInventory[i].Quantity = PlayerPrefs.GetInt("item " + i.ToString());
        // }
        itemInventory[0].Quantity = PlayerPrefs.GetInt("item 1");
        itemInventory[1].Quantity = PlayerPrefs.GetInt("item 2");
        itemInventory[2].Quantity = PlayerPrefs.GetInt("item 3");
        itemInventory[3].Quantity = PlayerPrefs.GetInt("item 4");
        itemInventory[4].Quantity = PlayerPrefs.GetInt("item 5");

    }
    public void LoadCoins()
    {
        coins = PlayerPrefs.GetInt("coins");
    }
    public void LoadMusic()
    {
        volume= PlayerPrefs.GetFloat("music");
    }
    public void LoadHp()
    {
        maxHp = PlayerPrefs.GetInt("life");
    }

    // / <summary>
    // / Load high scores from PlayerPrefs.
    // / </summary>
    // / 
    public void LoadEnemies()
    {
        //We retrieve the value of the corresponding key from the PlayerPrefs.
        enemies = PlayerPrefs.GetInt("enemy");

    }
    public void LoadScore()
    {
        //We retrieve the value of the corresponding key from the PlayerPrefs.
        maxScore = PlayerPrefs.GetInt("maxScore");

    }
    public void LoadSTeps()
    {
        //We retrieve the value of the corresponding key from the PlayerPrefs.
        maxStepsScore = PlayerPrefs.GetFloat("maxStepsScore");

    }

    [ContextMenu("Clear Data")]
    /// <summary>
    /// Clear all persistent memory.
    /// </summary>
    public void ClearAllData()
    {
        PlayerPrefs.DeleteAll();
    }
}
