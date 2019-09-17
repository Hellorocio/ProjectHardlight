using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBasicAttackAction : BasicAttackAction
{
    public override void DoBasicAttack()
    {
        Fighter thisFighter = GetComponent<Fighter>();
        float damage = thisFighter.basicAttackStats.damage + thisFighter.basicAttackStats.damage * thisFighter.attackBoost;
        thisFighter.currentTarget.GetComponent<Fighter>().TakeDamage(damage);

        if (thisFighter.team == CombatInfo.Team.Hero)
        {
            thisFighter.GainMana(10);
        }
    }
}
