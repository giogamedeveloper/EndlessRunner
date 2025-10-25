using TMPro;
using UnityEngine;

public class HoverDescription : MonoBehaviour
{
    public TextMeshProUGUI currentDescription;
    public void OnHoverEnter() => currentDescription.gameObject.SetActive(true);
    public void OnHoverExit() => currentDescription.gameObject.SetActive(false);
}
