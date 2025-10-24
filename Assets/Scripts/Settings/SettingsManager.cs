using UnityEngine;
using UnityEngine.EventSystems;

public class SettingsManager : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup audioCanvas;

    [SerializeField]
    private CanvasGroup graphicsCanvas;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void Audio()
    {
        audioCanvas.alpha =1;
        audioCanvas.interactable = true;
        audioCanvas.blocksRaycasts = true;
        graphicsCanvas.alpha = 0;
        graphicsCanvas.interactable = false;
        graphicsCanvas.blocksRaycasts = false;
    }

    public void Graphics()
    {
        audioCanvas.alpha =0;
        audioCanvas.interactable = false;
        audioCanvas.blocksRaycasts = false;
        graphicsCanvas.alpha = 1;
        graphicsCanvas.interactable = true;
        graphicsCanvas.blocksRaycasts = true;
    }
}
