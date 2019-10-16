using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerBasicAttackAction : BasicAttackAction
{
    public GameObject healBasicAction;

    public override void DoBasicAttack(Fighter sourceFighter, GameObject target)
    {
        GameObject healBasic = Instantiate(healBasicAction);
        
        healBasic.transform.parent = target.transform;
        healBasic.transform.localPosition = Vector3.zero;
        healBasic.transform.localScale = Vector3.one;

        float healAmt = sourceFighter.GetBasicAttackDamage();
        target.GetComponent<Fighter>().Heal(healAmt);
        sourceFighter.GainMana(10);
    }
}
