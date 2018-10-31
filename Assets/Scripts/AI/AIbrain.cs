﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIbrain : MonoBehaviour {

	public List<HexUnit> Units;
    public HexGrid grid;

    public Canvas Win;

    private int i;

    public void Activate() {
        if(Units.Count<=0) { 
            //Game Over Player Won!
            Win.gameObject.SetActive(true);
            return;
        }

        foreach (var U in Units) { 
            i=0;
            StartCoroutine(UnitAction(U));    
        }    
    }

    IEnumerator UnitAction(HexUnit U) {
        for (HexDirection D = HexDirection.NE; D <= HexDirection.NW; D++) {
                if (U.Location.GetNeighbor(D))
                    if (U.Location.GetNeighbor(D).Unit)
                        if(U.Location.GetNeighbor(D).Unit.Faccao!=U.Faccao){ 
				            U.Attack(U.Location.GetNeighbor(D).Unit);
                            i=3;
                            break;
                        }
            }
        
        if(i<3) {
            //se nao tiver ninguem para atacar
            HexDirection d = (HexDirection)Random.Range(0,6);
                grid.FindPath(U.Location, U.Location.GetNeighbor(d), U);
                    if (grid.HasPath) {
			            U.Travel(grid.GetPath());
			                grid.ClearPath();
		            }
            i++;
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(UnitAction(U));
        }
        yield return new WaitForSeconds(.1f);
    }
}
