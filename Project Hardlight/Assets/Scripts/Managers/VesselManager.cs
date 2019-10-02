using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VesselManager : Singleton<VesselManager>
{

    public List<VesselCatalogEntry> vesselCatalog;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetAllVesselEnabledTo(bool enabled)
    {
        foreach (VesselCatalogEntry entry in vesselCatalog)
        {
            entry.enabled = enabled;
        }
    }

    public VesselCatalogEntry GetVesselEntryById(string id)
    {
        VesselCatalogEntry foundEntry = null;
        foreach (VesselCatalogEntry entry in vesselCatalog)
        {
            if (entry.vesselId == id)
            {
                foundEntry = entry;
                break;
            }
        }

        return foundEntry;
    }
}

[System.Serializable]
public class VesselCatalogEntry
{
    public string vesselId = "default";
    public bool enabled = true;
    public GameObject vessel;
}