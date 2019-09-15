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
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void loadScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void unlockLevel(int index){
        unlockedLevels[index] = true;
    }

    public void levelSelect(int index){
        currentLevel = index;
    }
    public void startLevel(){
        switch(currentLevel){
            case 0:
            loadScene(2);
                break;
            case 1:
            loadScene(3);
                break;
            case 2:
            loadScene(4);
                break;
        }
    }

    public void winLevel(){
        levelsBeaten[currentLevel] = true;
        switch(currentLevel){
            case 0:
                unlockedLevels[1] = true;
                break;
            case 1:
                unlockedLevels[2] = true;
                break;
        }

    }


}
