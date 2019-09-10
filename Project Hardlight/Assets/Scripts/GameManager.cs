using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [HideInInspector]
    public bool[] unlockedLevels = {true,true,true};
    public bool[] levelsBeaten = {true,false,false};

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
        SceneManager.LoadScene(currentLevel);
    }

}
