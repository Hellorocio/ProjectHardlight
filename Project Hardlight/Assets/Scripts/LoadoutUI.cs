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

    public List<VesselIcon> loadoutVesselIcons;
    public List<SoulIcon> loadoutSoulIcons;

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

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
        loadoutVesselIcons.Clear();
        loadoutSoulIcons.Clear();
        foreach (Transform child in loadoutSlots.transform) {
            GameObject.Destroy(child.gameObject);
        }

        // Create per required slot
        for (int i = 0; i < requiredVessels; i++) {
            GameObject loadoutSlot = Instantiate(loadoutSlotPrefab, loadoutSlots.transform, true);
            loadoutVesselIcons.Add(loadoutSlot.GetComponentInChildren<VesselIcon>());
            loadoutSoulIcons.Add(loadoutSlot.GetComponentInChildren<SoulIcon>());
        }
    }

    public void PopulateVesselGrid()
    {
        // Destroy existing
        foreach (Transform child in vesselGrid.transform) {
            GameObject.Destroy(child.gameObject);
        }

        // Create per vessel
        foreach (GameObject vessel in VesselManager.Instance.vessels) {
            GameObject vesselIcon = Instantiate(vesselIconPrefab, vesselGrid.transform, true);
            vesselIcon.GetComponent<VesselIcon>().SetVessel(vessel);
        }
    }

    public void PopulateSoulGrid()
    {
        // Destroy existing
        foreach (Transform child in soulGrid.transform) {
            GameObject.Destroy(child.gameObject);
        }

        // Create per vessel
        foreach (Soul soul in GameManager.Instance.souls) {
            GameObject soulIcon = Instantiate(soulIconPrefab);
            soulIcon.GetComponent<SoulIcon>().SetSoul(soul);
            soulIcon.transform.SetParent(soulGrid.transform);
        }
    }

    public void ClickGoButton()
    {
        if (TrySettingLoadout())
        {
            GameManager.Instance.StartVesselPlacement();
        }
    }
    public bool TrySettingLoadout()
    {
        // Validate Vessels
        for (int i = 0; i < requiredVessels; i++)
        {
            GameObject slotVessel = loadoutVesselIcons[i].vessel;
            Soul slotSoul = loadoutSoulIcons[i].soul;
            if (slotVessel == null)
            {
                Debug.Log("Slot " + i + " missing Vessel");
                return false;
            }
            if (slotSoul == null)
            {
                Debug.Log("Slot " + i + " missing Soul");
                return false;
            }
        }
        
        // Nice! Create them. TODO(mchi) put these GOs somewhere sane
        for (int i = 0; i < requiredVessels; i++)
        {
            GameObject slotVessel = loadoutVesselIcons[i].vessel;
            Soul slotSoul = loadoutSoulIcons[i].soul;

            GameObject selectedVessel = Instantiate(slotVessel);
            selectedVessel.GetComponent<Fighter>().soul = slotSoul;
            BattleManager.Instance.selectedVessels.Add(selectedVessel);
        }

        return true;
    }
}
