using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapPopoverController : MonoBehaviour
{
    public Text titleText;
    public Text descriptionText;
    public Image[] allightDropImages;

    public Sprite[] allightSprites; //[0] = sunlight, [1] = moonlight, [2] = starlight

    [HideInInspector]
    public MapNode currentNode;

    private Transform saveParent;
    
    private void ResetAllightDropImages ()
    {
        for (int i = 0; i < allightDropImages.Length; i++)
        {
            allightDropImages[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Called by node on mouse hover and by mapmanager
    /// Sets title and description in panel
    /// If a battle node, also displays difficulty level and allight drops
    /// </summary>
    /// <param name="node"></param>
    public void ShowPopover (MapNode node)
    {
        //only show popover if this is a new one and it's not a locked node
        if (node == currentNode || node.status == MapNode.NodeStatus.LOCKED)
        {
            return;
        }

        currentNode = node;

        titleText.text = node.levelName;
        descriptionText.text = node.levelDescription;
        ResetAllightDropImages();

        if (node.type != MapNode.NodeType.HUB)
        {
            //add difficulty to description
            descriptionText.text += "\nDifficulty: " + node.difficulty.ToString().ToLower();

            //add allight drop icons
            ResetAllightDropImages();
            for (int i = 0; i < node.allightDrops.Length; i++)
            {
                allightDropImages[i].sprite = allightSprites[(int)node.allightDrops[i]];
                allightDropImages[i].gameObject.SetActive(true);
            }
        }

        //set transform
        if (saveParent == null)
        {
            saveParent = transform.parent;
        }

        
        transform.parent = node.transform;
        if (node.transform.localPosition.x > 100)
        {
            transform.localPosition = new Vector3(-180, 0, 0);
        }
        else
        {
            transform.localPosition = new Vector3(180, 0, 0);
        }
        
        transform.parent = saveParent;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Hides the popover
    /// Called when player stops hovering over a node
    /// </summary>
    public void HidePopover ()
    {
        currentNode = null;
        gameObject.SetActive(false);
    }
}
