using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CampaignStart : MonoBehaviour
{
    public void NewGame (){
        CampaignControl.actualFaccion = this.GetComponentInChildren<Text>().text;
        SceneManager.LoadScene(1);
    }
}
