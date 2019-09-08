using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroPlacer : MonoBehaviour
{
    private List<Vector2> heroes = new List<Vector2>();
    private int numHeroesLeftToPlace = -1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartHeroPlacement(List<Vector2> list)
    {
        heroes = list;
        numHeroesLeftToPlace = list.Count;

    }
}
