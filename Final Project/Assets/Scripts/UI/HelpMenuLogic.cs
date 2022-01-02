using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HelpMenuLogic : MonoBehaviour
{
    public void Continue() {
        UIManager.Instance.switchState(UIState.Game);
    }
    
    public void Load() {
        FPSplayerLogic playerLogic = FindObjectOfType<FPSplayerLogic>();
        SaveManager.Instance.Load();
        UIManager.Instance.switchState(UIState.Game);
    }

    public void Menu() {
        SceneManager.LoadScene("MainMenuScene");

    }
}
