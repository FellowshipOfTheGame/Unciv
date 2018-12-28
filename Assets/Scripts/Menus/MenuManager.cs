using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    //Panel with all the error messeges
    public GameObject errorPanel;
    public CampaignMenu campaignMenu;
    private Text text;

    private void Awake(){
        //Loads the .save file
        CampaignControl.Load();
        for (int i = 0; i < CampaignControl.faccoes.Count; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                Debug.Log("Facção " + CampaignControl.faccoes[i].factionName + " fase " + j + " = " + CampaignControl.faccoes[i].completedLevels[j]);
            }
        }
        //check if all expected maps exists
        text = errorPanel.transform.Find("MapsMissing").GetComponent<Text>();
        int check = CampaignControl.CheckCampaignMaps();
        //if it doesnt, shows an error messege for the player
        if (check != 1)
        {
            errorPanel.SetActive(true);
            if (check == -1)
                text.text = "Maps Folder missing. Please, re-download your game";
            else if (check == -2)
                text.text = "A faction folder is missing. Please update your game";
            else
                text.text = "A faction folder is empty. Please re-download your game";
            this.gameObject.SetActive(false);
        }
        campaignMenu.generateFactionButtons();
    }

    public void QuitGame()
    {
        Debug.Log("quit");
        Application.Quit();
    }
}
