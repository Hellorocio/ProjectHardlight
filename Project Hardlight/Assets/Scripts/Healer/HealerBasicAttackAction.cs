using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerBasicAttackAction : BasicAttackAction
{
    public GameObject healBasicAction;

    public override void DoBasicAttack()
    {
        GameObject healBasic = Instantiate(healBasicAction);
        Fighter thisFighter = GetComponent<Fighter>();
        healBasic.transform.parent = thisFighter.attackTarget.transform;
        healBasic.transform.localPosition = Vector3.zero;
        healBasic.transform.localScale = Vector3.one;
        float healAmt = thisFighter.basicAttackStats.damage + thisFighter.basicAttackStats.damage * thisFighter.attackBoost;
        thisFighter.attackTarget.GetComponent<Fighter>().Heal(healAmt);
    }
}
