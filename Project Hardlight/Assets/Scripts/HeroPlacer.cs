using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroPlacer : MonoBehaviour
{
    public List<GameObject> heroes;
    public int numHeroesLeftToPlace = -1;
    public int index = 0;

    public GameObject currentObj;
    private GameObject dropZoneParent; // All drop zones must be parented to a GameObject called "DropZones"
    private bool checkDropZonePlacement;

    public void StartHeroPlacement(List<GameObject> objs)
    {
        index = 0;
        heroes.Clear();
        heroes = objs;

        // hide all heros before we place them
        foreach (GameObject vessel in objs)
        {
            vessel.SetActive(false);
        }

        // init drop zones
        checkDropZonePlacement = false;
        dropZoneParent = GameObject.Find("DropZones");
        if (dropZoneParent != null)
        {
            // check for drop zones only if this parent has active children
            // so that we just allow any placement if there are no active drop zones
            Transform[] children = dropZoneParent.GetComponentsInChildren<Transform>();
            checkDropZonePlacement = (children.Length > 1);
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

        // disable drop zones 
        if (dropZoneParent != null)
        {
            dropZoneParent.SetActive(false);
        }

        GameManager.Instance.StartFighting();
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHeroPlacement();
    }

    /// <summary>
    /// Makes current hero follow the cursor and places hero if the player clicks in a valid location
    /// </summary>
    void UpdateHeroPlacement()
    {
        if (numHeroesLeftToPlace > 0)
        {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentObj.transform.position = new Vector3(worldPoint.x, worldPoint.y, -1);
            if (Input.GetMouseButtonDown(0) && BattleManager.Instance.AllowClick(Input.mousePosition, checkDropZonePlacement))
            {
                currentObj.transform.parent = GameObject.Find("Vessels").transform;
                numHeroesLeftToPlace--;
                index++;
                NextHeroPlacement();
            }

        }
    }
}