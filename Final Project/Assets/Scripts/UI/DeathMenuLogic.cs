using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenuLogic : MonoBehaviour
{
    public void Load() {
        FPSplayerLogic playerLogic = FindObjectOfType<FPSplayerLogic>();
        SaveManager.Instance.Load();
        playerLogic.DisplayRespawn();
    }

    public void Menu() {
        SceneManager.LoadScene("MainMenuScene");
    }
}
