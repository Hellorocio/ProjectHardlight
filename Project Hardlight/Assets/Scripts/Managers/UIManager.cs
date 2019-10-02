﻿using System.Collections;
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

    public GameObject heroPlacer;

    public void ToggleLoadoutUI()
    {
        SetLoadoutUI(!loadoutUI.active);
    }

    public void SetLoadoutUI(bool isActive)
    {
        loadoutUI.SetActive(isActive);
        if (isActive)
        {
            // Refresh what's displayed
            LoadoutUI.Instance.Refresh();
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