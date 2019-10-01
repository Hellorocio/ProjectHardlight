using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DraggableIcon : MonoBehaviour
{
    public bool allowReplacement;
    private DraggableIcon replaceObj;

    private Vector3 startPos;
    private bool dragging;

    private enum IconType { vessel, soul }
    private IconType iconType;

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
    }

    /// <summary>
    /// Returns true if this icon has a valid vessel or soul attached
    /// </summary>
    /// <returns></returns>
    bool IsIconReady ()
    {
        VesselIcon vesselIcon = GetComponent<VesselIcon>();
        if (vesselIcon != null && vesselIcon.vessel != null)
        {
            return true;
        }

        SoulIcon soulIcon = GetComponent<SoulIcon>();
        if (soulIcon != null && soulIcon.soul != null)
        {
            return true;
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
        if (IsIconReady())
        {
            startPos = transform.position;
            dragging = true;
        }
    }

    public void StopDragging ()
    {
        if (dragging)
        {
            dragging = false;
            
            //if distance is large and this is a selection box, snap back to grid
            if ((transform.position - startPos).magnitude > 50 && replaceObj != null)
            {
                replaceObj.GetComponent<Image>().enabled = true;
                GetComponent<Image>().enabled = false;
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
            DropDraggable(draggable);
        }
    }

    /// <summary>
    /// Sets this icons soul to draggable's soul
    /// </summary>
    /// <param name="draggable"></param>
    void DropDraggable(DraggableIcon draggable)
    {
        bool dropSucceeded = false;

        //add icon to new spot
        GetComponent<Image>().enabled = true;
        switch (iconType)
        {
            case IconType.vessel:
                VesselIcon myVessel = GetComponent<VesselIcon>();
                if (myVessel.vessel == null || allowReplacement)
                {
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
                }
                break;
            default:
                break;
        }

        if (dropSucceeded)
        {
            //replace icon if there was one already
            if (replaceObj != null)
            {
                replaceObj.GetComponent<Image>().enabled = true;
            }

            //set new replaceObj
            if (draggable.replaceObj != null)
            {
                replaceObj = draggable.replaceObj;
                print("replace obj * = " + draggable.replaceObj.gameObject.name);
            }
            else
            {
                print("replace obj = " + draggable.gameObject.name);
                replaceObj = draggable;
            }

            //remove icon from what this was dragged from
            draggable.GetComponent<Image>().enabled = false;
            switch (iconType)
            {
                case IconType.vessel:
                    draggable.GetComponent<VesselIcon>().Clear();
                    break;
                case IconType.soul:
                    draggable.GetComponent<SoulIcon>().Clear();
                    break;
                default:
                    break;
            }
            draggable.replaceObj = null;
            draggable.StopDragging();
        }
    }
}

