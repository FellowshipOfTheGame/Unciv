using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CampaignStart : MonoBehaviour
{
    //if true, sets all completed levels of a faction to false. Else, just load the game
    public bool newGame;

    public void StartGame (){
        if (newGame)
        {
            for (int i = 0; i < CampaignControl.faccoes.Count; i++)
                //finds the facction to be started again
                if (CampaignControl.faccoes[i].factionName == this.GetComponentInChildren<Text>().text)
                {
                    //sets all completed levels to false
                    for (int j = 0; j < 10; j++)
                        CampaignControl.faccoes[i].completedLevels[j] = false;
                    break;
                }
            //starts the game
            CampaignControl.actualFaction = this.GetComponentInChildren<Text>().text;
            SceneManager.LoadScene(1);
        }
        else
        {
            //simply starts the game
            CampaignControl.actualFaction = this.GetComponentInChildren<Text>().text;
            SceneManager.LoadScene(1);
        }
    }
}
