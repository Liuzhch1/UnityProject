using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Speaker{
    Agent,
    Commander
}

public class DialogueLogic : MonoBehaviour
{
    Transform m_playerDialogue;
    Transform m_npcDialogue;
    
    Text m_dialogueText;
    string m_fullMessage;
    int m_charIndex = 0;
    float m_displayTimer = 0;    
    float m_charDisplayTimer = 0;

    [SerializeField]
    float m_charDisplayTime = 0.02f;

    void Start()
    {
        m_playerDialogue = transform.GetChild(0);
        m_npcDialogue = transform.GetChild(1);
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
                m_playerDialogue.gameObject.SetActive(false);
            m_npcDialogue.gameObject.SetActive(false);
            } else {
                m_displayTimer -= Time.deltaTime;
            }
        }
    }

    public void Display(Speaker speaker, string key, float displayTime) {
        m_displayTimer = displayTime;
        m_fullMessage = LocalizationManager.Instance.GetLocalizedValue(key);
        Transform m_dialogue = (speaker == Speaker.Agent) ? m_playerDialogue : m_npcDialogue;
        m_playerDialogue.gameObject.SetActive(speaker == Speaker.Agent);
        m_npcDialogue.gameObject.SetActive(speaker == Speaker.Commander);
        m_dialogueText = m_dialogue.GetChild(2).GetComponent<Text>();
        m_dialogueText.text = "";
        m_charIndex = 0;
    }
}
