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
    private List<GameObject> heroes = new List<GameObject>();
    private List<Fighter> heroScripts = new List<Fighter>();
    private int numHeroesLeftToPlace = -1;
    private int index = 0;
    public SoulStats soul1;
    public SoulStats soul2;
    public SoulStats soul3;
    public SoulStats soul4;
    public SoulStats soul5;
    public SoulStats soul6;
    private GameObject enemyParent;
    // Start is called before the first frame update
    void Start()
    {
        enemyParent = GameObject.Find("Enemies");
        foreach (Fighter f in enemyParent.GetComponentsInChildren<Fighter>())
        {
            f.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(numHeroesLeftToPlace != -1)
        {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            tmpInstance.transform.position = new Vector3(worldPoint.x, worldPoint.y, 0);
            if (Input.GetMouseButtonDown(0))
            {
                GameObject h = Instantiate(heroes[index], tmpInstance.transform.position, tmpInstance.transform.rotation);
                h.transform.parent = GameObject.Find("Players").transform;
                heroScripts.Add(h.GetComponent<Fighter>());

                //Debug.Log(h.GetComponent<Fighter>().soul);
                
                heroScripts[heroScripts.Count - 1].enabled = false;
                h.GetComponent<Fighter>().enabled = false;
                Destroy(tmpInstance);
                numHeroesLeftToPlace--;
                index++;
                NextHeroPlacement();
            }
            
        }
    }


    //public void StartHeroPlacement(List<Vector2> list)
    //{
        //heroes = list;
        //numHeroesLeftToPlace = list.Count;
        //NextHeroPlacement();
        

    //}

    public void StartHeroPlacement(List<GameObject> prefabs)
    {
        heroes = prefabs;
        numHeroesLeftToPlace = prefabs.Count;
        NextHeroPlacement();


    }


    private void NextHeroPlacement()
    {
        if (numHeroesLeftToPlace > 0)
        {
            tmpInstance = Instantiate(tmpPrefab);
            tmpInstance.GetComponent<SpriteRenderer>().sprite = heroes[index].GetComponentInChildren<SpriteRenderer>().sprite;
            tmpInstance.GetComponent<SpriteRenderer>().color = heroes[index].GetComponentInChildren<SpriteRenderer>().color;
            //GetHeroPrefabType((int)heroes[index].x);
        } else
        {
            if(tmpInstance != null)
            {
                Destroy(tmpInstance);
            }

            if(tmpPrefab != null)
            {
                tmpPrefab.SetActive(false);
            }
            foreach(Fighter f in heroScripts)
            {
                f.enabled = true;
            }

            foreach (Fighter f in enemyParent.GetComponentsInChildren<Fighter>())
            {
                f.enabled = true;
            }
            gameObject.SetActive(false);
        }

    }

    /*
    private void GetHeroPrefabType(int i)
    {
        if (i < 3) // hero is a merc
        {
            currentHeroPrefab = mercenaryPrefab;
            tmpInstance.GetComponent<SpriteRenderer>().sprite = mercenaryPrefab.GetComponentInChildren<SpriteRenderer>().sprite;
            tmpInstance.GetComponent<SpriteRenderer>().color = mercenaryPrefab.GetComponentInChildren<SpriteRenderer>().color;
        }
        else if (i < 5) // hero is a mage
        {
            currentHeroPrefab = magePrefab;
            tmpInstance.GetComponent<SpriteRenderer>().sprite = magePrefab.GetComponentInChildren<SpriteRenderer>().sprite;
            tmpInstance.GetComponent<SpriteRenderer>().color = magePrefab.GetComponentInChildren<SpriteRenderer>().color;
        }
        else if (i < 7) // hero is a healer
        {
            currentHeroPrefab = healerPrefab;
            tmpInstance.GetComponent<SpriteRenderer>().sprite = healerPrefab.GetComponentInChildren<SpriteRenderer>().sprite;
            tmpInstance.GetComponent<SpriteRenderer>().color = healerPrefab.GetComponentInChildren<SpriteRenderer>().color;
        }
    }
    */
}
