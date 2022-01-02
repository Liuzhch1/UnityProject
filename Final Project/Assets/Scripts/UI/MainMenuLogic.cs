using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuLogic : MonoBehaviour
{
    void Update()
    {
        Cursor.visible = true;
    }

    public void OnStartClicked()
    {
        PlayerPrefs.SetInt("Load", 0);
        SceneManager.LoadScene("MainMapScene");
    }

    public void OnContinueClicked()
    {
        PlayerPrefs.SetInt("Load", 1);
        SceneManager.LoadScene("MainMapScene");
    }

    public void OnLanguageClicked()
    {
        LocalizationManager.Instance.SwitchLanguage();
    }

    public void OnQuitClicked()
    {
        Application.Quit();
    }
}
