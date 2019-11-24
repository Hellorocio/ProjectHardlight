using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DraggableIcon : MonoBehaviour
{
    private BattleUISoundManager soundManager;
    public bool allowDrag = true;
    public bool allowReplacement;
    private DraggableIcon replaceObj;

    [HideInInspector]
    public DraggableIcon dropLocation; 

    private Vector3 startPos;
    private Transform startParent;
    private Transform loadoutParent;
    [HideInInspector]
    public bool dragging;

    private enum IconType { vessel, soul }
    private IconType iconType;

    private LoadoutHoverController hoverController;

    private void Start()
    {
        
        //init icon type based on which icon script is attached
        if (GetComponent<VesselIcon>() != null)
        {
            iconType = IconType.vessel;
        }
        else if (GetComponent<SoulIcon>() != null)
        {
            iconType = IconType.soul;
        }

        //init hovertext
        GameObject hoverText = GameObject.Find("HoverText");
        if (hoverText != null)
        {
            hoverController = hoverText.GetComponent<LoadoutHoverController>();
        }

        startParent = transform.parent;

        // we assign loadoutParent to the transform of LoadoutMenu, which is at a different level depending
        // on if this is a selected slot or a regular slot
        loadoutParent = transform.parent.parent.parent;
        if (allowReplacement)
        {
            loadoutParent = loadoutParent.transform.parent.parent;
        }
        soundManager = loadoutParent.parent.parent.parent.GetComponent<BattleUISoundManager>();
    }

    /// <summary>
    /// Returns true if this icon has a valid vessel or soul attached
    /// </summary>
    /// <returns></returns>
    bool IsIconReady ()
    {
        BaseIcon icon = GetComponent<BaseIcon>();
        if (icon != null)
        {
            return icon.IconReady();
        }

        return false;
    }

    // Update is called once per frame
    void Update()
    {
        if (dragging)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10f;
            transform.position = mousePos;
        }
    }

    public void StartDragging ()
    {
        if (allowDrag && IsIconReady())
        {
            startPos = transform.position;

            // change parent to loadoutparent so dragged icon doesn't go under other panels
            // this causes grid issues with non-selected slots, so I'm only doing on selected slots for now
            if (allowReplacement)
            {
                transform.SetParent(loadoutParent);
            }

            if (hoverController != null)
            {
                hoverController.StartDragging(gameObject);
            }
            
            dragging = true;
        }
    }

    public void StopDragging ()
    {
        if (dragging)
        {
            dragging = false;

            // reparent to correct thing
            if (allowReplacement)
            {
                transform.SetParent(startParent);
            }

            if (hoverController != null)
            {
                hoverController.StopDragging(gameObject);
            }

            //check if there's a draggable to set
            if (dropLocation != null)
            {
                dropLocation.DropDraggable(this);
            }
            else         
            if ((transform.position - startPos).magnitude > 50 && replaceObj != null)
            {
                //if distance is large and this is a selection box, snap back to grid
                BaseIcon replaceIcon = replaceObj.GetComponent<BaseIcon>();
                if (replaceIcon != null)
                {
                    replaceIcon.ShowIcon(true);
                }

                BaseIcon icon = GetComponent<BaseIcon>();
                if (icon != null)
                {
                    icon.Clear();
                }

                //let loadoutUI know that selection has changed
                if (iconType == IconType.vessel)
                {
                    LoadoutUI.Instance.LoadoutUpdated(replaceObj.gameObject);
                }
                else 
                if (iconType == IconType.soul)
                {
                    LoadoutUI.Instance.LoadoutUpdated(gameObject);
                }
                replaceObj = null;
            }
            transform.position = startPos;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DraggableIcon draggable = collision.GetComponent<DraggableIcon>();
        if (allowReplacement && draggable != null && draggable.dragging && draggable.iconType == iconType)
        {
            draggable.dropLocation = this;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        DraggableIcon draggable = collision.GetComponent<DraggableIcon>();
        if (allowReplacement && draggable != null && draggable.dragging && draggable.iconType == iconType && draggable.dropLocation == null)
        {
            draggable.dropLocation = this;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        DraggableIcon draggable = collision.GetComponent<DraggableIcon>();
        if (draggable != null && draggable.dropLocation == this)
        {
            //draggable was moved away, so don't auto place here when drag ends
            draggable.dropLocation = null;
        }
    }

    /// <summary>
    /// Sets this icons soul to draggable's soul
    /// Called by a dragged icon on this icon, which is a drop slot
    /// </summary>
    /// <param name="draggable"></param>
    void DropDraggable(DraggableIcon draggable)
    {
        bool dropSucceeded = false;

        // enable this drop slot
        BaseIcon icon = GetComponent<BaseIcon>();
        if (icon != null)
        {
            icon.ShowIcon(true);
        }

        // add icon to new spot
        switch (iconType)
        {
            case IconType.vessel:
                VesselIcon myVessel = GetComponent<VesselIcon>();
                if (myVessel.vessel == null || allowReplacement)
                {
                    soundManager.PlayClip(soundManager.rpgClick);
                    myVessel.SetVessel(draggable.GetComponent<VesselIcon>().vessel);
                    dropSucceeded = true;
                }
                
                break;
            case IconType.soul:
                SoulIcon mySoul = GetComponent<SoulIcon>();
                if (mySoul.soul == null || allowReplacement)
                {

                    mySoul.SetSoul(draggable.GetComponent<SoulIcon>().soul);
                    dropSucceeded = true;

                    if (mySoul.soul.GetAllightValue(AllightType.SUNLIGHT) != 0)
                    {
                        soundManager.PlayClip(soundManager.sunlightSFX);
                    }
                    else if (mySoul.soul.GetAllightValue(AllightType.MOONLIGHT) != 0)
                    {
                        soundManager.PlayClip(soundManager.moonlightSFX);
                    }
                    else if (mySoul.soul.GetAllightValue(AllightType.STARLIGHT) != 0)
                    {
                        soundManager.PlayClip(soundManager.starlightSFX);
                    }
                }
                break;
            default:
                break;
        }

        if (dropSucceeded)
        {
            // replace icon if there was one already
            if (replaceObj != null)
            {
                BaseIcon replaceIcon = replaceObj.GetComponent<BaseIcon>();
                if (replaceIcon != null)
                {
                    replaceIcon.ShowIcon(true);
                }
            }

            // set new replaceObj
            if (draggable.replaceObj != null)
            {
                replaceObj = draggable.replaceObj;
            }
            else
            {
                replaceObj = draggable;
            }

            // remove icon from what this was dragged from
            BaseIcon draggableIcon = draggable.GetComponent<BaseIcon>();
            if (draggableIcon != null)
            {
                draggableIcon.ShowIcon(false);
            }
            
            draggable.replaceObj = null;
            draggable.StopDragging();

            //if this is a selectionWindow, check if everything has been set
            if (allowReplacement)
            {
                LoadoutUI.Instance.LoadoutUpdated(gameObject);
            }
        }
    }
}

