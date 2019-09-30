using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class LoadoutUI : Singleton<LoadoutUI>
{

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

    ///////////////////

    public void Refresh()
    {
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
    }

    public void PopulateVesselGrid()
    {
        // Destroy existing
        foreach (Transform child in vesselGrid.transform) {
            GameObject.Destroy(child.gameObject);
        }

        // Create per vessel
        foreach (GameObject vessel in VesselManager.Instance.vessels) {
            GameObject vesselIcon = Instantiate(vesselIconPrefab);
            vesselIcon.GetComponent<VesselIcon>().SetVessel(vessel);
            vesselIcon.transform.SetParent(vesselGrid.transform);
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
}
