using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    public GameObject Menupanel;//painel de menu de pausa

    public void ResturnTitleScreen()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void resume()
    {
        Time.timeScale = 1;
        Menupanel.SetActive(false);
    }
}