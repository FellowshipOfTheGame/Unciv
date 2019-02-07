using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class HexGrid : MonoBehaviour {

	public int cellCountX = 20, cellCountZ = 15;

    public AIbrain AB;
    public Text turns;
    private int turn =0;

	public bool wrapping;

	public HexCell cellPrefab;
	public Text cellLabelPrefab;
	public HexGridChunk chunkPrefab;
    public HexUnit[] unitsP;
    public HexUnit[] unitsI;
	public HexCity citiesP;
    public HexFort Fort;
	public static HexUnit[] unitPrefabs;
    public static HexUnit[] unitPrefabsI;
	public static HexCity cityPrefab;
    public static HexFort fortPrefab;
	public GameObject cityMenuPrefab;
	public GameObject cityMenuCanvas;

	public Texture2D noiseSource;
    private Player P;

	public int seed;

	public bool HasPath {
		get {
			return currentPathExists;
		}
	}

	Transform[] columns;
	HexGridChunk[] chunks;
	HexCell[] cells;

	int chunkCountX, chunkCountZ;

	HexCellPriorityQueue searchFrontier;

	int searchFrontierPhase;

	HexCell currentPathFrom, currentPathTo;
	bool currentPathExists;

	int currentCenterColumnIndex = -1;

	List<HexUnit> units = new List<HexUnit>();
	List<HexCity> cities = new List<HexCity> ();
    public List<HexFort> Forts = new List<HexFort> ();
    public HexGameUI HGU;

	HexCellShaderData cellShaderData;

	void Awake () {
        P = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
		HexMetrics.noiseSource = noiseSource;
		HexMetrics.InitializeHashGrid(seed);
		unitPrefabs=unitsP;
        unitPrefabsI=unitsI;
        cityPrefab=citiesP;
        fortPrefab=Fort;
		
		//stores the menu prefab
		HexCity.cityMenu = cityMenuPrefab;
		//stores the city menu canvas object
		HexCity.cityMenuCanvas = cityMenuCanvas;

		cellShaderData = gameObject.AddComponent<HexCellShaderData>();
		cellShaderData.Grid = this;
		CreateMap(cellCountX, cellCountZ, wrapping);
	}

	public void Pass(){ 
		foreach (HexUnit hu in units) {
			hu.CanMove=true;
            hu.canAttack=true;
            hu.canHeal = true;
            if(hu.consdelay > 0)
            {
                hu.consdelay -= 1;
            }
		}
        foreach (var c in cities) { 
            P.SetResources(c.ResPT);    
        }
        AB.Activate();
        turns.text = "Turn: " + (++turn).ToString();

        HGU.HighLight();
	}

	//Adds an city to the cell (identical to AddUnit())
	public void AddCity (HexCity city, HexCell location, float orientation) {
		cities.Add (city);
		city.Grid = this;
		city.Location = location;
		city.Orientation = orientation;
	}
    public void AddFort (HexFort city, HexCell location, float orientation) {
		Forts.Add (city);
		city.Grid = this;
		city.Location = location;
		city.Orientation = orientation;
	}

    public void RemoveFort (HexFort city) {
		Forts.Remove (city);
		city.Destroy();
	}
	//Removes an city (identical to RemoveUnit)
	public void RemoveCity (HexCity city) {
		cities.Remove (city);
		city.Destroy();
	}

	public void AddUnit (HexUnit unit, HexCell location, float orientation, string Fac) {

		unit.Grid = this;
		unit.Location = location;
		unit.Orientation = orientation;
        unit.Faccao=Fac;
        unit.engineer = false;
        if(Fac=="Barbaros" || Fac=="Minor")
            AB.Units.Add(unit);
        else
            units.Add(unit);
		unit.CanMove = false;
	}

	public void RemoveUnit (HexUnit unit) {
		
        if(unit.Faccao=="Barbaros" || unit.Faccao=="Minor")
            AB.Units.Remove(unit);
        else
            units.Remove(unit);
		unit.Die();
	}

	public void MakeChildOfColumn (Transform child, int columnIndex) {
		child.SetParent(columns[columnIndex], false);
	}

	public bool CreateMap (int x, int z, bool wrapping) {
		if (
			x <= 0 || x % HexMetrics.chunkSizeX != 0 ||
			z <= 0 || z % HexMetrics.chunkSizeZ != 0
		) {
			Debug.LogError("Unsupported map size.");
			return false;
		}

		ClearPath();
		ClearUnits();
        ClearCities();
        ClearForts();

		if (columns != null) {
			for (int i = 0; i < columns.Length; i++) {
				Destroy(columns[i].gameObject);
			}
		}

		cellCountX = x;
		cellCountZ = z;
		this.wrapping = wrapping;
		currentCenterColumnIndex = -1;
		HexMetrics.wrapSize = wrapping ? cellCountX : 0;
		chunkCountX = cellCountX / HexMetrics.chunkSizeX;
		chunkCountZ = cellCountZ / HexMetrics.chunkSizeZ;
		cellShaderData.Initialize(cellCountX, cellCountZ);
		CreateChunks();
		CreateCells();
		return true;
	}

	void CreateChunks () {
		columns = new Transform[chunkCountX];
		for (int x = 0; x < chunkCountX; x++) {
			columns[x] = new GameObject("Column").transform;
			columns[x].SetParent(transform, false);
		}

		chunks = new HexGridChunk[chunkCountX * chunkCountZ];
		for (int z = 0, i = 0; z < chunkCountZ; z++) {
			for (int x = 0; x < chunkCountX; x++) {
				HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
				chunk.transform.SetParent(columns[x], false);
			}
		}
	}

	void CreateCells () {
		cells = new HexCell[cellCountZ * cellCountX];

		for (int z = 0, i = 0; z < cellCountZ; z++) {
			for (int x = 0; x < cellCountX; x++) {
				CreateCell(x, z, i++);
			}
		}
	}

	void ClearUnits () {

		for (int i = 0; i < units.Count; i++) {
			units[i].Die();
		}

        for (int i = 0; i < AB.Units.Count; i++) {
			AB.Units[i].Die();
		}

		units.Clear();
        AB.Units.Clear();
	}

    void ClearCities () {
		for (int i = 0; i < cities.Count; i++) {
			cities[i].Destroy();
		}
		cities.Clear();
	}

    void ClearForts () {
		for (int i = 0; i < Forts.Count; i++) {
			Forts[i].Destroy();
		}
		Forts.Clear();
	}

	void OnEnable () {
		if (!HexMetrics.noiseSource) {
			HexMetrics.noiseSource = noiseSource;
			HexMetrics.InitializeHashGrid(seed);
			HexMetrics.wrapSize = wrapping ? cellCountX : 0;
			ResetVisibility();
		}
	}

	public HexCell GetCell (Ray ray) {
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit)) {
			return GetCell(hit.point);
		}
		return null;
	}

	public HexCell GetCell (Vector3 position) {
		position = transform.InverseTransformPoint(position);
		HexCoordinates coordinates = HexCoordinates.FromPosition(position);
		return GetCell(coordinates);
	}

	public HexCell GetCell (HexCoordinates coordinates) {
		int z = coordinates.Z;
		if (z < 0 || z >= cellCountZ) {
			return null;
		}
		int x = coordinates.X + z / 2;
		if (x < 0 || x >= cellCountX) {
			return null;
		}
		return cells[x + z * cellCountX];
	}

	public HexCell GetCell (int xOffset, int zOffset) {
		return cells[xOffset + zOffset * cellCountX];
	}

	public HexCell GetCell (int cellIndex) {
		return cells[cellIndex];
	}

	public void ShowUI (bool visible) {
		for (int i = 0; i < chunks.Length; i++) {
			chunks[i].ShowUI(visible);
		}
	}

	void CreateCell (int x, int z, int i) {
		Vector3 position;
		position.x = (x + z * 0.5f - z / 2) * HexMetrics.innerDiameter;
		position.y = 0f;
		position.z = z * (HexMetrics.outerRadius * 1.5f);

		HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
		cell.transform.localPosition = position;
		cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
		cell.Index = i;
		cell.ColumnIndex = x / HexMetrics.chunkSizeX;
		cell.ShaderData = cellShaderData;

		if (wrapping) {
			cell.Explorable = z > 0 && z < cellCountZ - 1;
		}
		else {
			cell.Explorable =
				x > 0 && z > 0 && x < cellCountX - 1 && z < cellCountZ - 1;
		}

		if (x > 0) {
			cell.SetNeighbor(HexDirection.W, cells[i - 1]);
			if (wrapping && x == cellCountX - 1) {
				cell.SetNeighbor(HexDirection.E, cells[i - x]);
			}
		}
		if (z > 0) {
			if ((z & 1) == 0) {
				cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX]);
				if (x > 0) {
					cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX - 1]);
				}
				else if (wrapping) {
					cell.SetNeighbor(HexDirection.SW, cells[i - 1]);
				}
			}
			else {
				cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX]);
				if (x < cellCountX - 1) {
					cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX + 1]);
				}
				else if (wrapping) {
					cell.SetNeighbor(
						HexDirection.SE, cells[i - cellCountX * 2 + 1]
					);
				}
			}
		}

		Text label = Instantiate<Text>(cellLabelPrefab);
		label.rectTransform.anchoredPosition =
			new Vector2(position.x, position.z);
		cell.uiRect = label.rectTransform;

		cell.Elevation = 0;

		AddCellToChunk(x, z, cell);
	}

	void AddCellToChunk (int x, int z, HexCell cell) {
		int chunkX = x / HexMetrics.chunkSizeX;
		int chunkZ = z / HexMetrics.chunkSizeZ;
		HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

		int localX = x - chunkX * HexMetrics.chunkSizeX;
		int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
		chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);
	}

	public void Save (BinaryWriter writer) {
		writer.Write(cellCountX);
		writer.Write(cellCountZ);
		writer.Write(wrapping);

		for (int i = 0; i < cells.Length; i++) {
			cells[i].Save(writer);
		}

		writer.Write(AB.Units.Count);
		for (int i = 0; i < AB.Units.Count; i++) {
			AB.Units[i].Save(writer);
		}

        writer.Write(cities.Count);
		for (int i = 0; i < cities.Count; i++) {
			cities[i].Save(writer);
		}
        writer.Write(Forts.Count);
		for (int i = 0; i < Forts.Count; i++) {
			Forts[i].Save(writer);
		}
	}

	public void Load (BinaryReader reader, int header) {
		ClearPath();
		ClearUnits();
        ClearCities();
        ClearForts();

		int x = 20, z = 15;
		if (header >= 1) {
			x = reader.ReadInt32();
			z = reader.ReadInt32();
		}
		bool wrapping = header >= 5 ? reader.ReadBoolean() : false;
		if (x != cellCountX || z != cellCountZ || this.wrapping != wrapping) {
			if (!CreateMap(x, z, wrapping)) {
				return;
			}
		}

		bool originalImmediateMode = cellShaderData.ImmediateMode;
		cellShaderData.ImmediateMode = true;

		for (int i = 0; i < cells.Length; i++) {
			cells[i].Load(reader, header);
		}
		for (int i = 0; i < chunks.Length; i++) {
			chunks[i].Refresh();
		}

		if (header >= 2) {
			int unitCount = reader.ReadInt32();
			for (int i = 0; i < unitCount; i++) {
				HexUnit.Load(reader, this);
			}
		}

        if (header >= 6) {
			int citiesCount = reader.ReadInt32();
			for (int i = 0; i < citiesCount; i++) {
				HexCity.Load(reader, this);
			}
		}
        
        if (header >= 7) {
			int fortsCount = reader.ReadInt32();
			for (int i = 0; i < fortsCount; i++) {
				HexFort.Load(reader, this);
			}
		}
		cellShaderData.ImmediateMode = originalImmediateMode;
	}

	public List<HexCell> GetPath () {
		if (!currentPathExists) {
			return null;
		}
		List<HexCell> path = ListPool<HexCell>.Get();
		for (HexCell c = currentPathTo; c != currentPathFrom; c = c.PathFrom) {
			path.Add(c);
		}
		path.Add(currentPathFrom);
		path.Reverse();
		return path;
	}

	public void ClearPath () {
		if (currentPathExists) {
			HexCell current = currentPathTo;
			while (current != currentPathFrom) {
				current.SetLabel(null);
				current.DisableHighlight();
				current = current.PathFrom;
			}
			current.DisableHighlight();
			currentPathExists = false;
		}
		else if (currentPathFrom) {
			currentPathFrom.DisableHighlight();
			currentPathTo.DisableHighlight();
		}
		currentPathFrom = currentPathTo = null;
	}

	void ShowPath (int speed) {
		if (currentPathExists) {
			HexCell current = currentPathTo;
			while (current != currentPathFrom){
				//current.SetLabel(turn.ToString());
				current.EnableHighlight(Color.white);
				current = current.PathFrom;
			}
			currentPathFrom.EnableHighlight(Color.blue);
			currentPathTo.EnableHighlight(Color.green);
		}
	}

	public void FindPath (HexCell fromCell, HexCell toCell, HexUnit unit) {
		ClearPath();
		currentPathFrom = fromCell;
		currentPathTo = toCell;

		currentPathExists = Search(fromCell, toCell, unit);

		if(unit.CanMove)
			ShowPath(unit.Speed);
	}

	bool Search (HexCell fromCell, HexCell toCell, HexUnit unit) {
		int speed = unit.Speed;
		searchFrontierPhase += 2;
		if (searchFrontier == null) {
			searchFrontier = new HexCellPriorityQueue();
		}
		else {
			searchFrontier.Clear();
		}

		fromCell.SearchPhase = searchFrontierPhase;
		fromCell.Distance = 0;
		searchFrontier.Enqueue(fromCell);
		while (searchFrontier.Count > 0) {
			HexCell current = searchFrontier.Dequeue();
			current.SearchPhase += 1;

			if (current == toCell) {
				return true;
			}

			int currentTurn = (current.Distance - 1) / speed;

			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
				HexCell neighbor = current.GetNeighbor(d);
				if (
					neighbor == null ||
					neighbor.SearchPhase > searchFrontierPhase
				) {
					continue;
				}
				if (!unit.IsValidDestination(neighbor)) {
					continue;
				}
				int moveCost = unit.GetMoveCost(current, neighbor, d);
				if (moveCost < 0) {
					continue;
				}

				int distance = current.Distance + moveCost;
				int turn = (distance - 1) / speed;
				if (turn > currentTurn) {
					distance = turn * speed + moveCost;
				}
				if(turn>0)
					return false;

				if (neighbor.SearchPhase < searchFrontierPhase) {
					neighbor.SearchPhase = searchFrontierPhase;
					neighbor.Distance = distance;
					neighbor.PathFrom = current;
					neighbor.SearchHeuristic =
						neighbor.coordinates.DistanceTo(toCell.coordinates);
					searchFrontier.Enqueue(neighbor);
				}
				else if (distance < neighbor.Distance) {
					int oldPriority = neighbor.SearchPriority;
					neighbor.Distance = distance;
					neighbor.PathFrom = current;
					searchFrontier.Change(neighbor, oldPriority);
				}
			}
		}
		return false;
	}

	public void IncreaseVisibility (HexCell fromCell, int range) {
		List<HexCell> cells = GetVisibleCells(fromCell, range);
		for (int i = 0; i < cells.Count; i++) {
			cells[i].IncreaseVisibility();
		}
		ListPool<HexCell>.Add(cells);
	}

	public void DecreaseVisibility (HexCell fromCell, int range) {
		List<HexCell> cells = GetVisibleCells(fromCell, range);
		for (int i = 0; i < cells.Count; i++) {
			cells[i].DecreaseVisibility();
		}
		ListPool<HexCell>.Add(cells);
	}

	public void ResetVisibility () {
		for (int i = 0; i < cells.Length; i++) {
			cells[i].ResetVisibility();
		}
        foreach (var c in cities) { 
            IncreaseVisibility(c.Location, c.VisionRange);    
        }
		for (int i = 0; i < units.Count; i++) {
			HexUnit unit = units[i];
            if(unit.Faccao==P.Faccao)
			    IncreaseVisibility(unit.Location, unit.VisionRange);
		}
        
	}

	List<HexCell> GetVisibleCells (HexCell fromCell, int range) {
		List<HexCell> visibleCells = ListPool<HexCell>.Get();

		searchFrontierPhase += 2;
		if (searchFrontier == null) {
			searchFrontier = new HexCellPriorityQueue();
		}
		else {
			searchFrontier.Clear();
		}

		range += fromCell.ViewElevation;
		fromCell.SearchPhase = searchFrontierPhase;
		fromCell.Distance = 0;
		searchFrontier.Enqueue(fromCell);
		HexCoordinates fromCoordinates = fromCell.coordinates;
		while (searchFrontier.Count > 0) {
			HexCell current = searchFrontier.Dequeue();
			current.SearchPhase += 1;
			visibleCells.Add(current);

			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
				HexCell neighbor = current.GetNeighbor(d);
				if (
					neighbor == null ||
					neighbor.SearchPhase > searchFrontierPhase ||
					!neighbor.Explorable
				) {
					continue;
				}

				int distance = current.Distance + 1;
				if (distance + neighbor.ViewElevation > range ||
					distance > fromCoordinates.DistanceTo(neighbor.coordinates)
				) {
					continue;
				}

				if (neighbor.SearchPhase < searchFrontierPhase) {
					neighbor.SearchPhase = searchFrontierPhase;
					neighbor.Distance = distance;
					neighbor.SearchHeuristic = 0;
					searchFrontier.Enqueue(neighbor);
				}
				else if (distance < neighbor.Distance) {
					int oldPriority = neighbor.SearchPriority;
					neighbor.Distance = distance;
					searchFrontier.Change(neighbor, oldPriority);
				}
			}
		}
		return visibleCells;
	}

	public void CenterMap (float xPosition) {
		int centerColumnIndex = (int)
			(xPosition / (HexMetrics.innerDiameter * HexMetrics.chunkSizeX));

		if (centerColumnIndex == currentCenterColumnIndex) {
			return;
		}
		currentCenterColumnIndex = centerColumnIndex;

		int minColumnIndex = centerColumnIndex - chunkCountX / 2;
		int maxColumnIndex = centerColumnIndex + chunkCountX / 2;

		Vector3 position;
		position.y = position.z = 0f;
		for (int i = 0; i < columns.Length; i++) {
			if (i < minColumnIndex) {
				position.x = chunkCountX *
					(HexMetrics.innerDiameter * HexMetrics.chunkSizeX);
			}
			else if (i > maxColumnIndex) {
				position.x = chunkCountX *
					-(HexMetrics.innerDiameter * HexMetrics.chunkSizeX);
			}
			else {
				position.x = 0f;
			}
			columns[i].localPosition = position;
		}
	}
}