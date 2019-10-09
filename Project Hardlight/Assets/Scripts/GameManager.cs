using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public enum GameState { UNKNOWN, START, CUTSCENE, MAP, PREBATTLE, FIGHTING};

public class GameManager : Singleton<GameManager>
{

    public GameState gameState;

    public List<Soul> souls;

    public string mapSceneName;
    public string cutsceneSceneName;
    public CutsceneLevel currentCutscene;
    
    public GameObject battleManager;

    public TextAsset levelStartDialogue;
    // You can change this in runtime
    public TextAsset fightingEndDialogue;
    // Pls don't change this in runtime
    public TextAsset defaultFightingEndDialogue;
    
    // Tutorial stuff
    public string tutorialBattleSceneName;
    public TextAsset loadoutTutorialDialogue;
    public TextAsset tutorialMeetupPrebattleDialogue;

    // Used to load dialogue after something for example
    string sceneToLoad = "";

    // Map state
    [HideInInspector]
    public MapNode.NodeStatus[] levelStatuses;
    public int currentLevel;

    public DialogueBoxController topDialogue;

    public void Start()
    {
        // Destroy self if already exists
        if (GameManager.Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void ClearUI()
    {
        // Turn things off
        UIManager.Instance.battleUI.SetActive(false);
    }

    public void InitializeGame()
    {
        ClearUI();
        gameState = GameState.START;
    }

    // Here in case we want to do Continue Campaign in the future
    public void NewCampaign()
    {
        InitializeGame();
        StartCampaign();
    }
    
    // Initialized everything needed in a new game
    public void StartCampaign()
    {
        Debug.Log("GameManager | Starting Campaign");
        GrantRandomSouls(3);
        StartCutscene("TaurinIntroCutscene");
    }

    public void StartCutscene(string name)
    {
        if (currentCutscene != null)
        {
            Debug.Log("WARNING: Trying to StartCutscene() when currentCutscene is not empty, is a cutscene already playing?");
        }
        // TODO Find the cutscene in CutsceneList
        Transform cutsceneList = transform.Find("CutsceneList");
        foreach (CutsceneLevel cutsceneLevel in cutsceneList.GetComponentsInChildren(typeof(CutsceneLevel)))
        {
            if (cutsceneLevel.cutsceneName == name)
            {
                currentCutscene = cutsceneLevel;
                break;
            }
        }

        if (currentCutscene != null)
        {
            // Load Cutscene Scene
            LoadScene(cutsceneSceneName);
        
            DialogueManager.Instance.onDialogueEnd.AddListener(EndCutscene);
            DialogueManager.Instance.StartDialogue(currentCutscene.cutsceneText);
        }
        else
        {
            // Didn't find cutscene with this name
            Debug.Log("WARNING: Couldn't find cutscene with name " + name);
            return;
        }

        gameState = GameState.CUTSCENE;
    }

    private void EndCutscene()
    {
        currentCutscene.onCutsceneEnd.Invoke();
        currentCutscene = null;
    }
    
    public void GrantRandomSouls(int qty)
    {
        // Generate 3 random souls
        for (int i = 0; i < qty; i++)
        {
            Soul soul = SoulManager.Instance.GenerateSoul();
            souls.Add(soul);
        }
    }

    public void EnterMap()
    {
        ClearUI();

        Debug.Log("init map");
        LoadScene(mapSceneName);
        gameState = GameState.MAP;
    }

    public void InitializeBattle()
    {
        ClearUI();

        Debug.Log("init battle");
        // Set to create loadout
        LoadoutUI.Instance.loadoutCreated = false;
        // Toggle correct UIs
        UIManager.Instance.SetLoadoutUI(false);
        UIManager.Instance.loadoutUIButton.SetActive(true);
        UIManager.Instance.battleUI.SetActive(true);

        battleManager.SetActive(true);
        BattleManager.Instance.Initialize();

        gameState = GameState.PREBATTLE;
    }

    public void StartVesselPlacement()
    {
        if (TutorialManager.Instance.tutorialEnabled)
        {
            if (TutorialManager.Instance.inTutorialBattle)
            {
                SayTop("Click to place each character before the battle. You can navigate the map with the scroll wheel and middle mouse.", 10);
            }
            else
            {
                SayTop("Healer: Hey! You better put me out of harm's way or we're all in trouble.");
            }
        }
        
        UIManager.Instance.StartVesselPlacement(BattleManager.Instance.selectedVessels);
    }

    public void StartFighting()
    {
        if (TutorialManager.Instance.tutorialEnabled)
        {
            if (TutorialManager.Instance.inTutorialBattle)
            {
                SayTop("Characters start fighting automatically. They gain mana based on the damage they deal with basic attacks.", 10);
            }
            else if (TutorialManager.Instance.inMeetupBattle)
            {
                SayTop("Mage: I have a powerful area attack called Light's Extosis, and Healer has Major Heal. You'll need them.", 10);
            }
        }
        
        BattleManager.Instance.StartBattle();

        gameState = GameState.FIGHTING;
    }
    
    public void EndFighting(bool win)
    {
        if (win)
        {
            Debug.Log("heros win");
        }
        else
        {
            Debug.Log("heros lose");
        }

        SetCameraControls(false);
        ClearUI();
        
        // Normally, return to map. Later, we may want to do things like play cutscenes for quest ends, or go to special scenes
        if (!TutorialManager.Instance.tutorialEnabled)
        {
            DialogueManager.Instance.onDialogueEnd.AddListener(EnterMap);
            DialogueManager.Instance.StartDialogue(new TextAsset("We did it!"));
        }
        else
        {
            if (TutorialManager.Instance.inTutorialBattle)
            {
                // Tutorial end of battle stuff
                if (win)
                {
                    // Go to next cutscene
                    TutorialManager.Instance.inTutorialBattle = false;
                    StartCutscene("TaurinMeetsFriends");
                }
                else
                {
                    // Restart tutorial battle
                    TutorialManager.Instance.usedAbility = false;
                    DialogueManager.Instance.onDialogueEnd.AddListener(EnterTutorialBattle);
                    DialogueManager.Instance.StartDialogue(new TextAsset("I've been defeated by a mere slime )-: Maybe I should try harder."));
                }
            }
            else if (TutorialManager.Instance.inMeetupBattle)
            {
                // Meetup end of battle
                if (win)
                {
                    TutorialManager.Instance.inMeetupBattle	 = false;
                    DialogueManager.Instance.onDialogueEnd.AddListener(EnterMap);
                    DialogueManager.Instance.StartDialogue(new TextAsset("We did it! Let's check out the rest of the forest."));
                }
                else
                {
                    DialogueManager.Instance.onDialogueEnd.AddListener(EnterTutorialMultibattle);
                    DialogueManager.Instance.StartDialogue(new TextAsset("Ow ow ow! Let's go heal up and try and get past these slimes again."));
                }
                
            }
        }
        
    }

    public void SayTop(string text)
    {
        topDialogue.PopupDialogue(text);
    }
    
    public void SayTop(string text, float duration)
    {
        topDialogue.PopupDialogue(text, duration);
    }
    
    public void SetCameraControls(bool on)
    {
        BattleManager.Instance.camController.Initialize();
        BattleManager.Instance.camController.enabled = on;
    }

    public void LoadScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }
    
    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void EnterBattleScene(string levelName)
    {
        TutorialManager.Instance.tutorialEnabled = false;

        Debug.Log("GameManager | Starting to load level " + levelName);
        sceneToLoad = levelName;
        DialogueManager.Instance.onDialogueEnd.AddListener(LoadSceneAfterDialogue);
        DialogueManager.Instance.StartDialogue(levelStartDialogue);
    }

    private void LoadSceneAfterDialogue()
    {
        ClearUI();
        gameState = GameState.CUTSCENE;
        LoadScene(sceneToLoad);
        GameManager.Instance.InitializeBattle();
    }
    
    ////////// Tutorial fun
    // Can pull out into TutorialManager if it gets too unwieldy
    // Battle with just Taurin, basics
    public void EnterTutorialBattle()
    {
        TutorialManager.Instance.inTutorialBattle = true;
        
        LoadScene(tutorialBattleSceneName);
        
        // Init battle, THEN do special stuff
        GameManager.Instance.InitializeBattle();
        
        // Open the Loadout by default
        UIManager.Instance.SetLoadoutUI(true);
        
        // Disable other vessels for now
        VesselManager.Instance.SetAllVesselEnabledTo(false);
        VesselManager.Instance.GetVesselEntryById("Taurin").enabled = true;

        // For tutorial, only need Taurin
        LoadoutUI.Instance.requiredVessels = 1;
        LoadoutUI.Instance.CreateLoadoutSlots();
        
        // Refresh for Loadout
        LoadoutUI.Instance.PopulateVesselGrid();
        LoadoutUI.Instance.Refresh();
        
        // Start loadout tutorial dialogue
        DialogueManager.Instance.StartDialogue(loadoutTutorialDialogue);
    }

    // Battle with multiple units
    public void EnterTutorialMultibattle()
    {
        TutorialManager.Instance.inMeetupBattle	= true;
        
        LoadScene("TutorialMeetupBattle");
        
        // Init battle, THEN do special stuff
        GameManager.Instance.InitializeBattle();
        
        // Open the Loadout by default
        UIManager.Instance.SetLoadoutUI(true);
        
        // Disable other vessels for now
        VesselManager.Instance.SetAllVesselEnabledTo(false);
        VesselManager.Instance.GetVesselEntryById("Taurin").enabled = true;
        VesselManager.Instance.GetVesselEntryById("Mage").enabled = true;
        VesselManager.Instance.GetVesselEntryById("Healer").enabled = true;

        // For tutorial, only need Taurin
        LoadoutUI.Instance.requiredVessels = 3;
        LoadoutUI.Instance.CreateLoadoutSlots();
        
        // Refresh for Loadout
        LoadoutUI.Instance.PopulateVesselGrid();
        LoadoutUI.Instance.Refresh();
        
        // Start loadout tutorial dialogue- Removed for now to prevent repeat dialogue
        //DialogueManager.Instance.StartDialogue(tutorialMeetupPrebattleDialogue);
    }

}
