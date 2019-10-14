using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    public GameObject party;
    public GameObject enterButton;
    public MapPopoverController popOver;

    //colors for nodes (replace with images?)
    public Color locked;
    public Color battle;
    public Color hub;
    public Color boss;

    private MapNode[] nodes;
    private MapNode currentNode;
    private MapNode currentTraveralNode;

    void Start()
    {
        //init nodes
        nodes = GetComponentsInChildren<MapNode>();

        
        if (GameManager.Instance.levelStatuses.Length != 0)
        {
            //set level statuses
            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i].status = GameManager.Instance.levelStatuses[i];
            }
        }
        else
        {
            //init level statuses
            MapNode.NodeStatus[] statuses = new MapNode.NodeStatus[nodes.Length];
            for (int i = 0; i < statuses.Length; i++)
            {
                statuses[i] = nodes[i].status;
            }
            GameManager.Instance.levelStatuses = statuses;
        }

        

        SetNodeAppearances();

        //set party location
        party.transform.position = nodes[GameManager.Instance.currentLevel].transform.position;
        currentNode = nodes[GameManager.Instance.currentLevel];
    }

    /// <summary>
    /// Called by node on button click
    /// Moves party to the clicked node and turns on GO button if node isn't locked
    /// </summary>
    public void ClickNode (MapNode node)
    {
        if (node.status != MapNode.NodeStatus.LOCKED)
        {
            //move party to node
            StopAllCoroutines();
            StartCoroutine(MoveParty(currentNode, node));

            //activate go button
            if (node.type == MapNode.NodeType.HUB)
            {
                enterButton.GetComponentInChildren<Text>().text = "ENTER";
            }
            else
            {
                enterButton.GetComponentInChildren<Text>().text = "FIGHT";
            }

            Vector3 tempPos = node.transform.position;
            tempPos.y -= 40f;
            enterButton.transform.position = tempPos;
            enterButton.SetActive(true);

            currentNode = node;
        }
    }

    /// <summary>
    /// Set appearence for each node based on the values saved in GameManager
    /// </summary>
    private void SetNodeAppearances ()
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            MapNode.NodeStatus status = GameManager.Instance.levelStatuses[i];
            nodes[i].SetStatus(status, GetNodeAppearance(status, nodes[i].type));
        }
    }

    /// <summary>
    /// Get the color for a node based on its status and type
    /// </summary>
    /// <param name="status"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public Color GetNodeAppearance (MapNode.NodeStatus status, MapNode.NodeType type)
    {
        if (status == MapNode.NodeStatus.LOCKED)
        {
            return locked;
        }
        else
        {
            if (type == MapNode.NodeType.BATTLE)
            {
                return battle;
            }
            else
            if (type == MapNode.NodeType.BOSS)
            {
                return boss;
            }
            else
            {
                return hub;
            }
        }
    }

    /// <summary>
    /// Moves the party on a path from start node to end node
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    IEnumerator MoveParty (MapNode start, MapNode end)
    {
        if (currentTraveralNode != null)
        {
            start = currentTraveralNode;
        }

        List<MapNode> path = GetNodePath(start, end);
        if (path != null)
        {
            foreach (MapNode node in path)
            {
                currentTraveralNode = node;
                while (Vector2.Distance(party.transform.position, node.transform.position) > 0.1f)
                {
                    party.transform.position = Vector3.MoveTowards(party.transform.position, node.transform.position, 2f);
                    yield return null;
                }
            }
        }
        currentTraveralNode = null;
        
    }

    /// <summary>
    /// Uses a breadth first search to find a route from one node to the next
    /// Ref: https://gist.github.com/hermesespinola/15cf66af8edf059df9f38c6c879db0cb
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    List<MapNode> GetNodePath (MapNode start, MapNode end)
    {
        Queue<MapNode> work = new Queue<MapNode>();
        List<MapNode> visited = new List<MapNode>();

        work.Enqueue(start);
        visited.Add(start);
        start.history = new List<MapNode>();

        while (work.Count > 0)
        {

            MapNode current = work.Dequeue();

            if (current == end)
            {
                // we found a solution!!
                List<MapNode> result = current.history;
                result.Add(current);
                return result;

            }
            else
            {

                for (int i = 0; i < current.adjacentNodes.Length; i++)
                {

                    MapNode currentChild = current.adjacentNodes[i];
                    if (!visited.Contains(currentChild))
                    {

                        work.Enqueue(currentChild);
                        visited.Add(currentChild);
                        currentChild.history = new List<MapNode>(current.history);
                        currentChild.history.Add(current);
                    }

                }
            }
        }

        // No availabe path
        return null;
    }

    /// <summary>
    /// Enters battle if the node does not have a cutscene, otherwise load beforeCutscene
    /// </summary>
    public void PressFightButton ()
    {
        GameManager.Instance.SetCurrentLevelInfo(GetIndexFromNode(currentNode), GetLevelsToUnlock(currentNode.unlockNodes), currentNode.cutsceneAfter);
        if (currentNode.cutsceneBefore != "")
        {
            GameManager.Instance.StartCutscene(currentNode.cutsceneBefore);
        }
        else if (currentNode.type == MapNode.NodeType.BATTLE)
        {
            GameManager.Instance.EnterBattleScene(currentNode.sceneToLoad);
        }
        else if (currentNode.type == MapNode.NodeType.HUB)
        {
            GameManager.Instance.EnterHub(currentNode.sceneToLoad);
        }
        party.SetActive(false);
    }

    public int GetIndexFromNode (MapNode node)
    {
        int index = -1;
        for (int i = 0; i < nodes.Length; i++)
        {
            if  (nodes[i] == node)
            {
                index = i;
                break;
            }
        }
        return index;
    }

    public int[] GetLevelsToUnlock (MapNode[] unlockLevels)
    {
        int[] returnLevels = new int[unlockLevels.Length];
        for (int i = 0; i < returnLevels.Length; i++)
        {
            returnLevels[i] = GetIndexFromNode(unlockLevels[i]);
        }
        return returnLevels;
    }
}
