using UnityEngine;
using UnityEngine.Events;

public class LifePlayer : MonoBehaviour
{
    public int currentLife;
    public int maxLife;


    public UnityEvent<int, int> changeLife;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentLife = maxLife;
    }

    public int TakeDamage(int damage)
    {
        Debug.Log(currentLife);
        currentLife -= damage;
        changeLife.Invoke(currentLife, maxLife);
        return currentLife;
    }

    public int HealLife(int heal)
    {
        currentLife += heal;
        changeLife.Invoke(currentLife, maxLife);
        return currentLife;
    }

    public void UpdateMaxLife(int _maxLife)
    {
        maxLife += _maxLife;
        DataManager.Instance.SaveHp(maxLife);
        currentLife += _maxLife;
        changeLife.Invoke(currentLife, maxLife);
    }
}
