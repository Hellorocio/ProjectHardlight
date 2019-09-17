using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerBasicAttackAction : BasicAttackAction
{
    public GameObject healBasicAction;

    public override void DoBasicAttack()
    {
        Fighter thisFighter = GetComponent<Fighter>();
        GameObject healBasic = Instantiate(healBasicAction);
        
        healBasic.transform.parent = thisFighter.currentTarget.transform;
        healBasic.transform.localPosition = Vector3.zero;
        healBasic.transform.localScale = Vector3.one;

        float healAmt = thisFighter.basicAttackStats.damage + thisFighter.basicAttackStats.damage * thisFighter.attackBoost;
        thisFighter.currentTarget.GetComponent<Fighter>().Heal(healAmt);
        thisFighter.GainMana(10);
    }
}
