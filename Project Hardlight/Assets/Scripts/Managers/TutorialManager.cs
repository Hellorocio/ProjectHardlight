using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.Events;

public enum PopupTrigger { MOVEMENT, POPUP_END, SELECT_HERO, ZOOM, PAN, SET_TARGET, GAIN_MANA, MAX_MANA, MONSTER_DEATH, BUTTON_PRESS, ABILITY_CAST }

// Keep track of state and things related to the tutorial, like events
// Tutorial dialogue is all handled in GameManager and BattleManager and Fighter, which uses TutorialManager to check status
public class TutorialManager : Singleton<TutorialManager>
{
    public List<TutorialPopupData> tutorialPopups;

    public bool tutorialEnabled = true;

    public bool inTutorialBattle = false;
    public bool usedAbility = false;

    public bool inMeetupBattle = false;


    public bool heroDeselectLocked = false;
    private int currentTutorialIndex = -1;

    public void Start()
    {
        // for testing
        InitTutorial();
    }

    public void InitTutorial ()
    {
        foreach (TutorialPopupData popup in tutorialPopups)
        {
            switch (popup.startPopupTrigger)
            {
                case PopupTrigger.GAIN_MANA:
                    // TODO
                    break;
                default:
                    break;
            }

        }
    }

    /// <summary>
    /// Starts popup dialogue, shows popup text, activates any objects indicated in activateObjs
    /// </summary>
    /// <param name="popupName"></param>
    public void ActivateTutorialPopup (string popupName)
    {
        CompleteTutorialStep();
        int popupIndex = GetPopupIndex(popupName);
        if (popupIndex != -1)
        {
            currentTutorialIndex = popupIndex;
            SetEndPopupTrigger();

            if (tutorialPopups[popupIndex].dialogue != null)
            {
                // play dialogue
                if (tutorialPopups[currentTutorialIndex].popupText != "")
                {
                    DialogueManager.Instance.onDialogueEnd.AddListener(TutorialPopupAfterDialogue);
                }
                DialogueManager.Instance.StartDialogue(tutorialPopups[popupIndex].dialogue);
            }
            else
            {
                TutorialPopupAfterDialogue();
            }
        }
    }

    /// <summary>
    /// Activates popup and activateObjects after dialogue plays
    /// </summary>
    public void TutorialPopupAfterDialogue ()
    {
        //print("TutorialPopupAfterDialogue: tutorial index = " + currentTutorialIndex);
        if (currentTutorialIndex != -1)
        {
            // show popup
            if (tutorialPopups[currentTutorialIndex].popupText != "")
            {
                GameManager.Instance.ShowTutorialPopup(tutorialPopups[currentTutorialIndex].popupText, tutorialPopups[currentTutorialIndex].pauseOnPopup);
            }

            // activate objects
            SetPopupActivateObjects(currentTutorialIndex, true);
        }
    }

    /// <summary>
    /// Uses endPopupTrigger to set what triggers CompleteTutorialStep in a popup
    /// </summary>
    private void SetEndPopupTrigger ()
    {
        if (currentTutorialIndex != -1)
        {
            switch (tutorialPopups[currentTutorialIndex].endPopupTrigger)
            {
                case PopupTrigger.SELECT_HERO:
                    BattleManager.Instance.onHeroSelected.AddListener(CompleteTutorialStep);
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// stop showing popup, deactivate objects in activateObjs, and play next popup if there is one
    /// </summary>
    public void CompleteTutorialStep ()
    {
        //print("CompleteTutorialStep: tutorial index = " + currentTutorialIndex);
        if (currentTutorialIndex != -1)
        {
            int index = currentTutorialIndex;

            GameManager.Instance.HideTutorialPopup();

            // deactivate objects
            SetPopupActivateObjects(currentTutorialIndex, false);

            currentTutorialIndex = -1;

            if (tutorialPopups[index].nextTutorialPopup != "")
            {
                ActivateTutorialPopup(tutorialPopups[index].nextTutorialPopup);
            }
        }
    }

    public void SetPopupActivateObjects (int popupIndex, bool activate)
    {
        if (tutorialPopups[popupIndex].activateObjectsParent != "")
        {
            GameObject activateObj = GameObject.Find(tutorialPopups[popupIndex].activateObjectsParent);
            if (activateObj != null)
            {
                foreach (SpriteRenderer spriteRenderer in activateObj.GetComponentsInChildren<SpriteRenderer>())
                {
                    spriteRenderer.enabled = activate;
                }
            }
        }
    }

    /// <summary>
    /// Finds index of popup with name popupName
    /// Returns -1 if popup isn't found
    /// </summary>
    /// <param name="popupName"></param>
    /// <returns></returns>
    private int GetPopupIndex (string popupName)
    {
        int popupIndex = -1;
        for (int i = 0; i < tutorialPopups.Count; i++)
        {
            if (tutorialPopups[i].name == popupName)
            {
                popupIndex = i;
                break;
            }
        }
        return popupIndex;
    }
     
}


[System.Serializable]
public struct TutorialPopupData
{
    public string name;
    public TextAsset dialogue;
    public string popupText;
    public bool pauseOnPopup;
    public string activateObjectsParent; // name of the objects that need their sprite renderers activated
    public PopupTrigger startPopupTrigger;
    public string startTriggerParam;
    public PopupTrigger endPopupTrigger;
    public string endTriggerParam;
    public string nextTutorialPopup;
}