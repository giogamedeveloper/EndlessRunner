using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonHoverText : MonoBehaviour
{
    public TextMeshProUGUI buttonText;
    public Color normalColor = new Color(1f, 0.65f, 0.23f); // naranja
    public Color hoverColor = new Color(1f, 0.95f, 0.46f); // amarillo suave
    public Color pressedColor = new Color(0.36f, 0.1f, 0f); // marrón oscuro

    public void OnHoverEnter() => buttonText.color = hoverColor;
    public void OnHoverExit() => buttonText.color = normalColor;
    public void OnPressed() => buttonText.color = pressedColor;
}
