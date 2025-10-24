using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HeartsUI : MonoBehaviour
{
    public Image emptyHeart;
    public Image fillHeart;

    [SerializeField]
    private DataManager data;

    public int size = 75;

    public LifePlayer lifePlayer;
    RectTransform rectTransform;

    [SerializeField]
    private PlayerController player;


    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
            UpdateMaxHp(player.maxHp, player.maxHp);
    }

    public void changeLife(int current, int maxLife)
    {
        if (maxLife > rectTransform.sizeDelta.x / size || maxLife == 0) UpdateMaxHp(current, maxLife);
        if (current > fillHeart.fillAmount) LessHeart(current, maxLife);
        else AddHeart(current, maxLife);
    }

    private void UpdateMaxHp(int currentLife, int maxLife)
    {
        rectTransform.sizeDelta = new Vector2(maxLife * size, 65);
        fillHeart.fillAmount = 1 - (float)currentLife / (float)maxLife;
        data.SaveHp(maxLife);
    }

    private void LessHeart(int currentLife, int maxLife)
    {
        fillHeart.fillAmount = 1 - (float)currentLife / (float)maxLife;
    }

    private void AddHeart(int currentLife, int maxLife)
    {
        if (currentLife < maxLife)
            fillHeart.fillAmount = (float)currentLife / (float)maxLife;
    }
}
