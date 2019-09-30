using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [HideInInspector]
    public bool[] unlockedLevels = {true,false,false};
    public bool[] levelsBeaten = {false,false,false};
    public int currentLevel;

    // TODO Pull out?
    public List<Soul> souls;

    public void StartCampaign()
    {
        // Generate 3 random souls
        for (int i = 0; i < 3; i++)
        {
            Soul soul = SoulManager.Instance.GenerateSoul();
            souls.Add(soul);
        }
    }

    public void LoadScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void UnlockLevel(int index)
    {
        unlockedLevels[index] = true;
    }

    public void LevelSelect(int index)
    {
        currentLevel = index;
    }
    public void StartLevel()
    {
        switch(currentLevel)
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

    public void WinLevel()
    {
        levelsBeaten[currentLevel] = true;
        switch(currentLevel)
        {
            case 0:
                unlockedLevels[1] = true;
                break;
            case 1:
                unlockedLevels[2] = true;
                break;
        }
    }
}
