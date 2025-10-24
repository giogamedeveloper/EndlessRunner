using UnityEngine;
using System.Collections;

public class PlayerFeedback : MonoBehaviour
{
    [Header("Trail Reference")]
    public TrailRenderer trailRenderer;

    // Eliminamos las referencias individuales de partículas porque usamos el ParticleManager

    private PlayerController playerController;
    private bool wasGrounded;
    
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        wasGrounded = playerController.isGrounded;
        
        // Validar que el ParticleManager existe
        if (ParticleManager.Instance == null)
        {
            Debug.LogError("ParticleManager no encontrado. Asegúrate de tenerlo en la escena.");
        }
    }

    void Update()
    {
        HandleRunParticles();
        HandleLandingFeedback();
        wasGrounded = playerController.isGrounded;
    }

    public void PlayJumpFeedback()
    {
        // Usar ParticleManager para partículas de salto
        ParticleManager.Instance.PlayEffect("Jump", transform.position);
        
        if (trailRenderer != null) trailRenderer.emitting = true;
        AudioController.Instance.PlaySound("Jump");
        CameraShake.Instance.Shake(0.1f, 0.05f);
    }

    public void PlayEnemyKillFeedback(Vector3 position)
    {
        // Usar ParticleManager para partículas de muerte enemiga
        ParticleManager.Instance.PlayEffect("EnemyDeath", position, Color.red);
        
        AudioController.Instance.PlaySound("EnemyKill");
        CameraShake.Instance.Shake(0.15f, 0.1f);
    }

    public void PlayDamageFeedback(Vector3 position)
    {
        // Usar ParticleManager para partículas de daño
        ParticleManager.Instance.PlayEffect("Damage", position, Color.red);
        
        AudioController.Instance.PlaySound("Damage");
        CameraShake.Instance.Shake(0.2f, 0.2f);

        // Flash del sprite
        StartCoroutine(FlashSprite());
    }

    private IEnumerator FlashSprite()
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            Color original = sprite.color;
            sprite.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            sprite.color = original;
        }
    }

    private void HandleRunParticles()
    {
        if (playerController != null && playerController.isGrounded)
        {
            Rigidbody2D rb = playerController.GetComponent<Rigidbody2D>();
            if (rb != null && Mathf.Abs(rb.linearVelocityX) > 1f)
            {
                // Partículas de correr continuas
                ParticleManager.Instance.PlayEffect("Run", 
                    transform.position + new Vector3(-0.5f, -0.5f, 0));
            }
        }
    }

    private void HandleLandingFeedback()
    {
        if (playerController != null && playerController.isGrounded && !wasGrounded)
        {
            // Partículas de aterrizaje
            ParticleManager.Instance.PlayEffect("Land", transform.position);
            
            if (trailRenderer != null) trailRenderer.emitting = false;
            AudioController.Instance.PlaySound("Land");
            // CameraShake.Instance.Shake(0.1f, 0.05f);
        }
    }
}