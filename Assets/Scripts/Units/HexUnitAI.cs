﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexUnitAI : HexUnit {

	public override int VisionRange {
		get {
			return 0;
		}
	}

    public override int Speed {
        get {
            return 100;
        }
    }

    public override bool IsValidDestination (HexCell cell) {
        if(Faccao=="Minor")
            return !cell.IsUnderwater && !cell.Unit && !cell.city;
		return cell.IsExplored && !cell.IsUnderwater && !cell.Unit && !cell.city;
	}

    public override int GetMoveCost (HexCell fromCell, HexCell toCell, HexDirection direction) {

		if (!IsValidDestination(toCell)) {
			return -1;
		}

		HexEdgeType edgeType = fromCell.GetEdgeType(toCell);
		if (edgeType == HexEdgeType.Cliff) {
			return 10;
		}
		int moveCost;
		if (fromCell.HasRoadThroughEdge(direction)) {
			moveCost = 3;
		}
		else if (fromCell.Walled != toCell.Walled) {
			return -1;
		}
		else {
			moveCost = edgeType == HexEdgeType.Flat ? 5 : 7;
			moveCost +=
				toCell.UrbanLevel + toCell.FarmLevel + toCell.PlantLevel;
		}
		return moveCost;
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
