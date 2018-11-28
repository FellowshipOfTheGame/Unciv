﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class SaveLoadMenu : MonoBehaviour {

	const int mapFileVersion = 7;

	public bool customMap;

	public Text menuLabel, actionButtonLabel;

	public InputField nameInput;

	public RectTransform listContent;

	public SaveLoadItem itemPrefab;

	public HexGrid hexGrid;

	bool saveMode;

	public void Open (bool saveMode) {
		this.saveMode = saveMode;
		if (saveMode) {
			menuLabel.text = "Save Map";
			actionButtonLabel.text = "Save";
		}
		else {
			menuLabel.text = "Load Map";
			actionButtonLabel.text = "Load";
		}
		FillList();
		gameObject.SetActive(true);
		HexMapCamera.Locked = true;
	}

	public void Close () {
		gameObject.SetActive(false);
		HexMapCamera.Locked = false;
	}

	public void Action () {
		string path = GetSelectedPath();
		if (path == null) {
			return;
		}
		if (saveMode) {
			Save(path);
		}
		else {
			Load(path);
		}
		Close();
	}

	public void SelectItem (string name) {
		nameInput.text = name;
	}

	public void Delete () {
		string path = GetSelectedPath();
		if (path == null) {
			return;
		}
		if (File.Exists(path)) {
			File.Delete(path);
		}
		nameInput.text = "";
		FillList();
	}

	void FillList () {
		for (int i = 0; i < listContent.childCount; i++) {
			Destroy(listContent.GetChild(i).gameObject);
		}
		string[] paths;
		if (!customMap)
			paths = Directory.GetFiles(Path.Combine(Application.dataPath, "Maps"), "*.map");
		else
			paths = Directory.GetFiles(Path.Combine(Application.persistentDataPath, "PlayerMaps"), "*.map");
		Array.Sort(paths);
		for (int i = 0; i < paths.Length; i++) {
			SaveLoadItem item = Instantiate(itemPrefab);
			item.menu = this;
			item.MapName = Path.GetFileNameWithoutExtension(paths[i]);
			item.transform.SetParent(listContent, false);
		}
	}

	string GetSelectedPath () {
		string mapName = nameInput.text;
		if (mapName.Length == 0)
			return null;
		
		string path;
		if (!customMap)
			path = Path.Combine(Application.dataPath, Path.Combine("Maps", mapName + ".map"));
		else
			path = Path.Combine(Application.persistentDataPath, Path.Combine("PlayerMaps", mapName + ".map"));
		
		return path;
	}

	public void Save (string path) {
		using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create))) {
			writer.Write(mapFileVersion);
			hexGrid.Save(writer);
		}
	}

	public void Load (string path) {
		if (!File.Exists(path)) {
			Debug.LogError("File does not exist " + path);
			return;
		}
		using (BinaryReader reader = new BinaryReader(File.OpenRead(path))) {
			int header = reader.ReadInt32();
			if (header <= mapFileVersion) {
				hexGrid.Load(reader, header);
				HexMapCamera.ValidatePosition();
			}
			else {
				Debug.LogWarning("Unknown map format " + header);
			}
		}
	}
}