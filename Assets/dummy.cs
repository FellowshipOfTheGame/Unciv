using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class dummy : MonoBehaviour {

    public SaveLoadMenu SLM;
	public Player player;
	
	// Use this for initialization
	void Start () {
		//Cria o diretorio de mapas personalizados
		if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "PlayerMaps")))
			Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "PlayerMaps"));
		//encontra o .map do level do player e carrega
		string path = Path.Combine(Application.persistentDataPath, Path.Combine("Maps", player.level.ToString() + ".map"));
		SLM.Load(path);
	}
}
