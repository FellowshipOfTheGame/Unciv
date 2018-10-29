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
				if (Input.GetMouseButtonDown (1)) {
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

			if (Input.GetMouseButtonDown (0)) {
				//selects the city
				DoCitySelection ();
				//if successfull selected:
				if (selectedCity) {
					selectedCity.ActiveCityMenu ();
				}
			}
		}
		if (Input.GetKeyDown (KeyCode.Escape)) {
			HexCity.DeactiveAllCitiesMenus ();
		}
		if(Input.GetButtonDown("Jump"))
			grid.Pass();
	}

	//selects the actual city and disables any different city menu
	void DoCitySelection () {
		grid.ClearPath ();
		UpdateCurrentCell ();

		HexCity.DeactiveAllCitiesMenus ();

		if (currentCell) {
			selectedCity = currentCell.city;
		}
        if(selectedCity)
            Debug.Log("Cidade Selecionada");
	}

	void DoUnitSelection () {
		grid.ClearPath();
		UpdateCurrentCell();
		if (currentCell) {
            if(currentCell.Unit)
                if(currentCell.Unit.Faccao=="Visokea")
			        selectedUnit = currentCell.Unit;
		}
        if(selectedUnit)
            Debug.Log("Unidade Selecionada");
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