using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class engineermenu : MonoBehaviour {

    public HexUnit EngUnit;
    public Canvas menuCanvas;
    public GameObject menuA;//é possivel construir
    public GameObject menuB;//não é possivel construir
    public bool construct;//se vai ou não construir
    public bool IsOpen;
    private void Awake()
    {
        menuCanvas.enabled = false;
        menuA.SetActive(false);
        menuB.SetActive(false);
        construct = false;
        IsOpen = false;
    }
    public void OpenMenu()
    {
        if(!IsOpen)
        {
            IsOpen = true;
            Debug.Log("abrindo o menu");
            menuCanvas.enabled = true;
            if (EngUnit.consdelay == 0 && EngUnit.CanCons)
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
    }

    public void close()
    {
        menuCanvas.enabled = false;
        menuA.SetActive(false);
        menuB.SetActive(false);
        
    }

    public void yes()
    {
        close();
        construct = true;
    }

    public void no_ok()
    {
        close();
        construct = false;
        IsOpen = false;
    }

}
