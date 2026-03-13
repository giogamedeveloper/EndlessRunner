using UnityEngine;

public class EnemyDead : MonoBehaviour, ISDamageable
{
    [SerializeField] private Collider2D coll2D;
    [SerializeField] private SpriteRenderer spriteRenderer;
    public Animator _animator;

    void Start()
    {
        if (coll2D == null) coll2D = GetComponent<Collider2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        _animator.SetBool("isDead", false);
    }

    public void Disappears()
    {
        coll2D.enabled = false;
        StartCoroutine(DisappearsRoutine());
    }

    private System.Collections.IEnumerator DisappearsRoutine()
    {
        yield return new WaitForSeconds(0.2f);
        KillEnemy();
    }

    public void KillEnemy()
    {
        _animator.SetBool("isDead", true);
        StartCoroutine(KillEnemyRoutine());
    }

    private System.Collections.IEnumerator KillEnemyRoutine()
    {
        yield return new WaitForSeconds(0.2f);
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;
    }
}
