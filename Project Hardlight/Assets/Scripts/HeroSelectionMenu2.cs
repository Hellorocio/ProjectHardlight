using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class HeroSelectionMenu2 : MonoBehaviour
{
    public GameObject characterStats;
    public GameObject lineup;
    public GameObject characterButtons;

    private Text[] statTextComponents;
    private string[] statTextDefaults;
    private Button lastClickedHeroButton;

    // Start is called before the first frame update
    void Start()
    {
        statTextComponents = gameObject.GetComponentsInChildren<Text>();
        statTextDefaults = new string[statTextComponents.Length];
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
    public void InputCharacterStats(Fighter hero, Button sender)
    {
        // Determines if the fighter is currently on lineup
        if (sender.transform.parent == lineup)
        {

        }
        resetStrings();
        statTextComponents[0].text = hero.characterName;
        statTextComponents[1].text = hero.fighterStats.maxHealth.ToString();
        statTextComponents[2].text = hero.basicAttackStats.damage.ToString();
        statTextComponents[3].text = hero.basicAttackStats.attackSpeed.ToString();
        statTextComponents[5].text = hero.basicAttackStats.range.ToString();
        statTextComponents[6].text = hero.fighterStats.movementSpeed.ToString();
        statTextComponents[7].text = hero.fighterStats.name.Substring(0, (hero.fighterStats.name.Length - 5));
        statTextComponents[8].text = hero.fighterStats.maxMana.ToString();
        //statTextComponents[9].text = hero.fighterStats.;
        if (hero.soul != null)
        {
            statTextComponents[10].text = hero.soul.defenseBoost.ToString();
        }
        else
        {
            statTextComponents[10].text = "0";
        }
        // statTextComponents[11] is the Soul Button's Text component
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
    }
}
