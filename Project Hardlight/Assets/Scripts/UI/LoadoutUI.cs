using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class LoadoutUI : Singleton<LoadoutUI>
{
    public bool loadoutCreated = false;
    public GameObject loadoutSlotPrefab;
    public GameObject loadoutSlots;
    
    public GameObject vesselIconPrefab;
    public GameObject vesselGrid;

    public GameObject soulIconPrefab;
    public GameObject soulGrid;

    public GameObject detailPane;

    public GameObject goButton;
    public TextMeshProUGUI missingPopupText;

    ////////////// Vessel detail
    public TextMeshProUGUI nameText;
    public Image vesselImage;

    public TextMeshProUGUI healthNumber;
    public TextMeshProUGUI abilityNumber;
    public TextMeshProUGUI attackSpeed;
    public TextMeshProUGUI speedNumber;

    public TextMeshProUGUI basicAttackName;
    public TextMeshProUGUI basicAttackDesc;
    public TextMeshProUGUI basicAttackDamage;
    public TextMeshProUGUI basicAttackRange;

    public AbilityDetail abilityOneDetail;
    public AbilityDetail abilityTwoDetail;
    
    public Soul defaultSoul;

    public GameObject selectedVessel;
    public GameObject selectedIcon;

    ///////////////////

    public void Refresh ()
    {
        loadoutCreated = false;
        CreateLoadout();
    }

    public void CreateLoadout()
    {
        if (!loadoutCreated)
        {
            loadoutCreated = true;
            CreateLoadoutSlots();
            PopulateVesselGrid();
            PopulateSoulGrid();
        }
        else
        {
            Debug.Log("Trying to create loadout UI when already made");
        }
    }

    // Sets vessel to display details for TODO do for Souls
    public void SetDetailPane(GameObject vessel, GameObject icon = null)
    {
        VesselData vesselData = vessel.GetComponent<VesselData>();
        Soul selectedSoul = defaultSoul;
        if (icon != null && icon.transform.parent.parent.gameObject.name == "LoadoutSlot(Clone)")
        {
            // set selected soul to the actual selected soul
            selectedSoul = icon.transform.parent.parent.gameObject.GetComponentInChildren<SoulIcon>().soul;

            if (selectedSoul == null)
            {
                selectedSoul = defaultSoul;
            }
        }

        nameText.text = vesselData.vesselName + " the " + vesselData.title;
        vesselImage.sprite = vesselData.appearance;

        string healthText = vesselData.baseHealth.ToString();

        BasicAttackAction basicAttack = (BasicAttackAction)vesselData.basicAttack;
        string attackDmgText = basicAttack.damage.ToString();
        basicAttackName.text = basicAttack.title + " (Basic Attack)";
        basicAttackDesc.text = basicAttack.description;
        basicAttackRange.text = basicAttack.range.ToString();
        basicAttackDamage.text = basicAttack.damage.ToString() + AddSoulBonusStatDetail(selectedSoul.GetAttackBonus(basicAttack.damage));
        
        healthNumber.text = vesselData.baseHealth.ToString() + AddSoulBonusStatDetail(selectedSoul.GetHealthBonus(vesselData.baseHealth));
        abilityNumber.text = vesselData.baseAbility.ToString() + AddSoulBonusStatDetail(selectedSoul.GetAbilityBonus(vesselData.baseAbility));
        attackSpeed.text = basicAttack.frequency.ToString() + AddSoulBonusStatDetail(selectedSoul.GetAttackSpeedBonus(basicAttack.frequency));
        speedNumber.text = vesselData.baseMovementSpeed.ToString();


        Ability abilityOne = (Ability) vesselData.abilities[0];
        abilityOneDetail.SetAbility(abilityOne);
        abilityOneDetail.UpdateSoul(selectedSoul);
        
        Ability abilityTwo = (Ability) vesselData.abilities[1];
        abilityTwoDetail.SetAbility(abilityTwo);
        abilityTwoDetail.UpdateSoul(selectedSoul);
    }

    private string AddSoulBonusStatDetail (float soulBonus)
    {
        string bonus = "";
        if (soulBonus > 0)
        {
            bonus = "<color=green> +" + soulBonus.ToString() + "</color>";
        }
        return bonus;
    }

    public void CreateLoadoutSlots()
    {
        // Destroy existing
        foreach (Transform child in loadoutSlots.transform) {
            GameObject.Destroy(child.gameObject);
        }

        // Create per required slot
        for (int i = 0; i < GameManager.Instance.requiredVessels; i++) {
            GameObject loadoutSlot = Instantiate(loadoutSlotPrefab, loadoutSlots.transform, true);
            loadoutSlot.transform.localScale = Vector3.one;
        }
    }

    // Create the icons in the grid based on the VesselManager
    // Also sets the detail window for the first vessel
    public void PopulateVesselGrid()
    {
        // Destroy existing
        foreach (Transform child in vesselGrid.transform) {
            GameObject.Destroy(child.gameObject);
        }

        // Create per vessel
        bool setDetails = false;
        foreach (VesselCatalogEntry entry in VesselManager.Instance.vesselCatalog) {
            if (entry.enabled)
            {
                GameObject vesselIcon = Instantiate(vesselIconPrefab, vesselGrid.transform, true);
                vesselIcon.GetComponent<VesselIcon>().SetVessel(entry.vessel);
                vesselIcon.transform.localScale = Vector3.one;

                if (!setDetails)
                {
                    SetDetailPane(entry.vessel);
                    setDetails = true;
                }
            }
        }
    }
    
    // Create the icons in the grid based on the souls in GameManager
    public void PopulateSoulGrid()
    {
        // Destroy existing
        foreach (Transform child in soulGrid.transform) {
            GameObject.Destroy(child.gameObject);
        }
        foreach (Soul soul in GameManager.Instance.souls) {
            GameObject soulIcon = Instantiate(soulIconPrefab);
            
            soulIcon.GetComponent<SoulIcon>().SetSoul(soul);
            soulIcon.transform.SetParent(soulGrid.transform);
            soulIcon.transform.localScale = Vector3.one;
        }
    }

    public void ClickGoButton()
    {
        if (IsLoadoutValid(true))
        {
            // Create vessels based on player's selected loadout. TODO(mchi) put these GOs somewhere sane
            foreach (Transform child in loadoutSlots.transform)
            {
                GameObject loadoutSlot = child.gameObject;
                GameObject slotVessel = loadoutSlot.GetComponentInChildren<VesselIcon>().vessel;
                Soul slotSoul = loadoutSlot.GetComponentInChildren<SoulIcon>().soul;

                GameObject selectedVessel = Instantiate(slotVessel);
                selectedVessel.GetComponent<Fighter>().soul = slotSoul;
                BattleManager.Instance.selectedVessels.Add(selectedVessel);
            }

            // Start placing newly created vessels
            GameManager.Instance.StartVesselPlacement();
        }
    }

    /// <summary>
    /// Returns true if loadout is valid, false otherwise
    /// </summary>
    /// <returns></returns>
    bool IsLoadoutValid (bool printMissing = false)
    {
        // Validate Vessels
        foreach (Transform child in loadoutSlots.transform)
        {
            GameObject loadoutSlot = child.gameObject;
            GameObject slotVessel = loadoutSlot.GetComponentInChildren<VesselIcon>().vessel;
            Soul slotSoul = loadoutSlot.GetComponentInChildren<SoulIcon>().soul;
            if (slotVessel == null)
            {
                if (printMissing && missingPopupText != null)
                {
                    missingPopupText.text = "Missing Vessel!";
                    missingPopupText.transform.parent.gameObject.SetActive(true);
                }

                return false;
            }
            if (slotSoul == null)
            {
                if (printMissing && missingPopupText != null)
                {
                    missingPopupText.text = "Missing Soul!";
                    missingPopupText.transform.parent.gameObject.SetActive(true);
                }

                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Change GO button between gray and orange depending on if loudout is set
    /// Also updates vessel details (switches panel to whichever was just changed)
    /// Called on every change to selection icons
    /// </summary>
    public void LoadoutUpdated (GameObject icon)
    {
        ActivateButton goButtonActivate = goButton.GetComponent<ActivateButton>();
        if (goButtonActivate != null)
        {
            goButtonActivate.SetButtonActivation(IsLoadoutValid(false));
        }

        GameObject changedVessel = null;
        if (icon.GetComponent<SoulIcon>() != null)
        {
            changedVessel = icon.transform.parent.parent.GetComponentInChildren<VesselIcon>().vessel;
        }
        else if (icon.GetComponent<VesselIcon>() != null)
        {
            changedVessel = icon.GetComponent<VesselIcon>().vessel;
        }

        if (changedVessel != null)
        {
            SetDetailPane(changedVessel, icon);
        }

        // stop showing missing popup so it's not confusing
        if (missingPopupText != null)
        {
            missingPopupText.transform.parent.gameObject.SetActive(false);
        }
    }
}
