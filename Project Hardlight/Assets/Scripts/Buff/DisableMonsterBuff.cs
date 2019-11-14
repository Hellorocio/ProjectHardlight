using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableMonsterBuff : Buff
{
    public float duration;

    
    public Attackable attackable;

    public IEnumerator loop;
    
    // Start is called before the first frame update
    public override void InitializeBuff(GameObject affectedObject)
    {
        attackable = affectedObject.GetComponent<Attackable>();
        loop = DisableMonster();
        StartCoroutine(loop);
    }

    public override void CleanupBuff(GameObject affectedObject)
    {
        if (loop != null)
        {
            StopCoroutine(loop);
            loop = null;
        }
    }

    IEnumerator DisableMonster()
    {
        attackable.GetComponent<MonsterAI>().StopBasicAttacking();
        attackable.GetComponent<MonsterAI>().enabled = false;
        yield return new WaitForSeconds(duration);

        attackable.GetComponent<MonsterAI>().enabled = true;
        // TODO End this buff somewhere
    }
}
