using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManage : MonoBehaviour {

    public SaveLoadMenu SLM;
    public Player player;


    public void Exit() {
        Application.Quit();
    }

    public void NextLevel() { 
        string path = Path.Combine (Application.persistentDataPath, Path.Combine("Maps", (player.level + 1).ToString() + ".map"));
        if (!File.Exists(path)) {
            Debug.Log("You finished all levels!");
            Transform panel = this.transform.GetChild(0);
            Text aux = panel.GetChild(1).GetComponent<Text>();
            aux.text = "Congratualations! You finished all levels";
            panel.GetChild(0).gameObject.SetActive(false);
        }
        else {
            this.gameObject.SetActive(false);
            SLM.Load (path);
            player.level++;
        }
    }
}
