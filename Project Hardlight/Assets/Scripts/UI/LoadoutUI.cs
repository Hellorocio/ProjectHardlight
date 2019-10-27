using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class LoadoutUI : Singleton<LoadoutUI>
{
    public int requiredVessels = 3;

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
    public TextMeshProUGUI manaNumber;
    public TextMeshProUGUI abilityNumber;
    public TextMeshProUGUI speedNumber;

    public TextMeshProUGUI basicAttackName;
    public TextMeshProUGUI basicAttackDesc;
    public TextMeshProUGUI basicAttackDamage;
    public TextMeshProUGUI basicAttackRange;

    public TextMeshProUGUI abilityOneName;
    public TextMeshProUGUI abilityOneDesc;
    public TextMeshProUGUI abilityTwoName;
    public TextMeshProUGUI abilityTwoDesc;

    ///////////////////

    public void Refresh()
    {
        if (!loadoutCreated)
        {
            loadoutCreated = true;
            CreateLoadoutSlots();
        }
        PopulateVesselGrid();
        PopulateSoulGrid();
    }

    // Sets vessel to display details for TODO do for Souls
    public void SetDetailPane(GameObject vessel)
    {
        VesselData vesselData = vessel.GetComponent<VesselData>();
        
        nameText.text = vesselData.vesselName;
        vesselImage.sprite = vesselData.appearance;

        healthNumber.text = vesselData.baseHealth.ToString();
        manaNumber.text = vesselData.baseMana.ToString();
        abilityNumber.text = vesselData.baseAbility.ToString();
        speedNumber.text = vesselData.baseMovementSpeed.ToString();

        BasicAttackAction basicAttack = (BasicAttackAction) vesselData.basicAttack;
        basicAttackName.text = basicAttack.title + " (Basic Attack)";
        basicAttackDesc.text = basicAttack.description;
        basicAttackRange.text = basicAttack.range.ToString();
        basicAttackDamage.text = basicAttack.damage.ToString();

        Ability abilityOne = (Ability) vesselData.abilities[0];
        abilityOneName.text = abilityOne.abilityName;
        abilityOneDesc.text = abilityOne.abilityDescription;

        Ability abilityTwo = (Ability) vesselData.abilities[1];
        abilityTwoName.text = abilityTwo.abilityName;
        abilityTwoDesc.text = abilityTwo.abilityDescription;
    }

    public void CreateLoadoutSlots()
    {
        // Destroy existing
        foreach (Transform child in loadoutSlots.transform) {
            GameObject.Destroy(child.gameObject);
        }

        // Create per required slot
        for (int i = 0; i < requiredVessels; i++) {
            GameObject loadoutSlot = Instantiate(loadoutSlotPrefab, loadoutSlots.transform, true);
            loadoutSlot.transform.localScale = Vector3.one;
        }
    }

    // Create the icons in the grid based on the VesselManager
    public void PopulateVesselGrid()
    {
        // Destroy existing
        foreach (Transform child in vesselGrid.transform) {
            GameObject.Destroy(child.gameObject);
        }

        // Create per vessel
        foreach (VesselCatalogEntry entry in VesselManager.Instance.vesselCatalog) {
            if (entry.enabled)
            {
                GameObject vesselIcon = Instantiate(vesselIconPrefab, vesselGrid.transform, true);
                vesselIcon.GetComponent<VesselIcon>().SetVessel(entry.vessel);
                vesselIcon.transform.localScale = Vector3.one;
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
    /// Called on every change to selection icons
    /// </summary>
    public void LoadoutUpdated ()
    {
        ActivateButton goButtonActivate = goButton.GetComponent<ActivateButton>();
        if (goButtonActivate != null)
        {
            goButtonActivate.SetButtonActivation(IsLoadoutValid(false));
        }
    }
}
