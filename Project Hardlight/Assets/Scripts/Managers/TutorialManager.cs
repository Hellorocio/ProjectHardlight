using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.Events;

public enum PopupTrigger { MOVEMENT, POPUP_END, SELECT_HERO, ZOOM, PAN, SET_TARGET, GAIN_MANA, MAX_MANA, MONSTER_DEATH, BUTTON_PRESS, ABILITY_CAST, VISIBILITY, OK_BUTTON, USE_ABILITY, SELECT_ALL, BATTLE_OVER }
public enum TutorialControl { NONE, LEFTCLICK, RIGHTCLICK, MIDDLEMOUSE }

// Keep track of state and things related to the tutorial, like events
// Tutorial dialogue is all handled in GameManager and BattleManager and Fighter, which uses TutorialManager to check status
public class TutorialManager : Singleton<TutorialManager>
{
    public List<TutorialLevelInfo> tutorialLevels;
    public List<TutorialPopupData> tutorialPopups;
    public int currentTutorialLevel;
    public bool testing = false;
    public bool startGeneratingMana = true;

    public bool tutorialEnabled = true;
    public bool inTutorialBattle = false;
    public bool usedAbility = false;
    public bool inMeetupBattle = false;

    public bool firstTutorialBattle;

    public bool heroDeselectLocked = false;
    private int currentTutorialIndex = -1;

    private bool saveCamEnabled;

    private GameObject fighterUI;
    private GameObject manaBar;

    private int enemiesDefeated = 0;
    private int testNumEnemies = 0;

    public void Start()
    {
        // for testing
        if (testing)
        {
            print("init tutorial for testing reasons");
            InitTutorial();
        }
    }

    public void InitTutorial()
    {
        tutorialPopups = tutorialLevels[currentTutorialLevel].tutorialSteps;
        currentTutorialIndex = -1;

        // move marrha
        GameObject moveLoc = GameObject.Find("StartMoveLoc");
        GameObject merrha = GameObject.Find("Merrha");
        fighterUI = GameObject.Find("FighterUI");
        if (currentTutorialLevel == 0)
        {
            fighterUI.SetActive(false);
            manaBar = GameObject.Find("ManaBar");
            manaBar.SetActive(false);
            startGeneratingMana = false;
            BattleManager.Instance.portraitHotKeyManager.SetAbilityStuff(false);
        }

        if (moveLoc != null && merrha != null)
        {
            BattleManager.Instance.selectedVessels = new List<GameObject>();
            BattleManager.Instance.selectedVessels.Add(merrha);

            merrha.GetComponent<FighterMove>().StartMovingCommandHandle(moveLoc.transform);
        }

        if (currentTutorialLevel == 1)
        {
            GameObject taurin = GameObject.Find("Taurin");
            BattleManager.Instance.selectedVessels.Add(taurin);
            GameManager.Instance.SetCameraControls(true);
        }

        BattleManager.Instance.inputState = BattleManager.InputState.NothingSelected;
        UIManager.Instance.battleUI.SetActive(true);
    }

    public void FinishTutorialLevel()
    {
        currentTutorialLevel++;
        
        if (currentTutorialLevel < tutorialLevels.Count)
        {
            GameManager.Instance.LoadScene(tutorialLevels[currentTutorialLevel].tutorialScene);
        }
        else
        {
            // tutorial over, enter map
            GameManager.Instance.EnterMap();
            tutorialEnabled = false;
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
        if (popupIndex != -1 && tutorialEnabled)
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
                    if (tutorialPopups[currentTutorialIndex].endTriggerParam == 0)
                    {
                        BattleManager.Instance.onMonsterDeath.AddListener(CompleteTutorialStep);
                    }
                    else
                    {
                        enemiesDefeated = 0;
                        print("monster death check start");
                        testNumEnemies = tutorialPopups[currentTutorialIndex].endTriggerParam;
                        BattleManager.Instance.onMonsterDeath.AddListener(UpdateMonsterCount);
                    }
                    
                    break;
                case PopupTrigger.USE_ABILITY:
                    BattleManager.Instance.onUseAbility.AddListener(CompleteTutorialStep);
                    break;
                case PopupTrigger.ABILITY_CAST:
                    BattleManager.Instance.onAbilityCast.AddListener(CompleteTutorialStep);
                    break;
                case PopupTrigger.SELECT_ALL:
                    BattleManager.Instance.onAllHerosSelected.AddListener(CompleteTutorialStep);
                    break;
                case PopupTrigger.MOVEMENT:
                    BattleManager.Instance.checkAllowMovement = true;
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
        BattleManager.Instance.onMonsterDeath.RemoveAllListeners();
        BattleManager.Instance.checkAllowMovement = false;
        //print("CompleteTutorialStep: tutorial index = " + currentTutorialIndex);
        if (currentTutorialIndex != -1)
        {
            int index = currentTutorialIndex;

            GameManager.Instance.HideTutorialPopup();
            
            // deactivate objects
            if (!tutorialPopups[currentTutorialIndex].keepObjectsActivated)
            {
                SetPopupActivateObjects(currentTutorialIndex, false);
            }

            // start battle
            if (tutorialPopups[currentTutorialIndex].startBattle)
            {
                GameManager.Instance.StartFighting();
                fighterUI.SetActive(true);
            }

            if (tutorialPopups[currentTutorialIndex].name == "FoundSecondMonster")
            {
                BattleManager.Instance.portraitHotKeyManager.SetAbilityStuff(true);
            }

            if (tutorialPopups[currentTutorialIndex].name == "LearnAbility2")
            {
                GameObject merrha = GameObject.Find("Merrha");
                startGeneratingMana = true;
                BattleManager.Instance.portraitHotKeyManager.SetAbilityStuff(true);
                merrha.GetComponent<Fighter>().SetMaxMana();
            }

            if (tutorialPopups[currentTutorialIndex].name == "TaurinAbility2")
            {
                GameObject taurin = GameObject.Find("Taurin");
                taurin.GetComponent<Fighter>().SetMaxMana();
            }

            if (tutorialPopups[currentTutorialIndex].name == "NeroAbility2")
            {
                GameObject nero = GameObject.Find("Nero");
                nero.GetComponent<Fighter>().SetMaxMana();
            }

            if (tutorialPopups[currentTutorialIndex].name == "SeesNero")
            {
                GameObject nero = GameObject.Find("Nero");
                BattleManager.Instance.selectedVessels.Add(nero);
                BattleManager.Instance.portraitHotKeyManager.InitBattlerUI(BattleManager.Instance.selectedVessels);
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
                for (int i = 0; i < activateObj.transform.childCount; i++)
                {
                    activateObj.transform.GetChild(i).gameObject.SetActive(activate);
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

    public string GetCurrentTutorialPopupName ()
    {
        string levelName = "";
        if (currentTutorialIndex != -1)
        {
            levelName = tutorialPopups[currentTutorialIndex].name;
        }
        return levelName;
    }

    public void UpdateMonsterCount ()
    {
        enemiesDefeated++;
        print("UpdateMonsterCount: enemiesDefeated = " + enemiesDefeated);
        if (enemiesDefeated >= testNumEnemies)
        {
            CompleteTutorialStep();
        }
    }     

    /// <summary>
    /// Called when skip tutorial button is pressed
    /// Removes listener on the current popup if there is one
    /// </summary>
    public void CancelTutorial ()
    {
        if (currentTutorialIndex != -1)
        {
            switch (tutorialPopups[currentTutorialIndex].endPopupTrigger)
            {
                case PopupTrigger.SELECT_HERO:
                    BattleManager.Instance.onHeroSelected.RemoveAllListeners();
                    break;
                case PopupTrigger.ZOOM:
                    BattleManager.Instance.camController.onCameraZoom.RemoveAllListeners();
                    break;
                case PopupTrigger.PAN:
                    BattleManager.Instance.camController.onCameraPan.RemoveAllListeners();
                    break;
                case PopupTrigger.SET_TARGET:
                    BattleManager.Instance.onSetTarget.RemoveAllListeners();
                    break;
                case PopupTrigger.MONSTER_DEATH:
                    BattleManager.Instance.onMonsterDeath.RemoveAllListeners();
                    break;
                case PopupTrigger.USE_ABILITY:
                    BattleManager.Instance.onUseAbility.RemoveAllListeners();
                    break;
                case PopupTrigger.ABILITY_CAST:
                    BattleManager.Instance.onAbilityCast.RemoveAllListeners();
                    break;
                case PopupTrigger.SELECT_ALL:
                    BattleManager.Instance.onAllHerosSelected.RemoveAllListeners();
                    break;
                default:
                    break;
            }
        }
    }
}


[System.Serializable]
public struct TutorialPopupData
{
    public string name;
    public TextAsset dialogue;
    [TextArea(2, 5)]
    public string popupText;
    public TutorialControl control;
    public bool pauseOnPopup;
    public bool disableMovementOnPopup;
    public bool startBattle;
    public bool keepObjectsActivated;
    public string activateObjectsParent; // name of the objects that need their sprite renderers activated
    public PopupTrigger endPopupTrigger;
    public int endTriggerParam;
    public string nextTutorialPopup;
}