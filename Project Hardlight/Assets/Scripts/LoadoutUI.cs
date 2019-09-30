using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadoutUI : MonoBehaviour
{

    public GameObject vesselIconPrefab;
    public GameObject vesselGrid;

    // Start is called before the first frame update
    void Start()
    {
        PopulateVesselGrid();
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
}
