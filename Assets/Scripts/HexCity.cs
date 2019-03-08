using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

public class HexCity : MonoBehaviour {

	public static GameObject cityMenuCanvas;
	public static GameObject cityMenu;
    
	GameObject individualCityMenu;
	HexCell location;
	float orientation;
	bool canSpawnZeppelin;
    bool canSpawnSupport;
    bool canSpawnAntiAircraft;
    bool canSpawnTank;

    private Player P;
    public string ResPT="5";
    public string cost="50";

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
        P = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
		//instantiate an city menu for this city
		individualCityMenu = Instantiate(cityMenu);
		//puts it in city canvas
		individualCityMenu.transform.SetParent(cityMenuCanvas.transform, false);
		//sets the button to spawning units
		individualCityMenu.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(SpawnZeppelin);
        individualCityMenu.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(SpawnSupport);
        individualCityMenu.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(SpawnAntiAircraft);
        individualCityMenu.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(SpawnTank);


        //moves the panel
        Vector3 aux = new Vector3(475, 0, 0);
		individualCityMenu.transform.localPosition = aux;
		canSpawnZeppelin = true;
        canSpawnSupport = true;
        canSpawnAntiAircraft = true;
        canSpawnTank = true;
    }

	void Update () {
        
		//if an unity can be spawned
		if (canSpawnZeppelin == true) {
			//when right click on a cell
			if (Input.GetMouseButtonDown (0)) {
				//get the cell
				HexCell aux2 = GetCellUnderCursor();
				//search the selected cell beetween location's neighbors, while disabling the highlights
				for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
					//if the correct cell is found, instantiate an unit there
					if (location.GetNeighbor(d) && location.GetNeighbor (d) == aux2 && !(aux2 = location.GetNeighbor (d)).Unit && !aux2.IsUnderwater && location.GetElevationDifference(d) < 2
						&& !location.GetNeighbor(d).city) {
                        if(P.CanCreate(HexGrid.unitPrefabs[0].cost)){
						    CreateZeppelin (aux2);
                            P.BuyUnit(HexGrid.unitPrefabs[0].cost);
                        }
					}
					
				}
				canSpawnZeppelin = false;
			}
		}
        if (canSpawnSupport == true){
            //when right click on a cell
            if (Input.GetMouseButtonDown(0))
            {
                //get the cell
                HexCell aux2 = GetCellUnderCursor();
                //search the selected cell beetween location's neighbors, while disabling the highlights
                for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
                {
                    //if the correct cell is found, instantiate an unit there
                    if (location.GetNeighbor(d) && location.GetNeighbor(d) == aux2 && !(aux2 = location.GetNeighbor(d)).Unit && !aux2.IsUnderwater && location.GetElevationDifference(d) < 2
                        && !location.GetNeighbor(d).city)
                    {
                        if (P.CanCreate(HexGrid.unitPrefabs[1].cost))
                        {
                            CreateSupport(aux2);
                            P.BuyUnit(HexGrid.unitPrefabs[1].cost);
                        }
                    }

                }
                canSpawnSupport = false;
            }
        }
        if (canSpawnAntiAircraft == true)
        {
            //when right click on a cell
            if (Input.GetMouseButtonDown(0))
            {
                //get the cell
                HexCell aux2 = GetCellUnderCursor();
                //search the selected cell beetween location's neighbors, while disabling the highlights
                for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
                {
                    //if the correct cell is found, instantiate an unit there
                    if (location.GetNeighbor(d) && location.GetNeighbor(d) == aux2 && !(aux2 = location.GetNeighbor(d)).Unit && !aux2.IsUnderwater && location.GetElevationDifference(d) < 2
                        && !location.GetNeighbor(d).city)
                    {
                        if (P.CanCreate(HexGrid.unitPrefabs[2].cost))
                        {
                            CreateAntiAircraft(aux2);
                            P.BuyUnit(HexGrid.unitPrefabs[2].cost);
                        }
                    }

                }
                canSpawnAntiAircraft = false;
            }
        }
        if (canSpawnTank == true)
        {
            //when right click on a cell
            if (Input.GetMouseButtonDown(0))
            {
                //get the cell
                HexCell aux2 = GetCellUnderCursor();
                //search the selected cell beetween location's neighbors, while disabling the highlights
                for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
                {
                    //if the correct cell is found, instantiate an unit there
                    if (location.GetNeighbor(d) && location.GetNeighbor(d) == aux2 && !(aux2 = location.GetNeighbor(d)).Unit && !aux2.IsUnderwater && location.GetElevationDifference(d) < 2
                        && !location.GetNeighbor(d).city)
                    {
                        if (P.CanCreate(HexGrid.unitPrefabs[3].cost))
                        {
                            CreateTank(aux2);
                            P.BuyUnit(HexGrid.unitPrefabs[3].cost);
                        }
                    }

                }
                canSpawnTank = false;
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
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
	        location.GetNeighbor (d).DisableHighlight ();
		}
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

	public void SpawnZeppelin() {
		//highlight all available cells around the city
		for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
			if (location.GetNeighbor(d) && !location.GetNeighbor(d).Unit && !location.GetNeighbor(d).IsUnderwater && location.GetElevationDifference(d) < 2
				&& !location.GetNeighbor(d).city)
				location.GetNeighbor (d).EnableHighlight (Color.blue);
		}
		//stores the information that an unity can be spawned
		canSpawnZeppelin = true;
		return;
	}

    public void SpawnSupport()
    {
        //highlight all available cells around the city
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++){
            if (location.GetNeighbor(d) && !location.GetNeighbor(d).Unit && !location.GetNeighbor(d).IsUnderwater && location.GetElevationDifference(d) < 2
                && !location.GetNeighbor(d).city)
                location.GetNeighbor(d).EnableHighlight(Color.blue);
        }
        //stores the information that an unity can be spawned
        canSpawnSupport = true;
        return;
    }

    public void SpawnAntiAircraft()
    {
        //highlight all available cells around the city
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            if (location.GetNeighbor(d) && !location.GetNeighbor(d).Unit && !location.GetNeighbor(d).IsUnderwater && location.GetElevationDifference(d) < 2
                && !location.GetNeighbor(d).city)
                location.GetNeighbor(d).EnableHighlight(Color.blue);
        }
        //stores the information that an unity can be spawned
        canSpawnAntiAircraft = true;
        return;
    }

    public void SpawnTank()
    {
        //highlight all available cells around the city
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            if (location.GetNeighbor(d) && !location.GetNeighbor(d).Unit && !location.GetNeighbor(d).IsUnderwater && location.GetElevationDifference(d) < 2
                && !location.GetNeighbor(d).city)
                location.GetNeighbor(d).EnableHighlight(Color.blue);
        }
        //stores the information that an unity can be spawned
        canSpawnTank = true;
        return;
    }

    void CreateZeppelin (HexCell cell) {
		if (cell && !cell.Unit) {
			Grid.AddUnit(Instantiate(HexGrid.unitPrefabs[0]), cell, Random.Range(0f, 360f), P.Faccao, false);
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
			    location.GetNeighbor (d).DisableHighlight ();
		    }
            
		}
	}

    void CreateSupport(HexCell cell)
    {
        if (cell && !cell.Unit)
        {
            Grid.AddUnit(Instantiate(HexGrid.unitPrefabs[1]), cell, Random.Range(0f, 360f), P.Faccao, true);
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                location.GetNeighbor(d).DisableHighlight();
            }

        }
    }

    void CreateAntiAircraft(HexCell cell)
    {
        if (cell && !cell.Unit)
        {
            Grid.AddUnit(Instantiate(HexGrid.unitPrefabs[2]), cell, Random.Range(0f, 360f), P.Faccao, false);
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                location.GetNeighbor(d).DisableHighlight();
            }

        }
    }

    void CreateTank(HexCell cell)
    {
        if (cell && !cell.Unit)
        {
            Grid.AddUnit(Instantiate(HexGrid.unitPrefabs[3]), cell, Random.Range(0f, 360f), P.Faccao, false);
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                location.GetNeighbor(d).DisableHighlight();
            }

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
			Instantiate(HexGrid.cityPrefab), grid.GetCell(coordinates), orientation
		);
	}
}
