using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBasicAttackAction : BasicAttackAction
{
    public override void DoBasicAttack(GameObject target)
    {
        Fighter thisFighter = GetComponent<Fighter>();
        float damage = thisFighter.GetBasicAttackDamage();
        target.GetComponent<Fighter>().TakeDamage(damage);

        if (thisFighter.team == CombatInfo.Team.Hero)
        {
            thisFighter.GainMana(10);
        }
    }
}
