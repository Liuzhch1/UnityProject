using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public enum Languages
{
    English,
    Chinese
}

public class LocalizationManager : MonoBehaviour
{
    static LocalizationManager m_instance;
    public static LocalizationManager Instance => m_instance;

    [SerializeField]
    Languages m_currentLanguage = Languages.English;

    Dictionary<Languages, TextAsset> m_localizationFiles = new Dictionary<Languages, TextAsset>();

    Dictionary<string, string> m_localizationData;

    void LoadLocalizationFiles()
    {
        foreach(Languages language in Languages.GetValues(typeof(Languages)))
        {
            string textAssetPath = "Localization/" + language;
            TextAsset textAsset = (TextAsset)Resources.Load(textAssetPath);

            if(textAsset)
            {
                m_localizationFiles[language] = textAsset;
                Debug.Log("Added Localization file for language: " + language);
            }
            else
            {
                Debug.LogError("Couldn't load Localization file for language: " + language);
            }
        }
    }

    void LoadLocalizationData()
    {
        TextAsset textAsset;

        // Search for selected Language file
        if(m_localizationFiles.ContainsKey(m_currentLanguage))
        {
            Debug.Log("Selected language: " + m_currentLanguage);
            textAsset = m_localizationFiles[m_currentLanguage];
        }
        else
        {
            Debug.Log("Couldn't find language file for language: " + m_currentLanguage);
            Debug.Log("Defaulting to English LOL");
            textAsset = m_localizationFiles[Languages.English];
        }

        // Load XML document
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(textAsset.text);

        // Get all elements called "Entry"
        XmlNodeList entryList = xmlDocument.GetElementsByTagName("Entry");

        m_localizationData = new Dictionary<string, string>();
        // Iterate over elements
        foreach(XmlNode entry in entryList)
        {
            string key = entry.FirstChild.InnerText;
            string value = entry.LastChild.InnerText;

            if(!m_localizationData.ContainsKey(key))
            {
                // Add the key
                Debug.Log("Adding key: " + key + " with value: " + value);
                m_localizationData[key] = value;
            }
            else
            {
                Debug.LogError("Already existing key: " + key);
            }
        }
    }

    public string GetLocalizedValue(string key)
    {
        return m_localizationData[key];
    }

    public void SwitchLanguage(){
        m_currentLanguage = (m_currentLanguage == Languages.English) ? Languages.Chinese : Languages.English;
        LoadLocalizationData();
        foreach (var locValue in FindObjectsOfType<LocValue>()) {
            locValue.LoadLocalizationData();
        }
    }

    void CreateSingleton()
    {
        if(m_instance == null)
        {
            m_instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    void Awake()
    {
        CreateSingleton();
        LoadLocalizationFiles();
        LoadLocalizationData();
    }
}
