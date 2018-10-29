using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacColors : MonoBehaviour {

	public Color[] facColors;
    public Renderer[] objs;
    private string Fac;

    private void Start() {
        Fac = this.GetComponent<HexUnit>().Faccao;
        Recolor();
    }

    void Recolor(){ 
        if(Fac=="Visokea") { 
            foreach (var o in objs) { 
                o.materials[0].color=facColors[0];
                o.materials[1].color=facColors[1];
                o.materials[2].color=facColors[2];
            }    
        }
        else
            foreach (var o in objs) { 
                o.materials[0].color=new Color( Random.value, Random.value, Random.value, 1.0f );
                o.materials[1].color=new Color( Random.value, Random.value, Random.value, 1.0f );
                o.materials[2].color=new Color( Random.value, Random.value, Random.value, 1.0f );
            }
    }
}
