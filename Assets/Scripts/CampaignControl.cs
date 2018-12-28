using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class CampaignControl : MonoBehaviour {
    
    public SaveLoadMenu SLM;
    public static string actualFaction;
    private static int actualLevel;
    private static int actualFactionIndex;
    public static List<Faccao> faccoes = new List<Faccao>();

    // Use this for initialization
    void Start () {
        Debug.Log(Application.persistentDataPath + " <- Persistent / Not persistent -> " + Application.dataPath);
        //Create the directory to player maps
        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "PlayerMaps")))
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "PlayerMaps"));
        //Searches the level in which the player shall continue
        if ((actualLevel = FindActualLevel()) == -1){
            Debug.LogWarning("No level for this faccion found");
        }
        //loads the path to the actualFaction and actuallevel
        string path = Path.Combine(Application.dataPath, Path.Combine("Maps", Path.Combine(actualFaction, actualLevel.ToString() + ".map")));
        SLM.Load(path);
    }

    public void Update()
    {
        Debug.LogWarning("Actual level = " + actualLevel);
    }

    public bool NextLevel (){
        faccoes[actualFactionIndex].completedLevels[actualLevel] = true;
        for (int i = 0; i < 10; i++)
        {
            Debug.Log("Fase "+ i + " " + faccoes[actualFactionIndex].completedLevels[i]);
        }
        actualLevel++;
        string path = Path.Combine(Application.dataPath, Path.Combine("Maps", Path.Combine(actualFaction, actualLevel.ToString() + ".map")));
        if (File.Exists(path))
        {
            SLM.Load(path);
            return true;
        }
        else
            return false;
    }

    int FindActualLevel (){
        for (int i = 0; i < faccoes.Count; i++)
            if (faccoes[i].factionName == actualFaction)
            {
                actualFactionIndex = i;
                for (int j = 0; j < 10; j++)
                    if (faccoes[i].completedLevels[j] == false)
                        return j;
                break;
            }
        return -1;
    }

    //Saves the list of completed levels to a file
    public static void Save (){
        string savePath = Path.Combine(Application.persistentDataPath, "Campaign.save");
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(savePath, FileMode.Create); //File.Create(savePath);
        bf.Serialize(file, faccoes);
        file.Close();
    }

    //Loads the list of completed levels from a file
    public static void Load (){
        //the path to the file
        string savePath = Path.Combine(Application.persistentDataPath, "Campaign.save");
        //if the file exists
        if (faccoes.Count.Equals(0))
        {
            if (File.Exists(savePath))
            {
                Debug.Log("campaign.save LOADED");
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(savePath, FileMode.Open);
                //opens and loads it
                faccoes = bf.Deserialize(file) as List<Faccao>;
                file.Close();
            }
            //if it doesnt
            else
            {
                //creates all factions and stores it with no completed level
                Faccao aux = new Faccao("Visokea");
                faccoes.Add(aux);
                aux = new Faccao("GenericFaccion");
                faccoes.Add(aux);
                //aqui deverá ser adicionado à lista todas as faccoes;
                //saves it
                Save();
            }
        }
        return;
    }

    public static int CheckCampaignMaps(){
        //if there isnt the maps folder, return -1
        if (!Directory.Exists(Path.Combine(Application.dataPath, "Maps")))
        {
            Debug.LogWarning("Maps não existe");
            return -1;
        }
        else
        {
            //if there isnt a expected faction, return -2
            string[] directoryPaths = Directory.GetDirectories(Path.Combine(Application.dataPath, "Maps"));
            if (directoryPaths.Length < faccoes.Count)
            {
                Debug.LogWarning("Número de pastas diferente do número de faccoes");
                return -2;
            }
            else
            {
                //if a faction has no maps, return -3
                for (int i = 0; i < directoryPaths.Length; i++)
                {
                    string[] filePaths = Directory.GetFiles(Path.Combine(Application.dataPath, Path.Combine("Maps", directoryPaths[i])), "*.map");
                    if (filePaths.Length <= 0)
                    {
                        Debug.LogWarning("Alguma pasta de mapa está vazia");
                        return -3;
                    }
                }
            }
        }
        //if there are at least 1 map in all registered factions, return 1
        return 1;
    }
}
