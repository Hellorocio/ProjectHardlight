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
    public string firstSceneName;

    public GameObject battleManager;

    // Map state
    public bool[] unlockedLevels = {true,false,false};
    public bool[] levelsBeaten = {false,false,false};
    public int currentLevel;

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

    public void Initialize()
    {
        ClearUI();
        gameState = GameState.START;
    }

    public void NewCampaign()
    {
        StartCampaign();
    }

    public void StartCampaign()
    {
        GrantRandomSouls(3);

        UIManager.Instance.battleUI.SetActive(false);
        SceneManager.LoadScene(firstSceneName);
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

    public void InitializeMap()
    {
        ClearUI();

        Debug.Log("init map");
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
        UIManager.Instance.StartVesselPlacement(BattleManager.Instance.selectedVessels);
    }

    public void StartFighting()
    {
        BattleManager.Instance.StartBattle();

        gameState = GameState.FIGHTING;
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
                LoadScene(2);
                break;
            case 1:
                LoadScene(3);
                break;
            case 2:
                LoadScene(4);
                break;
        }
    }
}
