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

    // Audio stuff
    public AudioClip UIMusic;
    public AudioClip battleMusic;

    // Used to load dialogue after something for example
    string sceneToLoad = "";

    // Map state
    [HideInInspector]
    public MapNode.NodeStatus[] levelStatuses;
    [HideInInspector]
    public int currentLevel;
    private string battleEndCutscene = "";
    private int[] nodesToUnlock;

    public DialogueBoxController topDialogue;

    public void Start()
    {
        // Destroy self if already exists
        if (GameManager.Instance != this)
        {
            Destroy(gameObject);
        }

        BattleManager.Instance.SetCursor(BattleManager.Instance.battleConfig.defaultCursor);
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

    public void EndCutscene()
    {
        DialogueManager.Instance.onDialogueEnd.RemoveAllListeners();
        CutsceneLevel cutscene = currentCutscene;
        currentCutscene = null;
        cutscene.onCutsceneEnd.Invoke();
        
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

        DialogueManager.Instance.onDialogueEnd.RemoveAllListeners();
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
        UIManager.Instance.SetLoadoutUI(true);
        UIManager.Instance.loadoutUIButton.SetActive(true);
        UIManager.Instance.battleUI.SetActive(true);

        battleManager.SetActive(true);
        BattleManager.Instance.Initialize();

        gameState = GameState.PREBATTLE;

        //switch music
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        if (camera != null && UIMusic != null)
        {
            AudioSource audioSource = camera.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.Stop();
                audioSource.clip = UIMusic;
                audioSource.Play();
            }
        }
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

        //switch music
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        if (camera != null && battleMusic != null)
        {
            AudioSource audioSource = camera.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.clip = battleMusic;
                audioSource.Play();
            }
        }
    }

    /// <summary>
    /// Plays a cutscene after the battle
    /// </summary>
    public void StartAfterBattleCutscene ()
    {
        DialogueManager.Instance.onDialogueEnd.RemoveAllListeners();
        StartCutscene(battleEndCutscene);
        battleEndCutscene = "";
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
            if (win)
            {
                if (battleEndCutscene != "")
                {
                    DialogueManager.Instance.onDialogueEnd.AddListener(StartAfterBattleCutscene);
                }
                else
                {
                    DialogueManager.Instance.onDialogueEnd.AddListener(EnterMap);
                }
                DialogueManager.Instance.StartDialogue(new TextAsset("We did it!"));

                //unlock levels
                UnlockLevels();
            }
            else
            {
                DialogueManager.Instance.onDialogueEnd.AddListener(EnterMap);
                DialogueManager.Instance.StartDialogue(new TextAsset("Shoot! Let's try this again."));
            }
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

    /// <summary>
    /// Loads the given scene
    /// If initBattle is true, battle will start when scene finishes loading
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="initBattle"></param>
    public void LoadScene(string scene, bool initBattle = false)
    {
        StartCoroutine(LoadNewScene(scene, initBattle));
        //SceneManager.LoadScene(scene);
    }

    /// <summary>
    /// Called when FIGHT! button is pressed on the map
    /// </summary>
    /// <param name="levelName"></param>
    public void EnterBattleScene(string levelName)
    {
        //disable tutorial stuff
        TutorialManager.Instance.tutorialEnabled = false;
        VesselManager.Instance.SetAllVesselEnabledTo(true);

        //load battle
        Debug.Log("GameManager | Starting to load level " + levelName);
        sceneToLoad = levelName;
        LoadScene(sceneToLoad, true);
        //DialogueManager.Instance.onDialogueEnd.AddListener(LoadSceneAfterDialogue);
        //DialogueManager.Instance.StartDialogue(levelStartDialogue);
    }

    /// <summary>
    /// Triggered after DialogueManager calls Dialogue End
    /// </summary>
    private void LoadSceneAfterDialogue()
    {
        ClearUI();
        gameState = GameState.CUTSCENE;
        LoadScene(sceneToLoad, true);
    }

    // Ref: https://blog.teamtreehouse.com/make-loading-screen-unity
    // TODO: Add loading screen stuff 
    IEnumerator LoadNewScene(string scene, bool initBattle)
    {
        // Start an asynchronous operation to load the scene that was passed to the LoadNewScene coroutine.
        AsyncOperation async = SceneManager.LoadSceneAsync(scene);

        // While the asynchronous operation to load the new scene is not yet complete, continue waiting until it's done.
        while (!async.isDone)
        {
            yield return null;
        }

        if (initBattle)
        {
            InitializeBattle();
        }
    }

    ////////// Tutorial fun
    // Can pull out into TutorialManager if it gets too unwieldy
    // Battle with just Taurin, basics
    public void EnterTutorialBattle()
    {
        TutorialManager.Instance.inTutorialBattle = true;
        
        LoadScene(tutorialBattleSceneName, true);
                
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
        DialogueManager.Instance.onDialogueEnd.RemoveAllListeners();
        DialogueManager.Instance.StartDialogue(loadoutTutorialDialogue);
    }

    // Battle with multiple units
    public void EnterTutorialMultibattle()
    {
        TutorialManager.Instance.inMeetupBattle	= true;
        
        LoadScene("TutorialMeetupBattle", true);
        
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
        DialogueManager.Instance.onDialogueEnd.RemoveAllListeners();
        DialogueManager.Instance.StartDialogue(tutorialMeetupPrebattleDialogue);
    }

    /// <summary>
    /// Called by MapNode, sets some info about levels
    /// </summary>
    public void SetCurrentLevelInfo (int current, int[] unlock, string cutscene = "")
    {
        currentLevel = current;
        nodesToUnlock = unlock;
        battleEndCutscene = cutscene;
    }

    public void UnlockLevels ()
    {
        //set level that was just beaten to discovered
        levelStatuses[currentLevel] = MapNode.NodeStatus.DISCOVERED;

        //set all levels that the beaten level unlocks to undiscovered
        if (nodesToUnlock != null)
        {
            for (int i = 0; i < nodesToUnlock.Length; i++)
            {
                levelStatuses[nodesToUnlock[i]] = MapNode.NodeStatus.UNDISCOVERED;
            }
        }
        nodesToUnlock = null;
    }

}
