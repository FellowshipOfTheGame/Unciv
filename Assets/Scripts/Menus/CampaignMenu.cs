using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CampaignMenu : MonoBehaviour {

    public GameObject faccionButton;
    private RectTransform position;
    private List<Faccao> faccoes;

    private void Start(){
        //idea: this start function should instantiate all facctions buttons based on Faccao.faccoes
        faccoes = CampaignControl.faccoes;
        for (int i = 0; i < faccoes.Count; i++){
            GameObject button = Instantiate(faccionButton, this.transform, false);
            button.transform.SetSiblingIndex(i);
            button.GetComponentInChildren<Text>().text = faccoes[i].factionName;
        }
    }
}
