using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Speaker{
    Agent,
    Commander
}

public struct Dialogue {
    public Speaker Speaker;
    public string Content;
    public float DisplayTime;

    public Dialogue(Speaker speaker, string content, float displayTime) {
        this.Speaker = speaker;
        this.Content = content;
        this.DisplayTime = displayTime;
    }
}

public class DialogueLogic : MonoBehaviour
{
    Transform m_playerDialogue;
    Transform m_npcDialogue;
    AudioSource m_audioSource;
    Queue<Dialogue> m_queue = new Queue<Dialogue>();
    Dialogue m_currentDialogue;

    Text m_dialogueText;
    string m_fullMessage;
    int m_charIndex = 0;

    float m_displayTimer = 0;    
    float m_charDisplayTimer = 0;

    [SerializeField]
    float m_charDisplayTime = 0.05f;

    [SerializeField]
    AudioClip m_dialogueSound;

    void Start()
    {
        m_playerDialogue = transform.GetChild(0);
        m_npcDialogue = transform.GetChild(1);
        m_audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Skip")) {
            m_displayTimer = 0;
        }
        if (m_displayTimer > 0) {
            // Dialogue queue is busy
            if (m_charIndex < m_currentDialogue.Content.Length) {
                // Display each character
                m_charDisplayTimer -= Time.deltaTime;
                if (m_charDisplayTimer <= 0) {
                    m_dialogueText.text += m_currentDialogue.Content[m_charIndex++]; 
                    m_charDisplayTimer = m_charDisplayTime;
                }
            } else {
                // Display whole sentence for a duration
                m_displayTimer -= Time.deltaTime;
                if (m_displayTimer <= 0) {
                    
                }
            }
        } else {
            m_playerDialogue.gameObject.SetActive(false);
            m_npcDialogue.gameObject.SetActive(false);
            // Dialogue queue is idle
            if (m_queue.Count > 0) {
                m_currentDialogue = m_queue.Dequeue();
                m_audioSource.PlayOneShot(m_dialogueSound);
                m_displayTimer = m_currentDialogue.DisplayTime;
                m_charIndex = 0;
                Transform m_dialogue = (m_currentDialogue.Speaker == Speaker.Agent) ? m_playerDialogue : m_npcDialogue;
                m_dialogue.gameObject.SetActive(true);
                m_dialogueText = m_dialogue.GetChild(2).GetComponent<Text>();
                m_dialogueText.text = "";
            }
        }
    }

    public void Display(Speaker speaker, string key, float displayTime) {
        m_queue.Enqueue(new Dialogue(speaker, LocalizationManager.Instance.GetLocalizedValue(key), displayTime));
    }

    void PlaySound(AudioClip tmp, float volume = 1.0f)
    {
        m_audioSource.volume = volume;
        m_audioSource.PlayOneShot(tmp);
    }
}
