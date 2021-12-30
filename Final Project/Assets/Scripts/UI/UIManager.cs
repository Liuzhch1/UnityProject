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

    Transform m_canvas;
    Transform m_pieMenu;
    Transform m_healthPanel;
    Transform m_ARPanel;
    Transform m_HandGunPanel;
    Transform m_crosshair;

    bool isAiming = false;
    UIState m_state = UIState.Game;
    public UIState State => m_state;

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
        if (m_canvas) {
            m_pieMenu = m_canvas.GetChild(0);
            m_healthPanel = m_canvas.GetChild(1);
            m_ARPanel = m_canvas.GetChild(2);
            m_HandGunPanel = m_canvas.GetChild(3);
            m_crosshair = m_canvas.GetChild(4);
        }
        
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

    public void displayWeapon(Weapon weapon) {
        m_ARPanel.gameObject.SetActive(weapon == Weapon.AR);
        m_HandGunPanel.gameObject.SetActive(weapon == Weapon.handgun);
    }
    
    public void setAmmoNumber(Weapon weapon, int ammoNumber, int magNumber) {
        // PieMenuLogic pieMenuLogic = FindObjectOfType<PieMenuLogic>();
        // if (pieMenuLogic)
        //     pieMenuLogic.setAmmoNumber(weapon, ammoNumber, magNumber);
        Transform weaponPanel = (weapon == Weapon.AR) ? m_ARPanel : m_HandGunPanel;
        Text ammoNumberText = weaponPanel.GetChild(1).GetComponent<Text>();
        Text magNumberText = weaponPanel.GetChild(2).GetComponent<Text>();
        ammoNumberText.text = "" + ammoNumber;
        magNumberText.text = "/" + magNumber;
    }

    public void setHealth(int health) {
        Slider healthSlider = m_healthPanel.GetChild(1).GetComponent<Slider>();
        healthSlider.value = health;
    }

    public void switchState(UIState state) {
        m_state = state;
        switch (state) {
            case UIState.Game:
                isAiming = false;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                m_pieMenu.gameObject.SetActive(false);
                m_crosshair.gameObject.SetActive(true);
                break;
            case UIState.Menu:
                //TODO: setactive menu ui
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                m_pieMenu.gameObject.SetActive(false);
                m_crosshair.gameObject.SetActive(false);
                break;
            case UIState.Inventory:
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
                m_pieMenu.gameObject.SetActive(true);
                m_crosshair.gameObject.SetActive(false);
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

        if (m_state == UIState.Game) {
            if (Input.GetButtonDown("Fire2")) {
                isAiming = !isAiming;
                m_crosshair.gameObject.SetActive(!isAiming);
            } 
        }


    }
}
