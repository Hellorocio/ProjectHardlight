using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    
    public int numOfEnemies;
    public int numOfHeroes;

    public GameObject[] enemies;
    public GameObject[] heroes;
    

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
            GameManager.Instance.loadScene(1);
        }
        else if( numOfEnemies <= 0){
            GameManager.Instance.winLevel();
             GameManager.Instance.loadScene(1);
        }
        
    }
}
