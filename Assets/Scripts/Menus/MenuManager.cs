using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {

    public void NewGame()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadGame()
    {

    }

    public void QuitGame()
    {
        Debug.Log("quit");
        Application.Quit();
    }
}
