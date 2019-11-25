﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class PortraitHotKeyManager : MonoBehaviour
{
    public GameObject HeroPortraitPanel;
    //public GameObject hotKeyPanel;
    public GameObject multiSelectHotKeyPanel;

    //Hero1 components
    private GameObject hero1;
    private Image hero1ManaFullImage;
    private Image hero1Image;
    private TextMeshProUGUI hero1Name;
    private PortraitHealthBar hero1HealthBar;
    private PortraitManaBar hero1ManaBar;
    private GameObject hero1Selected;
    private Button ability1Button;
    private Button ability2Button;
    public TextMeshProUGUI ability1Desc;
    public TextMeshProUGUI ability2Desc;
    public GameObject hero1deathShadow;

    //Hero2 components
    private GameObject hero2;
    private Image hero2ManaFullImage;
    private Image hero2Image;
    private TextMeshProUGUI hero2Name;
    private PortraitHealthBar hero2HealthBar;
    private PortraitManaBar hero2ManaBar;
    private GameObject hero2Selected;
    private Button ability3Button;
    private Button ability4Button;
    public TextMeshProUGUI ability3Desc;
    public TextMeshProUGUI ability4Desc;
    public GameObject hero2deathShadow;

    //Hero3 components
    private GameObject hero3;
    private Image hero3ManaFullImage;
    private Image hero3Image;
    private TextMeshProUGUI hero3Name;
    private PortraitHealthBar hero3HealthBar;
    private PortraitManaBar hero3ManaBar;
    private GameObject hero3Selected;
    private Button ability5Button;
    private Button ability6Button;
    public TextMeshProUGUI ability5Desc;
    public TextMeshProUGUI ability6Desc;
    public GameObject hero3deathShadow;

    /*
    //Command1 components
    private GameObject command1;
    private Image command1BackgroundImage;
    private Image command1Image;
    private Text command1Name;

    //Command2 components
    private GameObject command2;
    private Image command2BackgroundImage;
    private Image command2Image;
    private Text command2Name;
    */


    // Start is called before the first frame update
    void Start()
    {
        //Getting all the hero components
        hero1 = HeroPortraitPanel.transform.Find("Hero1").gameObject;
        hero2 = HeroPortraitPanel.transform.Find("Hero2").gameObject;
        hero3 = HeroPortraitPanel.transform.Find("Hero3").gameObject;

        hero1ManaFullImage = hero1.transform.Find("ManaFullImage").gameObject.GetComponent<Image>();
        hero1Image = hero1.transform.Find("Portrait/PortraitCircle/HeroImage").gameObject.GetComponent<Image>();
        hero1Name = hero1.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>();
        hero1HealthBar = hero1.transform.Find("HealthBar").gameObject.GetComponent<PortraitHealthBar>();
        hero1ManaBar = hero1.transform.Find("ManaBar").gameObject.GetComponent<PortraitManaBar>();
        hero1Selected = hero1.transform.Find("Selected").gameObject;
        ability1Button = hero1.transform.Find("Abilities/AbilityOneOutline/Ability1Box").gameObject.GetComponent<Button>();
        ability2Button = hero1.transform.Find("Abilities/AbilityTwoOutline/Ability2Box").gameObject.GetComponent<Button>();
        hero1deathShadow = hero1.transform.Find("DeathShadow").gameObject;


        hero2ManaFullImage = hero2.transform.Find("ManaFullImage").gameObject.GetComponent<Image>();
        hero2Image = hero2.transform.Find("Portrait/PortraitCircle/HeroImage").gameObject.GetComponent<Image>();
        hero2Name = hero2.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>();
        hero2HealthBar = hero2.transform.Find("HealthBar").gameObject.GetComponent<PortraitHealthBar>();
        hero2ManaBar = hero2.transform.Find("ManaBar").gameObject.GetComponent<PortraitManaBar>();
        hero2Selected = hero2.transform.Find("Selected").gameObject;
        ability3Button = hero2.transform.Find("Abilities/AbilityOneOutline/Ability1Box").gameObject.GetComponent<Button>();
        ability4Button = hero2.transform.Find("Abilities/AbilityTwoOutline/Ability2Box").gameObject.GetComponent<Button>();
        hero2deathShadow = hero2.transform.Find("DeathShadow").gameObject;

        hero3ManaFullImage = hero3.transform.Find("ManaFullImage").gameObject.GetComponent<Image>();
        hero3Image = hero3.transform.Find("Portrait/PortraitCircle/HeroImage").gameObject.GetComponent<Image>();
        hero3Name = hero3.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>();
        hero3HealthBar = hero3.transform.Find("HealthBar").gameObject.GetComponent<PortraitHealthBar>();
        hero3ManaBar = hero3.transform.Find("ManaBar").gameObject.GetComponent<PortraitManaBar>();
        hero3Selected = hero3.transform.Find("Selected").gameObject;
        ability5Button = hero3.transform.Find("Abilities/AbilityOneOutline/Ability1Box").gameObject.GetComponent<Button>();
        ability6Button = hero3.transform.Find("Abilities/AbilityTwoOutline/Ability2Box").gameObject.GetComponent<Button>();
        hero3deathShadow = hero3.transform.Find("DeathShadow").gameObject;

        /*
        GameObject ordersPanel = hotKeyPanel.transform.Find("OrdersPanel").gameObject;
        command1 = ordersPanel.transform.Find("Command1").gameObject;
        command2 = ordersPanel.transform.Find("Command2").gameObject;

        
        command1BackgroundImage = command1.transform.Find("Background").gameObject.GetComponent<Image>();
        command1Image = command1.transform.Find("CommandImage").gameObject.GetComponent<Image>();
        command1Name = command1.transform.Find("Name").gameObject.GetComponent<Text>();

        command2BackgroundImage = command2.transform.Find("Background").gameObject.GetComponent<Image>();
        command2Image = command2.transform.Find("CommandImage").gameObject.GetComponent<Image>();
        command2Name = command2.transform.Find("Name").gameObject.GetComponent<Text>();

         */

        AllUISwitch(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gameState == GameState.FIGHTING || GameManager.Instance.gameState == GameState.PAUSED)
        {
            UpdateManaProcs();
            UpdateDeathState();
        }
    }

    /// <summary>
    /// Checks to see if the heroes have full mana and enables/disables the ManaFullImage GameObject
    /// </summary>
    void UpdateManaProcs()
    {
        bool showHero1Mana = !hero1HealthBar.isDead && hero1ManaBar.hasMaxMana;
        hero1ManaFullImage.gameObject.SetActive(showHero1Mana);
        ability1Button.interactable = showHero1Mana;
        ability2Button.interactable = showHero1Mana;

        bool showHero2Mana = !hero2HealthBar.isDead && hero2ManaBar.hasMaxMana;
        hero2ManaFullImage.gameObject.SetActive(showHero2Mana);
        ability3Button.interactable = showHero2Mana;
        ability4Button.interactable = showHero2Mana;

        bool showHero3Mana = !hero3HealthBar.isDead && hero3ManaBar.hasMaxMana;
        hero3ManaFullImage.gameObject.SetActive(showHero3Mana);
        ability5Button.interactable = showHero3Mana;
        ability6Button.interactable = showHero3Mana;
    }

    void UpdateDeathState()
    {
        if (hero1HealthBar.isDead)
        {
            hero1.transform.Find("Portrait/PortraitCircle").GetComponent<Button>().interactable = false;
            hero1deathShadow.SetActive(true);
        }
        if (hero2HealthBar.isDead)
        {
            hero2.transform.Find("Portrait/PortraitCircle").GetComponent<Button>().interactable = false;
            hero2deathShadow.SetActive(true);
        }
        if (hero3HealthBar.isDead)
        {
            hero3.transform.Find("Portrait/PortraitCircle").GetComponent<Button>().interactable = false;
            hero3deathShadow.SetActive(true);
        }
    }

    /// <summary>
    /// Turns off both the commands and portraits
    /// </summary>
    public void AllUISwitch(bool s)
    {
        HeroPortraitSwitch(s);
        HotKeyPanelSwitch(s);
    }

    /// <summary>
    /// Turns off the portraits
    /// </summary>
    void HeroPortraitSwitch(bool s)
    {
        HeroPortraitPanel.SetActive(s);
        
    }

    /// <summary>
    /// Turns off the hotkey ability and commands UI for when no hero is selected
    /// </summary>
    void HotKeyPanelSwitch(bool s)
    {
        //hotKeyPanel.SetActive(s);

        //turns off multiselect panel if regular panel is being activated
        if (s)
        {
            multiSelectHotKeyPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Turns on the in battle hero portraits and abilities
    /// Initializes the components using the partyList
    /// </summary>
    public void InitBattlerUI(List<GameObject> partyList)
    {
        Start();
        hero1.SetActive(false);
        hero2.SetActive(false);
        hero3.SetActive(false);

        if (partyList.Count > 0)
        {
            // init first hero
            hero1.SetActive(true);
            hero1Image.sprite = partyList[0].GetComponent<VesselData>().appearance;
            hero1Name.text = partyList[0].GetComponent<VesselData>().vesselName.ToUpper();
            hero1HealthBar.GetComponent<PortraitHealthBar>().InitHero(partyList[0].GetComponent<Fighter>());
            hero1ManaBar.GetComponent<PortraitManaBar>().InitHero(partyList[0].GetComponent<Fighter>());

            ability1Button.GetComponent<Image>().sprite = ((Ability)partyList[0].gameObject.GetComponent<VesselData>().abilities[0]).abilityIcon;
            ability2Button.GetComponent<Image>().sprite = ((Ability)partyList[0].gameObject.GetComponent<VesselData>().abilities[1]).abilityIcon;

            ability1Desc.text = ((Ability)partyList[0].gameObject.GetComponent<VesselData>().abilities[0]).abilityDescription;
            ability2Desc.text = ((Ability)partyList[0].gameObject.GetComponent<VesselData>().abilities[1]).abilityDescription;

            hero1.transform.Find("Portrait/PortraitCircle").GetComponent<Button>().interactable = true;
            hero1deathShadow.SetActive(false);
        }
        if (partyList.Count > 1)
        {
            // init second hero
            hero2.SetActive(true);
            hero2Image.sprite = partyList[1].GetComponent<VesselData>().appearance;
            hero2Name.text = partyList[1].GetComponent<VesselData>().vesselName.ToUpper();
            hero2HealthBar.GetComponent<PortraitHealthBar>().InitHero(partyList[1].GetComponent<Fighter>());
            hero2ManaBar.GetComponent<PortraitManaBar>().InitHero(partyList[1].GetComponent<Fighter>());

            ability3Button.GetComponent<Image>().sprite = ((Ability)partyList[1].gameObject.GetComponent<VesselData>().abilities[0]).abilityIcon;
            ability4Button.GetComponent<Image>().sprite = ((Ability)partyList[1].gameObject.GetComponent<VesselData>().abilities[1]).abilityIcon;

            ability3Desc.text = ((Ability)partyList[1].gameObject.GetComponent<VesselData>().abilities[0]).abilityDescription;
            ability4Desc.text = ((Ability)partyList[1].gameObject.GetComponent<VesselData>().abilities[1]).abilityDescription;

            hero2.transform.Find("Portrait/PortraitCircle").GetComponent<Button>().interactable = true;
            hero2deathShadow.SetActive(false);

        }
        if (partyList.Count > 2)
        {
            // init third hero
            hero3.SetActive(true);
            hero3Image.sprite = partyList[2].GetComponent<VesselData>().appearance;
            hero3Name.text = partyList[2].GetComponent<VesselData>().vesselName.ToUpper();
            hero3HealthBar.GetComponent<PortraitHealthBar>().InitHero(partyList[2].GetComponent<Fighter>());
            hero3ManaBar.GetComponent<PortraitManaBar>().InitHero(partyList[2].GetComponent<Fighter>());

            ability5Button.GetComponent<Image>().sprite = ((Ability)partyList[2].gameObject.GetComponent<VesselData>().abilities[0]).abilityIcon;
            ability6Button.GetComponent<Image>().sprite = ((Ability)partyList[2].gameObject.GetComponent<VesselData>().abilities[1]).abilityIcon;

            ability5Desc.text = ((Ability)partyList[2].gameObject.GetComponent<VesselData>().abilities[0]).abilityDescription;
            ability6Desc.text = ((Ability)partyList[2].gameObject.GetComponent<VesselData>().abilities[1]).abilityDescription;

            hero3.transform.Find("Portrait/PortraitCircle").GetComponent<Button>().interactable = true;
            hero3deathShadow.SetActive(false);
        }

        HeroPortraitSwitch(true);

    }

    public void LoadNewlySelectedHero(Fighter f)
    {

        //ability1Name.text = ((Ability)f.gameObject.GetComponent<VesselData>().abilities[0]).abilityName.Replace(' ', '\n');
        //ability2Name.text = ((Ability)f.gameObject.GetComponent<VesselData>().abilities[1]).abilityName.Replace(' ', '\n');

        // set selection boxes
        hero1Selected.SetActive(f == BattleManager.Instance.selectedVessels[0].GetComponent<Fighter>());
        hero2Selected.SetActive(BattleManager.Instance.selectedVessels.Count >= 2 && f == BattleManager.Instance.selectedVessels[1].GetComponent<Fighter>());
        hero3Selected.SetActive(BattleManager.Instance.selectedVessels.Count >= 3 && f == BattleManager.Instance.selectedVessels[2].GetComponent<Fighter>());

        HotKeyPanelSwitch(true);
    }

    /// <summary>
    /// Turns off regular hotkey panel and opens the muli hotkey panel
    /// </summary>
    public void LoadMultiSelectedHeros ()
    {
        HotKeyPanelSwitch(false);
        //multiSelectHotKeyPanel.SetActive(true);

        // set selection boxes
        hero1Selected.SetActive(BattleManager.Instance.multiSelectedHeros.Contains(BattleManager.Instance.selectedVessels[0].GetComponent<Fighter>()));
        hero2Selected.SetActive(BattleManager.Instance.selectedVessels.Count >= 2 && BattleManager.Instance.multiSelectedHeros.Contains(BattleManager.Instance.selectedVessels[1].GetComponent<Fighter>()));
        hero3Selected.SetActive(BattleManager.Instance.selectedVessels.Count >= 3 && BattleManager.Instance.multiSelectedHeros.Contains(BattleManager.Instance.selectedVessels[2].GetComponent<Fighter>()));
    }

    public void DeselectedHero()
    {
        HotKeyPanelSwitch(false);
        multiSelectHotKeyPanel.SetActive(false);

        hero1Selected.SetActive(false);
        hero2Selected.SetActive(false);
        hero3Selected.SetActive(false);
    }

    /// <summary>
    /// Used in tutorial to remove ability stuff and mana bar before it is explained
    /// </summary>
    /// <param name="setActive"></param>
    public void SetAbilityStuff(bool setActive)
    {
        ability1Button.transform.parent.gameObject.SetActive(setActive);
        ability2Button.transform.parent.gameObject.SetActive(setActive);
        hero1ManaBar.gameObject.SetActive(setActive);

        UpdateManaProcs();
    }
}
