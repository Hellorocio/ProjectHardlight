using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroPlacer : MonoBehaviour
{
    public GameObject mercenaryPrefab; // hero 1 or 2
    public GameObject magePrefab; // hero 3 or 4
    public GameObject healerPrefab; // hero 5 or 6
    public GameObject tmpPrefab;
    private GameObject tmpInstance;
    private GameObject currentHeroPrefab;
    private List<Vector2> heroes = new List<Vector2>();
    private int numHeroesLeftToPlace = -1;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(numHeroesLeftToPlace != -1)
        {
            if(numHeroesLeftToPlace > 0)
            {
                Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                tmpInstance.transform.position = new Vector3(worldPoint.x, worldPoint.y, 0);
                if (Input.GetMouseButtonDown(0))
                {
                    Instantiate(currentHeroPrefab, tmpInstance.transform.position, tmpInstance.transform.rotation);
                    Destroy(tmpInstance);
                    // reset mouse pos and do next hero
                }
            } else
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void StartHeroPlacement(List<Vector2> list)
    {
        heroes = list;
        numHeroesLeftToPlace = list.Count;
        tmpInstance = Instantiate(tmpPrefab);

        if(heroes[0].x < 3) // hero is a merc
        {
            currentHeroPrefab = mercenaryPrefab;
            tmpInstance.GetComponent<SpriteRenderer>().sprite = mercenaryPrefab.GetComponentInChildren<SpriteRenderer>().sprite;
        } else if(heroes[0].x < 5) // hero is a mage
        {
            currentHeroPrefab = magePrefab;
            tmpInstance.GetComponent<SpriteRenderer>().sprite = magePrefab.GetComponentInChildren<SpriteRenderer>().sprite;
        } else if(heroes[0].x < 7) // hero is a healer
        {
            currentHeroPrefab = healerPrefab;
            tmpInstance.GetComponent<SpriteRenderer>().sprite = healerPrefab.GetComponentInChildren<SpriteRenderer>().sprite;
        }
        

    }
}
