using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisokeaF : HexUnit {

	public override int Speed {
		get {
			return 20;
		}
	}

    public override int VisionRange {
		get {
			return 5;
		}
	}

    public override int GetMoveCost(HexCell fromCell, HexCell toCell, HexDirection direction) {
        if (!IsValidDestination(toCell)) {
			return -1;
		}
        HexEdgeType edgeType = fromCell.GetEdgeType(toCell);
		int moveCost;
		if (fromCell.HasRoadThroughEdge(direction)) {
			moveCost = 2;
		}
		else {
			moveCost = 5;
			moveCost +=
				toCell.UrbanLevel + toCell.FarmLevel + toCell.PlantLevel;
		}
		return moveCost;
    }
}
