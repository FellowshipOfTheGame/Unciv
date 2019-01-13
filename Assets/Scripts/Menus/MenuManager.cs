using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour {

    public void QuitGame()
    {
        Debug.Log("quit");
        Application.Quit();
    }
}
