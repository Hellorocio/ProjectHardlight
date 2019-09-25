using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FighterUseAbilityPopup : MonoBehaviour
{
    Fighter[] abilityReadyFighters = new Fighter[3];
    public Button[] popupButtons;
    BattleManager battleManager;

    int numPopupsActive = 0;

    /// <summary>
    /// Subscribe to BattleStart event
    /// </summary>
    private void OnEnable()
    {
        battleManager = GameObject.Find("BattleManager").GetComponent<BattleManager>();

        if (battleManager != null)
        {
            battleManager.OnLevelStart += SubscribeFighterEvents;
            battleManager.OnLevelEnd += DeactivateAll;
        }
    }

    /// <summary>
    /// Unsubscribe from all events to avoid memory leaks
    /// </summary>
    private void OnDisable()
    {
        if (battleManager != null)
        {
            battleManager.OnLevelStart -= SubscribeFighterEvents;
            battleManager.OnLevelEnd -= DeactivateAll;
        }
    }

    /// <summary>
    /// Turns off all the buttons
    /// </summary>
    /// <param name="win"></param>
    private void DeactivateAll(bool win)
    {
        for (int i = 0; i < popupButtons.Length; i++)
        {
            popupButtons[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// When the battle starts, subscribe to all fighter events so we can know when any fighter reaches max mana
    /// </summary>
    void SubscribeFighterEvents()
    {
        GameObject playerParent = GameObject.Find("Players");
        Fighter[] fighters = playerParent.GetComponentsInChildren<Fighter>();
        foreach (Fighter f in fighters)
        {
            if (f.gameObject.activeSelf)
            {
                f.OnMaxMana += ActivateFighterAbilityPopup;
                f.OnFighterDeath += UnsubscribeFighterEvents;
                f.OnLoseMana += DeactivateFighterAbilityPopup;
            }
        }
    }

    /// <summary>
    /// When a fighter dies, unsubscribe from their events to avoid memory leaks
    /// Also remove popup if they had one
    /// </summary>
    /// <param name="f"></param>
    void UnsubscribeFighterEvents(Fighter f)
    {
        int idx = GetFighterIndex(f);
        if (idx != -1)
        {
            DeactivateFighterAbilityPopup(f);
        }

        f.OnMaxMana -= ActivateFighterAbilityPopup;
        f.OnFighterDeath -= UnsubscribeFighterEvents;
        f.OnLoseMana -= DeactivateFighterAbilityPopup;
    }

    /// <summary>
    /// Activates button telling player that a fighter can use an ability when they reach max mana
    /// </summary>
    /// <param name="f"></param>
    void ActivateFighterAbilityPopup(Fighter f)
    {
        popupButtons[numPopupsActive].GetComponentInChildren<Text>().text = f.characterName + " can use an ability!";
        popupButtons[numPopupsActive].gameObject.SetActive(true);
        abilityReadyFighters[numPopupsActive] = f;
        numPopupsActive++;
    }

    /// <summary>
    /// Selects hero associated with that button
    /// </summary>
    /// <param name="buttonNum"></param>
    public void PressPopupButton (int buttonNum)
    {
        battleManager.SetSelectedHero(abilityReadyFighters[buttonNum]);
    }

    /// <summary>
    /// Deactivates the popup when the player uses an ability
    /// </summary>
    /// <param name="f"></param>
    void DeactivateFighterAbilityPopup (Fighter f)
    {
        int idx = GetFighterIndex(f);
        if (idx != -1)
        {
            popupButtons[idx].gameObject.SetActive(false);
            abilityReadyFighters[idx] = null;

            numPopupsActive--;
        }
    }

    /// <summary>
    /// Find index of given fighter in abilityReadyFigters 
    /// </summary>
    /// <param name="f"></param>
    /// <returns></returns>
    int GetFighterIndex (Fighter f)
    {
        int fighterIndex = -1;
        for (int i = 0; i < abilityReadyFighters.Length; i++)
        {
            if (abilityReadyFighters[i] != null && abilityReadyFighters[i] == f)
            {
                fighterIndex = i;
                break;
            }
        }
        return fighterIndex;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
