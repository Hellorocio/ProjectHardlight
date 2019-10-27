using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class SoulIcon : BaseIcon
{

    // Set through the function SetSoul to actually update the UI
    [HideInInspector]
    public Soul soul;

    public void SetSoul(Soul inSoul)
    {
        soul = inSoul;
        GetComponent<Image>().sprite = soul.appearance;
    }

    public override void Clear()
    {
        soul = null;
    }

    public override void SelectDetail()
    {
        if (soul != null && UIManager.Instance.soulUpgradeUI.activeSelf)
        {
            SoulUpgradeUI.Instance.SetSoulDetails(soul);
        }
    }

    public override string GetHoverDesc()
    {
        string desc = "";
        if (soul != null)
        {
            desc = soul.GetDescription();
        }
        return desc;
    }

    public override bool IconReady()
    {
        return (soul != null);
    }
}
