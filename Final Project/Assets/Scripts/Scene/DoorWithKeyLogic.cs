using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorWithKeyLogic : MonoBehaviour
{
    Animator m_animator;
    GameObject m_player;
    WeaponLogic m_weaponLogic;
    AudioSource m_audioSource;
    bool isOpen = false;
    [SerializeField]
    AudioClip m_doorSound;
    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_weaponLogic = FindObjectOfType<WeaponLogic>();
        m_audioSource = GetComponent<AudioSource>();
    }
    // Update is called once per frame
    void Update()
    {
        if (isOpen == false && Input.GetKeyDown(KeyCode.E) && Vector3.Distance(transform.position, m_player.transform.position) < 3.0f && !m_weaponLogic.m_hasKey)
        {
            UIManager.Instance.displayDialogue(Speaker.Agent, "DoorDialogue", 3.0f);
        }
        if (isOpen == false && Input.GetKeyDown(KeyCode.E) && Vector3.Distance(transform.position, m_player.transform.position) < 3.0f && m_weaponLogic.m_hasKey){
            m_audioSource.PlayOneShot(m_doorSound);
            m_animator.SetBool("isOpen",true);
            isOpen = true;
        }
        else if(isOpen == true && Input.GetKeyDown(KeyCode.E) && Vector3.Distance(transform.position, m_player.transform.position) < 3.0f){
            m_animator.SetBool("isOpen",false);
            isOpen = false;
        }
        if(isOpen == true && Vector3.Distance(transform.position, m_player.transform.position) > 10.0f){
            m_animator.SetBool("isOpen",false);
            isOpen = false;
        }

    }
}

