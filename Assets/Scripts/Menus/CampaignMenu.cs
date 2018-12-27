using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CampaignMenu : MonoBehaviour {

    public GameObject faccionButton;
    private RectTransform position;
    private List<Faccao> faccoes;

    private void Start(){
        //idea: this start function should instantiate all facctions buttons based on Faccao.faccoes
        faccoes = CampaignControl.faccoes;
        for (int i = 0; i < faccoes.Count; i++){
            GameObject aux = Instantiate(faccionButton);
            aux.transform.SetParent(this.transform, false);
            aux.transform.SetSiblingIndex(i);
            aux.GetComponentInChildren<Text>().text = faccoes[i].factionName;
        }
    }

    public void NewGameVisokea () {
        CampaignControl.actualFaccion = "Visokea";
        SceneManager.LoadScene(1);
    }

    public void NewGameGenericFaccion(){
        CampaignControl.actualFaccion = "GenericFaccion";
        SceneManager.LoadScene(1);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
