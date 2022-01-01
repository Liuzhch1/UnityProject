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
        SceneManager.LoadScene("MainMapScene");
    }

    public void OnContinueClicked()
    {
        //SceneManager.LoadScene("MainMapScene", );
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
