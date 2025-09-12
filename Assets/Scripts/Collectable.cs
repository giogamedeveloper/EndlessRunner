using UnityEngine;

public enum CollectableType
{
    Collectable,
    PowerUP,
    PowerUpShield,
}

public class Collectable : MonoBehaviour, ISpawneable
{
    [Header("Configuration")]
    public int collectableValue = 1;

    public CollectableType type = CollectableType.Collectable;

    [Header("Feedback")]
    public Color flashColor = Color.white;

    public float flashTime = .4f;

    [SerializeField]
    private string sfxIndex = "pickup";

    [SerializeField]
    private Collider2D coll2D;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    public bool IsActive
    {
        get { return coll2D.enabled && spriteRenderer.enabled; }
    }


    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        if (coll2D == null) coll2D = GetComponent<Collider2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Method that verifies the collectible's contact with another collision.
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter2D(Collider2D other)
    {
        //If the collectible hits an object with the "Player" tag.
        if (other.gameObject.CompareTag("Player"))
        {
            //we deactivate the collectible.
            FeedBack(other.gameObject);
            Deactivate();
            ApplyEffect(other.gameObject);
        }
    }

    /// <summary>
    /// Method that manages the execution of the feedback of the collectible collection.
    /// </summary>
    /// <param name="other"></param>
    private void FeedBack(GameObject other)
    {
        AudioController.Instance.PlaySound(sfxIndex);
        if (other.TryGetComponent(out SpriteFlash spriteFlash)) spriteFlash.StartColorFlash(flashColor, flashTime);
    }

    /// <summary>
    /// Execute the activation of the collectible.
    /// </summary>
    public void Activate()
    {
        coll2D.enabled = true;
        spriteRenderer.enabled = true;
    }

    /// <summary>
    /// Execute the deactivation of the collectible.
    /// </summary>
    public void Deactivate()
    {
        coll2D.enabled = false;
        spriteRenderer.enabled = false;
    }

    /// <summary>
    /// Apply the consequences of picking up the collectible.
    /// </summary>
    private void ApplyEffect(GameObject player)
    {

        switch (type)
        {
            case CollectableType.Collectable:
                //We tell the Game Manager that the counter value is to be increased.
                GameManager.Instance.PickUpCollectable(collectableValue);
                break;
            case CollectableType.PowerUpShield:
                if (player != null && player.TryGetComponent(out PlayerController playerControllerShield))
                    playerControllerShield.ActivePowerUpShield();
                break;
            case CollectableType.PowerUP:
                if (player != null && player.TryGetComponent(out PlayerController playerController))
                    playerController.ActivePowerUp();
                break;
        }

    }
}
