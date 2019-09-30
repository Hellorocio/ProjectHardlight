using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class SoulIcon : MonoBehaviour
{

    // Set through the function SetSoul to actually update the UI
    public Soul soul;

    public void SetSoul(Soul inSoul)
    {
        soul = inSoul;
        GetComponent<Image>().sprite = soul.appearance;
    }
}
