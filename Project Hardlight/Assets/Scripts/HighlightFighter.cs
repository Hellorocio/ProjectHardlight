using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightFighter : MonoBehaviour
{
    public bool highlight;
    public SpriteRenderer appearence;

    //Ref: https://forum.unity.com/threads/changing-animation-sprites.213431/
    void LateUpdate()
    {
        if (highlight && appearence != null)
        {
            string spriteName = appearence.sprite.name; //finds the name of the sprite to be rendered
            Sprite[] subSprites = Resources.LoadAll<Sprite>(spriteName.Substring(0, spriteName.IndexOf("_")) + "_Stroke"); //loads all the sprites in your new sprite sheet
            if (subSprites != null)
            {
                foreach (var sprite in subSprites)
                {
                    if (sprite.name == spriteName) //if the sprite has the same name as one you're trying to replace than replace it
                    {
                        appearence.sprite = sprite;
                    }
                }
            }
        }
        
    }
}
