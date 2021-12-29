using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//using UnityEngine.Rendering.PostProcessing;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance => m_instance;
    static UIManager m_instance;

    //PostProcessVolume[] m_volumes;
    Text m_ammoNumberText;
    Text m_healthText;
    Image[] m_ammoIcons;

    void Awake() {
        if(m_instance == null) {
            m_instance = this;
        } else {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        Cursor.visible = false;
        //m_volumes = FindObjectsOfType<PostProcessVolume>();
        m_ammoNumberText = transform.GetChild(0).GetChild(0).GetComponent<Text>();
        m_ammoIcons = transform.GetChild(0).GetChild(3).GetComponentsInChildren<Image>();
        m_healthText = transform.GetChild(1).GetChild(0).GetComponent<Text>();
    }

    public void Retry() {
        SceneManager.LoadScene("GameScene");
    }

    public void Menu() {
        SceneManager.LoadScene("MenuScene");
    }

    public void DisplayRetryPanel() {
        transform.GetChild(3).gameObject.SetActive(true);
    }

    public void setAmmoNumber(int ammoNumber) {
        m_ammoNumberText.text = "" + ammoNumber;
        for (int i = 0; i < 3; i++) {
            m_ammoIcons[i].enabled = ammoNumber > i * 10;
        }
    }

    public void setHealth(int health) {
        m_healthText.text = "" + health;
    }

    void Update()
    {
        if (Input.GetButtonDown("Menu")) {
            bool isActive = transform.GetChild(2).gameObject.activeSelf;
            isActive = !isActive;
            //m_volumes[0].weight = isActive ? 0 : 1;
            //m_volumes[1].weight = isActive ? 1 : 0;
            transform.GetChild(2).gameObject.SetActive(isActive);
        }
    }
}
