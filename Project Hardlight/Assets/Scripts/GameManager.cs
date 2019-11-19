using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine.UI;

public enum GameState { UNKNOWN, START, CUTSCENE, MAP, PREBATTLE, FIGHTING, HUB, PAUSED };

public class GameManager : Singleton<GameManager>
{

    public GameState gameState;

    public List<Soul> souls;

    public int requiredVessels = 3;

    //[HideInInspector]
    public int[] fragments = new int[3]; //[0] = sunlight, [1] = moonlight, [2] = starlight

    // TODO Pull all this and cutscene logic to a Cutscene Manager
    public string mapSceneName;
    public string cutsceneSceneName;
    public CutsceneLevel currentCutscene;
    public GameObject cutsceneUI;
    public GameObject cutsceneBG;

    public GameObject battleManager;

    public TextAsset levelStartDialogue;

    public TextAsset fightingStartDialogue;

    // You can change this in runtime
    public TextAsset fightingEndDialogue;
    // Pls don't change this in runtime
    public TextAsset defaultFightingEndDialogue;

    public TextAsset fightingLoseDialogue;
    public TextAsset defaultFightingLoseDialogue;

    // Plays in the map after a battle is won (set by mapManager)
    public TextAsset enterMapAfterBattleDialogue;
    private bool lastBattleWon;

    // Tutorial stuff
    public string firstCutsceneName;
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
    private List<AllightType> allightDrops;
    private Vector2 allightDropRange;

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
        cutsceneUI.SetActive(false);
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
        GetStarterSouls();
        StartCutscene(firstCutsceneName);
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
            
            cutsceneUI.SetActive(true);
            if (cutsceneBG != null)
            {
                cutsceneBG.GetComponent<Image>().sprite = currentCutscene.bgImage;
            }
            
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
        // This order is important to being able to chain cutscenes
        CutsceneLevel endingCutscene = currentCutscene;
        currentCutscene = null;
        DialogueManager.Instance.onDialogueEnd.RemoveAllListeners();
        cutsceneUI.SetActive(false);
        endingCutscene.onCutsceneEnd.Invoke();
    }
    
    public void GrantRandomSouls(int qty)
    {
        // Generate random souls
        for (int i = 0; i < qty; i++)
        {
            Soul soul = SoulManager.Instance.GenerateSoul();
            souls.Add(soul);
        }

        LoadoutUI.Instance.PopulateSoulGrid();
    }

    public void GetStarterSouls()
    {
        Soul[] newSouls = SoulManager.Instance.GenerateDiverseSouls();
        for (int i = 0; i < newSouls.Length; i++)
        {
            souls.Add(newSouls[i]);
        }

        LoadoutUI.Instance.PopulateSoulGrid();
    }

    /// <summary>
    /// Used to ease battle testing pain, uses random souls and first 3 vessels
    /// </summary>
    public void AutoGenerateLoadout ()
    {
        if (souls.Count < 3)
        {
            GetStarterSouls();
        }

        /*
        for (int i = 0; i < 3; i ++)
        {
            GameObject selectedVessel = Instantiate(VesselManager.Instance.vesselCatalog[i].vessel);
            selectedVessel.GetComponent<Fighter>().soul = souls[i];
            BattleManager.Instance.selectedVessels.Add(selectedVessel);
        }
        */
        //This spawns a healer, mage, ninja combo

        for (int i = 0; i < requiredVessels; i++)
        {
            GameObject selectedVessel = Instantiate(VesselManager.Instance.vesselCatalog[i].vessel);
            selectedVessel.GetComponent<Fighter>().soul = souls[i];
            BattleManager.Instance.selectedVessels.Add(selectedVessel);
        }
        
        SetCameraControls(true);
        StartVesselPlacement();
    }

    public void EnterMap()
    {
        ClearUI();
        SetCameraControls(false);

        // if skip tutorial button is enabled when you enter the map, it's time to remove it
        if (TutorialManager.Instance.tutorialEnabled)
        {
            UIManager.Instance.skipTutorialButton.SetActive(false);
        }

        DialogueManager.Instance.onDialogueEnd.RemoveAllListeners();
        Debug.Log("init map");
        LoadScene(mapSceneName);
        gameState = GameState.MAP;
    }

    public void EnterHub (string sceneName)
    {
        DialogueManager.Instance.onDialogueEnd.RemoveAllListeners();

        LoadScene(sceneName);
        gameState = GameState.HUB;
    }

    public void InitializeBattle()
    {
        ClearUI();

        Debug.Log("init battle");
        // Set to create loadout
        //LoadoutUI.Instance.loadoutCreated = false;
        // Toggle correct UIs
        UIManager.Instance.SetLoadoutUI(true);
        LoadoutUI.Instance.CreateLoadout();

        UIManager.Instance.loadoutUIButton.SetActive(true);
        UIManager.Instance.battleUI.SetActive(true);

        battleManager.SetActive(true);
        BattleManager.Instance.Initialize();

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
                //SayTop("Click to place each character before the battle. You can navigate the map with the scroll wheel and middle mouse.", 10);
            }
            else
            {
                //SayTop("Healer: Hey! You better put me out of harm's way or we're all in trouble.", 10);
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
                //SayTop("Characters start fighting automatically. They gain mana based on the damage they deal with basic attacks.", 10);
            }
            else if (TutorialManager.Instance.inMeetupBattle)
            {
                //SayTop("Mage: I have a powerful area attack called Light's Extosis, and Healer has Major Heal. You'll need them.", 10);
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

        HideTutorialPopup();
        SetCameraControls(false);
        ClearUI();
        DialogueManager.Instance.onDialogueEnd.RemoveAllListeners();

        
        // Normally, return to map. Later, we may want to do things like play cutscenes for quest ends, or go to special scenes
        if (!TutorialManager.Instance.tutorialEnabled)
        {
            if (win)
            {
                lastBattleWon = true;

                if (fightingEndDialogue != null)
                {
                    DialogueManager.Instance.StartDialogue(fightingEndDialogue);

                    if (battleEndCutscene != "")
                    {
                        DialogueManager.Instance.onDialogueEnd.AddListener(StartAfterBattleCutscene);
                    }
                    else
                    {
                        DialogueManager.Instance.onDialogueEnd.AddListener(EnterMap);
                    }
                }
                else
                {
                    if (battleEndCutscene != "")
                    {
                        StartAfterBattleCutscene();
                    }
                    else
                    {
                        EnterMap();
                    }
                }

                //unlock levels
                UnlockLevels();
            }
            else
            {
                lastBattleWon = false;

                DialogueManager.Instance.onDialogueEnd.AddListener(EnterMap);
                if (fightingLoseDialogue != null)
                {
                    DialogueManager.Instance.StartDialogue(fightingLoseDialogue);
                }
                else
                {
                    DialogueManager.Instance.StartDialogue(defaultFightingLoseDialogue);
                }

                if (levelStatuses[currentLevel] == MapNode.NodeStatus.UNDISCOVERED)
                {
                    levelStatuses[currentLevel] = MapNode.NodeStatus.DISCOVERED;
                }
            }
        }
        else
        {
            TutorialManager.Instance.FinishTutorialLevel();

            // set all levels that the beaten level unlocks to undiscovered
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

   /// <summary>
   /// Shows tutorial text and pauses the game
   /// </summary>
   /// <param name="text"></param>
    public void ShowTutorialPopup(string text, bool pause, bool disableMovement, bool showOkay)
    {
        DialogueManager.Instance.onDialogueEnd.RemoveAllListeners();
        topDialogue.PopupDialogue(text, showOkay);
        if (pause)
        {
            Time.timeScale = 0;
        }
        if (disableMovement)
        {
            gameState = GameState.PAUSED;
        }
    }

    /// <summary>
    /// Hides tutortal text and unpauses the game
    /// </summary>
    public void HideTutorialPopup()
    {
        topDialogue.CancelPopupDialogue();
        Time.timeScale = 1;
        gameState = GameState.FIGHTING;
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
    public void LoadScene(string scene)
    {
        StartCoroutine(LoadNewScene(scene));
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
        DialogueManager.Instance.onDialogueEnd.RemoveAllListeners();
        Debug.Log("GameManager | Starting to load level " + levelName);
        sceneToLoad = levelName;
        gameState = GameState.PREBATTLE;

        if (levelStartDialogue != null)
        {
            DialogueManager.Instance.onDialogueEnd.AddListener(LoadSceneAfterDialogue);
            DialogueManager.Instance.StartDialogue(levelStartDialogue);
        }
        else
        {
            LoadScene(sceneToLoad);
        }
    }

    /// <summary>
    /// Triggered after DialogueManager calls Dialogue End
    /// </summary>
    private void LoadSceneAfterDialogue()
    {
        ClearUI();
        LoadScene(sceneToLoad);
    }

    // Ref: https://blog.teamtreehouse.com/make-loading-screen-unity
    // TODO: Add loading screen stuff 
    IEnumerator LoadNewScene(string scene)
    {
        // Start an asynchronous operation to load the scene that was passed to the LoadNewScene coroutine.
        AsyncOperation async = SceneManager.LoadSceneAsync(scene);

        // While the asynchronous operation to load the new scene is not yet complete, continue waiting until it's done.
        while (!async.isDone)
        {
            yield return null;
        }

        if (TutorialManager.Instance.tutorialEnabled && TutorialManager.Instance.tutorialLevels[TutorialManager.Instance.currentTutorialLevel].tutorialScene == scene)
        {
            TutorialManager.Instance.InitTutorial();
        }
        else
        if (gameState == GameState.PREBATTLE)
        {
            SetCurrentCombatLevelDialogue();
            if (fightingStartDialogue != null)
            {
                DialogueManager.Instance.onDialogueEnd.AddListener(InitializeBattle);
                DialogueManager.Instance.StartDialogue(fightingStartDialogue);
            }
            else
            {
                InitializeBattle();
            }            
        }
        else
        if (gameState == GameState.MAP && enterMapAfterBattleDialogue != null && lastBattleWon)
        {
            DialogueManager.Instance.StartDialogue(enterMapAfterBattleDialogue);
        }
    }

    /// <summary>
    /// Called by MapNode, sets some info about levels
    /// </summary>
    public void SetCurrentLevelInfo (int current, int[] unlock, MapNode node)
    {
        currentLevel = current;
        nodesToUnlock = unlock;

        if (levelStatuses[currentLevel] != MapNode.NodeStatus.COMPLETED)
        {
            battleEndCutscene = node.cutsceneAfter;
        }
        else
        {
            battleEndCutscene = "";
        }
        
        allightDrops = node.allightDrops;
        allightDropRange = node.allightDropRange;
    }

    public void UnlockLevels ()
    {
        // set level that was just beaten to discovered
        levelStatuses[currentLevel] = MapNode.NodeStatus.COMPLETED;

        // set all levels that the beaten level unlocks to undiscovered
        if (nodesToUnlock != null)
        {
            for (int i = 0; i < nodesToUnlock.Length; i++)
            {
                if (levelStatuses[nodesToUnlock[i]] == MapNode.NodeStatus.LOCKED)
                {
                    levelStatuses[nodesToUnlock[i]] = MapNode.NodeStatus.UNDISCOVERED;
                }
            }
        }
        nodesToUnlock = null;
    }

    public void StartTutorial()
    {
        LoadScene(TutorialManager.Instance.tutorialLevels[0].tutorialScene);
    }

    /// <summary>
    /// Called by the skip tutorial button, moves to map and unlocks all the heroes
    /// </summary>
    public void SkipTutorial()
    {
        TutorialManager.Instance.tutorialEnabled = false;
        TutorialManager.Instance.currentTutorialLevel = 1;
        UIManager.Instance.skipTutorialButton.SetActive(false);
        BattleManager.Instance.portraitHotKeyManager.SetAbilityStuff(true);
        TutorialManager.Instance.CancelTutorial();
        HideTutorialPopup();

        if (gameState == GameState.FIGHTING)
        {
            BattleManager.Instance.BattleOver(false);
            UIManager.Instance.postBattleUI.DisablePostBattleUI();
        }
        BattleManager.Instance.checkAllowMovement = false;

        currentCutscene = null;
        LoadoutUI.Instance.CreateLoadoutSlots();

        DialogueManager.Instance.onDialogueEnd.RemoveAllListeners();
        DialogueManager.Instance.EndDialogue();

        topDialogue.CancelPopupDialogue();

        if (souls.Count < 3)
        {
            GetStarterSouls();
        }

        EnterMap();
    }

    /// <summary>
    /// Called after a victorious battle (by PostBattleUI)
    /// Fragments are generated based on values from allightDrops and allightDropRange (orignally set in mapNode)
    /// Returns amounts of each fragments type that was generated ([0] = sun, [1] = moon, [2] = stars, so postBattleUI can display them
    /// </summary>
    /// <returns></returns>
    public int[] AddFragments ()
    {
        int[] newFragments = new int[3];

        if (allightDrops != null && allightDropRange != null)
        {
            for (int i = 0; i < newFragments.Length; i++)
            {
                if (allightDrops.Contains((AllightType)i))
                {
                    int numFragments = Random.Range((int)allightDropRange.x, (int)allightDropRange.y + 1);
                    newFragments[i] = numFragments;
                    fragments[i] += numFragments;
                }
            }
        }
        
        return newFragments;
    }

    /// <summary>
    /// This is all controlled by GameManager instead of CombatLevel in order to prevent race conditions between the level starting
    /// and CombatLevel setting the variables for dialogue stuff. It's uglier this way but I think it'll work better
    /// </summary>
    public void SetCurrentCombatLevelDialogue ()
    {
        GameObject combatLevelObj = GameObject.Find("CombatLevel");
        if (combatLevelObj != null)
        {
            CombatLevel combatLevel = combatLevelObj.GetComponent<CombatLevel>();
            if (combatLevel != null)
            {
                if (levelStatuses[currentLevel] != MapNode.NodeStatus.COMPLETED)
                {
                    fightingEndDialogue = combatLevel.postBattleDialogue;

                    if (fightingEndDialogue == null)
                    {
                        fightingEndDialogue = defaultFightingEndDialogue;
                    }
                }
                else
                {
                    fightingEndDialogue = null;
                }

                if (levelStatuses[currentLevel] == MapNode.NodeStatus.UNDISCOVERED)
                {
                    fightingStartDialogue = combatLevel.preBattleDialogue;
                }
                else
                {
                    fightingStartDialogue = null;
                }

                fightingLoseDialogue = combatLevel.loseDialogue;
            }
        }
    }

    public void SetRequiredVessels (int set)
    {
        if (set != requiredVessels)
        {
            requiredVessels = set;
            LoadoutUI.Instance.Refresh();
        }
    }

}
