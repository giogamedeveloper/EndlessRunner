// SOLUCIÓN: Sistema de colores consistente

using UnityEngine;

public class ColorManager : MonoBehaviour
{
    public static ColorManager Instance;

    [Header("Color Palette")]
    public Color playerColor = new Color(0.2f, 0.6f, 1f); // Azul claro

    public Color enemyColor = new Color(1f, 0.2f, 0.2f); // Rojo intenso
    public Color collectibleColor = new Color(1f, 0.8f, 0.2f); // Amarillo dorado
    public Color platformColor = new Color(0.3f, 0.3f, 0.4f); // Gris oscuro
    public Color backgroundColor = new Color(0.1f, 0.1f, 0.15f); // Negro azulado

    void Awake()
    {
        if (Instance == null) Instance = this;
    }
}
