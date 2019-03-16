using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CampaignMenu : MonoBehaviour {

    public GameObject levelButtonsParent;
    Button[] buttons;

    public void Start()
    {
        CampaignControl.Load();
        buttons = levelButtonsParent.GetComponentsInChildren<Button>();
    }

    public void SetFaction(string faction)
    {
        CampaignControl.isSkirmish = false;
        CampaignControl.actualFaction = faction;
    }

    public void SetLevel (int level)
    {
        CampaignControl.actualLevel = level;
    }

    public void NewGame(int level)
    {
        SetLevel(level);
        SceneManager.LoadScene(1);
    }

    public void ContinueGame ()
    {
        SetLevel(CampaignControl.FindLastLevel());
        SceneManager.LoadScene(1);
    }

    public void StartSkirmish()
    {
        CampaignControl.StartSkirmish();
    }

    public void LevelSelectionControl()
    {
        for (int i = 0; i < CampaignControl.faccoes.Count; i++)
            if (CampaignControl.actualFaction == CampaignControl.faccoes[i].factionName){
                CampaignControl.actualFactionIndex = i;
                break;
            }
        bool hasFoundNextLevel = false;
        for (int j = 0; j < 5; j++){
            if (CampaignControl.faccoes[CampaignControl.actualFactionIndex].completedLevels[j] == true)
                buttons[j].gameObject.SetActive(true);
            else if (!hasFoundNextLevel){
                hasFoundNextLevel = !hasFoundNextLevel;
                buttons[j].gameObject.SetActive(true);
            }
            else
                buttons[j].gameObject.SetActive(false);
        }
    }
}
