using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIbrain : MonoBehaviour {

	public List<HexUnit> Units;

    public Canvas Win;

    public void Activate() {
        if(Units.Count<=0) { 
            //Game Over Player Won!
            Win.gameObject.SetActive(true);
        }

        foreach (var U in Units) { 
                    
        }    
    }
}
