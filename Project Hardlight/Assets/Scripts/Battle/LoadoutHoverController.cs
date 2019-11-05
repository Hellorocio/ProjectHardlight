using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadoutHoverController : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI panelText;

    private Transform saveParent;
    private GameObject hoverIcon;
    private GameObject draggingObj;

    public void OnHoverStart (GameObject icon)
    {
        BaseIcon iconScript = icon.GetComponent<BaseIcon>();
        if (iconScript != null && draggingObj == null)
        {
            hoverIcon = icon;

            // initialize saveParent if we haven't already
            if (saveParent == null)
            {
                saveParent = transform.parent;
            }

            // set transform
            transform.SetParent(icon.transform);
            Vector2 iconScale = icon.GetComponent<RectTransform>().sizeDelta;
            transform.localPosition = new Vector3(iconScale.x / 2, iconScale.y / 2, 0);

            transform.SetParent(saveParent);

            // set text
            panelText.text = iconScript.GetHoverDesc();
            panel.SetActive(true);
        }
    }

    /// <summary>
    /// Stops hover if the icon hasn't changed
    /// </summary>
    /// <param name="icon"></param>
    public void OnHoverEnd(GameObject icon)
    {
        if (icon == hoverIcon)
        {
            panel.SetActive(false);
        }
    }

    /// <summary>
    /// Used to prevent hover text from showing up when anything is being dragged
    /// Disables any active hovertext
    /// </summary>
    /// <param name="obj"></param>
    public void StartDragging (GameObject obj)
    {
        draggingObj = obj;
        OnHoverEnd(hoverIcon);
    }

    /// <summary>
    /// Used to renable dragging if we stop dragging
    /// Check may be unnecessary but I want to prevent any races with  multiple draggers
    /// </summary>
    /// <param name="obj"></param>
    public void StopDragging (GameObject obj)
    {
        if (obj == draggingObj)
        {
            draggingObj = null;
        }
    }
}
