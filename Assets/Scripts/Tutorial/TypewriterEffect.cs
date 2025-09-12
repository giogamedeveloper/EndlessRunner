using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Events;

public class TypewriterEffect : MonoBehaviour
{
    [Header("Referencias")]
    public TextMeshProUGUI messageText; // Texto a modificar
    public float typingSpeed = 0.05f;   // Velocidad de escritura

    [Header("Eventos")]
    public UnityEvent OnTypingComplete = new UnityEvent();

    private Coroutine typingCoroutine;
    private bool isTyping = false;

    void Start()
    {
        // Asegurar que el evento esté inicializado
        if (OnTypingComplete == null)
            OnTypingComplete = new UnityEvent();
            
    }

    public bool IsTypingComplete()
    {
        return !isTyping;
    }

    public void StartTyping(string message)
    {
        if (messageText == null)
        {
            Debug.LogError("MessageText no asignado!");
            return;
        }

        // Detener escritura actual si hay una
        if (isTyping && typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeText(message));
    }

    public void StopTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        isTyping = false;
    }

    private IEnumerator TypeText(string message)
    {
        isTyping = true;
        messageText.text = "";
    
        // Asegurar que el texto es visible y activo
        if (messageText != null)
        {
            messageText.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("MessageText es null!");
            yield break;
        }
    
        foreach (char letter in message.ToCharArray())
        {
            // Verificar si el objeto sigue activo
            if (messageText == null || !messageText.gameObject.activeInHierarchy)
            {
                Debug.LogWarning("Typewriter interrumpido - objeto desactivado");
                break;
            }
        
            messageText.text += letter;
        
            // ✅ CAMBIO CRÍTICO: Usar WaitForSecondsRealtime
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
    
        isTyping = false;
        typingCoroutine = null;
    
        // Disparar evento
        OnTypingComplete?.Invoke();
    }

    // Método para test rápido
    public void TestTypewriter()
    {
        StartTyping("Texto de prueba del typewriter");
    }
}