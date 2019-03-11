using UnityEngine;
using UnityEngine.EventSystems;

public class HexGameUI : MonoBehaviour {

	public HexGrid grid;

	public HexMapEditor hexMap;

	HexCell currentCell;

	HexUnit selectedUnit;

	HexCity selectedCity;

    private int turn;

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

    public HexCell GetCellUnderCursor () {
		return
			grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
	}

    void Update () {
		if (!EventSystem.current.IsPointerOverGameObject()) {
			if (selectedUnit) {
				if (Input.GetMouseButtonDown (1)) {
                    if (selectedUnit.engineer)
                    {
                        Debug.Log("pode abrir o menu");
                        /*open new city menu*/
                        selectedUnit.CanConstruct();
                        selectedUnit.Emenu.OpenMenu();

                        if(selectedUnit.Emenu.construct)
                        {
                            NCityHighLight();
                            HexCell currentCell = GetCellUnderCursor();
                            if (!currentCell.Unit && !currentCell.city)
                            {
                                hexMap.CreateCity(currentCell);
                                selectedUnit.construct();
                            }
                            else
                                selectedUnit.Emenu.construct = false;

                            selectedUnit.Emenu.IsOpen = false;
                        }
                    }
                    if (selectedUnit.canAttack){                        
                        Debug.Log("Unidade pode atacar");
                        HexCell currentCell = GetCellUnderCursor();
                        if(currentCell.Unit) {
                            Debug.Log("Tem unidade");
                            if (selectedUnit.engineer && (currentCell.Unit.Faccao == selectedUnit.Faccao))
                            {
                                Debug.Log("Unidade Aliada");
                                selectedUnit.heal(currentCell.Unit);
                                for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
                                    selectedUnit.Location.GetNeighbor(d).DisableHighlight();
                            }
                                
                            else if(currentCell.Unit.Faccao!=selectedUnit.Faccao) {
                                Debug.Log("Unidade inimiga");
                                selectedUnit.Attack(currentCell.Unit);
                                for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
                                    selectedUnit.Location.GetNeighbor (d).DisableHighlight ();
                            }
                        }
                        else if(currentCell.Fort) { 
                            selectedUnit.Seize(currentCell.Fort);
                            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
                                    selectedUnit.Location.GetNeighbor (d).DisableHighlight ();
                        }
                    }
                    if(selectedUnit.CanMove) {
					    DoMove ();
                        if(selectedUnit.canAttack)
                            HighLight();
                    }
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
		
	}

    public void HighLight() { 
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
            if(selectedUnit)
			    if (selectedUnit.Location.GetNeighbor(d)) {
                    if(selectedUnit.Location.GetNeighbor(d).Fort)
                        selectedUnit.Location.GetNeighbor(d).EnableHighlight (Color.red);
                    if (selectedUnit.Location.GetNeighbor(d).Unit)
                        if(selectedUnit.Location.GetNeighbor(d).Unit.Faccao!=selectedUnit.Faccao)
				            selectedUnit.Location.GetNeighbor(d).EnableHighlight (Color.red);
                }
            if (selectedUnit)
                if (selectedUnit.Location.GetNeighbor(d))
                    if (selectedUnit.Location.GetNeighbor(d).Unit)
                    {
                        if (selectedUnit.Location.GetNeighbor(d).Unit.Faccao != selectedUnit.Faccao)
                            selectedUnit.Location.GetNeighbor(d).EnableHighlight(Color.red);
                        if (selectedUnit.engineer && (selectedUnit.Location.GetNeighbor(d).Unit.Faccao == selectedUnit.Faccao))
                            selectedUnit.Location.GetNeighbor(d).EnableHighlight(Color.green);
                    }
        }    
    }

    //locais possiveis para criar uma nova cidade
    public void NCityHighLight()
    {
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            if (selectedUnit)
                if (selectedUnit.Location.GetNeighbor(d))
                    if (selectedUnit.Location.GetNeighbor(d).Unit)
                    {
                        if (!selectedUnit.Location.GetNeighbor(d).city && !selectedUnit.Location.GetNeighbor(d).Unit)
                            selectedUnit.Location.GetNeighbor(d).EnableHighlight(Color.grey);
                    }
        }
    }

	//selects the actual city and disables any different city menu
	void DoCitySelection () {
        if(selectedUnit)
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
                selectedUnit.Location.GetNeighbor (d).DisableHighlight ();

		grid.ClearPath ();
		UpdateCurrentCell ();

		HexCity.DeactiveAllCitiesMenus ();

		if (currentCell) {
			selectedCity = currentCell.city;
		}
        if(selectedCity)
            selectedUnit=null;
	}

	void DoUnitSelection () {
        //limpa o highlight de ataque
        if(selectedUnit)
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
                selectedUnit.Location.GetNeighbor (d).DisableHighlight ();

		grid.ClearPath();
		UpdateCurrentCell();
		if (currentCell) {
            if(currentCell.Unit)
                if(currentCell.Unit.Faccao=="Visokea")
			        selectedUnit = currentCell.Unit;
		}
        if(selectedUnit)
            if(selectedUnit.canAttack)
                HighLight();
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