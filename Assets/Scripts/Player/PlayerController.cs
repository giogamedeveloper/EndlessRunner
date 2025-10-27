using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    #region Variables

    [Header("References")]
    [SerializeField]
    private PlayerFeedback _playerFeedback;

    [SerializeField]
    private TutorialController _tutorialController;

    [Header("Jump Settings")]
    public float gravityMultiplier = 3f;

    public bool isJump = false;

    [SerializeField]
    private DataManager data;

    public float jumpForce = 5f;

    public int maxHp;
    public int currentLife;

    //variables para controlar la cantidad de saltos que se puede dar
    public int jumpInAirAllowed = 2;

    //Saltos consecutivos realizados
    public int jumpInAirCounter = 0;
    private bool isAtack = false;
    private Rigidbody2D _rb2d;

    [Header("Ground Check")]
    public Transform groundCheck;

    public LayerMask groundLayerMask;
    public Vector2 groundCheckSize = new Vector2(.75f, .1f);

    public bool isGrounded = false;

    [Header("States")]
    public bool controlsEnabled = true;

    //Limite mortal de altura
    public float deadLimit = -5f;

    [Header("Movement")]
    public bool isAutoMove = true;

    public float maxSpeed = 5f;
    public float acceleration = 20f;

    [Header("PowerUp")]
    public float powerUpDuration;

    public float powerUpCounter = 0f;
    public bool isPowerUp = false;
    public bool isPowerUpShield = false;

    [Serialize]
    public int enemyCount;

    private Coroutine powerUpTimer;
    public Animator _animator;
    private bool isDead = false;
    public bool IsPlayingTuto;
    private Shop shop;

    [Header("Items")]
    public int maxItems = 5;

    public LifePlayer lifePlayer;

    [Header("Respawn")]
    public Vector3 respawnPosition;

    public bool isRespawning = false;

    [Header("Tutorial")]
    public bool tutorialReady = false;

    #endregion

    public bool IsDead => isDead;
    public bool IsAtack => isAtack;
    public int EnemyCount => enemyCount;

    #region Events

    public UnityEvent<int> changeItems;

    /// <summary>
    /// Callback to draw gizmos that are pickable and always drawn.
    /// </summary>
    void OnDrawGizmos()
    {
        if (groundCheck == null) return;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 2) IsPlayingTuto = true;
        else IsPlayingTuto = false;
        _rb2d.gravityScale = gravityMultiplier;
        if (!IsPlayingTuto)
        {
            data.LoadHp();
            maxHp = DataManager.Instance.maxHp;
            lifePlayer.maxLife = maxHp;
            currentLife = maxHp;
            lifePlayer.changeLife.Invoke(currentLife, maxHp);
            if (DataManager.Instance.itemInventory[DataManager.Instance.itemInventory.Length - 1].Quantity == 1)
                SectionManager.Instance.ActivePowerUp();
        }
        tutorialReady = !IsPlayingTuto;
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        Movement();
        GroundCheck();
        UpdateAnimator();
        CheckLimitDead();

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleAutoMove();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetAutoMove(true);
            Debug.Log("Movimiento automático ACTIVADO");
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SetAutoMove(false);
            Debug.Log("Movimiento automático DESACTIVADO");
        }
#endif
    }

    /// <summary>
    /// This method is called when colliding with an enemy.
    /// </summary>
    /// <param name="other"></param>
    void OnCollisionEnter2D(Collision2D other)
    {
        //If the other object is an enemy and does not have any power up activated, it dies.
        if (other.gameObject.CompareTag("Enemies"))
        {
            //If you attack an enemy before it is attacked it eliminates it. 
            if ((powerUpCounter > 0f || isAtack) && other.transform.TryGetComponent(out EnemyDead enemyDead))
            {
                enemyDead.Disappears();
                _playerFeedback.PlayEnemyKillFeedback(other.contacts[0].point);
                GameManager.Instance.PickUpCollectable(2);
                enemyCount++;
            }
            else if (lifePlayer.currentLife > 1)
            {
                lifePlayer.TakeDamage(1);
                _playerFeedback.PlayDamageFeedback(transform.position);
            }
            else Dead();
        }
    }

    /// <summary>
    /// Event that is triggered from the input system when the corresponding action is executed.
    /// </summary>
    /// <param name="context"></param>
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started) Jump();
    }

    public void Atacking(InputAction.CallbackContext context)
    {
        if (context.started) ActiveAtack();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Method that activates the boolean that gives way to the corresponding animation.
    /// </summary>
    private void ActiveAtack()
    {
        if (!isAtack)
        {
            isAtack = true;
        }
    }

    /// <summary>
    /// Method that deactivates the boolean that gives way to the corresponding animation.
    /// </summary>
    public void DesactiveAtack()
    {
        isAtack = false;
    }

    /// <summary>
    /// Jump -> Is called when Player want to Jump up and apply force UP. 
    /// </summary>
    private void Jump()
    {
        if (isGrounded || jumpInAirCounter < jumpInAirAllowed)
        {
            //We reset the speed on the Y axis
            _rb2d.linearVelocityY = 0f;
            _rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            //If we don't touch the ground we increase the jump counter
            if (!isGrounded) jumpInAirCounter++;
        }
        _playerFeedback.PlayJumpFeedback();
    }

    /// <summary>
    /// GroundCheck -> It is just to update the status of the ground.
    /// </summary>
    private void GroundCheck()
    {
        //We check the contact with the ground using overlap box indicating its position, size, angle of rotation in z and layer Mask.
        Collider2D groundCollider = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayerMask);
        //If GroundCollider is not null, the grounded state will be true, otherwise it will be false.
        isGrounded = groundCollider != null;
        //If the player is grounded we do not apply gravity, otherwise we will apply it.
        _rb2d.gravityScale = isGrounded ? 0 : gravityMultiplier;
        //If we touch the ground the jump counter is reset to 0.
        if (isGrounded) jumpInAirCounter = 0;

    }

    /// <summary>
    /// Movement ->It´s called for move the player horizontally.
    /// </summary>
    private void Movement()
    {
        if (IsPlayingTuto && !tutorialReady)
        {
            _rb2d.linearVelocityX = 0f;
            return;
        }
        // === MODO MANUAL/DETENIDO ===
        if (!isAutoMove)
        {
            // Solo si tenemos velocidad, aplicar desaceleración
            if (Mathf.Abs(_rb2d.linearVelocityX) > 0.01f)
            {
                float deceleration = acceleration * 80f; // Desaceleración más rápida
                _rb2d.linearVelocityX = Mathf.MoveTowards(
                    _rb2d.linearVelocityX,
                    0f,
                    deceleration * Time.deltaTime
                );
            }
            else
            {
                // Asegurar velocidad cero
                _rb2d.linearVelocityX = 0f;
            }
            return;
        }

        // === MODO AUTOMÁTICO ===

        // Aceleración hacia la velocidad máxima
        if (Mathf.Abs(_rb2d.linearVelocityX - maxSpeed) > 0.1f)
        {
            _rb2d.linearVelocityX = Mathf.MoveTowards(
                _rb2d.linearVelocityX,
                maxSpeed,
                acceleration * 60f * Time.deltaTime
            );
        }
        else
        {
            // Mantener velocidad máxima exacta
            _rb2d.linearVelocityX = maxSpeed;
        }
    }

    public void SetTutorialReady(bool ready)
    {
        tutorialReady = ready;
    }

    /// <summary>
    /// Updates all states of the Animator.
    /// </summary>
    private void UpdateAnimator()
    {
        _animator.SetBool("grounded", isGrounded);
        _animator.SetFloat("velocity", MathF.Abs(_rb2d.linearVelocityX));
        _animator.SetBool("jumpRoll", jumpInAirCounter > 0);
        _animator.SetBool("isAtack", isAtack);
        _animator.SetBool("isPowerUp", isPowerUp);
        _animator.SetBool("isPowerShield", isPowerUpShield);

    }

    /// <summary>
    /// Check if the player has exceeded the height limit to stay alive.
    /// </summary>
    private void CheckLimitDead()
    {
        //If it's already dead, we ignore the method.
        if (isDead) return;
        //If the player falls below the indicated height.
        if (transform.position.y < deadLimit) Dead();
    }

    public void Respawn(Vector3 position)
    {
        if (isRespawning) return;

        isRespawning = true;
        isDead = false;

        // Detener movimiento
        isAutoMove = false;
        _rb2d.linearVelocity = Vector3.zero;
        _rb2d.angularVelocity = 0f;

        // Reposicionar
        transform.position = position;
        // Resetear animaciones
        _animator.SetBool("dead", false);
        _animator.Play("Idle", 0, 0f);
        // Reactivar controles después de un breve delay
        StartCoroutine(EnableAfterRespawn());
    }

    private IEnumerator EnableAfterRespawn()
    {
        // Esperar a que las secciones se estabilicen
        yield return new WaitForSeconds(0.3f);

        // ✅ Esperar a que la primera sección esté en posición correcta
        if (IsPlayingTuto)
        {
            yield return new WaitUntil(() =>
            {
                if (SectionManager.Instance != null && SectionManager.Instance.currentSection != null)
                {
                    float distance = Vector3.Distance(transform.position,
                        SectionManager.Instance.currentSection.transform.position);
                    return distance < 10f; // Ajusta este valor según necesites
                }
                return true;
            });
        }

        isAutoMove = true;
        isRespawning = false;

        Debug.Log("Respawn completado - Player listo");
    }

    /// <summary>
    /// Execute the player's death actions.
    /// </summary>
    private void Dead()
    {
        if (IsPlayingTuto)
        {
            // En tutorial, no llamar EndGame, solo notificar
            isDead = true;
            isAutoMove = false;
            _rb2d.linearVelocity = Vector3.zero;
            _animator.SetBool("dead", true);

            Debug.Log("Player murió en tutorial - Esperando respawn");
        }
        else
        {
            // Comportamiento normal para juego
            isDead = true;
            isAutoMove = false;
            _rb2d.linearVelocity = Vector3.zero;
            _animator.SetBool("dead", true);
            Invoke("EndGame", 1f);
        }

    }

    /// <summary>
    /// Method that communicates with the GameManager to stop the game.
    /// </summary>
    private void EndGame()
    {
        GameManager.Instance.EndGame();
    }

    /// <summary>
    /// Activate the power up effect by starting its coroutine.
    /// </summary>
    public void ActivePowerUp()
    {
        if (powerUpTimer != null)
        {
            StopCoroutine(powerUpTimer);
        }
        powerUpTimer = StartCoroutine(PowerUpEffect(1));
    }

    /// <summary>
    /// Coroutine that manages the passage of time.
    /// </summary>
    /// <returns></returns>
    public void ActivePowerUpShield()
    {
        if (powerUpTimer != null)
        {
            StopCoroutine(powerUpTimer);
        }
        powerUpTimer = StartCoroutine(PowerUpEffect(2));
    }

    public void MaxCoins()
    {
        DataManager.Instance.SaveCoins(10000);
        for (int i = 0; i < 3; i++)
        {
            DataManager.Instance.SaveItem(i);
            DataManager.Instance.SaveItem(i);
        }
    }

    /// <summary>
    /// Coroutine that manages the passage of time.
    /// </summary>
    /// <returns></returns>
    private IEnumerator PowerUpEffect(int value)
    {
        switch (value)
        {

            case 1:
                powerUpCounter = powerUpDuration;
                while (powerUpCounter > 0)
                {
                    isPowerUp = true;
                    powerUpCounter -= Time.deltaTime;
                    yield return null;
                }
                isPowerUp = false;
                break;
            case 2:
                powerUpCounter = powerUpDuration;
                while (powerUpCounter > 0)
                {
                    isPowerUpShield = true;
                    powerUpCounter -= Time.deltaTime;
                    yield return null;
                }
                isPowerUpShield = false;
                break;
        }
    }

    #endregion

    #region Tutorial

    // En la región Tutorial del PlayerController, agrega:

    public void SetTutorialMode(bool isTutorial)
    {
        // Durante tutorial, controlamos el auto-movement
        if (isTutorial)
        {
            isAutoMove = false;
            _rb2d.linearVelocityX = 0f;
        }
        else
        {
            isAutoMove = true;
        }

        controlsEnabled = !isTutorial;
    }


    public void EnableControls(bool enable)
    {
        controlsEnabled = enable;

        // También puedes controlar el auto-movement si es necesario
        if (enable)
        {
            isAutoMove = true;
        }
        else
        {
            isAutoMove = false;
            _rb2d.linearVelocityX = 0f; // Detener movimiento
        }

        Debug.Log($"Controles {(enable ? "activados" : "desactivados")}");
    }

// En la región Tutorial o Methods, agrega:
    public void ToggleAutoMove()
    {
        isAutoMove = !isAutoMove;
        Debug.Log($"AutoMove toggled: {isAutoMove}");
    }

    public void SetAutoMove(bool autoMove)
    {
        isAutoMove = autoMove;
    }

    #endregion
}
