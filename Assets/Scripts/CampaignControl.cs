using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;

public class CampaignControl : MonoBehaviour {
    
    public SaveLoadMenu SLM;
    public static string actualFaction;
    public static int actualLevel;
    public static int actualFactionIndex;
    public static List<Faccao> faccoes = new List<Faccao>();
    public List<GameObject> storyIntro = new List<GameObject>();
    public List<GameObject> storyOutro = new List<GameObject>();

    // Use this for initialization
    void Start () {
        Debug.Log(Application.persistentDataPath + " <- Persistent / Not persistent -> " + Application.dataPath);
        //Create the directory to player maps
        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "PlayerMaps")))
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "PlayerMaps"));
        // activates the story intro
        storyIntro[actualLevel].SetActive(true);
        //loads the path to the actualFaction and actuallevel
        SLM.Load(Path.Combine(Application.dataPath, Path.Combine("Maps", Path.Combine(actualFaction, actualLevel.ToString() + ".map"))));
    }
    //activates the story outro
    public bool ActiveOutro(){
        Debug.Log(actualLevel);
        storyOutro[actualLevel].SetActive(true);
        return true;
    }

    public bool NextLevel (){
        if (actualLevel + 1 == 5)
            return false;
        faccoes[actualFactionIndex].completedLevels[actualLevel] = true;
        storyOutro[actualLevel].SetActive(false);
        actualLevel++;
        Save();
        string path = Path.Combine(Application.dataPath, Path.Combine("Maps", Path.Combine(actualFaction, actualLevel.ToString() + ".map")));
        if (File.Exists(path))
        {
            SceneManager.LoadScene(1);
            return true;
        }
        else
            return false;
    }

    public static int FindLastLevel (){
        for (int j = 0; j < 5; j++)
            if (faccoes[actualFactionIndex].completedLevels[j] == false)
                return j;
        return 4;
    }

    //Saves the list of completed levels to a file
    public static void Save (){
        string savePath = Path.Combine(Application.persistentDataPath, "Campaign.save");
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(savePath);
        bf.Serialize(file, faccoes);
        file.Close();
    }

    //Loads the list of completed levels from a file
    public static void Load (){
        //the path to the file
        string savePath = Path.Combine(Application.persistentDataPath, "Campaign.save");
        //if the file exists
        if (File.Exists(savePath)){
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(savePath, FileMode.Open);
            //opens and loads it
            faccoes = bf.Deserialize(file) as List<Faccao>;
            file.Close();
        }
        //if it doesnt
        else{
            //creates all factions and stores it with no completed level
            Faccao aux = new Faccao("Visokea");
            for (int i = 0; i < 5; i++)
                aux.completedLevels[i] = false;
            faccoes.Add(aux);
            //aqui deverá ser adicionado à lista todas as faccoes;
            //saves it
            Save();
        }
        return;
    }
}
