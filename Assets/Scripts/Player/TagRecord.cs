using UnityEngine;

public class TagRecord : MonoBehaviour
{
    public float maxDistance;
    [SerializeField]
    private GameObject tagRecord;
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        //It is used to mark the greatest distance traveled greater than 1.
        maxDistance = PlayerPrefs.GetInt("maxScore");
        if (maxDistance > 1)
        {
            tagRecord.transform.position = new Vector2(maxDistance, 0f);
        }
    }
}
