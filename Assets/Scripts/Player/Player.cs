using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    private string Resources;
    public string Faccao;

    //Text Res;

    private void Awake() {
        //Res = individualCityMenu.transform.GetChild(1).GetComponent<Text>();
        Resources = "30";
    }

    private void Update() {
        //Res.text = "Resources: " + Resources;
    }

    public void SetResources(string Bonus){ 
        int resources=IntParseFast(Resources); 
        int bonus = IntParseFast(Bonus); 
        resources+=bonus; 
        Resources=resources.ToString(); 
    }

    public bool CanCreate(string cost) {
        return IntParseFast(Resources)>=IntParseFast(cost);
    }
	
    //conversor string to int
    public int IntParseFast(string value) {
        int result = 0;
        for (int i = 0; i < value.Length; i++) {
            char letter = value[i];
            result = 10 * result + (letter - 48);
        }
        return result;
    }
}
