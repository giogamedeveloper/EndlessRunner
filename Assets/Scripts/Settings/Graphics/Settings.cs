using UnityEngine;

public class Settings : MonoBehaviour
{
    bool _changeLenguage;
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    public void ChangeIdioma(int value)
    {
        Debug.Log($"TranslateManager.Instance: {TranslateManager.Instance}");
        if (TranslateManager.Instance == null) return;
        _changeLenguage = value == 0;
        TranslateManager.Instance.ChangeLanguage(_changeLenguage ? "Spanish" : "English");
    }
}
