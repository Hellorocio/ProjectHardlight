using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoTBuff : Buff
{
    public float damagePerTick = 1.0f;
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
            attackable.TakeDamage(damagePerTick);
            yield return new WaitForSeconds(tickCooldown);
        }
    }
}
