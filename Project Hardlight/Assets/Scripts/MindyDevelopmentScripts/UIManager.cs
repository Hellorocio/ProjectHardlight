using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public GameObject loadoutUI;

    public void ToggleLoudoutUI()
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
}
