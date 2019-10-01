using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLocation : MonoBehaviour
{
    public string locationName;

    public string sceneName;

    public List<MapNeighbor> neighbors;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[System.Serializable]
public class MapNeighbor
{
    public GameObject path;
    public MapLocation neighbor;
}