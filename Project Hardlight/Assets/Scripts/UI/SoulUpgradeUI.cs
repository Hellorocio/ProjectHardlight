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

    private Soul currentSoul;

    [Header("Fragment Details")]
    public GameObject fragmentDetails;
    public TextMeshProUGUI fragmentTitle;
    public Image fragmentIcon;
    public TextMeshProUGUI numFragments;
    public TMP_InputField inputField;
    public Button plusButton;
    public Button minusButton;
    public float fragmentChangeTime = 0.5f;
    private int fragmentsToAdd = 0;
    private AllightAttribute currentAllight;

    private IEnumerator holdButton;

    private SoulManager soulManager;
    
    private void OnEnable()
    {
        soulManager = SoulManager.Instance;
        
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
        currentSoul = soul;

        nameText.text = soul.title;
        levelText.text = soul.level.ToString();
        soulImage.sprite = soul.appearance;

        SetStatBoosts(soul);
        SetStatFocusBoxes(soul);
        SetAllightDetails(soul);
        CloseFragmentDetailsPanel();
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

    /// <summary>
    /// Called by "add" button under the allight values in the soul details window
    /// </summary>
    /// <param name="type"></param>
    public void SetFragmentDetails(int type)
    {
        fragmentDetails.SetActive(true);

        if (currentSoul.allightAttributes[0].allightType == (AllightType)type)
        {
            print("add to first attribute");
            currentAllight = currentSoul.allightAttributes[0];
        }
        else
        {
            // If the first soul doesn't have a matching allight attribute, then it must be on the second one
            // should there be a null check here? probably
            print("add to second attribute");
            currentAllight = currentSoul.allightAttributes[1];
        }

        fragmentsToAdd = 0;
        UpdateFragmentsToAdd();

        string title = currentAllight.allightType.ToString();
        fragmentTitle.text = title[0] + title.Substring(1).ToLower() + "\nFragment";
        fragmentIcon.sprite = soulManager.allightAppearances[type];
        numFragments.text = GameManager.Instance.fragments[type].ToString();
    }

    /// <summary>
    /// Called every time fragmentsToAdd changes
    /// Sets editText field and enables/disables appropriate buttons
    /// </summary>
    public void UpdateFragmentsToAdd (bool changeInput = true)
    {
        if (changeInput)
        {
            inputField.text = fragmentsToAdd.ToString();
        }

        minusButton.interactable = (fragmentsToAdd > 0);
        plusButton.interactable = (fragmentsToAdd < GameManager.Instance.fragments[(int)currentAllight.allightType]);
    }

    /// <summary>
    /// Called by "+" and "-" buttons in fragment details window
    /// </summary>
    public void ChangeFragmentsToAdd (int change)
    {
        fragmentsToAdd += change;
        UpdateFragmentsToAdd();
    }

    /// <summary>
    /// Called by edit text box in fragment details window
    /// </summary>
    /// <param name="set"></param>
    public void SetFragmentsToAdd (string set)
    {
        int setFragments = 0;
        
        int.TryParse(set, out setFragments);
        fragmentsToAdd = Mathf.Clamp(setFragments, 0, GameManager.Instance.fragments[(int)currentAllight.allightType]);

        UpdateFragmentsToAdd(false);
    }

    /// <summary>
    /// Called by "max" button in fragment details window
    /// Sets fragments to add to max of num fragments you have
    /// </summary>
    public void SetFragmentsMax ()
    {
        fragmentsToAdd = GameManager.Instance.fragments[(int)currentAllight.allightType];
        UpdateFragmentsToAdd();
    }

    /// <summary>
    /// Called by "clear" button in fragment details window
    /// Sets fragmentsToAdd to 0
    /// </summary>
    public void ClearFragmentsToAdd ()
    {
        fragmentsToAdd = 0;
        UpdateFragmentsToAdd();
    }

    /// <summary>
    /// Called by "add fragments" button in fragment details window
    /// Actually adds to currentValue of allightAttribute and 
    /// </summary>
    public void AddFragments ()
    {
        int allightIndex = currentSoul.allightAttributes.IndexOf(currentAllight);
        GameManager.Instance.fragments[(int)currentAllight.allightType] -= currentSoul.AddAllightValue(allightIndex, fragmentsToAdd);

        CloseFragmentDetailsPanel();
        SetSoulDetails(currentSoul);
    }

    /// <summary>
    /// Called by "x" button on FragmentDetailsPanel
    /// </summary>
    public void CloseFragmentDetailsPanel ()
    {
        fragmentDetails.SetActive(false);
    }

    public void OnChangeButtonDown (int change)
    {
        if (holdButton != null)
        {
            StopAllCoroutines();
        }

        holdButton = HoldButton(change);
        StartCoroutine(holdButton);

    }

    public void OnChangeButtonUp ()
    {
        if (holdButton != null)
        {
            StopAllCoroutines();
        }
    }

    IEnumerator HoldButton (int change)
    {
        Button currentButton = plusButton;
        if (change < 0)
        {
            currentButton = minusButton;
        }

        while (currentButton.IsInteractable())
        {
            yield return new WaitForSeconds(fragmentChangeTime);
            ChangeFragmentsToAdd(change);
        }
    }
}
