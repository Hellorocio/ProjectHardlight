using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PortraitHotKeyManager : MonoBehaviour
{
    public GameObject HeroPortraitPanel;
    public GameObject hotKeyPanel;


    //Hero1 components
    private GameObject hero1;
    private Image hero1ManaFullImage;
    private Image hero1BackgroundImage;
    private Image hero1Image;
    private PortraitHealthBar hero1HealthBar;
    private PortraitManaBar hero1ManaBar;

    //Hero2 components
    private GameObject hero2;
    private Image hero2ManaFullImage;
    private Image hero2BackgroundImage;
    private Image hero2Image;
    private PortraitHealthBar hero2HealthBar;
    private PortraitManaBar hero2ManaBar;

    //Hero3 components
    private GameObject hero3;
    private Image hero3ManaFullImage;
    private Image hero3BackgroundImage;
    private Image hero3Image;
    private PortraitHealthBar hero3HealthBar;
    private PortraitManaBar hero3ManaBar;

    //Ability1 components
    private GameObject ability1;
    private Image ability1BackgroundImage;
    private Image ability1Image;
    private Text ability1Name;

    //Ability2 components
    private GameObject ability2;
    private Image ability2BackgroundImage;
    private Image ability2Image;
    private Text ability2Name;

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


    // Start is called before the first frame update
    void Start()
    {
        //Getting all the hero components
        hero1 = HeroPortraitPanel.transform.Find("Hero1").gameObject;
        hero2 = HeroPortraitPanel.transform.Find("Hero2").gameObject;
        hero3 = HeroPortraitPanel.transform.Find("Hero3").gameObject;

        hero1ManaFullImage = hero1.transform.Find("ManaFullImage").gameObject.GetComponent<Image>();
        hero1BackgroundImage = hero1.transform.Find("BackgroundImage").gameObject.GetComponent<Image>();
        hero1Image = hero1.transform.Find("HeroImage").gameObject.GetComponent<Image>();
        hero1HealthBar = hero1.transform.Find("HealthBar").gameObject.GetComponent<PortraitHealthBar>(); ;
        hero1ManaBar = hero1.transform.Find("ManaBar").gameObject.GetComponent<PortraitManaBar>();

        hero2ManaFullImage = hero2.transform.Find("ManaFullImage").gameObject.GetComponent<Image>();
        hero2BackgroundImage = hero2.transform.Find("BackgroundImage").gameObject.GetComponent<Image>();
        hero2Image = hero2.transform.Find("HeroImage").gameObject.GetComponent<Image>();
        hero2HealthBar = hero2.transform.Find("HealthBar").gameObject.GetComponent<PortraitHealthBar>();
         hero2ManaBar = hero2.transform.Find("ManaBar").gameObject.GetComponent<PortraitManaBar>();

        hero3ManaFullImage = hero3.transform.Find("ManaFullImage").gameObject.GetComponent<Image>();
        hero3BackgroundImage = hero3.transform.Find("BackgroundImage").gameObject.GetComponent<Image>();
        hero3Image = hero3.transform.Find("HeroImage").gameObject.GetComponent<Image>();
        hero3HealthBar = hero3.transform.Find("HealthBar").gameObject.GetComponent<PortraitHealthBar>(); ;
        hero3ManaBar = hero3.transform.Find("ManaBar").gameObject.GetComponent<PortraitManaBar>();

        //Getting all the ability/order components
        GameObject abilitiesPanel = hotKeyPanel.transform.Find("AbilitiesPanel").gameObject;
        GameObject ordersPanel = hotKeyPanel.transform.Find("OrdersPanel").gameObject;

        ability1 = abilitiesPanel.transform.Find("Ability1").gameObject;
        ability2 = abilitiesPanel.transform.Find("Ability2").gameObject;

        command1 = ordersPanel.transform.Find("Command1").gameObject;
        command2 = ordersPanel.transform.Find("Command2").gameObject;

        ability1BackgroundImage = ability1.transform.Find("Background").gameObject.GetComponent<Image>();
        ability1Image = ability1.transform.Find("AbilityImage").gameObject.GetComponent<Image>();
        ability1Name = ability1.transform.Find("Name").gameObject.GetComponent<Text>();

        ability2BackgroundImage = ability2.transform.Find("Background").gameObject.GetComponent<Image>();
        ability2Image = ability2.transform.Find("AbilityImage").gameObject.GetComponent<Image>();
        ability2Name = ability2.transform.Find("Name").gameObject.GetComponent<Text>();

        command1BackgroundImage = command1.transform.Find("Background").gameObject.GetComponent<Image>();
        command1Image = command1.transform.Find("CommandImage").gameObject.GetComponent<Image>();
        command1Name = command1.transform.Find("Name").gameObject.GetComponent<Text>();

        command2BackgroundImage = command2.transform.Find("Background").gameObject.GetComponent<Image>();
        command2Image = command2.transform.Find("CommandImage").gameObject.GetComponent<Image>();
        command2Name = command2.transform.Find("Name").gameObject.GetComponent<Text>();

        AllUISwitch(false);

    }

    // Update is called once per frame
    void Update()
    {
        UpdateManaProcs();
        UpdateDeathState();
    }

    /// <summary>
    /// Checks to see if the heroes have full mana and enables/disables the ManaFullImage GameObject
    /// </summary>
    void UpdateManaProcs()
    {
        hero1ManaFullImage.gameObject.SetActive(hero1ManaBar.hasMaxMana);

        hero2ManaFullImage.gameObject.SetActive(hero2ManaBar.hasMaxMana);

        hero3ManaFullImage.gameObject.SetActive(hero3ManaBar.hasMaxMana);
    }

    void UpdateDeathState()
    {
        if (hero1HealthBar.isDead)
        {
            hero1.transform.Find("Button").gameObject.SetActive(false);
        }
        if (hero2HealthBar.isDead)
        {
            hero2.transform.Find("Button").gameObject.SetActive(false);
        }
        if (hero3HealthBar.isDead)
        {
            hero3.transform.Find("Button").gameObject.SetActive(false);
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
        hotKeyPanel.SetActive(s);
    }

    /// <summary>
    /// Turns on the in battle hero portraits and abilities
    /// Initializes the components using the partyList
    /// </summary>
    public void InitBattlerUI(List<GameObject> partyList)
    {
        if (partyList.Count > 2)
        {
            hero2.SetActive(true);
            hero3.SetActive(true);
            Start();
            hero1Image.sprite = partyList[0].GetComponent<VesselData>().appearance;
            hero2Image.sprite = partyList[1].GetComponent<VesselData>().appearance;
            hero3Image.sprite = partyList[2].GetComponent<VesselData>().appearance;

            hero1HealthBar.GetComponent<PortraitHealthBar>().InitHero(partyList[0].GetComponent<Fighter>());
            hero1ManaBar.GetComponent<PortraitManaBar>().InitHero(partyList[0].GetComponent<Fighter>());

            hero2HealthBar.GetComponent<PortraitHealthBar>().InitHero(partyList[1].GetComponent<Fighter>());
            hero2ManaBar.GetComponent<PortraitManaBar>().InitHero(partyList[1].GetComponent<Fighter>());

            hero3HealthBar.GetComponent<PortraitHealthBar>().InitHero(partyList[2].GetComponent<Fighter>());
            hero3ManaBar.GetComponent<PortraitManaBar>().InitHero(partyList[2].GetComponent<Fighter>());
            
            HeroPortraitSwitch(true);
        } else if(partyList.Count == 1)
        {
            Start();
            hero1Image.sprite = partyList[0].GetComponent<VesselData>().appearance;
            hero1HealthBar.GetComponent<PortraitHealthBar>().InitHero(partyList[0].GetComponent<Fighter>());
            hero1ManaBar.GetComponent<PortraitManaBar>().InitHero(partyList[0].GetComponent<Fighter>());
            hero2.SetActive(false);
            hero3.SetActive(false);
            HeroPortraitSwitch(true);
        }

    }

    public void LoadNewlySelectedHero(Fighter f)
    {

        ability1Name.text = ((Ability)f.gameObject.GetComponent<HeroAbilities>().abilityList[0]).abilityName.Replace(' ', '\n');
        ability2Name.text = ((Ability)f.gameObject.GetComponent<HeroAbilities>().abilityList[1]).abilityName.Replace(' ', '\n');
        HotKeyPanelSwitch(true);
    }

    public void DeselectedHero()
    {
        HotKeyPanelSwitch(false);
    }



}
