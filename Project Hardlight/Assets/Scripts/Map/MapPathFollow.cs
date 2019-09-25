using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPathFollow : MonoBehaviour
{
    public Node[] Nodes;
    public GameObject Party;
    public float Speed;
    float Timer;
    [HideInInspector]
    public Vector3 CurrentPosition;

    [HideInInspector]
    public Vector2 StartPosition;
    int CurrentNode = 0;

    private bool pathChosen;
    public GameObject[] paths;

    [HideInInspector]
    public bool traveling;

    // Start is called before the first frame update
    void Start()
    {
        StartPosition = Party.transform.position;
        CurrentPosition = Party.transform.position;
    }

    void CheckNode()
    {
            StartPosition = Party.transform.position;
            Timer = 0;
            CurrentPosition = Nodes[CurrentNode].transform.position;       
    }

    // Update is called once per frame
    void Update()
    {   if(pathChosen)
        {

            Timer += Time.deltaTime * Speed;
            if(Party.transform.position != CurrentPosition)
            {
                Party.transform.position = Vector3.Lerp(StartPosition, CurrentPosition,Timer);
            }
            else
            {
                if(CurrentNode < Nodes.Length -1)
                {
                    CurrentNode++;
                    CheckNode();
                }

                else
                {
                    traveling = false;
                    pathChosen = false;
                }
            }
        }
    }
         
    public void travel(int path)
    {
        Nodes = paths[path].GetComponentsInChildren<Node>();
        CurrentNode = 0;
        traveling = true;
        pathChosen = true;
       
    }
}
    

