using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Override then use with BuffInstance
public class Buff : MonoBehaviour
{
    // Used to check for stacking
    public string buffNameId = "Default buff name";
    public string buffDescription = "Default buff description";
    // TODO appear on UI
    public Sprite buffIcon;

    // 0 for permanent-- must explicitly call EndBuff() on your own
    public float buffDuration = 0.0f;
    // TODO implement maxStacks based on buffNameId
    public int maxStacks = 1;

    // Override in the buff class
    public virtual void InitializeBuff(GameObject affectedObject)
    {
        Debug.Log("Default StartBuff()");
    }

    // Override in the buff class
    public virtual void CleanupBuff(GameObject affectedObject)
    {
        Debug.Log("Default EndBuff(). Make sure you clean up your buffs in the overriding EndBuff()!! Especially if doing stat changes to a Fighter.");
    }
}
