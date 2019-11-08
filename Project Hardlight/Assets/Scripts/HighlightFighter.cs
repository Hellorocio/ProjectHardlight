using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightFighter : MonoBehaviour
{
    public bool highlight;
    public SpriteRenderer appearence;
    public GameObject outline;

    SpriteRenderer[] outlines;

    private void Start()
    {
        if (outline != null)
        {
            outlines = outline.GetComponentsInChildren<SpriteRenderer>();
        }
    }
    
    void LateUpdate()
    {
        if (highlight && appearence != null)
        {
            if (!outline.activeSelf)
            {
                outline.SetActive(true);
            }
            
            foreach (SpriteRenderer s in outlines)
            {
                s.sprite = appearence.sprite;
                s.flipX = appearence.flipX;
            }
        }
        else if (!highlight && outline.activeSelf)
        {
            outline.SetActive(false);
        }
        
    }
}
