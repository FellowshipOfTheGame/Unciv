using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class dummy : MonoBehaviour {

    public SaveLoadMenu SLM;

	// Use this for initialization
	void Start () {
		SLM.Load(Path.Combine(Application.dataPath, "Teste.map"));
	}
}
