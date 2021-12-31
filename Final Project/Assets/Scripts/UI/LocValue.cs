using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocValue : MonoBehaviour
{
    [SerializeField]
    string m_key;

    void Start()
    {
        LoadLocalizationData();
    }

    public void LoadLocalizationData() {
        Text text = GetComponent<Text>();
        if(text && m_key != "")
        {
            text.text = LocalizationManager.Instance.GetLocalizedValue(m_key);
        }
    }
}
