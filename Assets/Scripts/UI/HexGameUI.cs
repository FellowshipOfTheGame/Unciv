using UnityEngine;
using UnityEngine.EventSystems;

public class HexGameUI : MonoBehaviour {

	public HexGrid grid;

	public HexMapEditor hexMap;

	HexCell currentCell;

	HexUnit selectedUnit;

	HexCity selectedCity;

	public void SetEditMode (bool toggle) {
		enabled = !toggle;
		grid.ShowUI(!toggle);
		grid.ClearPath();
		if (toggle) {
			Shader.EnableKeyword("HEX_MAP_EDIT_MODE");
		}
		else {
			Shader.DisableKeyword("HEX_MAP_EDIT_MODE");
		}
	}

	void Update () {
		if (!EventSystem.current.IsPointerOverGameObject()) {
			if (selectedUnit && selectedUnit.CanMove) {
				if (Input.GetMouseButtonDown (0)) {
					DoMove ();
				} 
				else {
					DoPathfinding ();
				}
			}
			if (Input.GetMouseButtonDown (0)) {
				selectedCity = null;
				DoUnitSelection ();
			} 
			//checks the right button
			else if (Input.GetMouseButtonDown (1)) {
				Debug.Log ("Entrou");
				//selects the city
				DoCitySelection (selectedCity);
				//if successfull selected:
				if (selectedCity) {
					//active if deactivated, deactive if actvated;
					if (!selectedCity.IsCityMenuActivated) {
						selectedCity.ActiveCityMenu ();
					} else {
						selectedCity.DeactiveCityMenu ();
					}
				}
			}
		}
		if(Input.GetButtonDown("Jump"))
			grid.Pass();
	}

	//selects the actual city and disables any different city menu
	void DoCitySelection (HexCity city) {
		grid.ClearPath ();
		UpdateCurrentCell ();
		if (city && currentCell.city != city) {
			Debug.Log ("Different cities");
			city.DeactiveCityMenu();
		}
		if (currentCell) {
			selectedCity = currentCell.city;
		}
	}

	void DoUnitSelection () {
		grid.ClearPath();
		UpdateCurrentCell();
		if (currentCell) {
			selectedUnit = currentCell.Unit;
		}
	}

	void DoPathfinding () {
		if (UpdateCurrentCell()) {
			if (currentCell && selectedUnit.IsValidDestination(currentCell)) {
				grid.FindPath(selectedUnit.Location, currentCell, selectedUnit);
			}
			else {
				grid.ClearPath();
			}
		}
	}

	void DoMove () {
		if (grid.HasPath) {
			selectedUnit.Travel(grid.GetPath());
			grid.ClearPath();
		}
	}

	bool UpdateCurrentCell () {
		HexCell cell =
			grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
		if (cell != currentCell) {
			currentCell = cell;
			return true;
		}
		return false;
	}
}