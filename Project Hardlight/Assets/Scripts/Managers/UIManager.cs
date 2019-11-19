using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Helpers to manipulate the UI on a larger scale-- like turning off or on whole menus
public class UIManager : Singleton<UIManager>
{
    public GameObject battleUI;
    public GameObject dialogueUI;

    public GameObject loadoutUI;
    public GameObject loadoutUIButton;
    public GameObject commandsUI;
    public GameObject soulUpgradeUI;
    public GameObject heroPlacer;

    public PostBattleUI postBattleUI;

    public GameObject debugUI;
    public GameObject skipTutorialButton;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            ToggleDebugUI();
        }
    }
    public void ToggleDebugUI ()
    {
        if (debugUI != null)
        {
            debugUI.SetActive(!debugUI.activeSelf);
        }
    }

    public void ToggleLoadoutUI()
    {
        SetLoadoutUI(!loadoutUI.active);
    }

    public void ToggleSoulUpgradeUI()
    {
        soulUpgradeUI.SetActive(!soulUpgradeUI.gameObject.activeSelf);
        if (soulUpgradeUI.activeSelf)
        {
            GetComponent<SoulUpgradeUI>().UpdateSoulUI();
        }
    }

    public void SetLoadoutUI(bool isActive)
    {
        loadoutUI.SetActive(isActive);
        if (isActive)
        {
            // Disable camera controls
            if (GameManager.Instance.gameState == GameState.PREBATTLE)
            {
                GameManager.Instance.SetCameraControls(false);
            }
        }
        else
        {
            // Enable camera controls, panning around the map
            if (GameManager.Instance.gameState == GameState.PREBATTLE)
            {
                GameManager.Instance.SetCameraControls(true);
            }
        }
    }

    public void StartVesselPlacement(List<GameObject> objs)
    {
        SetLoadoutUI(false);
        loadoutUIButton.SetActive(false);
        heroPlacer.SetActive(true);
        heroPlacer.GetComponent<HeroPlacer>().StartHeroPlacement(objs); // This method requires a list of gameobject representing each selected hero
    }
}
