using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour {

    private Camera Camera;

	public Canvas HBC;
    public HexUnit HU;
    private Image HB;

    private float maxHP;
    private float curHP;

    private void Awake() {
        maxHP=curHP=HU.HitP;
        HB=HBC.transform.GetChild(0).gameObject.GetComponent<Image>();
        HBcalc();
        Camera = GameObject.FindGameObjectWithTag("MainaCamera").GetComponent<Camera>();
    }

    public void SetHP(int hp) { 
        curHP = (float)hp;    
    }

    private void HBcalc() {
        if (curHP>maxHP){ 
            curHP=maxHP;
            HU.HitP=(int)maxHP;
        }
        HB.fillAmount=(curHP/maxHP);    
    }

    private void LateUpdate() {
            HBC.transform.LookAt(transform.position + Camera.transform.rotation * Vector3.forward,
            Camera.transform.rotation * Vector3.up);
    }
}
