using System.Threading.Tasks;
using UnityEngine;

public class EnemyDead : MonoBehaviour, ISDamageable
{
    [SerializeField]
    private Collider2D coll2D;

    

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    public Animator _animator;


    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        if (coll2D == null) coll2D = GetComponent<Collider2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        _animator.SetBool("isDead", false);
    }

    //After the enemy's death animation event is over, this method is invoked.
    public async void Disappears()
    {

        coll2D.enabled = false;
        await Task.Delay(200);
        KillEnemy();

    }



    /// <summary>
    /// This method is called from the player's controller to indicate that an enemy has been eliminated.
    /// </summary>
    public async void KillEnemy()
    {
        _animator.SetBool("isDead", true);
        await Task.Delay(200);
        spriteRenderer.enabled = false;

    }
}
