using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class HealPerSecondBuff : Buff
{
    public float healPerTick = 1.0f;
    public float tickCooldown = 1.0f;
    
    [Header("donut touch")]
    public Attackable attackable;

    public IEnumerator healPerSecondLoop;
    
    // Start is called before the first frame update
    public override void InitializeBuff(GameObject affectedObject)
    {
        attackable = affectedObject.GetComponent<Attackable>();
        healPerSecondLoop = HealPerSecond();
        StartCoroutine(healPerSecondLoop);
    }

    public override void CleanupBuff(GameObject affectedObject)
    {
        if (healPerSecondLoop != null)
        {
            StopCoroutine(healPerSecondLoop);
            healPerSecondLoop = null;
        }
    }

    IEnumerator HealPerSecond()
    {
        while (true)
        {
            attackable.Heal(healPerTick);
            yield return new WaitForSeconds(tickCooldown);
        }
    }
}
