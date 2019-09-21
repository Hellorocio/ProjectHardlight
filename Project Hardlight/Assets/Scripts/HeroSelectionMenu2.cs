using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class HeroSelectionMenu2 : MonoBehaviour
{
    public GameObject characterStats;
    public GameObject lineup;
    public GameObject characterButtons;
    public GameObject soulSelection;
    public GameObject soulButton;
    public Button startButton;
    private Sprite soulButtonDefaultSprite;
    public GameObject heroPlacement;

    public Sprite[] soulIcons;
    public Sprite[] heroSprites;
    public SoulStats[] soulStats;

    public Text[] statTextComponents;
    private string[] statTextDefaults;
    private string[] statTextPreSoul;
    private Fighter currentlyDisplayedHero;
    private GameObject currentlyDisplayedPrefab;
    private SoulStats currentlyDisplayedSoul;
    private Sprite currentlyDisplayedSoulIcon;
    private Sprite currentlyDisplayedHeroSprite;
    private Button lastClickedHeroButton;
    private List<GameObject> lineupInstantiatables = new List<GameObject>();
    public GameObject[] redXObjects = new GameObject[3];

    public Fighter[] heroList;
    public GameObject[] prefabs;
    private List<Sprite> lineupSoulIcons = new List<Sprite>();
    private List<Sprite> lineupHeroSprites = new List<Sprite>();


    // Start is called before the first frame update
    void Start()
    {
        //statTextComponents = gameObject.GetComponentsInChildren<Text>();
        statTextDefaults = new string[statTextComponents.Length];
        statTextPreSoul = new string[statTextDefaults.Length];
        soulButtonDefaultSprite = soulButton.GetComponent<Image>().sprite;
        for (int i = 0; i < statTextComponents.Length; i++)
        {
            statTextDefaults[i] = statTextComponents[i].text;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    

    /// <summary>
    /// Pass in a Fighter and this will update all the stat UI
    /// Defense boost and mana regen should be refactored and included in fighter stats
    /// </summary>
    /// <param name="hero"></param>
    public void InputCharacterStats(int i)
    {
        // Determines if the fighter is currently on lineup
        //if (inLineup)
        //{

        //}
        Fighter hero = heroList[i];
        resetStrings();
        currentlyDisplayedHero = hero;
        currentlyDisplayedPrefab = hero.gameObject;
        currentlyDisplayedHeroSprite = heroSprites[i];
        statTextComponents[0].text += hero.characterName;
        statTextComponents[1].text += hero.fighterStats.maxHealth.ToString();
        statTextComponents[2].text += hero.basicAttackStats.damage.ToString();
        statTextComponents[3].text += hero.basicAttackStats.attackSpeed.ToString();
        statTextComponents[4].text += hero.basicAttackStats.range.ToString();
        statTextComponents[5].text += hero.fighterStats.movementSpeed.ToString();
        if(i == 0 || i == 2 || i == 4)
        {
            statTextComponents[6].text += hero.fighterStats.name.Substring(0, (hero.fighterStats.name.Length - 5));
        }

        if(i == 1)
        {
            statTextComponents[6].text += "Ninja";
        }
        if (i == 3)
        {
            statTextComponents[6].text += "Dark Healer";
        }
        if (i == 5)
        {
            statTextComponents[6].text += "Alchemist";
        }
        statTextComponents[7].text += hero.fighterStats.maxMana.ToString();
        if(i != 0)
        {
            statTextComponents[8].text += hero.basicAttackStats.attackSpeed.ToString();
        }
        else
        {
            statTextComponents[8].text += "0";
        }
        
        if (hero.soul != null)
        {
            statTextComponents[9].text += hero.soul.defenseBoost.ToString();
        }
        else
        {
            statTextComponents[9].text += "0";
        }
        for (int c = 0; c < statTextComponents.Length; c++)
        {
            statTextPreSoul[c] = statTextComponents[c].text;
        }
    }

    /// <summary>
    /// This function will restore all stat strings to their default labels without any numbers
    /// </summary>
    public void resetStrings()
    {
        for (int i = 0; i < statTextComponents.Length; i++)
        {
            statTextComponents[i].text = statTextDefaults[i];
        }
        currentlyDisplayedHero = null;
        currentlyDisplayedPrefab = null;
        currentlyDisplayedSoul = null;
        currentlyDisplayedSoulIcon = null;
        currentlyDisplayedHeroSprite = null;
        soulButton.GetComponentInChildren<Text>().text = "Select a Soul";
        soulButton.GetComponentInChildren<Image>().sprite = soulButtonDefaultSprite;
    }

    /// <summary>
    /// This is called when the select a soul button is clicked
    /// </summary>
    public void OpenSoulSelection()
    {
        if (currentlyDisplayedHero != null)
        {
            soulSelection.SetActive(true);
            for (int c = 0; c < statTextComponents.Length; c++)
            {
                statTextComponents[c].text = statTextPreSoul[c];
            }

        }
    }

    /// <summary>
    /// This is called when a soul is chosen
    /// This function will load the soul image onto the stats page
    /// and update the fighter's stats
    /// </summary>
    /// <param name="i"></param>
    public void SelectedASoul(int i)
    {


        soulButton.GetComponentInChildren<Text>().text = "";
        SelectedASoulHelper(i - 1);
        currentlyDisplayedSoul = soulStats[i-1];
        currentlyDisplayedSoulIcon = soulIcons[i-1];
        soulSelection.SetActive(false);
        AddHeroToLineup();
        
    }

    private void SelectedASoulHelper(int i)
    {

        soulButton.GetComponent<Image>().sprite = soulIcons[i];
        if (soulStats[i].healthBoost > 0) {
            statTextComponents[1].text += " +" + soulStats[i].healthBoost;
        }
        if (soulStats[i].movementSpeedBoost > 0)
        {
            statTextComponents[5].text += " +" + soulStats[i].movementSpeedBoost;
        }
        if (soulStats[i].attackSpeedBoost > 0)
        {
            statTextComponents[3].text += " +" + soulStats[i].attackSpeedBoost;
        }
        if (soulStats[i].defenseBoost > 0)
        {
            statTextComponents[9].text += " +" + soulStats[i].defenseBoost;
        }
        if (soulStats[i].attackBoost > 0)
        {
            statTextComponents[2].text += " +" + soulStats[i].attackBoost;
        }
        if (soulStats[i].manaGenerationBoost > 0)
        {
            statTextComponents[8].text += " +" + soulStats[i].manaGenerationBoost;
        }
    }

    /// <summary>
    /// Called by the Add Selected Hero button
    /// This will add the hero icon and soul icon to the lineup
    /// </summary>
    public void AddHeroToLineup()
    {
        if(currentlyDisplayedSoulIcon != null && lineupInstantiatables.Count < 3)
        {
            currentlyDisplayedHero.soul = currentlyDisplayedSoul;
            currentlyDisplayedPrefab = currentlyDisplayedHero.gameObject;
            lineupInstantiatables.Add(currentlyDisplayedPrefab);
            lineupSoulIcons.Add(currentlyDisplayedSoulIcon);
            lineupHeroSprites.Add(currentlyDisplayedHeroSprite);
            redXObjects[lineupInstantiatables.Count - 1].SetActive(true);
            updateLineUp();
        }
        
    }

    private void updateLineUp()
    {
        int count = 0;
        if(lineupSoulIcons.Count == 3)
        {
            startButton.GetComponentInChildren<Text>().text = "Go!";
            startButton.GetComponent<Image>().color = Color.yellow;
        } else
        {
            startButton.GetComponentInChildren<Text>().text = lineupSoulIcons.Count + "/3 Heroes Selected";
            startButton.GetComponent<Image>().color = Color.white;
        }
        
        foreach(Button but in lineup.GetComponentsInChildren<Button>())
        {
            if (but.name == "Hero1" || but.name == "Hero2" || but.name == "Hero3")
            {
                foreach (Image imag in but.GetComponentsInChildren<Image>())
                {
                    if (imag.name == "Hero_Image")
                    {
                        if (lineupHeroSprites.Count > count)
                        {
                            imag.sprite = lineupHeroSprites[count];
                            imag.enabled = true;

                        }
                        else
                        {
                            imag.enabled = false;
                        }
                    }
                    else if (imag.name == "Soul_Image")
                    {
                        if (lineupSoulIcons.Count > count)
                        {
                            imag.sprite = lineupSoulIcons[count];
                            imag.enabled = true;
                        }
                        else
                        {
                            imag.enabled = false;
                        }
                    }
                    else
                    {

                    }
                }
                count++;
            }
        }
    }

    public void RemoveHeroAtIndex(int index)
    {
        if (lineupInstantiatables.Count > index)
        {
            lineupInstantiatables.RemoveAt(index);
            lineupSoulIcons.RemoveAt(index);
            lineupHeroSprites.RemoveAt(index);
            updateLineUp();
            redXObjects[lineupInstantiatables.Count].SetActive(false);
        }
    }

    public void StartHeroPlacement()
    {
        if(lineupSoulIcons.Count == 3)
        {
            heroPlacement.SetActive(true);
            heroPlacement.GetComponent<HeroPlacer>().StartHeroPlacement(lineupInstantiatables);
            gameObject.SetActive(false);
        }
    }
}
