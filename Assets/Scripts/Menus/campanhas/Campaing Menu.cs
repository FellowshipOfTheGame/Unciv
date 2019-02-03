using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Campaing_Menu : MonoBehaviour {

	public void Continue()
    {

    }

    public void NewGame()
    {
        SceneManager.LoadScene(1);
    }
}
