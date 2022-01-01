using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DialogueLogic : MonoBehaviour
{
    Text m_dialogueText;
    string m_fullMessage;
    int m_charIndex = 0;
    float m_displayTimer = 0;    
    float m_charDisplayTimer = 0;

    [SerializeField]
    float m_charDisplayTime = 0.02f;

    void Start()
    {
        m_dialogueText = transform.GetChild(2).GetComponent<Text>();
    }

    void Update()
    {
        if (m_charIndex < m_fullMessage.Length) {
            if (m_charDisplayTimer < 0) {
                m_dialogueText.text += m_fullMessage[m_charIndex++]; 
                m_charDisplayTimer = m_charDisplayTime;
            } else {
                m_charDisplayTimer -= Time.deltaTime;
            }
        } else {
            if (m_displayTimer < 0) {
                transform.gameObject.SetActive(false);
            } else {
                m_displayTimer -= Time.deltaTime;
            }

        }
    }

    public void Display(string key, float displayTime) {
        m_displayTimer = displayTime;
        m_fullMessage = LocalizationManager.Instance.GetLocalizedValue(key);
        m_dialogueText.text = "";
        transform.gameObject.SetActive(true);
        m_charIndex = 0;
    }
}
