using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManage : MonoBehaviour {

    public void Exit() {
        Application.Quit();
    }

    public void NewGame() { 
        SceneManager.LoadScene(0);
    }
}
