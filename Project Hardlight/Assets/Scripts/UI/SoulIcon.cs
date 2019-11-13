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

        ClearIcons();

        // Instantiate allight
        if (soul.GetAllightValue(AllightType.SUNLIGHT) != 0)
        {
            AddAllightIcon(AllightType.SUNLIGHT);
        }
        if (soul.GetAllightValue(AllightType.MOONLIGHT) != 0)
        {
            AddAllightIcon(AllightType.MOONLIGHT);
        }
        if (soul.GetAllightValue(AllightType.STARLIGHT) != 0)
        {
            AddAllightIcon(AllightType.STARLIGHT);
        }
        
        // Instantiate stat focuses
        if (soul.statFocuses.Contains(StatFocusType.HEALTH))
        {
            AddStatIcon(StatFocusType.HEALTH);
        }
        if (soul.statFocuses.Contains(StatFocusType.ABILITY))
        {
            AddStatIcon(StatFocusType.ABILITY);
        }
        if (soul.statFocuses.Contains(StatFocusType.ATTACK))
        {
            AddStatIcon(StatFocusType.ATTACK);
        }
        if (soul.statFocuses.Contains(StatFocusType.ATTACKSPEED))
        {
            AddStatIcon(StatFocusType.ATTACKSPEED);
        }
    }

    private void AddAllightIcon(AllightType type)
    {
        allightIconPanel.transform.GetChild((int)type).gameObject.SetActive(true);
    }
    
    private void AddStatIcon(StatFocusType type)
    {
        statfocusIconPanel.transform.GetChild((int)type).gameObject.SetActive(true);
    }

    private void ClearIcons()
    {
        for (int i = 0; i < allightIconPanel.transform.childCount; i++)
        {
            allightIconPanel.transform.GetChild(i).gameObject.SetActive(false);
        }

        for (int i = 0; i < statfocusIconPanel.transform.childCount; i++)
        {
            statfocusIconPanel.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Removes soul and disables image (from base class)
    /// </summary>
    public override void Clear()
    {
        base.Clear();
        soul = null;

        ClearIcons();

        allightIconPanel.SetActive(false);
        statfocusIconPanel.SetActive(false);
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

    public override void ShowIcon(bool show)
    {
        base.ShowIcon(show);
        allightIconPanel.SetActive(show);
        statfocusIconPanel.SetActive(show);
    }
}
