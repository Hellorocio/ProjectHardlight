using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapNode : MonoBehaviour
{
    public enum NodeType { BATTLE, HUB, BOSS }

    //LOCKED- can't go to, UNDISCOVERED- can go to, haven't beaten yet, DISCOVERED- can go to, have beaten already 
    public enum NodeStatus { LOCKED, UNDISCOVERED, DISCOVERED }

    public enum Difficulty { NONE, EASY, MEDIUM, HARD }

    [Header("Node Details")]
    public NodeType type;
    public NodeStatus status;
    
    public string levelName;
    [TextArea(5,5)]
    public string levelDescription;
    public string sceneToLoad; //note that this will only load if there is not a cutscene before the battle

    public MapNode[] adjacentNodes;

    [Header("Quest Details")]
    public MapNode[] unlockNodes; //nodes that unlock once this node has been discovered
    public string cutsceneBefore = "";
    public string cutsceneAfter = "";

    [Header("Battle Details")]
    public Difficulty difficulty;
    public AllightType[] allightDrops;

    private Text nameText;

    private void Start()
    {
        nameText = GetComponentInChildren<Text>();
        nameText.text = levelName;
    }
    /// <summary>
    /// Sets the status of this node, which also updates the color
    /// </summary>
    public void SetStatus (NodeStatus s, Color c)
    {
        status = s;
        GetComponent<Image>().color = c;
    }

    public void SetNameTextShowing ()
    {
        nameText.gameObject.SetActive(status != NodeStatus.LOCKED);
    }
}
