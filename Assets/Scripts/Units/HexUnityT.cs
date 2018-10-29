using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexUnityT : HexUnit {

    public override int Speed {
		get {
			return 12;
		}
	}

    public override int UnitPrefab {
		get {
			return 1;
		}
	}
}
