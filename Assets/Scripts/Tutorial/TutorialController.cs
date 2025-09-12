using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TutorialController : MonoBehaviour
{
    public enum TutorialState
    {
        Inactive,
        MoveIntro,
        MovePractice,
        JumpIntro,
        JumpPractice,
        DoubleJumpIntro,
        DoubleJumpPractice,
        AttackIntro,
        AttackPractice,
        Completed
    }

    [Header("Respawn Settings")]
    public Vector3 respawnPoint = new Vector3(-2, -1.6f, 0);

    public float respawnDelay = 1.5f;

    [Header("Referencias")]
    public PlayerController playerController;

    public TypewriterEffect typewriter;
    public GameManager gameManager;

    [Header("Mensajes del tutorial")]
    [TextArea(2, 6)]
    public string welcomeMessage = "¡Bienvenido al tutorial!\nCuando estés listo, pulsa la letra J para comenzar.";

    [TextArea(2, 6)]
    public string moveMessage = "Usa M para moverte lateralmente al igual si mueres";

    [TextArea(2, 6)]
    public string jumpMessage = "Pulsa SPACE para saltar obstáculos\n¡Cuidado! Si te caes, volverás aquí.";

    [TextArea(2, 6)]
    public string doubleJumpMessage = "¡Bien hecho! Ahora intenta un DOBLE SALTO.\nPresiona SPACE nuevamente en el aire.";

    [TextArea(2, 6)]
    public string attackMessage = "¡Excelente! Ahora acaba con el enemigo.\nUsa CLIC IZQUIERDO o J para atacar.";

    [Header("Configuración")]
    public float messageDelay = 1f;

    public float inputWaitTime = 3f;

    [Header("Scene Management")]
    public bool reloadSceneOnDeath = true;

    // Variable estática para mantener el estado entre recargas
    private static TutorialState savedState = TutorialState.Inactive;
    private static bool tutorialWasActive = false;

    [Header("Eventos")]
    public UnityEvent OnTutorialStart;

    public UnityEvent OnTutorialComplete;
    public UnityEvent OnPlayerRespawn;

    private TutorialState currentState = TutorialState.Inactive;
    private bool isTutorialActive = false;
    private Coroutine tutorialRoutine;
    private bool playerFailed = false;


    void Start()
    {
        // ✅ CARGAR ESTADO GUARDADO SI EXISTE
        if (savedState != TutorialState.Inactive && tutorialWasActive)
        {
            currentState = savedState;
            isTutorialActive = tutorialWasActive;

            // Iniciar desde la fase guardada
            StartCoroutine(ContinueFromSavedState());
        }
        else
        {
            // Inicio normal
            StartCoroutine(InitializeWithDelay());
        }
    }

    private IEnumerator ContinueFromSavedState()
    {
        yield return null;
        yield return null;

        Debug.Log($"Continuando tutorial desde: {currentState}");

        // Configurar player
        if (playerController != null)
        {
            playerController.respawnPosition = respawnPoint;
            playerController.transform.position = respawnPoint;
            playerController.SetTutorialMode(currentState != TutorialState.Completed);

            // Si estamos continuando desde un estado guardado, activar el movimiento
            if (currentState > TutorialState.MoveIntro)
            {
                playerController.SetTutorialReady(true);
            }
            else
            {
                playerController.SetTutorialReady(false);
            }
        }

        // Continuar según el estado guardado
        switch (currentState)
        {
            case TutorialState.MoveIntro:
                tutorialRoutine = StartCoroutine(WelcomePhase());
                break;

            case TutorialState.MovePractice:
                tutorialRoutine = StartCoroutine(TeachMovement());
                break;

            case TutorialState.JumpIntro:
            case TutorialState.JumpPractice:
                tutorialRoutine = StartCoroutine(TeachJump());
                break;

            case TutorialState.DoubleJumpIntro:
            case TutorialState.DoubleJumpPractice:
                tutorialRoutine = StartCoroutine(TeachDoubleJump());
                break;

            case TutorialState.AttackIntro:
            case TutorialState.AttackPractice:
                tutorialRoutine = StartCoroutine(TeachAttack());
                break;

            case TutorialState.Completed:
                Debug.Log("Tutorial ya estaba completado");
                break;
        }
    }

    private IEnumerator InitializeWithDelay()
    {
        yield return null;
        yield return null;
        if (Time.timeScale < 0.1f)
        {
            Time.timeScale = 1f;
        }

        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();

        if (typewriter == null)
            typewriter = FindObjectOfType<TypewriterEffect>();

        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();
        if (playerController != null && playerController.IsPlayingTuto)
        {
            playerController.SetTutorialReady(false);
            yield return new WaitForSecondsRealtime(1f);
            StartTutorial();
        }
    }

    public void StartTutorial()
    {
        if (playerController != null)
        {
            playerController.SetTutorialReady(false); // El jugador no se moverá hasta que el tutorial esté listo
        }
        if (tutorialRoutine != null)
        {
            StopCoroutine(tutorialRoutine);
        }

        tutorialRoutine = StartCoroutine(TutorialSequence());
        OnTutorialStart?.Invoke();
    }

    private IEnumerator TutorialSequence()
    {
        isTutorialActive = true;

        // Establecer punto de respawn inicial
        if (playerController != null)
        {
            playerController.respawnPosition = respawnPoint;
            playerController.transform.position = respawnPoint;
        }

        // Paso 1: Bienvenida
        yield return StartCoroutine(WelcomePhase());

        // Paso 2: Movimiento
        yield return StartCoroutine(TeachMovement());

        // Paso 3: Salto
        yield return StartCoroutine(TeachJump());

        // Paso 4: Doble salto
        yield return StartCoroutine(TeachDoubleJump());

        // Paso 5: Ataque
        yield return StartCoroutine(TeachAttack());

        currentState = TutorialState.Completed;
        isTutorialActive = false;

        OnTutorialComplete?.Invoke();
        Debug.Log("=== TUTORIAL COMPLETADO ===");
    }

    private IEnumerator WelcomePhase()
    {
        currentState = TutorialState.MoveIntro;

        // Configurar player para tutorial
        if (playerController != null)
        {
            playerController.SetTutorialMode(true);
            playerController.transform.position = respawnPoint;
            playerController.SetTutorialReady(false);
        }

        // Mostrar mensaje de bienvenida
        if (typewriter != null)
        {
            typewriter.StartTyping(welcomeMessage);
        }
        // Esperar a que el player presione J
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.J));
        if (playerController != null)
        {
            playerController.SetTutorialReady(true);
        }
    }

    // Método para forzar guardado de estado
    public void SaveTutorialState()
    {
        savedState = currentState;
        tutorialWasActive = isTutorialActive;
        Debug.Log($"Estado guardado manualmente: {savedState}");
    }

    // Método para limpiar estado guardado
    public static void ClearSavedState()
    {
        savedState = TutorialState.Inactive;
        tutorialWasActive = false;
        Debug.Log("Estado del tutorial limpiado");
    }

    // Al destruirse, guardar estado si está activo
    private void OnDestroy()
    {
        if (isTutorialActive && currentState != TutorialState.Completed)
        {
            savedState = currentState;
            tutorialWasActive = true;
        }
    }

    private IEnumerator TeachMovement()
    {
        Debug.Log("Fase 2 - Movimiento");
        currentState = TutorialState.MoveIntro;
        playerFailed = false;

        // Mostrar mensaje de movimiento
        if (typewriter != null)
        {
            typewriter.StartTyping(moveMessage);
            yield return new WaitUntil(() => typewriter.IsTypingComplete());
            yield return new WaitForSecondsRealtime(messageDelay);
        }

        // Práctica
        currentState = TutorialState.MovePractice;
        Debug.Log("¡Ahora practica moviéndote con A/D!");

        // Esperar input del jugador
        float timer = 0f;
        while (timer < inputWaitTime && !playerFailed)
        {
            if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f)
            {
                Debug.Log("Input de movimiento detectado!");
                break;
            }

            // Verificar si player murió
            if (IsPlayerDeadOrFallen())
            {
                yield return StartCoroutine(HandlePlayerDeath());
                timer = 0f; // Reset timer
            }

            timer += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Paso movimiento completado");
    }

    private IEnumerator TeachJump()
    {
        Debug.Log("Fase 3 - Salto");
        currentState = TutorialState.JumpIntro;
        playerFailed = false;

        // Activar movimiento automático
        if (playerController != null)
        {
            playerController.SetTutorialMode(false);
            playerController.transform.position = respawnPoint;
        }

        // Mostrar mensaje de salto
        if (typewriter != null)
        {
            typewriter.StartTyping(jumpMessage);
            yield return new WaitUntil(() => typewriter.IsTypingComplete());
            yield return new WaitForSecondsRealtime(messageDelay);
        }

        // Práctica
        currentState = TutorialState.JumpPractice;
        Debug.Log("¡Salta con SPACE!");

        // Bucle hasta salto exitoso
        bool jumpSuccessful = false;
        float phaseStartTime = Time.time;

        while (!jumpSuccessful && Time.time - phaseStartTime < 30f && !playerFailed)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                jumpSuccessful = true;
                Debug.Log("Salto exitoso detectado");
            }

            // Verificar si player murió
            if (IsPlayerDeadOrFallen())
            {
                yield return StartCoroutine(HandlePlayerDeath());
                phaseStartTime = Time.time; // Reset timer
            }

            yield return null;
        }

        Debug.Log("Paso salto completado");
    }

    private IEnumerator TeachDoubleJump()
    {
        Debug.Log("Fase 4 - Doble salto");
        currentState = TutorialState.DoubleJumpIntro;
        playerFailed = false;

        // Reposicionar player
        if (playerController != null)
        {
            playerController.transform.position = respawnPoint;
        }

        // Mostrar mensaje de doble salto
        if (typewriter != null)
        {
            typewriter.StartTyping(doubleJumpMessage);
            yield return new WaitUntil(() => typewriter.IsTypingComplete());
            yield return new WaitForSecondsRealtime(messageDelay);
        }

        // Práctica
        currentState = TutorialState.DoubleJumpPractice;
        Debug.Log("¡Haz un doble salto!");

        // Esperar doble salto con verificación de muerte
        float timer = 0f;
        bool doubleJumpDetected = false;

        while (timer < 15f && !doubleJumpDetected && !playerFailed)
        {
            // Detectar doble salto
            if (playerController != null && !playerController.isGrounded && playerController.jumpInAirCounter >= 1)
            {
                doubleJumpDetected = true;
                Debug.Log("Doble salto detectado!");
            }

            // Verificar si player murió
            if (IsPlayerDeadOrFallen())
            {
                yield return StartCoroutine(HandlePlayerDeath());
                timer = 0f; // Reset timer
            }

            timer += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Paso doble salto completado");
    }

    private IEnumerator TeachAttack()
    {
        Debug.Log("Fase 5 - Ataque");
        currentState = TutorialState.AttackIntro;
        playerFailed = false;

        // Mostrar mensaje de ataque
        if (typewriter != null)
        {
            typewriter.StartTyping(attackMessage);
            yield return new WaitUntil(() => typewriter.IsTypingComplete());
            yield return new WaitForSecondsRealtime(messageDelay);
        }

        // Práctica
        currentState = TutorialState.AttackPractice;
        // Esperar ataque con verificación de muerte
        float timer = 0f;
        bool attackDetected = false;

        while (timer < 20f && !attackDetected && !playerFailed)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.LeftControl))
            {
                attackDetected = true;
            }

            // Verificar si player murió
            if (IsPlayerDeadOrFallen())
            {
                yield return StartCoroutine(HandlePlayerDeath());
                timer = 0f; // Reset timer
            }

            timer += Time.deltaTime;
            yield return null;
        }

    }

    private IEnumerator HandlePlayerDeath()
    {
        Debug.Log("Manejando muerte del jugador...");
        playerFailed = true;

        // ✅ GUARDAR ESTADO ACTUAL
        savedState = currentState;
        tutorialWasActive = isTutorialActive;

        // ✅ RESETEAR SECCIONES DE MANERA ROBUSTA
        yield return new WaitForSeconds(0.1f); // Pequeña espera

        // Reposicionar jugador
        if (playerController != null)
        {
            playerController.transform.position = respawnPoint;
        }

        yield return new WaitForSeconds(respawnDelay);

        if (reloadSceneOnDeath)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            if (playerController != null && playerController.IsDead)
            {
                playerController.Respawn(respawnPoint);
            }
            playerFailed = false;
        }
    }

    private bool IsPlayerDeadOrFallen()
    {
        if (playerController == null) return false;

        // Verificar si player cayó o murió
        bool isDeadOrFallen = playerController.IsDead || playerController.transform.position.y < -10f;

        if (isDeadOrFallen)
        {
            Debug.Log("Player detectado como muerto/caído");
        }

        return isDeadOrFallen;
    }

    private void Update()
    {
        // Verificar muerte continuamente
        if (isTutorialActive && IsPlayerDeadOrFallen() && !playerFailed)
        {
            StartCoroutine(HandlePlayerDeath());
        }

        // Controles de debug
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("=== REINICIAR TUTORIAL ===");
            StartTutorial();
        }
    }

    public void SkipTutorial()
    {
        if (tutorialRoutine != null)
            StopCoroutine(tutorialRoutine);

        currentState = TutorialState.Completed;
        isTutorialActive = false;

        if (typewriter != null)
            typewriter.StopTyping();

        OnTutorialComplete?.Invoke();
    }

    public bool IsTutorialComplete()
    {
        return currentState == TutorialState.Completed;
    }

    public void SetRespawnPoint(Vector3 newPosition)
    {
        respawnPoint = newPosition;
        if (playerController != null)
        {
            playerController.respawnPosition = newPosition;
        }
        Debug.Log($"Nuevo punto de respawn: {newPosition}");
    }
}
