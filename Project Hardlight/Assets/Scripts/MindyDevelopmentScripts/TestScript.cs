using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{

    public TextAsset dialogue;

    public void TestDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogue);
    }

    public void TestVesselManagerStuff()
    {
        VesselManager.Instance.SetAllVesselEnabledTo(false);
    }
}
