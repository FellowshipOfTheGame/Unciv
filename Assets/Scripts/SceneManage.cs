using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManage : MonoBehaviour {

    public SaveLoadMenu SLM;
    public CampaignControl campaign;


    public void Exit() {
        campaign.NextLevel();
        CampaignControl.Save();
        SceneManager.LoadScene(0);
    }

    public void NextLevel() {
        if (!campaign.NextLevel())
        {
            this.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "Congratulations!\n You finished all levels";
            this.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            CampaignControl.Save();
        }
        else
        {
            this.gameObject.SetActive(false);
            CampaignControl.Save();
        }
    }
}
