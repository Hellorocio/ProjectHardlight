using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SoulUpgradeUI : Singleton<SoulUpgradeUI>
{
    [Header("Soul Grid")]
    public GameObject soulIconPrefab;
    public GameObject soulGrid;

    [Header("Soul Details")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public Image soulImage;

    public TextMeshProUGUI[] boosts; //[0] = attackDmgBoost, [1] = attackSpeedBoost, [2] = abilityBoost, [3] = healthBoost
    public GameObject[] statFocusBoxes;

    public GameObject[] allightIcons; //[0] = sun, [1] = moon, [2] = stars
    public TextMeshProUGUI[] allightValues;

    private SoulManager soulManager;
    
    private void OnEnable()
    {
        soulManager = SoulManager.Instance;

        print("enable soul upgrade UI");
        PopulateSoulGrid();
        if (GameManager.Instance.souls != null && GameManager.Instance.souls.Count > 0)
        {
            SetSoulDetails(GameManager.Instance.souls[0]);
        }
    }

    // Create the icons in the grid based on the souls in GameManager
    public void PopulateSoulGrid()
    {
        // Destroy existing
        foreach (Transform child in soulGrid.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        foreach (Soul soul in GameManager.Instance.souls)
        {
            GameObject soulIcon = Instantiate(soulIconPrefab);

            soulIcon.GetComponent<SoulIcon>().SetSoul(soul);
            soulIcon.GetComponent<DraggableIcon>().allowDrag = false;
            soulIcon.transform.SetParent(soulGrid.transform);
            soulIcon.transform.localScale = Vector3.one;
        }
    }

    public void SetSoulDetails (Soul soul)
    {
        nameText.text = soul.title;
        levelText.text = soul.level.ToString();
        soulImage.sprite = soul.appearance;

        SetStatBoosts(soul);
        SetStatFocusBoxes(soul);
        SetAllightDetails(soul);
    }

    private void SetStatFocusBoxes(Soul soul)
    {
        foreach(GameObject g in statFocusBoxes)
        {
            g.SetActive(false);
        }
        if (soul.statFocuses.Contains(StatFocusType.ATTACK))
        {
            statFocusBoxes[0].SetActive(true);
        }
        if (soul.statFocuses.Contains(StatFocusType.ATTACKSPEED))
        {
            statFocusBoxes[1].SetActive(true);
        }
        if (soul.statFocuses.Contains(StatFocusType.ABILITY))
        {
            statFocusBoxes[2].SetActive(true);
        }
        if (soul.statFocuses.Contains(StatFocusType.HEALTH))
        {
            statFocusBoxes[3].SetActive(true);
        }
    }


    private void SetAllightDetails (Soul soul)
    {
        foreach (GameObject g in allightIcons)
        {
            g.SetActive(false);
        }

        foreach (AllightAttribute a in soul.allightAttributes)
        {
            string allightValueText = a.currentValue + "/" + a.baseValue;
            switch (a.allightType)
            {
                case AllightType.SUNLIGHT:
                    allightIcons[0].SetActive(true);
                    allightValues[0].text = allightValueText;
                    break;
                case AllightType.MOONLIGHT:
                    allightIcons[1].SetActive(true);
                    allightValues[1].text = allightValueText;
                    break;
                case AllightType.STARLIGHT:
                    allightIcons[2].SetActive(true);
                    allightValues[2].text = allightValueText;
                    break;
                default:
                    break;
            }
        }
        
    }

    /// <summary>
    /// Shows +X on bonuses if newLevel is less than or equal to the current level
    /// </summary>
    /// <param name="currentLevel"></param>
    /// <param name="newLevel"></param>
    public void SetStatBoosts (Soul soul, int newLevel = -1)
    {
        string[] percentChange = new string[4];
        string[] flatChange = new string[4];
        
        if (newLevel >= soul.level)
        {
            percentChange[0] = " <color=green>+" + (soul.GetPercentAttackBoost(newLevel) - soul.GetPercentAttackBoost()) + "%</color>";
            flatChange[0] = " <color=green>+" + (soul.GetFlatAttackBoost(newLevel) - soul.GetFlatAttackBoost()) + "%</color>";

            percentChange[1] = " <color=green>+" + (soul.GetPercentAttackSpeedBoost(newLevel) - soul.GetPercentAttackSpeedBoost()) + "%</color>";
            flatChange[1] = " <color=green>+" + (soul.GetFlatAttackSpeedBoost(newLevel) - soul.GetFlatAttackSpeedBoost()) + "%</color>";

            percentChange[2] = " <color=green>+" + (soul.GetPercentHealthBoost(newLevel) - soul.GetPercentHealthBoost()) + "%</color>";
            flatChange[2] = " <color=green>+" + (soul.GetFlatHealthBoost(newLevel) - soul.GetFlatHealthBoost()) + "%</color>";

            percentChange[3] = " <color=green>+" + (soul.GetPercentAbilityBoost(newLevel) - soul.GetPercentAbilityBoost()) + "%</color>";
            flatChange[3] = " <color=green>+" + (soul.GetFlatAbilityBoost(newLevel) - soul.GetFlatAbilityBoost()) + "%</color>";
        }

        boosts[0].text = "+" + soul.GetPercentAttackBoost() + "%" + percentChange[0] + 
            "\n+" + soul.GetFlatAttackBoost() + flatChange[0];

        boosts[1].text = "+" + soul.GetPercentAttackSpeedBoost() + "%" + percentChange[1] +
            "\n+" + soul.GetFlatAttackSpeedBoost() + flatChange[1];

        boosts[2].text = "+" + soul.GetPercentAbilityBoost() + "%" + percentChange[3] +
            "\n+" + soul.GetFlatAbilityBoost() + flatChange[2];

        boosts[3].text = "+" + soul.GetPercentHealthBoost() + "%" + percentChange[2] +
            "\n+" + soul.GetFlatHealthBoost() + flatChange[3];

    }
}
