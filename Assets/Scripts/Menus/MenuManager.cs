using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour {

    private void Awake(){
        CampaignControl.Load();
    }

    public void QuitGame()
    {
        Debug.Log("quit");
        Application.Quit();
    }
}
