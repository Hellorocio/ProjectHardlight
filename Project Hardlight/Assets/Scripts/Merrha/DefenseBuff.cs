using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A description of a buff. Not actually attached to something to affect it
public class DefenseBuff : Buff
{
    // e.g. -.2 = 20% less damage taken
    public float percentDamageTakenModifier;

    public override void InitializeBuff(GameObject affectedObject)
    {
        Debug.Log("Start defense buff");
        affectedObject.GetComponent<Fighter>().percentDamageTakenModifier += percentDamageTakenModifier;
    }

    public override void CleanupBuff(GameObject affectedObject)
    {
        Debug.Log("End defense buff");
        affectedObject.GetComponent<Fighter>().percentDamageTakenModifier -= percentDamageTakenModifier;
    }
}
