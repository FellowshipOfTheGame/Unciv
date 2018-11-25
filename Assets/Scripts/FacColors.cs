using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacColors : MonoBehaviour {

	public Color[] facColors;
    public Renderer[] objs;
    //private string Fac;

    private void Start() {
        //Fac = this.GetComponent<HexUnit>().Faccao;
        Recolor();
    }

    void Recolor(){ 
            foreach (var o in objs) { 
                o.materials[0].color=facColors[0];
                o.materials[1].color=facColors[1];
                o.materials[2].color=facColors[2];
            }    
    }
}
