using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CommandsUIHandler : MonoBehaviour
{
    public Image background;
    public Text heroNameText;
    public Button ability1Button;
    public Button ability2Button;
    public Text desc1;
    public Text desc2;
    public Button targetButton;
    private bool isUIShowing = false;
    private GameObject currentlySelectedHero;
    private bool selectingTarget = false;

    private ColorBlock disabledAbilityColors;
    private ColorBlock enabledAbilityColors;

    private void Start()
    {
        //initialize button colors
        disabledAbilityColors = ability1Button.colors;
        disabledAbilityColors.normalColor = new Color(200, 200, 200);
        disabledAbilityColors.highlightedColor = Color.white;
        disabledAbilityColors.pressedColor = Color.white;

        enabledAbilityColors = ability1Button.colors;
        enabledAbilityColors.normalColor = new Color(0, 255, 233);
        enabledAbilityColors.selectedColor = new Color(0, 255, 233);
        enabledAbilityColors.highlightedColor = new Color(0, 241, 220);
        enabledAbilityColors.pressedColor = new Color(0, 222, 203);
        SwitchButtonColor(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(currentlySelectedHero != null && !currentlySelectedHero.activeSelf)
        {
            DisableUI();
        }
    }

    public void DisableUI()
    {
        currentlySelectedHero = null;
        UISwitch(false);
        
    }

    public void EnableUI(GameObject f)
    {
        if (!selectingTarget)
        {

            UISwitch(true);
            currentlySelectedHero = f;
            heroNameText.text = f.GetComponent<Fighter>().characterName;
            HeroAbilities tmpAbilities = f.GetComponent<HeroAbilities>();
            ability1Button.GetComponentInChildren<Text>().text = ((Ability)tmpAbilities.abilityList[0]).abilityName;
            desc1.text = ((Ability)tmpAbilities.abilityList[0]).abilityDescription;

            if (tmpAbilities.abilityList.Count > 1)
            {
                ability2Button.GetComponentInChildren<Text>().text = ((Ability)tmpAbilities.abilityList[1]).abilityName;
                desc2.text = ((Ability)tmpAbilities.abilityList[1]).abilityDescription;
            }
        }

    }

    private void UISwitch(bool b)
    {
        isUIShowing = b;
        background.gameObject.SetActive(b);
        heroNameText.gameObject.SetActive(b);
        ability1Button.gameObject.SetActive(b);
        ability2Button.gameObject.SetActive(b);
        targetButton.gameObject.SetActive(b);

        //if switching off, make sure we also switch off descriptions
        if (!b)
        {
            desc1.transform.parent.gameObject.SetActive(false);
            desc2.transform.parent.gameObject.SetActive(false);
        }
    }

    public void SwitchButtonColor (bool activate)
    {
        if (activate)
        {
            ability1Button.colors = enabledAbilityColors;
            ability2Button.colors = enabledAbilityColors;
        }
        else
        {
            ability1Button.colors = disabledAbilityColors;
            ability2Button.colors = disabledAbilityColors;
        }
    }
}
