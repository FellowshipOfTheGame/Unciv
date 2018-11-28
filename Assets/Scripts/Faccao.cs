using UnityEngine;
using System;

[Serializable]
public class Faccao {
    public string factionName;
    public bool[] completedLevels = new bool[10];
    public Faccao (string factionNameLocal) {
        factionName = factionNameLocal;
        for (int i = 0; i < 10; i++)
            completedLevels[i] = false;
        return;
    }
}
