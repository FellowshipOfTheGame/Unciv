using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Player : MonoBehaviour {

    private string Resources;
    public string Faccao;
    public HexGrid grid;
    public Canvas Lose;
    public SaveLoadMenu SLM;

    public Text Res;

    private void Awake() {
        //Res = individualCityMenu.transform.GetChild(1).GetComponent<Text>();
        Resources = "15";
    }

    private void Update() {
        Res.text = "Resources: " + Resources;

        if(grid.cities.Count<=0)
            Lose.gameObject.SetActive(true);
    }

    public void SetResources(string Bonus){ 
        int resources=IntParseFast(Resources); 
        int bonus = IntParseFast(Bonus); 
        resources+=bonus; 
        Resources=resources.ToString(); 
    }

    public void BuyUnit(string Cost) { 
        int resources=IntParseFast(Resources); 
        int cost = IntParseFast(Cost); 
        resources-=cost; 
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
