using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;

public class TranslateManager : MonoBehaviour
{
    [Header("References text Buttons")]
    // public TextMeshProUGUI[] _startText;
    //
    // [SerializeField] string[] _texts;
    public string defaultLanguage = "Spanish";

    public Dictionary<string, string> texts;

    public static TranslateManager Instance { get; private set; }

    public static Action OnLanguageChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadLenguage();
        
    }

    private void LoadLenguage()
    {
        string savedLanguage = PlayerPrefs.GetString("language", "");
        string systemLanguage = string.IsNullOrEmpty(savedLanguage)
            ? Application.systemLanguage.ToString()
            : savedLanguage;

        TextAsset textAsset = Resources.Load<TextAsset>(systemLanguage);
        if (textAsset == null)
            textAsset = Resources.Load<TextAsset>(defaultLanguage);

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(textAsset.text);
        LoadTexts(xmlDoc);
    }

    void LoadTexts(XmlDocument xmlDoc)
    {
        //Inicializamos el diccionario
        texts = new Dictionary<string, string>();
        //Recuperamos el bloque con el idioma seleccionado
        XmlElement element = xmlDoc.DocumentElement["lang"];
        if (element == null)
        {
            Debug.LogError("No se encontró el elemento 'lang' en el XML.");
            return;
        }
        foreach (XmlNode node in element.ChildNodes)
        {
            if (node.NodeType == XmlNodeType.Element)
            {
                XmlElement xmlItem = (XmlElement)node;
                string key = xmlItem.GetAttribute("key");
                string value = xmlItem.InnerText;
                if (!string.IsNullOrEmpty(key))
                {
                    if (!texts.ContainsKey(key))
                    {
                        texts.Add(key, value);
                    }
                    else
                    {
                        Debug.LogWarning($"Clave duplicada en XML: {key}. Se omite o reemplaza según configuración.");
                    }
                }
            }
        }
    }

    /// <summary>
    /// Devuelve el tecto que coincida con la key proporcionada
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public string GetText(string key)
    {
        if (texts == null)
        {
            Debug.LogWarning("Dictionary 'texts' is not initialized. Attempting to load language.");
            LoadLenguage(); // Forzar la carga

            // Si después de cargar sigue siendo null, devolver la key.
            if (texts == null)
            {
                Debug.LogError("Failed to initialize language dictionary.");
                return key;
            }
        }
        //Si no existe la clave indicada
        if (!texts.ContainsKey(key))
        {
            //mostramos un warning y retornamos la key tal cual
            Debug.LogWarning("La key" + key + "no existe.");
            return key;
        }
        //Si existe, devolvemos el texto correspondiente directamente
        return texts[key];
    }

    public void ChangeLanguage(string targetLanguage)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(targetLanguage);
        if (textAsset == null)
        {
            Debug.LogWarning($"Language file '{targetLanguage}' not found. Loading default '{defaultLanguage}'.");
            textAsset = Resources.Load<TextAsset>(defaultLanguage);
            if (textAsset == null)
            {
                Debug.LogError($"Default language file '{defaultLanguage}' not found!");
                return;
            }
        }
        PlayerPrefs.SetString("language", targetLanguage); // ← guarda la elección
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(textAsset.text);
        LoadTexts(xmlDoc);
        OnLanguageChanged?.Invoke();
    }
}
