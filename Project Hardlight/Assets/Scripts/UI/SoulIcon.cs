using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class SoulIcon : BaseIcon
{
    public GameObject allightIconPanel;
    public GameObject statfocusIconPanel;
    
    [Header("donut touch")]
    // Set through the function SetSoul to actually update the UI
    public Soul soul;

    public void SetSoul(Soul inSoul)
    {
        soul = inSoul;
        GetComponent<Image>().sprite = soul.appearance;
        
        // Instantiate allight
        if (soul.GetAllightValue(AllightType.SUNLIGHT) != 0)
        {
            AddAllightIcon(SoulManager.Instance.sunlightIcon);
        }
        if (soul.GetAllightValue(AllightType.MOONLIGHT) != 0)
        {
            AddAllightIcon(SoulManager.Instance.moonlightIcon);
        }
        if (soul.GetAllightValue(AllightType.STARLIGHT) != 0)
        {
            AddAllightIcon(SoulManager.Instance.starlightIcon);
        }
    }

    private void AddAllightIcon(Sprite sprite)
    {
        GameObject icon = Instantiate(SoulManager.Instance.iconPrefab, allightIconPanel.transform);
        icon.GetComponent<Image>().sprite = sprite;
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
