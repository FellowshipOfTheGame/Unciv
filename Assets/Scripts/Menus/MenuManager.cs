using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour {

    public MapsErrorPanel errorPanel;

    private void Awake(){
        CampaignControl.Load();
        int check = CampaignControl.CheckCampaignMaps();
        if (check != 1)
        {
            errorPanel.gameObject.SetActive(true);
            if (check == -1)
                errorPanel.MissingMaps();
            else if (check == -2)
                errorPanel.MissingFactionFolder();
            else
                errorPanel.MissingFactionMaps();
            this.gameObject.SetActive(false);
        }
    }

    public void QuitGame()
    {
        Debug.Log("quit");
        Application.Quit();
    }
}
