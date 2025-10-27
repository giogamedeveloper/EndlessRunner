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
        TranslateManager.OnLanguageChanged += () => ChangeText();
    }

    void OnDisable()
    {
        TranslateManager.OnLanguageChanged -= () => ChangeText();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _textMesh.text = TranslateManager.Instance.GetText(_text);
    }

    public void ChangeText()
    {
        if (_dropdown != null)
        {
            _textMesh.text = _dropdown.options[_dropdown.value].text;
            Debug.Log(_textMesh.text);
        }
        _textMesh.text = TranslateManager.Instance.GetText(_text);
    }
}
