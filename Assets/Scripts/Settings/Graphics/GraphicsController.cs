using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class GraphicsController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI graphicsText;

    [SerializeField]
    private TextMeshProUGUI resolutionText;

    [SerializeField]
    private TextMeshProUGUI idiomaText;

    public void Idioma()
    {
        idiomaText.gameObject.SetActive(true);
        graphicsText.gameObject.SetActive(false);
        resolutionText.gameObject.SetActive(false);
    }

    public void Resolution()
    {
        idiomaText.gameObject.SetActive(false);
        graphicsText.gameObject.SetActive(false);
        resolutionText.gameObject.SetActive(true);
    }

    public void Graphics()
    {
        idiomaText.gameObject.SetActive(false);
        resolutionText.gameObject.SetActive(false);
        graphicsText.gameObject.SetActive(true);

    }
}
