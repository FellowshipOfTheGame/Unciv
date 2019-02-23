using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class HexFort : MonoBehaviour {

    HexCell location;
	float orientation;
	int canSpawn;

    public HexCell Location {
		get {
			return location;
		}
		set {
			if (location) {
				location.Fort = null;
			}
			location = value;
			value.Fort = this;
			transform.localPosition = value.Position;
			Grid.MakeChildOfColumn(transform, value.ColumnIndex);
		}
	}

    public HexGrid Grid { get; set; }

	public float Orientation {
		get {
			return orientation;
		}
		set {
			orientation = value;
			transform.localRotation = Quaternion.Euler(0f, value, 0f);
		}
	}

	// Use this for initialization
	void Awake () {
		canSpawn=Random.Range(0,3);
	}

    public void SpawnUnit() {
		Debug.Log ("Spawnando");
        canSpawn++;
        if(canSpawn>=3) //se tiver mais que 3 turnos spawna
		//highlight all available cells around the city
		    for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
			    if (location.GetNeighbor(d) && !location.GetNeighbor(d).Unit && !location.GetNeighbor(d).IsUnderwater && location.GetElevationDifference(d) < 2
				    && !location.GetNeighbor(d).city) { 
                        canSpawn -= 3;
                        CreateUnit(location.GetNeighbor(d));
                }
                if(canSpawn<=0) //se nao tem mais como spawnar, cai fora, senao continua;
				    break;
		    }
        if(canSpawn<0)
            canSpawn=0;
	}
	
    void CreateUnit (HexCell cell) {
		if (cell && !cell.Unit) {
			Grid.AddUnit(Instantiate(HexGrid.unitPrefabsI[1]), cell, Random.Range(0f, 360f), "Minor", false);
		}
	}

    public void Save (BinaryWriter writer) {
		location.coordinates.Save(writer);
		writer.Write(orientation);
	}

    public static void Load (BinaryReader reader, HexGrid grid) {
		HexCoordinates coordinates = HexCoordinates.Load(reader);
		float orientation = reader.ReadSingle();
		grid.AddFort(
			Instantiate(HexGrid.fortPrefab), grid.GetCell(coordinates), orientation);
	}

    public void Destroy () {
		location.Fort = null;
		Destroy(gameObject);
	}

}
