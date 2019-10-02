using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public enum GameState { UNKNOWN, START, CUTSCENE, MAP, PREBATTLE, FIGHTING, OTHER };

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

    // Used to load dialogue after something for example
    int sceneToLoad = -1;

    // Map state
    public bool[] unlockedLevels = {true,false,false};
    public bool[] levelsBeaten = {false,false,false};
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
    }

    private void EndCutscene()
    {
        currentCutscene.onCutsceneEnd.Invoke();
        DialogueManager.Instance.onDialogueEnd.RemoveListener(EndCutscene);
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
        DialogueManager.Instance.onDialogueEnd.RemoveListener(EnterMap);

        ClearUI();

        Debug.Log("init map");
        gameState = GameState.MAP;
        LoadScene(mapSceneName);
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
        UIManager.Instance.StartVesselPlacement(BattleManager.Instance.selectedVessels);
    }

    public void StartFighting()
    {
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

        // Tutorial end of battle stuff
        if (TutorialManager.Instance.tutorialEnabled && TutorialManager.Instance.inTutorialBattle)
        {
            topDialogue.PopupDialogue("hello!");
            TutorialManager.Instance.inTutorialBattle = false;
            DialogueManager.Instance.onDialogueEnd.AddListener(EnterTutorialBattle);
        }
        else
        {
            // Normally, return to map. Later, we may want to do things like play cutscenes for quest ends, or go to special scenes
            DialogueManager.Instance.onDialogueEnd.AddListener(EnterMap);
        }
        
        DialogueManager.Instance.StartDialogue(fightingEndDialogue);
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

    public void EnterBattleScene(int level)
    {
        Debug.Log("GameManager | Starting to load level number " + level + " (Not the same as scene number)");
        switch(level)
        {
            case 0:
                sceneToLoad = 8;
                break;
            case 1:
                sceneToLoad = 1;
                break;
            case 2:
                sceneToLoad = 4;
                break;
        }

        DialogueManager.Instance.onDialogueEnd.AddListener(LoadSceneAfterDialogue);
        DialogueManager.Instance.StartDialogue(levelStartDialogue);
        
        GameManager.Instance.InitializeBattle();
    }

    public void EnterTutorialBattle()
    {
        DialogueManager.Instance.onDialogueEnd.RemoveListener(EnterTutorialBattle);

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

    private void LoadSceneAfterDialogue()
    {
        ClearUI();
        gameState = GameState.CUTSCENE;
        LoadScene(sceneToLoad);
    }
}
