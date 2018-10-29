﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

public class HexCity : MonoBehaviour {

	public static GameObject cityMenuCanvas;
	public static GameObject cityMenu;
    Text Res;
	GameObject individualCityMenu;
	HexCell location;
	float orientation;
	bool canSpawn;

    public int Resources=30;
    public int ResPT=5;

	//informs if the city menu is active
	public bool IsCityMenuActivated {
		get {
			return individualCityMenu.activeSelf;
		}
	}

	public virtual int VisionRange {
		get {
			return 6;
		}
	}

	public HexCell Location {
		get {
			return location;
		}
		set {
			if (location) {
				Grid.DecreaseVisibility(location, VisionRange);
				location.city = null;
			}
			location = value;
			value.city = this;
			Grid.IncreaseVisibility(value, VisionRange);
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

	void Awake() {
		//instantiate an city menu for this city
		individualCityMenu = Instantiate(cityMenu);
		//puts it in city canvas
		individualCityMenu.transform.SetParent(cityMenuCanvas.transform, false);
		//sets the button to spawning units
		individualCityMenu.GetComponentInChildren<Button> ().onClick.AddListener(SpawnUnit);

        Res = individualCityMenu.transform.GetChild(1).GetComponent<Text>();
		//moves the panel
		Vector3 aux = new Vector3(475, 0, 0);
		individualCityMenu.transform.localPosition = aux;
		canSpawn = true;
	}

	void Update () {
        Res.text = "Resources: " + Resources.ToString();
		//if an unity can be spawned
		if (canSpawn == true) {
			//when right click on a cell
			if (Input.GetMouseButtonDown (0)) {
				//get the cell
				HexCell aux2 = GetCellUnderCursor();
				//search the selected cell beetween location's neighbors, while disabling the highlights
				for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
					//if the correct cell is found, instantiate an unit there
					if (location.GetNeighbor(d) && location.GetNeighbor (d) == aux2 && !(aux2 = location.GetNeighbor (d)).Unit && !aux2.IsUnderwater && location.GetElevationDifference(d) < 2
						&& !location.GetNeighbor(d).city) {
                        if(Resources >= HexGrid.unitPrefabs[0].cost){
						    CreateUnit (aux2);
                            Resources-=HexGrid.unitPrefabs[0].cost;
                        }
					}
					location.GetNeighbor (d).DisableHighlight ();
				}
				canSpawn = false;
			}
		}
        Grid.IncreaseVisibility(Location, VisionRange);
        
	}

	//simply actives the city menu
	public void ActiveCityMenu() {
		individualCityMenu.SetActive (true);
		return;
	}

	//simply deactives the city menu
	public void DeactiveCityMenu() {
		individualCityMenu.SetActive (false);
		return;
	}

	public static void DeactiveAllCitiesMenus() {
		for (int i = 0; i < HexCity.cityMenuCanvas.transform.childCount; ++i) {
			GameObject aux = HexCity.cityMenuCanvas.transform.GetChild(i).gameObject;
			if (aux) {
				aux.SetActive (false);
			}
		}

		return;
	}

	public void SpawnUnit() {
		Debug.Log ("Spawnando");
		//highlight all available cells around the city
		for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
			if (location.GetNeighbor(d) && !location.GetNeighbor(d).Unit && !location.GetNeighbor(d).IsUnderwater && location.GetElevationDifference(d) < 2
				&& !location.GetNeighbor(d).city)
				location.GetNeighbor (d).EnableHighlight (Color.blue);
		}
		//stores the information that an unity can be spawned
		canSpawn = true;
		return;
	}

	void CreateUnit (HexCell cell) {
		if (cell && !cell.Unit) {
			Grid.AddUnit(Instantiate(HexGrid.unitPrefabs[0]), cell, Random.Range(0f, 360f), "Visokea");
		}
	}

	public HexCell GetCellUnderCursor () {
		return Grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
	}

	public void Destroy () {
		if (location) {
			Grid.DecreaseVisibility(location, VisionRange);
		}
		location.city = null;
		Destroy(gameObject);
	}

    public void Save (BinaryWriter writer) {
		location.coordinates.Save(writer);
		writer.Write(orientation);
	}

    public static void Load (BinaryReader reader, HexGrid grid) {
		HexCoordinates coordinates = HexCoordinates.Load(reader);
		float orientation = reader.ReadSingle();
		grid.AddCity(
			Instantiate(HexGrid.cityPrefabs[0]), grid.GetCell(coordinates), orientation
		);
	}
}
