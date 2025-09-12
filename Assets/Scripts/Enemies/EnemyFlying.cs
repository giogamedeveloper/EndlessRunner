using System.Collections;
using UnityEngine;

public class EnemyFlying : MonoBehaviour
{
    #region Variables
    [Header("References")]
    [SerializeField]
    private Rigidbody2D _rb2d;
    [SerializeField]
    private Animator _animator;
    [Header("Configuration")]
    [SerializeField]
    public float jumpForce = 15f;
    public Vector2 randomTimeMinMax = new Vector2(1f, 2f);
    public float groundCheckHeight = .2f;
    public LayerMask groundLayerMask;
    private bool isGrounded;
    private float jumpTimer;
    #endregion
    #region Unity Events
    /// <summary>
    /// Callback to draw gizmos that are pickable and always drawn.
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position,
                        new Vector2(transform.position.x,
                        transform.position.y - groundCheckHeight));
    }
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        //Method that will serve to identify when you are touching the ground.
        GroundCheck();
        //In this method it will be determined when you are in the air to apply the attack animation.
        FlyChek();
        if (Time.time >= jumpTimer)
        {
            jumpTimer = Time.time + Random.Range(randomTimeMinMax.x, randomTimeMinMax.y);
            Jump();
        }
        UpdateAnimator();
    }
    #endregion
    #region Methods

    /// <summary>
    /// Method that updates the grounded state.
    /// </summary>
    private void GroundCheck()
    {
        //We perform a linecast with the ground layer mask to identify whether the enemy is touching the ground or not.
        isGrounded = Physics2D.Linecast(transform.position,
                                        new Vector2(transform.position.x,
                                        transform.position.y - groundCheckHeight),
                                        groundLayerMask);
    }
    /// <summary>
    /// Updates the status of the animator.
    /// </summary>
    private void UpdateAnimator()
    {
        _animator.SetBool("Grounded", isGrounded);
        _animator.SetFloat("height",transform.position.y);
        // _animator.SetBool("OnAir", !isGrounded);
    }
    /// <summary>
    /// Execute the jump by applying momentum force.
    /// </summary>
    private void Jump()
    {
        if (!isGrounded) return;
        _rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

    }
    /// <summary>
    /// Cheqeua cuando es que tiene que activar la animacion de ataque
    /// </summary>
    private void FlyChek()
    {
        if (gameObject.transform.position.y > 3f) ActiveAtack();
    }
    /// <summary>
    /// Method that activates the boolean that gives way to the corresponding animation.
    /// </summary>
    private void ActiveAtack()
    {
        if (!isGrounded)_animator.SetBool("OnAir", true);
    }
    /// <summary>
    /// Method that deactivates the boolean that gives way to the corresponding animation.
    /// </summary>
    public void DesactiveAtack()
    {
        _animator.SetBool("OnAir", false);
    }
    #endregion
}
