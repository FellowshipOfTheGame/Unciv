using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class engineermenu : MonoBehaviour {

    
    public HexUnit EngUnit;
    public Canvas menuCanvas;
    public GameObject menuA;//é possivel construir
    public GameObject menuB;//não é possivel construir


    private void Awake()
    {
        menuCanvas.enabled = false;
        menuA.SetActive(false);
        menuB.SetActive(false);
    }
    public void OpenMenu()
    {
        menuCanvas.enabled = true;

        if (EngUnit.consdelay == 0)
        {
            menuA.SetActive(true);
            menuB.SetActive(false);
        }
        else
        {
            menuA.SetActive(false);
            menuB.SetActive(true);
        }
    }

    public void construc()
    {
        
    }

}
