using UnityEngine;
using UnityEngine.EventSystems;

public class TagetingItemsShop : MonoBehaviour, IPointerEnterHandler
{
    private static TargetingItemShop _intance;
    public static TargetingItemShop Intance =>_intance;
    private new Renderer renderer;
    public int targetValue;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        //  if (_intance == null) _intance = this;
         Destroy(this);
    }
    void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log(gameObject.name);
        
    }
}
