﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CampaingMenu : MonoBehaviour
{

    public void NewGame()
    {
        SceneManager.LoadScene(1);
    }
}
