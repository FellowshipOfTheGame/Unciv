using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class HexUnit : MonoBehaviour {

	const float rotationSpeed = 180f;
	const float travelSpeed = 4f;

    public Healthbar HB;
    
	public bool CanMove=true;

    public virtual int UnitPrefab {
		get {
			return 0;
		}
	}

	public HexGrid Grid { get; set; }

    private void Awake()
    {
        Maxhp = HitP;
    }

    public HexCell Location {
		get {
			return location;
		}
		set {
			if (location) {
				Grid.DecreaseVisibility(location, VisionRange);
				location.Unit = null;
			}
			location = value;
			value.Unit = this;
            if(Faccao==P.Faccao)
			    Grid.IncreaseVisibility(value, VisionRange);
			transform.localPosition = value.Position;
			Grid.MakeChildOfColumn(transform, value.ColumnIndex);
		}
	}

	HexCell location, currentTravelLocation;

	public float Orientation {
		get {
			return orientation;
		}
		set {
			orientation = value;
			transform.localRotation = Quaternion.Euler(0f, value, 0f);
		}
	}

    private Player P;

	/// <summary>
	/// INICIO DOS VALORES ESPECICOS; 
	/// </summary>
	public virtual int Speed {
		get {
			return 24;
		}
	}

	//bloco de atributos pro combate;
	public int ATK;
	public int SPD;
	public int HitP;
    private int Maxhp;
	public int DEF;
	public int RNG;
    public int type;

    public string cost;

    public bool canAttack;
    public bool canHeal;
    public int consdelay = 1;//delay apos ter construido algo
    public bool CanCons;

    public engineermenu Emenu;
	//atributo de controle;
	public string Faccao;

    public bool engineer;

	public virtual void Attack(HexUnit Target) {
        if(!Location.isNeighbour(Target.Location))
            return;
		//verificar se a distancia esta adequada para o ataque ao alvo;
		var DMG = this.ATK*(1f-Target.DEF/100f);
        DMG = vantagem(type, Target.type, DMG);
        Target.HitP-=(int)DMG; 
		Target.UpdateHP();
		if(Target.HitP>0) { //se o alvo sobreviveu o ataque tem chances de revidar;
			//verifica se o contra-ataque tem alcance;
			DMG = Target.ATK*(1f-this.DEF/100f);
            DMG = vantagem(Target.type, type, DMG);
			this.HitP-=(int)DMG;
			this.UpdateHP();
		}
		if(this.HitP>0&&Target.HitP>0) { //se ambos ainda estao vivos, iniciar segunda rodada;
			if(this.SPD>Target.SPD+5) { //se minha velocidade eh maior que 5 a mais do que meu inimigo, eu ataco denovo;
				DMG=this.ATK*(1f-Target.DEF/100f);
				Target.HitP-=(int)DMG;
				Target.UpdateHP();
			}
			else if(Target.SPD>this.SPD+5) { //se o alvo tem mais velocidade, ele tem direito ao ataque extra;
				//verifica distancia mais uma vez;
				DMG = Target.ATK*(1f-this.DEF/100f);
				this.HitP-=(int)DMG;
				this.UpdateHP();
			}
		}
        canAttack=false;
        canHeal=false;
        CanMove=false;
	}

    public void construct()
    {
        Emenu.construct = false;
        canAttack = false;
        canHeal = false;
        CanMove = false;
        consdelay = 4;
    }

    public void CanConstruct()
    {
        string cost = "40";

        CanCons = P.CanCreate(cost);
    }

    public virtual void heal(HexUnit Target)
    {
        if (!Location.isNeighbour(Target.Location))
            return;

        Target.HitP += (int)(Maxhp * 0.3);
        Target.UpdateHP();

        canAttack = false;
        canHeal = false;
        CanMove = false;
    }

	public void UpdateHP() {
        HB.SetHP(HitP);
		if(HitP<=0)
            Grid.RemoveUnit(this);
        if (HitP > Maxhp)
            HitP = Maxhp;
            
	}

    public virtual void Seize(HexCity HC) { 
        Grid.RemoveCity(HC);
        canAttack=false;
        CanMove=false;
    }

    public virtual void Seize(HexFort HF) { 
        Grid.RemoveFort(HF);
    }

	public virtual int VisionRange {
		get {
			return 3;
		}
	}
	/// <summary>
	/// FIM DOS VALORES ESPECIFICOS
	/// </summary>

	float orientation;

	List<HexCell> pathToTravel;

	public void ValidateLocation () {
		transform.localPosition = location.Position;
	}

	//says if a cell is valid to be occupied 
	public virtual bool IsValidDestination (HexCell cell) {
		return cell.IsExplored && !cell.IsUnderwater && !cell.Unit && !cell.city && !cell.Fort;
	}

	public void Travel (List<HexCell> path) {
		location.Unit = null;
		location = path[path.Count - 1];
		location.Unit = this;
		pathToTravel = path;
		StopAllCoroutines();
		StartCoroutine(TravelPath());
		CanMove=false;
        if(canAttack)
            location.EnableHighlight(Color.magenta);
	}

	IEnumerator TravelPath () {
		Vector3 a, b, c = pathToTravel[0].Position;
		yield return LookAt(pathToTravel[1].Position);

		if (!currentTravelLocation) {
			currentTravelLocation = pathToTravel[0];
		}
        if(this.Faccao==P.Faccao)
		    Grid.DecreaseVisibility(currentTravelLocation, VisionRange);
		int currentColumn = currentTravelLocation.ColumnIndex;

		float t = Time.deltaTime * travelSpeed;
		for (int i = 1; i < pathToTravel.Count; i++) {
			currentTravelLocation = pathToTravel[i];
			a = c;
			b = pathToTravel[i - 1].Position;

			int nextColumn = currentTravelLocation.ColumnIndex;
			if (currentColumn != nextColumn) {
				if (nextColumn < currentColumn - 1) {
					a.x -= HexMetrics.innerDiameter * HexMetrics.wrapSize;
					b.x -= HexMetrics.innerDiameter * HexMetrics.wrapSize;
				}
				else if (nextColumn > currentColumn + 1) {
					a.x += HexMetrics.innerDiameter * HexMetrics.wrapSize;
					b.x += HexMetrics.innerDiameter * HexMetrics.wrapSize;
				}
				Grid.MakeChildOfColumn(transform, nextColumn);
				currentColumn = nextColumn;
			}

			c = (b + currentTravelLocation.Position) * 0.5f;
            if(this.Faccao==P.Faccao)
			    Grid.IncreaseVisibility(pathToTravel[i], VisionRange);

			for (; t < 1f; t += Time.deltaTime * travelSpeed) {
				transform.localPosition = Bezier.GetPoint(a, b, c, t);
				Vector3 d = Bezier.GetDerivative(a, b, c, t);
				d.y = 0f;
				transform.localRotation = Quaternion.LookRotation(d);
				yield return null;
			}
            if(this.Faccao==P.Faccao)
			    Grid.DecreaseVisibility(pathToTravel[i], VisionRange);
			t -= 1f;
		}
		currentTravelLocation = null;

		a = c;
		b = location.Position;
		c = b;
        if(this.Faccao==P.Faccao)
		    Grid.IncreaseVisibility(location, VisionRange);
		for (; t < 1f; t += Time.deltaTime * travelSpeed) {
			transform.localPosition = Bezier.GetPoint(a, b, c, t);
			Vector3 d = Bezier.GetDerivative(a, b, c, t);
			d.y = 0f;
			transform.localRotation = Quaternion.LookRotation(d);
			yield return null;
		}

		transform.localPosition = location.Position;
		orientation = transform.localRotation.eulerAngles.y;
		ListPool<HexCell>.Add(pathToTravel);
		pathToTravel = null;
	}

	IEnumerator LookAt (Vector3 point) {
		if (HexMetrics.Wrapping) {
			float xDistance = point.x - transform.localPosition.x;
			if (xDistance < -HexMetrics.innerRadius * HexMetrics.wrapSize) {
				point.x += HexMetrics.innerDiameter * HexMetrics.wrapSize;
			}
			else if (xDistance > HexMetrics.innerRadius * HexMetrics.wrapSize) {
				point.x -= HexMetrics.innerDiameter * HexMetrics.wrapSize;
			}
		}

		point.y = transform.localPosition.y;
		Quaternion fromRotation = transform.localRotation;
		Quaternion toRotation =
			Quaternion.LookRotation(point - transform.localPosition);
		float angle = Quaternion.Angle(fromRotation, toRotation);

		if (angle > 0f) {
			float speed = rotationSpeed / angle;
			for (
				float t = Time.deltaTime * speed;
				t < 1f;
				t += Time.deltaTime * speed
			) {
				transform.localRotation =
					Quaternion.Slerp(fromRotation, toRotation, t);
				yield return null;
			}
		}

		transform.LookAt(point);
		orientation = transform.localRotation.eulerAngles.y;
	}

	public virtual int GetMoveCost (
		HexCell fromCell, HexCell toCell, HexDirection direction)
	{
		if (!IsValidDestination(toCell)) {
			return -1;
		}
		HexEdgeType edgeType = fromCell.GetEdgeType(toCell);
		if (edgeType == HexEdgeType.Cliff) {
			return -1;
		}
		int moveCost;
		if (fromCell.HasRoadThroughEdge(direction)) {
			moveCost = 2;
		}
		else if (fromCell.Walled != toCell.Walled) {
			return -1;
		}
		else {
			moveCost = edgeType == HexEdgeType.Flat ? 5 : 7;
			moveCost +=
				toCell.UrbanLevel + toCell.FarmLevel + toCell.PlantLevel;
		}
		return moveCost;
	}

	public void Die () {
		if (location) {
			Grid.DecreaseVisibility(location, VisionRange);
		}
		location.Unit = null;
		Destroy(gameObject);
	}

	public void Save (BinaryWriter writer) {
		location.coordinates.Save(writer);
		writer.Write(orientation);
	}

    public static void Load (BinaryReader reader, HexGrid grid) {
		HexCoordinates coordinates = HexCoordinates.Load(reader);
		float orientation = reader.ReadSingle();
		grid.AddUnit(Instantiate(HexGrid.unitPrefabsI[0]), grid.GetCell(coordinates), orientation, "Barbaros", false);
	}

	void OnEnable () {
        P = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
		if (location) {
			transform.localPosition = location.Position;
			if (currentTravelLocation) {
                if(Faccao==P.Faccao){
				    Grid.IncreaseVisibility(location, VisionRange);
				    Grid.DecreaseVisibility(currentTravelLocation, VisionRange);
                }
				currentTravelLocation = null;
			}
		}
	}

    public void Passe() { 
        canAttack=true;
        CanMove=true;
        location.EnableHighlight(Color.black);
    }

    public float vantagem(int AtcType, int DefType, float dano)
    {
        float vant = 0;
        if (0 <= AtcType && AtcType < 3)
        {
            if (0 <= DefType && DefType < 3)
                vant = DefType - AtcType;

            else if (AtcType == 2 && (DefType == 0 || DefType == 3))
                vant = 1;
            else if (AtcType == 0 && DefType == 4)
                vant = -1;
        }
        else if (AtcType == 3)
        {
            if (DefType == 0)
                vant = 1;
            else if (DefType == 2)
            {
                vant = -1;
            }
        }
        else if (AtcType == 4 && DefType == 0)
            vant = -1;

        if (vant == 1)
            dano = (float)1.25 * dano;
        else if (vant == -1)
            dano = (float)0.75 * dano;

        return dano;
    }

    //	void OnDrawGizmos () {
    //		if (pathToTravel == null || pathToTravel.Count == 0) {
    //			return;
    //		}
    //
    //		Vector3 a, b, c = pathToTravel[0].Position;
    //
    //		for (int i = 1; i < pathToTravel.Count; i++) {
    //			a = c;
    //			b = pathToTravel[i - 1].Position;
    //			c = (b + pathToTravel[i].Position) * 0.5f;
    //			for (float t = 0f; t < 1f; t += 0.1f) {
    //				Gizmos.DrawSphere(Bezier.GetPoint(a, b, c, t), 2f);
    //			}
    //		}
    //
    //		a = c;
    //		b = pathToTravel[pathToTravel.Count - 1].Position;
    //		c = b;
    //		for (float t = 0f; t < 1f; t += 0.1f) {
    //			Gizmos.DrawSphere(Bezier.GetPoint(a, b, c, t), 2f);
    //		}
    //	}
}