using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    
    private int numOfEnemies;
    private int numOfHeroes;

    private GameObject[] enemies;
    private GameObject[] heroes;

    public GameObject DialoguePanel;
    

    // Start is called before the first frame update
    void Start()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        heroes = GameObject.FindGameObjectsWithTag("Player");

        numOfEnemies = enemies.Length;
        numOfHeroes = heroes.Length;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void checkFighters(){
         enemies = GameObject.FindGameObjectsWithTag("Enemy");
        heroes = GameObject.FindGameObjectsWithTag("Player");

        numOfEnemies = enemies.Length;
        numOfHeroes = heroes.Length;

        if(numOfHeroes <= 0){
            DialoguePanel.SetActive(true);
            DialoguePanel.GetComponentInChildren<Text>().text = "Aw man!";
        }
        else if( numOfEnemies <= 0){
            DialoguePanel.SetActive(true);
            DialoguePanel.GetComponentInChildren<Text>().text = "We did it!";
            GameManager.Instance.winLevel();
        }
        
    }

    public void returnToMap(){
        GameManager.Instance.loadScene(1);
    }
}
