using UnityEngine;

public class Settings : MonoBehaviour
{
    bool _changeLenguage;
    public void SetQuality(int qualityIndex)
    {
        Debug.Log("SetQuality");
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    public void ChangeIdioma(int value)
    {
        if (value == 0)
            _changeLenguage = true;
        else if (value == 1)
            _changeLenguage = false;
        Debug.Log(value);
        TranslateManager.Instance.ChangeLanguage(_changeLenguage ? "Spanish" : "English");
    }
}
