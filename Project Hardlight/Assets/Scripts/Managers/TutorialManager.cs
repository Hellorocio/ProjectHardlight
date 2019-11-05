using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.Events;

public enum PopupTrigger { MOVEMENT, POPUP_END, SELECT_HERO, ZOOM, PAN, SET_TARGET, GAIN_MANA, MAX_MANA, MONSTER_DEATH, BUTTON_PRESS, ABILITY_CAST, VISIBILITY, OK_BUTTON }

// Keep track of state and things related to the tutorial, like events
// Tutorial dialogue is all handled in GameManager and BattleManager and Fighter, which uses TutorialManager to check status
public class TutorialManager : Singleton<TutorialManager>
{
    public List<TutorialPopupData> tutorialPopups;

    public bool tutorialEnabled = true;

    public bool inTutorialBattle = false;
    public bool usedAbility = false;

    public bool inMeetupBattle = false;


    public bool firstTutorialBattle;

    public bool heroDeselectLocked = false;
    private int currentTutorialIndex = -1;

    private bool saveCamEnabled ;

    public void Start()
    {
        // for testing
        InitTutorial();
    }

    public void InitTutorial ()
    {
        // set up start events
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

        // move marrha
        GameObject moveLoc = GameObject.Find("StartMoveLoc");
        GameObject merrha = GameObject.Find("Merrha");
        if (moveLoc != null && merrha != null)
        {
            merrha.GetComponent<FighterMove>().StartMovingCommandHandle(moveLoc.transform);
        }
    }

    /// <summary>
    /// Starts popup dialogue, shows popup text, activates any objects indicated in activateObjs
    /// </summary>
    /// <param name="popupName"></param>
    public void ActivateTutorialPopup (string popupName)
    {
        //print("activate tutorial " + popupName);
        int popupIndex = GetPopupIndex(popupName);
        if (popupIndex != -1)
        {
            if (currentTutorialIndex == popupIndex)
            {
                // prevent duplicate popups
                return;
            }
            else
            if (currentTutorialIndex != -1)
            {
                // prevent overlapping popups
                CompleteTutorialStep();
            }

            currentTutorialIndex = popupIndex;
            SetEndPopupTrigger();

            if (tutorialPopups[popupIndex].dialogue != null)
            {
                // play dialogue
                if (tutorialPopups[currentTutorialIndex].popupText != "")
                {
                    //print("Tutorial popup with dialogue");
                    DialogueManager.Instance.onDialogueEnd.AddListener(TutorialPopupAfterDialogue);
                }
                GameManager.Instance.gameState = GameState.PAUSED;
                saveCamEnabled = BattleManager.Instance.camController.enabled;
                BattleManager.Instance.camController.enabled = false;
                DialogueManager.Instance.StartDialogue(tutorialPopups[popupIndex].dialogue);
            }
            else
            {
                //print("Tutorial popup with no dialogue");
                TutorialPopupAfterDialogue();
            }
        }
    }

    /// <summary>
    /// Activates popup and activateObjects after dialogue plays
    /// </summary>
    public void TutorialPopupAfterDialogue ()
    {
        DialogueManager.Instance.onDialogueEnd.RemoveAllListeners();
        GameManager.Instance.gameState = GameState.FIGHTING;
        BattleManager.Instance.camController.enabled = saveCamEnabled;

        //print("TutorialPopupAfterDialogue: tutorial index = " + currentTutorialIndex);
        if (currentTutorialIndex != -1)
        {
            //print("TutorialPopupAfterDialogue: " + tutorialPopups[currentTutorialIndex].name);
            // show popup
            if (tutorialPopups[currentTutorialIndex].popupText != "")
            {
                TutorialPopupData data = tutorialPopups[currentTutorialIndex];
                GameManager.Instance.ShowTutorialPopup(data.popupText, data.pauseOnPopup, data.disableMovementOnPopup, data.endPopupTrigger == PopupTrigger.OK_BUTTON);
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
                    heroDeselectLocked = false;
                    BattleManager.Instance.DeselectHero();
                    heroDeselectLocked = true;
                    BattleManager.Instance.onHeroSelected.AddListener(CompleteTutorialStep);
                    break;
                case PopupTrigger.ZOOM:
                    GameManager.Instance.SetCameraControls(true);
                    BattleManager.Instance.camController.onCameraZoom.AddListener(CompleteTutorialStep);
                    break;
                case PopupTrigger.PAN:
                    BattleManager.Instance.camController.onCameraPan.AddListener(CompleteTutorialStep);
                    break;
                case PopupTrigger.SET_TARGET:
                    BattleManager.Instance.onSetTarget.AddListener(CompleteTutorialStep);
                    break;
                case PopupTrigger.MONSTER_DEATH:
                    BattleManager.Instance.onMonsterDeath.AddListener(CompleteTutorialStep);
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

            // start battle
            if (tutorialPopups[currentTutorialIndex].startBattle)
            {
                BattleManager.Instance.StartBattle();
                //firstTutorialBattle = true;
            }

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
                    //print("Setting " + spriteRenderer.gameObject + ", child of " + spriteRenderer.transform.parent.gameObject.name + " to active = " + activate);
                    spriteRenderer.enabled = activate;
                }

                foreach (Image image in activateObj.GetComponentsInChildren<Image>())
                {
                    image.enabled = activate;
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

    public void OkayButtonPressed ()
    {
        CompleteTutorialStep();
    }
     
}


[System.Serializable]
public struct TutorialPopupData
{
    public string name;
    public TextAsset dialogue;
    [TextArea(2, 5)]
    public string popupText;
    public bool pauseOnPopup;
    public bool disableMovementOnPopup;
    public bool startBattle;
    public string activateObjectsParent; // name of the objects that need their sprite renderers activated
    public PopupTrigger startPopupTrigger;
    public string startTriggerParam;
    public PopupTrigger endPopupTrigger;
    public string endTriggerParam;
    public string nextTutorialPopup;
}