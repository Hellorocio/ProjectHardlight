using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : Singleton<MapManager>
{
    // Start is called before the first frame update
    public Node[] nodes;
    public GameObject Party;
    public GameObject[] panels;
    
    [HideInInspector]
    public bool[] unlockedLevels = {true,false,false};
    public bool[] levelsBeaten = {false,false,false};
    public int currentLevel;

    public TextAsset levelStartDialogue;

    int currentNode;

    void Start()
    {
        if (levelsBeaten[2] == true)
        {
            GameManager.Instance.LoadScene(5);
        }

        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i].unlocked = unlockedLevels[i];
        }
        Party.transform.position = nodes[currentNode].transform.position;
        panels[currentNode].SetActive(true);
        updatePanel();
        GetComponent<MapPathFollow>().StartPosition = Party.transform.position;
        GetComponent<MapPathFollow>().CurrentPosition = Party.transform.position;
    }

    void checkPos(){
        Party.transform.position = nodes[currentNode].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void goLeft()
    {
        currentNode -= 1;
        if (currentNode < 0)
        {
            currentNode = 0;   
        }
        //checkPos();
    }

    public void goRight()
    {
        currentNode += 1;
        if(currentNode > 2){
            currentNode = 2;
        }
        //checkPos();
    }

    public void travel(int destination){
        if(nodes[destination].unlocked == false || destination == currentNode || GetComponent<MapPathFollow>().traveling){
            return;
        }
        foreach(GameObject p in panels){
            p.SetActive(false);
        }
        

        switch(currentNode){
            case 0:
                if(destination == 1){
                    goRight();
                    GetComponent<MapPathFollow>().travel(0);
                }
                else{
                    goRight();
                    goRight();
                    GetComponent<MapPathFollow>().travel(2);
                }
                
                break;
            case 1:
                if(destination == 0){
                    GetComponent<MapPathFollow>().travel(3);
                    goLeft();
                }
                else{
                    GetComponent<MapPathFollow>().travel(1);
                    goRight();
                }
                 break;

            case 2:
                if(destination == 1){
                    GetComponent<MapPathFollow>().travel(4);
                    goLeft();
                }
                else{
                    GetComponent<MapPathFollow>().travel(5);
                    goLeft();
                    goLeft();
                }
                break;
        }
        panels[currentNode].SetActive(true);
        updatePanel();

    }
    
    public void updatePanel(){
        if(levelsBeaten[currentNode] == true){
            if(panels[currentNode].GetComponentInChildren<Button>()){
            panels[currentNode].GetComponentInChildren<Button>().gameObject.SetActive(false);
            }
        }
        
    }

    public void go(int index){
        DialogueManager.Instance.StartDialogue(levelStartDialogue);
        LevelSelect(index);
    }

    public void startLevel()
    {
        GameManager.Instance.EnterBattleScene(currentLevel);
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
    
    public void UnlockLevel(int index)
    {
        unlockedLevels[index] = true;
    }

    public void LevelSelect(int index)
    {
        currentLevel = index;
    }
}
