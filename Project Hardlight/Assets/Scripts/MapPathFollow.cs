using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPathFollow : MonoBehaviour
{
    public Transform[] Nodes;
    public GameObject Party;
    public float Speed;
    float Timer;
    static Vector3 CurrentPosition;

    private Vector2 StartPosition;
    int CurrentNode = 0;
    public GameObject[] FirstNodes;

    private bool PathChosen;
    public bool traveling;


    // Start is called before the first frame update
    void Start()
    {
        //Nodes = FirstNodes[0].GetComponentsInChildren<Transform>();
        CurrentPosition = Party.transform.position;
        //CheckPos();

    }

    void CheckPos()
    {
        
            StartPosition = Party.transform.position;
            Timer = 0;
            CurrentPosition = Nodes[CurrentNode].position;
            
    }

    // Update is called once per frame
    void Update()
    {   
        if(PathChosen)
        {
            Timer += Time.deltaTime * Speed;
            if(Party.transform.position != CurrentPosition)
            {
                traveling = true;
                Party.transform.position = Vector3.Lerp(StartPosition, CurrentPosition,Timer);
            }
            else
            {
                if( CurrentNode < Nodes.Length -1)
                {
                    CurrentNode++;
                    CheckPos();
                }
                else if(CurrentNode == Nodes.Length -1)
                {
                    PathChosen = false;
                    traveling = false;
                }
                
                
            }
        }
    }

    public void newDestination(int start){
        if(traveling){
            return;
        }
        Nodes = FirstNodes[start].GetComponentsInChildren<Transform>();
        CurrentNode = 0;
        PathChosen = true;
        //CheckPos();
        

    }
}
