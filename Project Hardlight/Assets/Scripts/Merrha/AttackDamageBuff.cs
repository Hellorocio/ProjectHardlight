using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A description of a buff. Not actually attached to something to affect it
public class AttackDamage : Buff
{
    // e.g. -.2 = 20% less damage taken
    public float percentAttackDamageModifier;

    public override void InitializeBuff(GameObject affectedObject)
    {
        Debug.Log("Start defense buff");
        affectedObject.GetComponent<Attackable>().percentAttackDamageModifier += percentAttackDamageModifier;
    }

    public override void CleanupBuff(GameObject affectedObject)
    {
        Debug.Log("End defense buff");
        affectedObject.GetComponent<Attackable>().percentAttackDamageModifier -= percentAttackDamageModifier;
    }
}
