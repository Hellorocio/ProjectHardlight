using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableMonsterBuff : Buff
{
    // Start is called before the first frame update
    public override void InitializeBuff(GameObject affectedObject)
    {
        affectedObject.GetComponent<MonsterAI>().StopBasicAttacking();
        affectedObject.GetComponent<MonsterAI>().enabled = false;

        affectedObject.GetComponent<Attackable>().fighting = false;
    }

    public override void CleanupBuff(GameObject affectedObject)
    {
        affectedObject.GetComponent<MonsterAI>().enabled = true;
        affectedObject.GetComponent<Attackable>().fighting = true;
    }
}
