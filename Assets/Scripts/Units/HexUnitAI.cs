using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexUnitAI : HexUnit {

	public override int VisionRange {
		get {
			return 0;
		}
	}

    private void Update() {
        if(!HexMapEditor.isEditMode)
            if(Location.IsVisible) {
                this.transform.GetChild(0).gameObject.SetActive(true);
                this.transform.GetChild(1).gameObject.SetActive(true);
            }
            else {
                this.transform.GetChild(0).gameObject.SetActive(false);
                this.transform.GetChild(1).gameObject.SetActive(false);
            }
        else {
            this.transform.GetChild(0).gameObject.SetActive(true);
            this.transform.GetChild(1).gameObject.SetActive(true);
        }
    }
}
