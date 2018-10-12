using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;

public class HexMapEditor : MonoBehaviour {

	public HexGrid hexGrid;

	//reference to the city menu deactivated
	public GameObject cityMenu;

	public Material terrainMaterial;

	int activeElevation;
	int activeWaterLevel;

	int activeUrbanLevel, activeFarmLevel, activePlantLevel, activeSpecialIndex;

	int activeTerrainTypeIndex;

	int brushSize;

	bool applyElevation = true;
	bool applyWaterLevel = true;

	bool canSpawn;

	bool applyUrbanLevel, applyFarmLevel, applyPlantLevel, applySpecialIndex;

	enum OptionalToggle {
		Ignore, Yes, No
	}

	OptionalToggle riverMode, roadMode, walledMode;

	bool isDrag;
	HexDirection dragDirection;
	HexCell previousCell;
	HexCell selectedCity;

	public void SetTerrainTypeIndex (int index) {
		activeTerrainTypeIndex = index;
	}

	public void SetApplyElevation (bool toggle) {
		applyElevation = toggle;
	}

	public void SetElevation (float elevation) {
		activeElevation = (int)elevation;
	}

	public void SetApplyWaterLevel (bool toggle) {
		applyWaterLevel = toggle;
	}

	public void SetWaterLevel (float level) {
		activeWaterLevel = (int)level;
	}

	public void SetApplyUrbanLevel (bool toggle) {
		applyUrbanLevel = toggle;
	}

	public void SetUrbanLevel (float level) {
		activeUrbanLevel = (int)level;
	}

	public void SetApplyFarmLevel (bool toggle) {
		applyFarmLevel = toggle;
	}

	public void SetFarmLevel (float level) {
		activeFarmLevel = (int)level;
	}

	public void SetApplyPlantLevel (bool toggle) {
		applyPlantLevel = toggle;
	}

	public void SetPlantLevel (float level) {
		activePlantLevel = (int)level;
	}

	public void SetApplySpecialIndex (bool toggle) {
		applySpecialIndex = toggle;
	}

	public void SetSpecialIndex (float index) {
		activeSpecialIndex = (int)index;
	}

	public void SetBrushSize (float size) {
		brushSize = (int)size;
	}

	public void SetRiverMode (int mode) {
		riverMode = (OptionalToggle)mode;
	}

	public void SetRoadMode (int mode) {
		roadMode = (OptionalToggle)mode;
	}

	public void SetWalledMode (int mode) {
		walledMode = (OptionalToggle)mode;
	}

	public void SetEditMode (bool toggle) {
		enabled = toggle;
	}

	public void ShowGrid (bool visible) {
		if (visible) {
			terrainMaterial.EnableKeyword("GRID_ON");
		}
		else {
			terrainMaterial.DisableKeyword("GRID_ON");
		}
	}

	void Awake () {
		terrainMaterial.EnableKeyword("GRID_ON");
		Shader.DisableKeyword("HEX_MAP_EDIT_MODE");
		SetEditMode(false);
		canSpawn = false;
		selectedCity = null;
	}

	void Update () {
		HexCell aux;
		if (!EventSystem.current.IsPointerOverGameObject()) {
			if (Input.GetMouseButton(0)) {
				HandleInput();
				return;
			}
            if(Input.GetButtonDown("Jump")){ 
                hexGrid.changeUnit(1);
                return;
            }
			if (Input.GetKeyDown(KeyCode.U)) {
				if (Input.GetKey(KeyCode.LeftShift)) {
					DestroyUnit();
				}
				else {
					CreateUnit(GetCellUnderCursor());
				}
				return;
			}
			//Checks if the right button of the mouse was press on an cell with a pyramid (city)
			if (Input.GetMouseButtonDown(1) && (aux = GetCellUnderCursor()).SpecialIndex == 2) {
				//checks if the right click was on the same city, if yes deactive the menu
				if (selectedCity != aux) {
					//if the player selects another city, simply change the selectedCity (that shall includes changind the menu)
					selectedCity = aux;
					cityMenu.SetActive (true);
				} else { 
					//else, sets the city menu on and off
					if (cityMenu.activeSelf)
						cityMenu.SetActive (false);
					else
						cityMenu.SetActive (true);
				}
			}
		}
		//if an unity can be spawned
		if (canSpawn == true) {
			//when right click on a cell
			if (Input.GetMouseButtonDown (1)) {
				//get the cell
				HexCell aux2 = GetCellUnderCursor();
				//search the selected cell beetween selectedcity neighbors, disabling the highlight
				for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
					//if the correct cell is found, instantiate an unit there
					if (selectedCity.GetNeighbor (d) == aux2 && !(aux2 = selectedCity.GetNeighbor (d)).Unit && !aux2.IsUnderwater && aux2.GetElevationDifference(d) < 2) {
						CreateUnit (aux2);
					}
					selectedCity.GetNeighbor (d).DisableHighlight ();
				}
				canSpawn = false;
			}
		}
		previousCell = null;
	}

	HexCell GetCellUnderCursor () {
		return
			hexGrid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
	}

	public void SpawnUnit() {
		//highlight all available cells around the city
		for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
			if (!selectedCity.GetNeighbor(d).Unit && !selectedCity.GetNeighbor(d).IsUnderwater && selectedCity.GetElevationDifference(d) < 2)
				selectedCity.GetNeighbor (d).EnableHighlight (Color.blue);
		}
		//stores the information that an unity can be spawned
		canSpawn = true;
		return;
	}

	void CreateUnit (HexCell cell) {
		if (cell && !cell.Unit) {
			hexGrid.AddUnit(Instantiate(HexUnit.unitPrefab), cell, Random.Range(0f, 360f));
		}
	}

	void DestroyUnit () {
		HexCell cell = GetCellUnderCursor();
		if (cell && cell.Unit) {
			hexGrid.RemoveUnit(cell.Unit);
		}
	}

	void HandleInput () {
		HexCell currentCell = GetCellUnderCursor();
		if (currentCell) {
			if (previousCell && previousCell != currentCell) {
				ValidateDrag(currentCell);
			}
			else {
				isDrag = false;
			}
			EditCells(currentCell);
			previousCell = currentCell;
		}
		else {
			previousCell = null;
		}
	}

	void ValidateDrag (HexCell currentCell) {
		for (
			dragDirection = HexDirection.NE;
			dragDirection <= HexDirection.NW;
			dragDirection++
		) {
			if (previousCell.GetNeighbor(dragDirection) == currentCell) {
				isDrag = true;
				return;
			}
		}
		isDrag = false;
	}

	void EditCells (HexCell center) {
		int centerX = center.coordinates.X;
		int centerZ = center.coordinates.Z;

		for (int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++) {
			for (int x = centerX - r; x <= centerX + brushSize; x++) {
				EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
			}
		}
		for (int r = 0, z = centerZ + brushSize; z > centerZ; z--, r++) {
			for (int x = centerX - brushSize; x <= centerX + r; x++) {
				EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
			}
		}
	}

	void EditCell (HexCell cell) {
		if (cell) {
			if (activeTerrainTypeIndex >= 0) {
				cell.TerrainTypeIndex = activeTerrainTypeIndex;
			}
			if (applyElevation) {
				cell.Elevation = activeElevation;
			}
			if (applyWaterLevel) {
				cell.WaterLevel = activeWaterLevel;
			}
			if (applySpecialIndex) {
				cell.SpecialIndex = activeSpecialIndex;
			}
			if (applyUrbanLevel) {
				cell.UrbanLevel = activeUrbanLevel;
			}
			if (applyFarmLevel) {
				cell.FarmLevel = activeFarmLevel;
			}
			if (applyPlantLevel) {
				cell.PlantLevel = activePlantLevel;
			}
			if (riverMode == OptionalToggle.No) {
				cell.RemoveRiver();
			}
			if (roadMode == OptionalToggle.No) {
				cell.RemoveRoads();
			}
			if (walledMode != OptionalToggle.Ignore) {
				cell.Walled = walledMode == OptionalToggle.Yes;
			}
			if (isDrag) {
				HexCell otherCell = cell.GetNeighbor(dragDirection.Opposite());
				if (otherCell) {
					if (riverMode == OptionalToggle.Yes) {
						otherCell.SetOutgoingRiver(dragDirection);
					}
					if (roadMode == OptionalToggle.Yes) {
						otherCell.AddRoad(dragDirection);
					}
				}
			}
		}
	}
}