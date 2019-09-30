using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class VesselIcon : MonoBehaviour
{

    // Set through the function SetVessel to actually update the UI
    public GameObject vessel;
    public VesselData vesselData;

    public void SetVessel(GameObject inVessel)
    {
        vessel = inVessel;
        vesselData = vessel.GetComponent<VesselData>();
        GetComponent<Image>().sprite = vesselData.appearance;

    }
}
