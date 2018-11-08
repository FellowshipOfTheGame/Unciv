using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{

    public GameObject Menupanel;//painel de menu de pausa

    public void QuitGame()
    {
        SceneManager.LoadScene(0);
    }

    public void resume()
    {
        Time.timeScale = 1;
        Menupanel.SetActive(false);
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        Menupanel.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }

}