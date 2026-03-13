using TMPro;
using UnityEngine;

public class TranslateText : MonoBehaviour
{
    [SerializeField]
    private string _text;

    [SerializeField]
    private TMP_Dropdown _dropdown;

    [SerializeField] TextMeshProUGUI _textMesh;

    void OnEnable()
    {
        TranslateManager.OnLanguageChanged += ChangeText;
        if (TranslateManager.Instance != null)
            ChangeText();
    }

    void OnDisable()
    {
        TranslateManager.OnLanguageChanged -= ChangeText;
    }

    void Start()
    {
        if (TranslateManager.Instance != null)
            _textMesh.text = TranslateManager.Instance.GetText(_text);
    }

    public void ChangeText()
    {
        if (TranslateManager.Instance == null) return;
        if (_textMesh == null) return;
        if (_dropdown != null)
            _textMesh.text = _dropdown.options[_dropdown.value].text;
        else
            _textMesh.text = TranslateManager.Instance.GetText(_text);
    }
}
