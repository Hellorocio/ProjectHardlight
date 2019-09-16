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
    public Button targetButton;
    private bool isUIShowing = false;
    private GameObject currentlySelectedHero;
    private bool selectingTarget = false;

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
            if (tmpAbilities.abilityList.Count > 1)
            {
                ability2Button.GetComponentInChildren<Text>().text = ((Ability)tmpAbilities.abilityList[1]).abilityName;
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
    }
}
