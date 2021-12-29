using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//using UnityEngine.Rendering.PostProcessing;

public enum UIState{
    Game,
    Inventory,
    Menu
}

public class UIManager : MonoBehaviour
{
    public static UIManager Instance => m_instance;
    static UIManager m_instance;

    //PostProcessVolume[] m_volumes;
    Transform m_canvas;
    Transform m_pieMenu;
    UIState m_state = UIState.Game;
    public UIState State => m_state;
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
        Cursor.lockState = CursorLockMode.Locked;
        m_canvas = GameObject.Find("Canvas").transform;
        m_pieMenu = m_canvas.GetChild(0);
        //m_volumes = FindObjectsOfType<PostProcessVolume>();
        // m_ammoNumberText = transform.GetChild(0).GetChild(0).GetComponent<Text>();
        // m_ammoIcons = transform.GetChild(0).GetChild(3).GetComponentsInChildren<Image>();
        // m_healthText = transform.GetChild(1).GetChild(0).GetComponent<Text>();
    }

    // public void Retry() {
    //     SceneManager.LoadScene("GameScene");
    // }

    // public void Menu() {
    //     SceneManager.LoadScene("MenuScene");
    // }

    // public void DisplayRetryPanel() {
    //     transform.GetChild(3).gameObject.SetActive(true);
    // }

    public void setAmmoNumber(int ammoNumber) {
        m_ammoNumberText.text = "" + ammoNumber;
        for (int i = 0; i < 3; i++) {
            m_ammoIcons[i].enabled = ammoNumber > i * 10;
        }
    }

    public void setHealth(int health) {
        m_healthText.text = "" + health;
    }

    public void switchState(UIState state) {
        m_state = state;
        switch (state) {
            case UIState.Game:
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                m_pieMenu.gameObject.SetActive(false);
                break;
            case UIState.Menu:
                //TODO: setactive menu ui
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                m_pieMenu.gameObject.SetActive(false);
                break;
            case UIState.Inventory:
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
                m_pieMenu.gameObject.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                break;
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Menu")) {
            if (m_state == UIState.Game) {
                //transform.GetChild(2).gameObject.SetActive(true);
                switchState(UIState.Menu);
            } else if (m_state == UIState.Menu) {
                //transform.GetChild(2).gameObject.SetActive(false);
                switchState(UIState.Game);
            }
        }
        if (Input.GetButton("Inventory")) {
            if (m_state == UIState.Game) {
                switchState(UIState.Inventory);
            }
        } else {
            if (m_state == UIState.Inventory) {
                switchState(UIState.Game);
            }
        }


    }
}
