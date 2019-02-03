﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engineer : HexUnit {

    public override int Speed
    {
        get
        {
            return 20;
        }
    }

    public override int VisionRange
    {
        get
        {
            return 5;
        }
    }

    public override bool IsValidDestination(HexCell cell)
    {
        return cell.IsExplored && (cell.WaterLevel - cell.Elevation < 3) && !cell.Unit && !cell.city && !cell.Fort;
    }

    public override int GetMoveCost(HexCell fromCell, HexCell toCell, HexDirection direction)
    {
        if (!IsValidDestination(toCell))
        {
            return -1;
        }

        int moveCost;
        if (fromCell.HasRoadThroughEdge(direction))
        {
            moveCost = 4;
        }
        else
        {
            moveCost = 5;

        }
        return moveCost;
    }
}
