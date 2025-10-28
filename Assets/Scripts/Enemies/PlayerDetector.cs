using System;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    [Header("Configuración Raycast")]
    public float detectionDistance = 5f;

    [SerializeField]
    EnemyDead _enemyDead;

    public LayerMask playerLayer;
    public Vector2 rayDirection = Vector2.right;
    bool isExploted = false;
    [SerializeField] Transform enemyTransform;
    Transform currentTransform;
    float _timer;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField] float autoSize = 0.001f;

    void Start()
    {
        _timer = 0;
        isExploted = false;
        if (enemyTransform != null)
            currentTransform = enemyTransform;
    }

    private void Update()
    {
        DetectPlayer();
        if (isExploted) Explote();
    }

    private void DetectPlayer()
    {
        // Lanzar el raycast
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            rayDirection,
            detectionDistance,
            playerLayer
        );

        // Dibujar el raycast en el editor (solo para debug)
        Debug.DrawRay(transform.position, rayDirection * detectionDistance, Color.red);

        // Verificar si golpeó al jugador
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Player"))
            {
                isExploted = true;
            }
        }
    }

    public void Explote()
    {

        if (enemyTransform != null && _timer < 1f)
        {
            _timer += Time.deltaTime;
            enemyTransform.localScale += new Vector3(autoSize, autoSize, autoSize);
            enemyTransform.position += new Vector3(0f, autoSize, 0f);
            spriteRenderer.color += new Color(0f, -autoSize, -autoSize);
        }
        else if (_timer != null && _timer > 1f)
        {
            _timer = 0f;
            enemyTransform = currentTransform;
            _enemyDead.Disappears();
        }
    }
}
