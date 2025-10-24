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
        JumpIntro,
        JumpPractice,
        DoubleJumpIntro,
        DoubleJumpPractice,
        AttackIntro,
        AttackPractice,
        Completed
    }

    [Header("Referencias")]
    public PlayerController playerController;

    public TypewriterEffect typewriter;
    public SceneController sceneController;

    [Header("Mensajes del tutorial")]
    [TextArea(2, 6)]
    public string welcomeMessage = "¡Bienvenido al tutorial!";

    [TextArea(2, 6)]
    public string jumpMessage = "Pulsa SPACE para saltar obstáculos\n¡Cuidado! Si te caes, volverás aquí.";

    [TextArea(2, 6)]
    public string doubleJumpMessage = "¡Bien hecho! Ahora intenta un DOBLE SALTO.\nPresiona SPACE nuevamente en el aire.";

    [TextArea(2, 6)]
    public string attackMessage = "¡Excelente! Ahora acaba con el enemigo.\nUsa CLIC IZQUIERDO o J para atacar.";

    [TextArea(2, 6)]
    public string completionMessage = "¡Tutorial Finalizado!\nRegresando al menú principal...";

    public float completionDelay = 3f;

    [Header("Configuración")]
    public float messageDelay = 1f;

    // Variable estática para mantener el estado entre recargas
    private static bool tutorialWasActive = false;

    [Header("Eventos")]
    public UnityEvent OnTutorialStart;

    public UnityEvent OnTutorialComplete;

    private TutorialState currentState = TutorialState.Inactive;
    public bool isTutorialActive = false;
    private Coroutine tutorialRoutine;
    private bool playerFailed = false;
    private bool attackDetected;


    void Start()
    {
        playerFailed = false;
        attackDetected = false;
        // Inicio normal
        StartCoroutine(InitializeWithDelay());
    }

    private void Update()
    {
        if (playerController.enemyCount > 0)
        {
            attackDetected = true;
        }
        IsPlayerDeadOrFallen();
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

        // Paso 1: Bienvenida
        yield return StartCoroutine(WelcomePhase());

        // Paso 3: Salto
        yield return StartCoroutine(TeachJump());

        // Paso 4: Doble salto
        yield return StartCoroutine(TeachDoubleJump());

        // Paso 5: Ataque
        yield return StartCoroutine(TeachAttack());
    }

    private IEnumerator WelcomePhase()
    {
        currentState = TutorialState.MoveIntro;

        // Configurar player para tutorial
        if (playerController != null)
        {
            playerController.SetTutorialMode(true);
            playerController.SetTutorialReady(false);
        }

        // Mostrar mensaje de bienvenida
        if (typewriter != null)
        {
            typewriter.StartTyping(welcomeMessage);
        }
        yield return new WaitForSeconds(3f);
        if (playerController != null)
        {
            playerController.SetTutorialReady(true);
        }
    }

    // Al destruirse, guardar estado si está activo
    private void OnDestroy()
    {
        if (isTutorialActive && currentState != TutorialState.Completed)
        {
            tutorialWasActive = true;
        }
    }

    private IEnumerator TeachJump()
    {
        Debug.Log("Fase 3 - Salto");
        currentState = TutorialState.JumpIntro;
        playerFailed = false;

        // Activar movimiento automático
        if (playerController != null)
        {
            playerController.SetTutorialMode(true);
            playerController.SetTutorialReady(true);
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

        while (!jumpSuccessful && !playerFailed)
        {
            jumpSuccessful = true;
            yield return null;
        }
    }

    private IEnumerator TeachDoubleJump()
    {
        Debug.Log("Fase 4 - Doble salto");
        currentState = TutorialState.DoubleJumpIntro;
        playerFailed = false;

        // Reposicionar player
        if (playerController != null)
        {
            playerController.SetTutorialMode(true);
            playerController.SetTutorialReady(true);
        }

        // Mostrar mensaje de doble salto
        if (typewriter != null)
        {
            typewriter.StartTyping(doubleJumpMessage);
            yield return new WaitUntil(() => typewriter.IsTypingComplete());
            yield return new WaitForSecondsRealtime(messageDelay);
        }
        currentState = TutorialState.DoubleJumpPractice;
        // Esperar doble salto con verificación de muerte
        float timer = 0f;
        bool doubleJumpDetected = false;
        while (timer < 15f && !doubleJumpDetected && !playerFailed)
        {
            // Detectar doble salto
            if (playerController != null && !playerController.isGrounded && playerController.jumpInAirCounter >= 1)
            {
                doubleJumpDetected = true;
            }
            timer += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator TeachAttack()
    {
        currentState = TutorialState.AttackIntro;
        playerFailed = false;
        attackDetected = false;
        if (typewriter != null)
        {
            typewriter.StartTyping(attackMessage);
            yield return new WaitUntil(() => typewriter.IsTypingComplete());
            yield return new WaitForSecondsRealtime(messageDelay);
            if (typewriter.IsTypingComplete())
            {
                typewriter.messageText.enabled = false;
                typewriter._panel.alpha = 0;
            }
        }
        if (playerController != null)
        {
            playerController.SetTutorialMode(false);
            playerController.SetTutorialReady(true);
            playerController.SetAutoMove(true);
            Debug.Log("👤 Player configurado para ataque");
        }
        else
        {
            Debug.LogError("❌❌❌ PlayerController es NULL!");
        }

        // Mostrar mensaje de ataque
        currentState = TutorialState.AttackPractice;

        float timeout = 30f;
        float timer = 0f;

        while (!attackDetected && !playerFailed && timer < timeout)
        {
            timer += Time.deltaTime;

            yield return null;
        }
        if (attackDetected)
        {
            yield return StartCoroutine(CompleteTutorial());
            currentState = TutorialState.Completed;
            isTutorialActive = false;
            OnTutorialComplete?.Invoke();
        }
        else if (playerFailed)
        {
            Debug.Log("❌ Jugador falló durante ataque");
        }
        else
        {
            Debug.LogWarning("⏰ Timeout en espera de ataque");
        }
    }

    private IEnumerator CompleteTutorial()
    {
        typewriter._panel.alpha = 1;
        typewriter.messageText.enabled = true;
        currentState = TutorialState.Completed;
        isTutorialActive = false;
        if (playerController != null)
            playerController.SetAutoMove(false);
        // Mostrar mensaje de finalización
        if (typewriter != null)
        {
            typewriter.StartTyping(completionMessage);
            yield return new WaitUntil(() => typewriter.IsTypingComplete());
        }

        // Esperar un momento para que el jugador lea el mensaje
        yield return new WaitForSeconds(completionDelay);

        sceneController.ChangeScene("MainMenu");
        OnTutorialComplete?.Invoke();
    }

    private bool IsPlayerDeadOrFallen()
    {
        if (playerController == null) return false;

        // Verificar si player cayó o murió
        bool isDeadOrFallen = playerController.IsDead || playerController.transform.position.y < -10f;

        if (isDeadOrFallen)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Debug.Log("Player detectado como muerto/caído");
        }

        return isDeadOrFallen;
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
}
