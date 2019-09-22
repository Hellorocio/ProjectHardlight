using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerBasicAttackAction : BasicAttackAction
{
    public GameObject healBasicAction;

    public override void DoBasicAttack(GameObject target)
    {
        Fighter thisFighter = GetComponent<Fighter>();
        GameObject healBasic = Instantiate(healBasicAction);
        
        healBasic.transform.parent = target.transform;
        healBasic.transform.localPosition = Vector3.zero;
        healBasic.transform.localScale = Vector3.one;

        float healAmt = thisFighter.GetBasicAttackDamage();
        target.GetComponent<Fighter>().Heal(healAmt);
        thisFighter.GainMana(10);
    }
}
