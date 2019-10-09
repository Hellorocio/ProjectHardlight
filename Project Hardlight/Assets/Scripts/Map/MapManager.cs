using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    public GameObject party;
    public MapPopoverController popOver;

    //colors for nodes (replace with images?)
    public Color locked;
    public Color battle;
    public Color hub;
    public Color boss;

    private MapNode[] nodes;
    private MapNode currentNode;

    void Start()
    {
        //init nodes
        nodes = GetComponentsInChildren<MapNode>();

        //init level statuses
        if (GameManager.Instance.levelStatuses.Length == 0)
        {
            MapNode.NodeStatus[] statuses = new MapNode.NodeStatus[nodes.Length];
            for (int i = 0; i < statuses.Length; i++)
            {
                statuses[i] = nodes[i].status;
            }
            GameManager.Instance.levelStatuses = statuses;
        }

        SetNodeAppearances();

        //set party location
        SetPartyLocation(nodes[GameManager.Instance.currentLevel].transform.position, true);
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
            SetPartyLocation(node.transform.position, false);

            //activate go button
            party.SetActive(true);

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

    void SetPartyLocation (Vector3 nodePos, bool snapToLocation)
    {
        Vector3 tempPos = nodePos;
        tempPos.y += 50f;
        party.transform.position = tempPos;
    }

    public void PressFightButton ()
    {
        GameManager.Instance.EnterBattleScene(currentNode.sceneToLoad);
        party.SetActive(false);
    }
    
}
