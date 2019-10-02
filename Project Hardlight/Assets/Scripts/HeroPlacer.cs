using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroPlacer : MonoBehaviour
{
    public List<GameObject> heroes;
    public int numHeroesLeftToPlace = -1;
    public int index = 0;

    public GameObject currentObj;

    public void StartHeroPlacement(List<GameObject> objs)
    {
        heroes.Clear();
        heroes = objs;
        //hide all heros before we place them
        foreach (GameObject vessel in objs)
        {
            vessel.SetActive(false);
        }

        numHeroesLeftToPlace = objs.Count;
        NextHeroPlacement();
    }

    private void NextHeroPlacement()
    {
        if (numHeroesLeftToPlace > 0)
        {
            currentObj = heroes[index];
            currentObj.SetActive(true);
        } else
        {
            EndHeroPlacement();
        }
    }

    private void EndHeroPlacement()
    {
        numHeroesLeftToPlace = 0;
        GameManager.Instance.StartFighting();
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(numHeroesLeftToPlace > 0)
        {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentObj.transform.position = new Vector3(worldPoint.x, worldPoint.y, -1);
            if (Input.GetMouseButtonDown(0))
            {
                currentObj.transform.parent = GameObject.Find("Vessels").transform;
                numHeroesLeftToPlace--;
                index++;
                NextHeroPlacement();
            }
            
        }
    }
}