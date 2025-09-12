using UnityEngine;

public class Enemy : MonoBehaviour, ISpawneable
{
    bool ISpawneable.IsActive => gameObject.activeSelf;

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
