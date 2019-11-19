using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapNode : MonoBehaviour
{
    public enum NodeType { BATTLE, HUB, BOSS, TUTORIAL }

    //LOCKED- can't go to, UNDISCOVERED- can go to, haven't beaten yet, DISCOVERED- can go to, have attempted at least once, but haven't beaten yet, COMPLETED- can go to, have beaten
    public enum NodeStatus { LOCKED, UNDISCOVERED, DISCOVERED, COMPLETED }

    public enum Difficulty { NONE, EASY, MEDIUM, HARD }

    [Header("Node Details")]
    public NodeType type;
    public NodeStatus status;
    public int allowedVessels = 1;
    
    public string levelName;
    [TextArea(5,5)]
    public string levelDescription;
    public string sceneToLoad; // note that this will only load if there is not a cutscene before the battle

    public MapNode[] adjacentNodes;

    [Header("Quest Details")]
    public MapNode[] unlockNodes; // nodes that unlock once this node has been discovered
    public MapLine[] mapPaths; // should correspond to the connecting unlockNode
    public string cutsceneBefore = "";
    public string cutsceneAfter = "";
    public TextAsset dialogueBefore;
    public TextAsset dialogueAfter;

    [Header("Battle Details")]
    public Difficulty difficulty;
    public List<AllightType> allightDrops;
    public Vector2 allightDropRange = new Vector2(1, 10);

    [Header("Bosses ONLY")]
    [Tooltip("Nodes that must be completed before this node unlocks- just use for bosses pls b/c I'm a dummy")]
    public MapNode[] requiredNodesCompleted;

    private Text nameText;
    Animator animator;

    [HideInInspector]
    public List<MapNode> history; //this is just used for node searching (see GetNodePath in MapManager)

    private void Start()
    {
        nameText = GetComponentInChildren<Text>();
        nameText.text = levelName;
        animator = GetComponent<Animator>();
        //nameText.gameObject.SetActive(false);
    }
    /// <summary>
    /// Sets the status of this node, which also updates the color
    /// </summary>
    public void SetStatus (NodeStatus s, Color c, bool showLockedNodes = false)
    {
        status = s;
        //GetComponent<Image>().color = c;

        // give this node a chance to init
        if (animator == null)
        {
            Start();
        }

        if (animator != null)
        {
            if (status == NodeStatus.COMPLETED)
            {
                animator.Play("LevelCompleteBanner");
            }
            else
            {
                animator.Play("LevelIncompleteBanner");
            }
        }
        

        gameObject.SetActive(true);

        if (status == NodeStatus.LOCKED)
        {
            gameObject.SetActive(showLockedNodes);
        }
    }

    /// <summary>
    /// Sets paths from completed nodes
    /// </summary>
    public void SetPaths ()
    {
        if (status == NodeStatus.COMPLETED)
        {
            for (int i = 0; i < unlockNodes.Length; i++)
            {
                if (mapPaths.Length <= i)
                {
                    break; // paths havent been set properly for this node, so stop here
                }
                
                if (unlockNodes[i].status == NodeStatus.UNDISCOVERED)
                {
                    mapPaths[i].ShowDottedLine();
                }
                else if (unlockNodes[i].status == NodeStatus.DISCOVERED || unlockNodes[i].status == NodeStatus.COMPLETED)
                {
                    mapPaths[i].ShowSolidLine();
                }
            }
        }
    }

    public void SetNameTextShowing ()
    {
        nameText.gameObject.SetActive(status != NodeStatus.LOCKED);
    }
}
