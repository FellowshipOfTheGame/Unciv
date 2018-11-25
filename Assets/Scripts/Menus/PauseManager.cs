using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour {

    public GameObject PauseMenu;
    public GameObject turnCanvas;
    public GameObject BaseMenu;//menu de pause inicial
    public GameObject OptionsMenu;//menu de opcoes utilizado

    private void Update()
    {   
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseMenu.SetActive(!(PauseMenu.activeSelf));//se o menu esta ativado ele é desativado e viceversa
            turnCanvas.SetActive(!(PauseMenu.activeSelf));//o oposto ocorre com o turn canvas( se o menu estava ativado ele é ativado)
            if ((PauseMenu.activeSelf))//quando o menu e ativado, timescale = 0
            {
                BaseMenu.SetActive(true);//garante a configuracao inicial do menu
                OptionsMenu.SetActive(false);
                Time.timeScale = 0;
            }
            else//quando o oposto ocorre timescale = 1
            {
                Time.timeScale = 1;
            }
        }
    }
}
