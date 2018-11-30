using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CampaignMenu : MonoBehaviour {

    public GameObject faccionButton;
    private RectTransform position;

    private void Start(){
        //idea: this start function should instantiate all facctions buttons based on Faccao.faccoes
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
