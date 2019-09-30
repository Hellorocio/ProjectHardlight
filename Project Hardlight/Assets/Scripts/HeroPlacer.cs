using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroPlacer : MonoBehaviour
{
    public GameObject tmpPrefab;
    private GameObject tmpInstance;
    private GameObject currentHeroPrefab;
    private List<GameObject> heroes = new List<GameObject>();
    private List<Fighter> heroScripts = new List<Fighter>();
    private int numHeroesLeftToPlace = -1;
    private int index = 0;
    private GameObject enemyParent;

    // Start is called before the first frame update
    void Start()
    {
        enemyParent = GameObject.Find("Enemies");
        
    }

    // Update is called once per frame
    void Update()
    {
        if(numHeroesLeftToPlace != -1)
        {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            tmpInstance.transform.position = new Vector3(worldPoint.x, worldPoint.y, -1);
            if (Input.GetMouseButtonDown(0))
            {
                GameObject h = Instantiate(heroes[index], tmpInstance.transform.position, tmpInstance.transform.rotation);
                h.transform.parent = GameObject.Find("Vessels").transform;
                heroScripts.Add(h.GetComponent<Fighter>());
                Destroy(tmpInstance);
                numHeroesLeftToPlace--;
                index++;
                NextHeroPlacement();
            }
            
        }
    }

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


            numHeroesLeftToPlace = -1;
            BattleManager battleManager = GameObject.Find("BattleManager").GetComponent<BattleManager>();
            battleManager.StartBattle();
            gameObject.SetActive(false);
        }

    }
}
