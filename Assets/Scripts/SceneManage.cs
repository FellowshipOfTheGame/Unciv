using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManage : MonoBehaviour {

    public void Exit() {
        Exit();
    }

    public void NewGame() { 
        SceneManager.LoadScene(0);
    }
}
