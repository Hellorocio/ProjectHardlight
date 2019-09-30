using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadoutUI : Singleton<LoadoutUI>
{

    public GameObject vesselIconPrefab;
    public GameObject vesselGrid;

    public GameObject soulIconPrefab;
    public GameObject soulGrid;

    public void Refresh()
    {
        PopulateVesselGrid();
        PopulateSoulGrid();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PopulateVesselGrid()
    {
        // Destroy existing
        foreach (Transform child in vesselGrid.transform) {
            GameObject.Destroy(child.gameObject);
        }

        // Create per vessel
        foreach (GameObject vessel in VesselManager.Instance.vessels) {
            GameObject vesselIcon = Instantiate(vesselIconPrefab);
            vesselIcon.GetComponent<VesselIcon>().SetVessel(vessel);
            vesselIcon.transform.SetParent(vesselGrid.transform);
        }
    }

    public void PopulateSoulGrid()
    {
        // Destroy existing
        foreach (Transform child in soulGrid.transform) {
            GameObject.Destroy(child.gameObject);
        }

        // Create per vessel
        foreach (Soul soul in GameManager.Instance.souls) {
            GameObject soulIcon = Instantiate(soulIconPrefab);
            soulIcon.GetComponent<SoulIcon>().SetSoul(soul);
            soulIcon.transform.SetParent(soulGrid.transform);
        }
    }
}
