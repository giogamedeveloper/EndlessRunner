using UnityEngine;

public class RandomObjectSpawnPoint : MonoBehaviour
{
    public GameObject prefab;
    [Range(0, 1)]
    public float spawnRatio = 1f;

    private ISpawneable spawneable;
    public ISpawneable Spawneable
    {
        get { return spawneable;}
    }

    /// <summary>
    /// Called when the script is loaded or a value is changed in the
    /// inspector (Called in the editor only).
    /// </summary>
    void OnValidate()
    {
        if (prefab != null && !prefab.TryGetComponent(out ISpawneable temp))
        {
            prefab = null;
        }
    }
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        // If there is no referenced prefab we show a warning in the console and stop the process.
        if (!prefab)
        {
            Debug.LogWarning("No collectible to be instantiated has been specified");
            return;
        }
        GameObject gameObject = Instantiate(prefab, transform.position, Quaternion.identity, transform);
        spawneable = gameObject.GetComponent<ISpawneable>();
    }
    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    private void OnEnable()
    {
        //If the roll result is between 0.0 and 1.0, it is within the spawn range.
        if (Random.value <= spawnRatio) spawneable.Activate();
        else spawneable.Deactivate();
    }
}
