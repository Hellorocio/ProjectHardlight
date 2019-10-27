using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class VesselIcon : BaseIcon
{
    // Set through the function SetVessel to actually update the UI
    [HideInInspector]
    public GameObject vessel;
    [HideInInspector]
    public VesselData vesselData;

    public void SetVessel (GameObject inVessel)
    {
        vessel = inVessel;
        vesselData = vessel.GetComponent<VesselData>();
        GetComponent<Image>().sprite = vesselData.appearance;
     }

    public override void Clear()
    {
        vessel = null;
        vesselData = null;
    }

    public override void SelectDetail()
    {
        if (vessel != null)
        {
            LoadoutUI.Instance.SetDetailPane(vessel);
        }
    }

    public override string GetHoverDesc()
    {
        string desc = "";
        if (vesselData != null)
        {
            desc = vesselData.vesselName;
        }
        return desc;
    }

    public override bool IconReady()
    {
        return (vesselData != null);
    }

}
