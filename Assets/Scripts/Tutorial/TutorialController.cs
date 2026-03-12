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
        OnTutorialComplete.AddListener(() =>
            Time.timeScale = 1f); // Asegurar que el tiempo se normalice al completar tutorial
        StartCoroutine(InitializeWithDelay());
    }

    private void Update()
    {
        if (playerController == null) return;
        if (!attackDetected && playerController.enemyCount > 0)
            attackDetected = true;
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
        SetPlayerState(tutorialMode: true, tutorialReady: false);
        yield return StartCoroutine(ShowMessage(welcomeMessage));
        yield return new WaitForSecondsRealtime(2f);
        if (playerController != null)
        {
            playerController.SetTutorialReady(true);
        }
    }


    private IEnumerator TeachJump()
    {
        currentState = TutorialState.JumpIntro;
        playerFailed = false;
        SetPlayerState(tutorialMode: true, tutorialReady: true);
        yield return StartCoroutine(ShowMessage(jumpMessage));
        currentState = TutorialState.JumpPractice;
        while (!playerFailed)
        {
            if (playerController != null && !playerController.isGrounded)
                break;
            yield return null;
        }
    }

    private IEnumerator TeachDoubleJump()
    {

        currentState = TutorialState.DoubleJumpIntro;
        playerFailed = false;
        SetPlayerState(tutorialMode: true, tutorialReady: true);
        yield return StartCoroutine(ShowMessage(doubleJumpMessage));
        currentState = TutorialState.DoubleJumpPractice;
        // Esperar doble salto con verificación de muerte
        float timer = 0f;
        while (timer < 15f && !playerFailed)
        {
            // Detectar doble salto
            if (playerController != null && !playerController.isGrounded && playerController.jumpInAirCounter >= 1)
                break;
            timer += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator TeachAttack()
    {
        currentState = TutorialState.AttackIntro;
        playerFailed = false;
        attackDetected = false;
        yield return StartCoroutine(ShowMessage(attackMessage));

        if (typewriter != null)
        {
            typewriter.messageText.enabled = false;
            typewriter._panel.alpha = 0;
        }
        SetPlayerState(tutorialMode: true, tutorialReady: true);
        playerController.SetAutoMove(true);
        currentState = TutorialState.AttackPractice;
        float timer = 0f;

        while (!attackDetected && !playerFailed && timer < 30f)
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
            typewriter.StartTyping(TranslateManager.Instance.GetText(completionMessage));
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
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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

    private void SetPlayerState(bool tutorialMode, bool tutorialReady)
    {
        if (playerController == null) return;
        playerController.SetTutorialMode(tutorialMode);
        playerController.SetTutorialReady(tutorialReady);
    }

    private IEnumerator ShowMessage(string message)
    {
        if (typewriter == null) yield break;
        typewriter.StartTyping(TranslateManager.Instance.GetText(message));
        yield return new WaitUntil(() => typewriter.IsTypingComplete());
        yield return new WaitForSecondsRealtime(messageDelay);
    }
}
