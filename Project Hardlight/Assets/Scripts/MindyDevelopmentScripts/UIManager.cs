using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public GameObject battleUI;
    public GameObject loadoutUI;
    public GameObject loadoutUIButton;

    public GameObject commandsUI;

    public GameObject heroPlacer;
    public List<GameObject> DEBUGPREFABS; // delete this when no longer needed

    public void ToggleLoadoutUI()
    {
        SetLoadoutUI(!loadoutUI.active);
    }

    public void SetLoadoutUI(bool isActive)
    {
        loadoutUI.SetActive(isActive);
        if (isActive)
        {
            loadoutUI.GetComponent<LoadoutUI>().Refresh();
        }
    }

    public void StartHeroPlacementFromLoadout()
    {
        loadoutUI.SetActive(false);
        loadoutUIButton.SetActive(false);
        heroPlacer.SetActive(true);

        heroPlacer.GetComponent<HeroPlacer>().StartHeroPlacement(DEBUGPREFABS); // This method requires a list of gameobject representing each selected hero

    }
}
