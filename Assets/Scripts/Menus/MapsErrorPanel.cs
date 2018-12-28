using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapsErrorPanel : MonoBehaviour
{
    [Header("Missing:")]
    public GameObject MapsFolder;
    public GameObject FactionFolder;
    public GameObject FactionMaps;

    public void MissingMaps ()
    {
        MapsFolder.SetActive(true);
    }

    public void MissingFactionFolder ()
    {
        FactionFolder.SetActive(true);
    }

    public void MissingFactionMaps ()
    {
        FactionMaps.SetActive(true);
    }
}
