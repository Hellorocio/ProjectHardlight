using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseIcon : MonoBehaviour
{
    private LoadoutHoverController hoverController;

    public virtual void Clear()
    {
        GetComponent<Image>().enabled = false;
    }

    public virtual string GetHoverDesc ()
    {
        return "";
    }

    public virtual bool IconReady ()
    {
        return false;
    }

    public virtual void SelectDetail() { }

    /// <summary>
    /// Starts hovering if start is true, otherwise stops hovering
    /// </summary>
    /// <param name="start"></param>
    public virtual void OnHover (bool start)
    {
        if (InitHoverController())
        {
            if (start)
            {
                hoverController.OnHoverStart(gameObject);
            }
            else
            {
                hoverController.OnHoverEnd(gameObject);
            }
        }
    }

    /// <summary>
    /// Returns true if hoverController has been initialized (attempts to init if it hasn't already)
    /// </summary>
    /// <returns></returns>
    bool InitHoverController ()
    {
        if (hoverController == null)
        {
            GameObject hoverText = GameObject.Find("HoverText");
            if (hoverText != null)
            {
                hoverController = hoverText.GetComponent<LoadoutHoverController>();
            }
        }
        return hoverController != null;
    }

    public virtual void ShowIcon(bool show)
    {
        GetComponent<Image>().enabled = show;
    }
}
