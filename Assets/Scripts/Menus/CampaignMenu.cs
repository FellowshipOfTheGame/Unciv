using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CampaignMenu : MonoBehaviour {

    public GameObject faccionButton;
    private RectTransform position;
    private List<Faccao> faccoes;

    public void generateFactionButtons(){
        //gets the faction list
        faccoes = CampaignControl.faccoes;
        //for each faction, creates a button for selecting it
        for (int i = 0; i < faccoes.Count; i++){
            GameObject button = Instantiate(faccionButton, this.transform, false);
            button.transform.SetSiblingIndex(i);
            button.GetComponentInChildren<Text>().text = faccoes[i].factionName;
        }
    }

    public void setNewGame ()
    {
        for (int i = 0; i < this.transform.childCount; i++)
            if (this.transform.GetChild(i).GetComponent<CampaignStart>())
                this.transform.GetChild(i).GetComponent<CampaignStart>().newGame = true;
        return;
    }

    public void setLoadGame ()
    {

    }
}
